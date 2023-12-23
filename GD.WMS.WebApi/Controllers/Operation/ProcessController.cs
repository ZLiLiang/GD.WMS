using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.Operation;
using GD.Model.Enums;
using GD.Service.Interface.Operation;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Operation
{
    /// <summary>
    /// 仓库加工控制器
    /// </summary>
    [Verify]
    [Route("/operation/process")]
    [ApiExplorerSettings(GroupName = "Operation")]
    public class ProcessController : BaseController
    {
        private IProcessService processService;

        public ProcessController(IProcessService processService)
        {
            this.processService = processService;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="processQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] ProcessQueryDto processQueryDto)
        {
            var result = processService.GetAll(processQueryDto);

            
            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取仓库加工数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult Get(long id)
        {
            var result = processService.Get(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增仓库加工
        /// </summary>
        /// <param name="processDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增仓库加工", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] ProcessDto processDto)
        {
            var username = HttpContext.GetName();
            var result = processService.Add(processDto, username);

            if (result > 0)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, false);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 编辑仓库加工
        /// </summary>
        /// <param name="processDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑仓库加工", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit([FromBody] ProcessDto processDto)
        {
            var username = HttpContext.GetName();
            var result = processService.Edit(processDto, username);

            if (result > 0)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, false);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 删除仓库加工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除仓库加工", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = processService.Delete(id);

            if (result > 0)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, false);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 确认加工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("confirmProcess/{id:long}")]
        [Log(Title = "确认加工", BusinessType = BusinessType.UPDATE)]
        public IActionResult ConfirmProcess(long id)
        {
            var username = HttpContext.GetName();
            var result = processService.ConfirmProcess(id, username);

            if (result > 0)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, false);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 确认调整
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("confirmAdjustment/{id:long}")]
        [Log(Title = "确认调整", BusinessType = BusinessType.UPDATE)]
        public IActionResult ConfirmAdjustment(long id)
        {
            var username = HttpContext.GetName();
            var result = processService.ConfirmAdjustment(id, username);

            if (result > 0)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, false);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 导出仓库加工
        /// </summary>
        /// <returns></returns>
        [HttpGet("processExport")]
        [Log(Title = "导出仓库加工", BusinessType = BusinessType.EXPORT)]
        public IActionResult ProcessExport()
        {
            var process = processService.GetAll();
            var processExcelDto = process
                .Select(it => it.Adapt<ProcessExcelDto>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(processExcelDto, "process", "仓库加工");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
