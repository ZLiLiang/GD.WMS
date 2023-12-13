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
        /// 分页获取商品sku的选择列表
        /// </summary>
        /// <param name="SkuSelectQueryDto"></param>
        /// <returns></returns>
        PagedInfo<SkuSelectVm> GetCommoditySkuSelect(SkuSelectQueryDto SkuSelectQueryDto);

        /// <summary>
        /// 分页获取库位选择列表
        /// </summary>
        /// <param name="stockSelectDto"></param>
        /// <returns></returns>
        PagedInfo<StockSelectVm> GetStockSelect(StockSelectDto stockSelectDto);
    }
}
