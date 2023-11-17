using GD.Infrastructure.Attribute;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;
using GD.Repository;
using GD.Service.Interface.WarehouseManagement;
using SqlSugar;

namespace GD.Service.WarehouseManagement
{
    [AppService(ServiceType = typeof(ICommodityService), ServiceLifetime = LifeTime.Transient)]
    public class CommodityService : BaseService<CommoditySPU>, ICommodityService
    {
        public long AddCommodity(CommoditySPU commoditySPU, string userName)
        {
            commoditySPU.Create_by = userName;
            commoditySPU.Create_time = DateTime.Now;
            commoditySPU.DetailList?.ForEach(it =>
            {
                it.Create_by = userName;
                it.Create_time = DateTime.Now;
            });
            commoditySPU.SupplierName = Context.Queryable<Supplier>()
                .First(supplier => supplier.SupplierId == commoditySPU.SupplierId)
                .SupplierName;
            commoditySPU.CategoryName = Context.Queryable<Category>()
                .First(category => category.CategoryId == commoditySPU.CategoryId)
                .CategoryName;

            return Context
                .InsertNav(commoditySPU)
                .Include(z1 => z1.DetailList)
                .ExecuteReturnEntity()
                .CommoditySPUId;
        }

        public long DeleteCommodityById(long id)
        {
            var result = Context.DeleteNav<CommoditySPU>(it => it.CommoditySPUId == id)
                .Include(z1 => z1.DetailList)
                .ExecuteCommand();

            if (result)
            {
                return id;
            }
            else
            {
                return -1;
            }
        }

        public long DeleteSKUById(long id)
        {
            var result = Context
                .Deleteable<CommoditySKU>()
                .Where(it => it.CommoditySKUId == id)
                .ExecuteCommand();
            if (result > 0)
            {
                return id;
            }
            else
            {
                return -1;
            }
        }

        public long EditCommodity(CommoditySPU commoditySPU, string userName)
        {
            commoditySPU.Update_by = userName;
            commoditySPU.Update_time = DateTime.Now;
            commoditySPU.DetailList.ForEach(it =>
            {
                it.Update_by = userName;
                it.Update_time = DateTime.Now;
                it.Volume = it.Length * it.Height * it.Width;
            });
            commoditySPU.SupplierName = Context.Queryable<Supplier>()
                .First(supplier => supplier.SupplierId == commoditySPU.SupplierId)
                .SupplierName;
            commoditySPU.CategoryName = Context.Queryable<Category>()
                .First(category => category.CategoryId == commoditySPU.CategoryId)
                .CategoryName;

            var result = Context.UpdateNav(commoditySPU)
                .Include(zi => zi.DetailList)
                .ExecuteCommand();

            if (result)
            {
                return commoditySPU.CommoditySPUId;
            }
            else
            {
                return -1;
            }
        }

        public PagedInfo<CommoditySPU> GetAllCommodities(CommodityQueryDto commodityQueryDto)
        {
            var expression = Expressionable.Create<CommoditySPU>()
                .AndIF(!string.IsNullOrEmpty(commodityQueryDto.CommoditySPUCode), commodity => commodity.CommoditySPUCode.Contains(commodityQueryDto.CommoditySPUCode))
                .AndIF(!string.IsNullOrEmpty(commodityQueryDto.CommoditySPUName), commodity => commodity.CommoditySPUName.Contains(commodityQueryDto.CommoditySPUName))
                .AndIF(commodityQueryDto.CategoryId!=null, commodity => commodity.CategoryId==commodityQueryDto.CategoryId);

            return Queryable()
                .Includes(it => it.DetailList)
                .Where(expression.ToExpression())
                .ToPage(commodityQueryDto);
        }

        public List<CommoditySPU> GetAllCommodities()
        {
            return Queryable()
                .Includes(it => it.DetailList)
                .ToList();
        }

        public CommoditySPU GetCommodityById(long id)
        {
            return Queryable()
                .Includes(it => it.DetailList)
                .Where(it => it.CommoditySPUId == id)
                .Single();
        }
    }
}
