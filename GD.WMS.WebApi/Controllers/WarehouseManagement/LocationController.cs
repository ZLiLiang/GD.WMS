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
    /// 库位控制器
    /// </summary>
    //[Verify]
    [Route("/warehousemanagement/location")]
    //[ApiExplorerSettings(GroupName = "wm")]
    public class LocationController : BaseController
    {
        private IWarehouseService warehouseService;
        private IRegionService regionService;
        private ILocationService locationService;

        public LocationController(IWarehouseService warehouseService, IRegionService regionService, ILocationService locationService)
        {
            this.warehouseService = warehouseService;
            this.regionService = regionService;
            this.locationService = locationService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="locationQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] LocationQueryDto locationQueryDto)
        {
            var result = locationService.GetAllLocations(locationQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取仓库选项
        /// </summary>
        /// <returns></returns>
        [HttpGet("warehouseOptions")]
        public IActionResult GetWarehouseOptions()
        {
            var result = warehouseService
                .GetAllWarehouses()
                .Select(it => new { it.WarehouseId, it.WarehouseName })
                .ToList();

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取库区选项
        /// </summary>
        /// <param name="id">仓库id</param>
        /// <returns></returns>
        [HttpGet("regionOptions/{id:long}")]
        public IActionResult GetRegionOptions(long id)
        {
            var result = regionService
                .GetRegionsByWarehouseId(id)
                .Select(it => new { it.RegionId, it.RegionName, it.RegionProperty })
                .ToList();

            return SUCCESS(result);
        }

        /// <summary>
        /// 格局id获取库位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult GetLocation(long id)
        {
            var result = locationService.GetLocation(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增库位信息
        /// </summary>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增库位信息", BusinessType = BusinessType.INSERT)]
        public IActionResult AddLocation([FromBody] LocationDto locationDto)
        {
            var location = locationDto.Adapt<Location>();
            var username = HttpContext.GetName();
            var result = locationService.AddLocation(location, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑库位信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑库位信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult EditLocation(long id, [FromBody] LocationDto locationDto)
        {
            var location = locationDto.Adapt<Location>();
            location.LocationId = id;
            var username = HttpContext.GetName();
            var result = locationService.EditLocation(location, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除库位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除库位信息", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteLocation(long id)
        {
            var result = locationService.DeleteLocation(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出库位信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "导出库位信息", BusinessType = BusinessType.EXPORT)]
        public IActionResult LocationExport()
        {
            var locations = locationService.GetAllLocations();
            var locationExcelDto = locations
                .Select(it => it.Adapt<LocationExcelDto>())
                .ToList();

            var result = ExcelHelper.ExportExcelMini(locationExcelDto, "location", "库位信息");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
