using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Model.Dto.Operation;
using GD.Model.Enums;
using GD.Model.Vm.Operation;
using GD.Service.Interface.Operation;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Operation
{
    /// <summary>
    /// 仓库移动控制器
    /// </summary>
    [Verify]
    [Route("/operation/adjust")]
    [ApiExplorerSettings(GroupName = "Operation")]
    public class AdjustController : BaseController
    {
        private IAdjustService adjustService;

        public AdjustController(IAdjustService adjustService)
        {
            this.adjustService = adjustService;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="adjustQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] AdjustQueryDto adjustQueryDto)
        {
            var result = adjustService.GetAll(adjustQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出仓库调整
        /// </summary>
        /// <returns></returns>
        [HttpGet("adjustExport")]
        [Log(Title = "导出仓库调整", BusinessType = BusinessType.EXPORT)]
        public IActionResult AdjustExport()
        {
            var adjusts = adjustService.GetAll();
            var adjustExcelDto = adjusts
                .Select(it => it.Adapt<AdjustExcelVm>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(adjustExcelDto, "adjusts", "仓库调整");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
