using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;

namespace GD.Service.Interface.WarehouseManagement
{
    /// <summary>
    /// 仓库服务
    /// </summary>
    public interface IWarehouseService : IBaseService<Warehouse>
    {
        /// <summary>
        /// 分页获取所有仓库信息
        /// </summary>
        /// <returns></returns>
        PagedInfo<Warehouse> GetAllWarehouses(WarehouseQueryDto warehouseQueryDto);

        /// <summary>
        /// 获取所有仓库信息
        /// </summary>
        /// <returns></returns>
        List<Warehouse> GetAllWarehouses();

        /// <summary>
        /// 根据id获取仓库信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        Warehouse GetWarehouse(long warehouseId);

        /// <summary>
        /// 新增仓库信息
        /// </summary>
        /// <param name="warehouse"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddWarehouse(Warehouse warehouse, string userName);

        /// <summary>
        /// 修改仓库信息
        /// </summary>
        /// <param name="warehouse"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EidtWarehouse(Warehouse warehouse, string userName);

        /// <summary>
        /// 根据id删除仓库信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        long DeleteWarehouse(long warehouseId);

        /// <summary>
        /// 导入供应商信息
        /// </summary>
        /// <returns></returns>
        (string, object) ImportWarehouses(List<Warehouse> warehouse);

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        (string, string) DownloadImportTemplate();

        /// <summary>
        /// 查询是否被其他表引用
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        bool IsOtherUse(long warehouseId);
    }
}
