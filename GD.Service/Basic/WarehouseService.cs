using GD.Infrastructure.App;
using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;
using GD.Repository;
using GD.Service.Interface.Basic;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;

namespace GD.Service.Basic
{
    [AppService(ServiceType = typeof(IWarehouseService), ServiceLifetime = LifeTime.Transient)]
    public class WarehouseService : BaseService<Warehouse>, IWarehouseService
    {
        public long AddWarehouse(Warehouse warehouse, string userName)
        {
            warehouse.Create_by = userName;
            warehouse.Create_time = DateTime.Now;

            return InsertReturnBigIdentity(warehouse);
        }

        public long DeleteWarehouse(long warehouseId)
        {
            return Delete(warehouseId);
        }

        public (string, string) DownloadImportTemplate()
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = "仓库信息.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            return (sFileName, fullPath);
        }

        public long EidtWarehouse(Warehouse warehouse, string userName)
        {
            warehouse.Update_by = userName;
            warehouse.Update_time = DateTime.Now;

            Context
                .Updateable<Location>()
                .SetColumns(it => it.WarehouseName == warehouse.WarehouseName)
                .Where(it => it.WarehouseId == warehouse.WarehouseId)
                .ExecuteCommand();

            Context
                .Updateable<Region>()
                .SetColumns(it => it.WarehouseName == warehouse.WarehouseName)
                .Where(it => it.WarehouseId == warehouse.WarehouseId)
                .ExecuteCommand();

            return Update(warehouse);
        }

        public PagedInfo<Warehouse> GetAllWarehouses(WarehouseQueryDto warehouseQueryDto)
        {
            var expression = Expressionable.Create<Warehouse>()
                .AndIF(!string.IsNullOrEmpty(warehouseQueryDto.WarehouseName), warehouse => warehouse.WarehouseName.Contains(warehouseQueryDto.WarehouseName))
                .AndIF(!string.IsNullOrEmpty(warehouseQueryDto.City), warehouse => warehouse.City.Contains(warehouseQueryDto.City))
                .AndIF(!string.IsNullOrEmpty(warehouseQueryDto.Manager), warehouse => warehouse.Manager.Contains(warehouseQueryDto.Manager))
                .AndIF(warehouseQueryDto.BeginTime != DateTime.MinValue && warehouseQueryDto.BeginTime != null, exp => exp.Create_time >= warehouseQueryDto.BeginTime)
                .AndIF(warehouseQueryDto.EndTime != DateTime.MaxValue && warehouseQueryDto.EndTime != null, exp => exp.Create_time <= warehouseQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(warehouseQueryDto);
        }

        public List<Warehouse> GetAllWarehouses()
        {
            return Queryable()
                .ToList();
        }

        public Warehouse GetWarehouse(long warehouseId)
        {
            return Queryable()
                .Where(it => it.WarehouseId == warehouseId)
                .First();
        }

        public (string, object) ImportWarehouses(List<Warehouse> warehouse)
        {
            var storage = Context.Storageable(warehouse)
                .SplitUpdate(it => it.Any()) //存在更新
                .SplitInsert(it => true) //否则插入（更新优先级大于插入）
                .SplitError(it => string.IsNullOrEmpty(it.Item.WarehouseName), "仓库名称不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.Manager), "负责人不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.ContactTel), "联系方式不能为空")
                .WhereColumns(it => it.WarehouseName) //如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
                .ToStorage();

            storage.AsInsertable.ExecuteCommand(); //执行插入
            storage.AsUpdateable.ExecuteCommand(); //执行更新
            //storage.AsDeleteable.ExecuteCommand(); //执行删除　

            string msg = string.Format(" 插入{0} 更新{1} 错误{2} 不计算{3} 删除{4} 总共{5}",
                               storage.InsertList.Count,
                               storage.UpdateList.Count,
                               storage.ErrorList.Count,
                               storage.IgnoreList.Count,
                               storage.DeleteList.Count,
                               storage.TotalList.Count);
            //输出统计                      
            Console.WriteLine(msg);

            //输出错误信息               
            foreach (var item in storage.ErrorList)
            {
                Console.WriteLine("userName为" + item.Item.WarehouseName + " : " + item.StorageMessage);
            }

            return (msg, storage.ErrorList);
        }

        public bool IsOtherUse(long warehouseId)
        {
            var regionQuery = Context
                .Queryable<Region>()
                .Where(it => it.WarehouseId == warehouseId)
                .Select(it => (object)new { Name = it.RegionName });

            var locationQuery = Context
                .Queryable<Location>()
                .Where(it => it.WarehouseId == warehouseId)
                .Select(it => (object)new { Name = it.LocationCode });

            var result = Context
                .UnionAll(regionQuery, locationQuery)
                .ToList();

            return result.Count > 0;
        }
    }
}
