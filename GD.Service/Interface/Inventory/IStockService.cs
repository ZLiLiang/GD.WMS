using GD.Model.Basic;
using GD.Model.Dto.Inventory;
using GD.Model.Page;
using GD.Model.Vm.Inventory;

namespace GD.Service.Interface.Inventory
{
    /// <summary>
    /// 库存服务
    /// </summary>
    public interface IStockService : IBaseService<CommoditySKU>
    {
        /// <summary>
        /// 分页获取商品sku的选择列表
        /// </summary>
        /// <param name="commoditySkuSelectQueryDto"></param>
        /// <returns></returns>
        PagedInfo<CommoditySkuSelect> GetCommoditySkuSelect(CommoditySkuSelectQueryDto commoditySkuSelectQueryDto);
    }
}
