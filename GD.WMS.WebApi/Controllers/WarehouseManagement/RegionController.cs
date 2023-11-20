using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Enums;
using GD.Model.WarehouseManagement;
using GD.Service.Interface.WarehouseManagement;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.WarehouseManagement
{
    /// <summary>
    /// 库区控制器
    /// </summary>
    [Verify]
    [Route("/warehousemanagement/region")]
    [ApiExplorerSettings(GroupName = "wm")]
    public class RegionController : BaseController
    {
        private IRegionService regionService;
        private IWarehouseService warehouseService;

        public RegionController(IRegionService regionService, IWarehouseService warehouseService)
        {
            this.regionService = regionService;
            this.warehouseService = warehouseService;
        }


        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="regionQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] RegionQueryDto regionQueryDto)
        {
            var result = regionService.GetAllRegions(regionQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取仓库选项
        /// </summary>
        /// <returns></returns>
        [HttpGet("options")]
        public IActionResult GetWarehouseOptions()
        {
            var result = warehouseService
                .GetAllWarehouses()
                .Select(it => new { it.WarehouseId, it.WarehouseName })
                .ToList();

            return SUCCESS(result);
        }

        /// <summary>
        /// 格局id获取库区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult GetRegion(long id)
        {
            var result = regionService.GetRegion(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增库区信息
        /// </summary>
        /// <param name="regionDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增库区信息", BusinessType = BusinessType.INSERT)]
        public IActionResult AddRegion([FromBody] RegionDto regionDto)
        {
            var region = regionDto.Adapt<Region>();
            var username = HttpContext.GetName();
            var result = regionService.AddRegion(region, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑库区信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="regionDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑库区信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult EditRegion(long id, [FromBody] RegionDto regionDto)
        {
            var region = regionDto.Adapt<Region>();
            region.RegionId = id;
            var username = HttpContext.GetName();
            var result = regionService.EidtRegion(region, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除库区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除库区信息", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteRegion(long id)
        {
            var result= regionService.DeleteRegion(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出库区信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "导出库区信息", BusinessType = BusinessType.EXPORT)]
        public IActionResult RegionExport()
        {
            var regions = regionService.GetAllRegions();
            var regionExcelDtos = regions
                .Select(it => it.Adapt<RegionExcelDto>())
                .ToList();

            var result = ExcelHelper.ExportExcelMini(regionExcelDtos, "region", "库区信息");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
