using GD.Model.Dto.Inventory;
using GD.Model.Inventory;
using GD.Model.Page;
using GD.Model.Vm.Inventory;

namespace GD.Service.Interface.Inventory
{
    /// <summary>
    /// 库存服务
    /// </summary>
    public interface IStockService : IBaseService<Stock>
    {
        /// <summary>
        /// 分页获取库存列表
        /// </summary>
        /// <param name="stockQueryDto"></param>
        /// <returns></returns>
        PagedInfo<StockVm> GetStock(StockQueryDto stockQueryDto);

        /// <summary>
        /// 获取库存列表
        /// </summary>
        /// <returns></returns>
        List<StockVm> GetStock();

        /// <summary>
        /// 分页获取库位列表
        /// </summary>
        /// <param name="locationStockQueryDto"></param>
        /// <returns></returns>
        PagedInfo<LocationStockVm> GetLocationStock(LocationStockQueryDto locationStockQueryDto);

        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <returns></returns>
        List<LocationStockVm> GetLocationStock();

        /// <summary>
        /// 分页获取库位选择列表
        /// </summary>
        /// <param name="stockSelectQueryDto"></param>
        /// <returns></returns>
        PagedInfo<StockSelectVm> GetStockSelect(StockSelectQueryDto stockSelectQueryDto);

        /// <summary>
        /// 分页获取商品sku的选择列表
        /// </summary>
        /// <param name="SkuSelectQueryDto"></param>
        /// <returns></returns>
        PagedInfo<SkuSelectVm> GetSkuSelect(SkuSelectQueryDto SkuSelectQueryDto);
    }
}
