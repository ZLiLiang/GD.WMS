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
    /// 运费设置
    /// </summary>
    //[Verify]
    [Route("/warehousemanagement/freightFee")]
    //[ApiExplorerSettings(GroupName = "wm")]
    public class FreightFeeController : BaseController
    {
        private IFreightFeeService freightFeeService;

        public FreightFeeController(IFreightFeeService freightFeeService)
        {
            this.freightFeeService = freightFeeService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="freightFeeQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] FreightFeeQueryDto freightFeeQueryDto)
        {
            var result = freightFeeService.GetAllFreightFees(freightFeeQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取运费信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult GetFreightFee(long id)
        {
            var result = freightFeeService.GetFreightFee(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增运费信息
        /// </summary>
        /// <param name="freightFeeDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增运费信息", BusinessType = BusinessType.INSERT)]
        public IActionResult AddFreightFee([FromBody] FreightFeeDto freightFeeDto)
        {
            var freightFee = freightFeeDto.Adapt<FreightFee>();
            var username = HttpContext.GetName();
            var result = freightFeeService.AddFreightFee(freightFee, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑运费信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="freightFeeDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑运费信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult EditFreightFee(long id, [FromBody] FreightFeeDto freightFeeDto)
        {
            var freightFee = freightFeeDto.Adapt<FreightFee>();
            freightFee.FreightFeeId = id;
            var username = HttpContext.GetName();
            var result = freightFeeService.EidtFreightFee(freightFee, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除运费信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除运费信息", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteFreightFee(long id)
        {
            var result = freightFeeService.DeleteFreightFee(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出运费信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "导出运费信息", BusinessType = BusinessType.EXPORT)]
        public IActionResult FreightFeeExport()
        {
            var freightFees = freightFeeService.GetAllFreightFees();
            var freightFeeExcelDtos = freightFees
                .Select(freightFee => freightFee.Adapt<FreightFeeExcelDto>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(freightFeeExcelDtos, "freightFee", "运费信息");

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("exportTemplate")]
        [Log(Title = "运费模板", BusinessType = BusinessType.EXPORT, IsSaveRequestData = true, IsSaveResponseData = false)]
        public IActionResult DownloadSupplierTemplate()
        {
            var result = freightFeeService.DownloadImportTemplate();

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 从excel导入信息
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("importData")]
        [Log(Title = "运费信息导入", BusinessType = BusinessType.IMPORT, IsSaveRequestData = false, IsSaveResponseData = true)]
        public IActionResult ImportData([FromForm(Name = "file")] IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            var freightFees = stream
                .Query<FreightFeeExcelDto>()
                .Select(it => it.Adapt<FreightFee>())
                .ToList();
            var result = freightFeeService.ImportFreightFees(freightFees);

            return SUCCESS(result);
        }
    }
}
