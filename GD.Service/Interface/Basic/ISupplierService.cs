using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;

namespace GD.Service.Interface.Basic
{
    public interface ISupplierService : IBaseService<Supplier>
    {
        /// <summary>
        /// 分页获取所有供应商信息
        /// </summary>
        /// <param name="supplierQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Supplier> GetAllSupplier(SupplierQueryDto supplierQueryDto);

        /// <summary>
        /// 获取所有供应商信息
        /// </summary>
        /// <returns></returns>
        List<Supplier> GetAllSupplier();

        /// <summary>
        /// 导入供应商信息
        /// </summary>
        /// <returns></returns>
        (string, object) ImportSupplers(List<Supplier> suppliers);

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        (string, string) DownloadImportTemplate();

        /// <summary>
        /// 根据id获取单个供应商信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Supplier GetSupplierById(long supplierId);

        /// <summary>
        /// 新增供应商信息
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddSupplier(Supplier supplier, string userName);

        /// <summary>
        /// 修改供应商信息
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EditSupplier(Supplier supplier, string userName);

        /// <summary>
        /// 删除供应商信息
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        long DeleteBySupplierId(long supplierId);

        /// <summary>
        /// 查询是否被其他表引用
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool IsOtherUse(long supplierId);
    }
}
