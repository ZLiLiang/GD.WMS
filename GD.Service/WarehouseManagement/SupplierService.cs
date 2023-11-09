using GD.Infrastructure.App;
using GD.Infrastructure.Attribute;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;
using GD.Repository;
using GD.Service.Interface.WarehouseManagement;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;

namespace GD.Service.WarehouseManagement
{
    [AppService(ServiceType = typeof(ISupplierService), ServiceLifetime = LifeTime.Transient)]
    public class SupplierService : BaseService<Supplier>, ISupplierService
    {
        public long AddSupplier(Supplier supplier, string userName)
        {
            supplier.Create_by = userName;
            supplier.Create_time = DateTime.Now;
            return InsertReturnBigIdentity(supplier);
        }

        public long DeleteBySupplierId(long supplierId)
        {
            return Delete(supplierId);
        }

        public (string, string) DownloadImportTemplate()
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = "供应商信息.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            return (sFileName, fullPath);
        }

        public long EditSupplier(Supplier supplier, string userName)
        {
            supplier.Update_by = userName;
            supplier.Update_time = DateTime.Now;
            return Update(supplier);
        }

        public PagedInfo<Supplier> GetAllSupplier(SupplierQueryDto supplierQueryDto)
        {
            var expression = Expressionable.Create<Supplier>()
                .AndIF(!string.IsNullOrEmpty(supplierQueryDto.SupplierName), supplier => supplier.SupplierName.Contains(supplierQueryDto.SupplierName))
                .AndIF(!string.IsNullOrEmpty(supplierQueryDto.Manager), supplier => supplier.Manager.Contains(supplierQueryDto.Manager))
                .AndIF(!string.IsNullOrEmpty(supplierQueryDto.ContactTel), supplier => supplier.ContactTel.Contains(supplierQueryDto.ContactTel))
                .AndIF(supplierQueryDto.BeginTime != DateTime.MinValue && supplierQueryDto.BeginTime != null, exp => exp.Create_time >= supplierQueryDto.BeginTime)
                .AndIF(supplierQueryDto.EndTime != DateTime.MaxValue && supplierQueryDto.EndTime != null, exp => exp.Create_time <= supplierQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(supplierQueryDto);
        }

        public List<Supplier> GetAllSupplier()
        {
            return Queryable()
                .ToList();
        }

        public Supplier GetSupplierById(long supplierId)
        {
            return Queryable()
                .Where(supplier => supplier.SupplierId == supplierId)
                .Single();
        }

        public (string, object) ImportSupplers(List<Supplier> suppliers)
        {
            var storage = Context.Storageable(suppliers)
                .SplitUpdate(it => it.Any()) //存在更新
                .SplitInsert(it => true) //否则插入（更新优先级大于插入）
                .SplitError(it => string.IsNullOrEmpty(it.Item.SupplierName), "供应商名称不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.Manager), "负责人不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.Manager), "联系方式不能为空")
                .WhereColumns(it => it.SupplierName) //如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
                .ToStorage();

            storage.AsInsertable.ExecuteCommand(); //插入可插入部分

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
                Console.WriteLine("userName为" + item.Item.SupplierName + " : " + item.StorageMessage);
            }

            return (msg, storage.ErrorList);
        }

        public bool IsOtherUse(long supplierId)
        {
            throw new NotImplementedException();
        }
    }
}
