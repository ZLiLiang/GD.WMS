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
    /// 库位控制器
    /// </summary>
    [Verify]
    [Route("/warehousemanagement/owner")]
    [ApiExplorerSettings(GroupName = "wm")]
    public class OwnerController : BaseController
    {
        private IOwnerService ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            this.ownerService = ownerService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="ownerQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] OwnerQueryDto ownerQueryDto)
        {
            var result = ownerService.GetAllOwners(ownerQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 格局id获取货主信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult GetOwner(long id)
        {
            var result = ownerService.GetOwner(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增货主信息
        /// </summary>
        /// <param name="ownerDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增货主信息", BusinessType = BusinessType.INSERT)]
        public IActionResult AddOwner([FromBody] OwnerDto ownerDto)
        {
            var owner = ownerDto.Adapt<Owner>();
            var username = HttpContext.GetName();
            var result = ownerService.AddOwner(owner, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑货主信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ownerDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑货主信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult EditOwner(long id, [FromBody] OwnerDto ownerDto)
        {
            var owner = ownerDto.Adapt<Owner>();
            owner.OwnerId = id;
            var username = HttpContext.GetName();
            var result = ownerService.EditOwner(owner, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除货主信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除货主信息", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteOwner(long id)
        {
            var result = ownerService.DeleteOwner(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出货主信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "导出货主信息", BusinessType = BusinessType.EXPORT)]
        public IActionResult OwnerExport()
        {
            var owners = ownerService.GetAllOwners();
            var ownerExcelDto = owners
                .Select(it => it.Adapt<OwnerExcelDto>())
                .ToList();

            var result = ExcelHelper.ExportExcelMini(ownerExcelDto, "owner", "货主信息");

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("exportTemplate")]
        [Log(Title = "货主模板", BusinessType = BusinessType.EXPORT, IsSaveRequestData = true, IsSaveResponseData = false)]
        public IActionResult DownloadSupplierTemplate()
        {
            var result = ownerService.DownloadImportTemplate();

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 货主信息导入
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("importData")]
        [Log(Title = "货主信息导入", BusinessType = BusinessType.IMPORT, IsSaveRequestData = false, IsSaveResponseData = true)]
        public IActionResult ImportData([FromForm(Name = "file")] IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            var owner = stream
                .Query<OwnerExcelDto>()
                .Select(it => it.Adapt<Owner>())
                .ToList();
            var result = ownerService.ImportOwners(owner);

            return SUCCESS(result);
        }
    }
}
