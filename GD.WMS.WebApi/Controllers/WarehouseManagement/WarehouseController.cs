using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Enums;
using GD.Model.WarehouseManagement;
using GD.Service.Interface.WarehouseManagement;
using GD.Service.WarehouseManagement;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace GD.WMS.WebApi.Controllers.WarehouseManagement
{
    /// <summary>
    /// 仓库信息
    /// </summary>
    [Verify]
    [Route("/warehousemanagement/warehouse")]
    [ApiExplorerSettings(GroupName = "wm")]
    public class WarehouseController : BaseController
    {
        private IWarehouseService warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            this.warehouseService = warehouseService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="warehouseQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] WarehouseQueryDto warehouseQueryDto)
        {
            var result = warehouseService.GetAllWarehouses(warehouseQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetWarehouseInfo(long id)
        {
            var result = warehouseService.GetWarehouse(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增仓库信息
        /// </summary>
        /// <param name="warehouseDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Log(Title = "新增仓库信息", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] WarehouseDto warehouseDto)
        {
            var warehouse = warehouseDto.Adapt<Warehouse>();
            var username = HttpContext.GetName();
            var result = warehouseService.AddWarehouse(warehouse, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warehouseDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id}")]
        [Log(Title = "编辑仓库信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit(long id, [FromBody] WarehouseDto warehouseDto)
        {
            var warehouse = warehouseDto.Adapt<Warehouse>();
            warehouse.WarehouseId = id;
            var username = HttpContext.GetName();
            var result = warehouseService.EidtWarehouse(warehouse, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Log(Title = "删除仓库信息", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            if (warehouseService.IsOtherUse(id))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "该类别存在库区，不允许删除");
            }
            var result = warehouseService.DeleteWarehouse(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出仓库信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "导出仓库信息", BusinessType = BusinessType.EXPORT)]
        public IActionResult WarehouseExport()
        {
            var warehouses = warehouseService.GetAllWarehouses();
            var warehouseExcelDtos = warehouses
                .Select(warehouse => warehouse.Adapt<WarehouseExcelDto>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(warehouseExcelDtos, "warehouse", "仓库信息");

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("exportTemplate")]
        [Log(Title = "仓库模板", BusinessType = BusinessType.EXPORT, IsSaveRequestData = true, IsSaveResponseData = false)]
        public IActionResult DownloadSupplierTemplate()
        {
            var result = warehouseService.DownloadImportTemplate();

            return ExportExcel(result.Item2, result.Item1);
        }

        [HttpPost("importData")]
        [Log(Title = "仓库信息导入", BusinessType = BusinessType.IMPORT, IsSaveRequestData = false, IsSaveResponseData = true)]
        public IActionResult ImportData([FromForm(Name = "file")] IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            var warehouse = stream
                .Query<WarehouseExcelDto>()
                .Select(it => it.Adapt<Warehouse>())
                .ToList();
            var result = warehouseService.ImportWarehouses(warehouse);

            return SUCCESS(result);
        }
    }
}
