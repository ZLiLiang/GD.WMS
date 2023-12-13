using GD.Model.Dto.Inventory;
using GD.Service.Interface.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Inventory
{
    /// <summary>
    /// 库存控制器
    /// </summary>
    //[Verify]
    [Route("/inventory/stock")]
    //[ApiExplorerSettings(GroupName = "inventory")]
    public class StockController : BaseController
    {
        private IStockService stockService;

        public StockController(IStockService stockService)
        {
            this.stockService = stockService;
        }

        /// <summary>
        /// 获取商品选择列表
        /// </summary>
        /// <param name="commoditySkuSelectQueryDto"></param>
        /// <returns></returns>
        [HttpGet("skuselect")]
        public IActionResult GetSkuSelect([FromQuery] SkuSelectQueryDto commoditySkuSelectQueryDto)
        {
            var result = stockService.GetCommoditySkuSelect(commoditySkuSelectQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取库存选择列表
        /// </summary>
        /// <param name="stockSelectDto"></param>
        /// <returns></returns>
        [HttpGet("stockselect")]
        public IActionResult GetStockSelect([FromQuery] StockSelectDto stockSelectDto)
        {
            var result = stockService.GetStockSelect(stockSelectDto);

            return SUCCESS(result);
        }
    }
}
