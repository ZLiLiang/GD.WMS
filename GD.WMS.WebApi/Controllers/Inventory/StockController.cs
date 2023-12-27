using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Model.Dto.Inventory;
using GD.Model.Enums;
using GD.Model.Vm.Inventory;
using GD.Service.Interface.Inventory;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Inventory
{
    /// <summary>
    /// 库存控制器
    /// </summary>
    [Verify]
    [Route("/inventory/stock")]
    [ApiExplorerSettings(GroupName = "inventory")]
    public class StockController : BaseController
    {
        private IStockService stockService;

        public StockController(IStockService stockService)
        {
            this.stockService = stockService;
        }

        /// <summary>
        /// 获取库存列表
        /// </summary>
        /// <param name="stockQueryDto"></param>
        /// <returns></returns>
        [HttpGet("stock")]
        public IActionResult GetStock([FromQuery] StockQueryDto stockQueryDto)
        {
            var result = stockService.GetStock(stockQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取库位列表
        /// </summary>
        /// <param name="locationStockQueryDto"></param>
        /// <returns></returns>
        [HttpGet("locationstock")]
        public IActionResult GetLocationStock([FromQuery] LocationStockQueryDto locationStockQueryDto)
        {
            var result = stockService.GetLocationStock(locationStockQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取库存选择列表
        /// </summary>
        /// <param name="stockSelectQueryDto"></param>
        /// <returns></returns>
        [HttpGet("stockselect")]
        public IActionResult GetStockSelect([FromQuery] StockSelectQueryDto stockSelectQueryDto)
        {
            var result = stockService.GetStockSelect(stockSelectQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取商品选择列表
        /// </summary>
        /// <param name="skuSelectQueryDto"></param>
        /// <returns></returns>
        [HttpGet("skuselect")]
        public IActionResult GetSkuSelect([FromQuery] SkuSelectQueryDto skuSelectQueryDto)
        {
            var result = stockService.GetSkuSelect(skuSelectQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出库存列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("stockExport")]
        [Log(Title = "导出库存列表", BusinessType = BusinessType.EXPORT)]
        public IActionResult StockExport()
        {
            var stocks = stockService.GetStock();
            var stockExcelDto = stocks.Select(it => it.Adapt<StockExcelVm>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(stockExcelDto, "stocks", "库存列表");

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出库位列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("locationStockExport")]
        [Log(Title = "导出库位列表", BusinessType = BusinessType.EXPORT)]
        public IActionResult LocationStockExport()
        {
            var locationsStocks = stockService.GetLocationStock();
            var locationsStockExcelDto = locationsStocks.Select(it => it.Adapt<LocationStockExcelVm>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(locationsStockExcelDto, "locationsStocks", "库位列表");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
