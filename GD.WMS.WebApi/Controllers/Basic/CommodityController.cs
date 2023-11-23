using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Enums;
using GD.Service.Interface.Basic;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Basic
{
    /// <summary>
    /// 商品信息
    /// </summary>
    [Verify]
    [Route("/basic/commodity")]
    [ApiExplorerSettings(GroupName = "basic")]
    public class CommodityController : BaseController
    {
        private ICommodityService commodityService;
        private ICategoryService categoryService;
        private ISupplierService supplierService;

        public CommodityController(ICommodityService commodityService, ICategoryService categoryService, ISupplierService supplierService)
        {
            this.commodityService = commodityService;
            this.categoryService = categoryService;
            this.supplierService = supplierService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="commodityQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] CommodityQueryDto commodityQueryDto)
        {
            var result = commodityService.GetAllCommodities(commodityQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取商品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetCommodityInfo(long id)
        {
            var result = commodityService.GetCommodityById(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增商品信息
        /// </summary>
        /// <param name="commoditySPUDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增商品信息", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] CommoditySPUDto commoditySPUDto)
        {
            var commodity = commoditySPUDto.Adapt<CommoditySPU>();
            var username = HttpContext.GetName();
            var result = commodityService.AddCommodity(commodity, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑商品信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commoditySPUDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id}")]
        [Log(Title = "编辑商品信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit(long id, [FromBody] CommoditySPUDto commoditySPUDto)
        {
            var commodity = commoditySPUDto.Adapt<CommoditySPU>();
            commodity.CommoditySPUId = id;
            var username = HttpContext.GetName();
            var result = commodityService.EditCommodity(commodity, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id删除商品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Log(Title = "删除商品信息", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = commodityService.DeleteCommodityById(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id删除商品sku信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("sku/{id}")]
        [Log(Title = "删除商品sku信息", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteSKU(long id)
        {
            var result = commodityService.DeleteSKUById(id);

            return SUCCESS(result);
        }


        /// <summary>
        /// 获取商品类别列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("categoryOptions")]
        public IActionResult GetCategoryOptions()
        {
            var result = categoryService.GetAllCategoryTree();

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取供应商列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("supplierOptions")]
        public IActionResult GetSupplierOptions()
        {
            var result = supplierService.GetAllSupplier();

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出商品报表
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "商品导出", BusinessType = BusinessType.EXPORT)]
        public IActionResult CommodityExport()
        {
            var commodities = commodityService.GetAllCommodities();
            var commodityExcelDtos = commodities
                .Select(commodity => commodity.Adapt<CommoditySPUExcelDto>())
                .ToList();
            var result = ExcelHelper.ExportExcelMiniTwoNestList(commodityExcelDtos, "commodity", "商品信息");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
