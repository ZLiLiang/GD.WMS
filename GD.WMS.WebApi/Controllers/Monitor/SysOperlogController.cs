using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.System;
using GD.Model.Enums;
using GD.Model;
using GD.Service.Interface.System;
using GD.WMS.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Monitor
{
    /// <summary>
    /// 操作日志记录
    /// </summary>
    [Verify]
    [Route("/monitor/operlog")]
    [ApiExplorerSettings(GroupName = "sys")]
    public class SysOperlogController : BaseController
    {
        private ISysOperLogService sysOperLogService;

        public SysOperlogController(ISysOperLogService sysOperLogService)
        {
            this.sysOperLogService = sysOperLogService;
        }

        /// <summary>
        /// 查询操作日志
        /// </summary>
        /// <param name="sysOperLog"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult OperList([FromQuery] SysOperLogQueryDto sysOperLog)
        {
            sysOperLog.OperName = !HttpContext.IsAdmin() ? HttpContext.GetName() : sysOperLog.OperName;
            var list = sysOperLogService.SelectOperLogList(sysOperLog);

            return SUCCESS(list);
        }

        /// <summary>
        /// 删除操作日志
        /// </summary>
        /// <param name="operIds"></param>
        /// <returns></returns>
        [Log(Title = "操作日志", BusinessType = BusinessType.DELETE)]
        [HttpDelete("{operIds}")]
        public IActionResult Remove(string operIds)
        {
            if (!HttpContext.IsAdmin())
            {
                return ToResponse(ApiResult.Error("操作失败"));
            }
            long[] operIdss = Tools.SpitLongArrary(operIds);
            return SUCCESS(sysOperLogService.DeleteOperLogByIds(operIdss));
        }

        /// <summary>
        /// 清空操作日志
        /// </summary>
        /// <returns></returns>
        [Log(Title = "清空操作日志", BusinessType = BusinessType.CLEAN)]
        [HttpDelete("clean")]
        public IActionResult ClearOperLog()
        {
            if (!HttpContext.IsAdmin())
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "操作失败");
            }
            sysOperLogService.CleanOperLog();

            return SUCCESS(1);
        }

        /// <summary>
        /// 导出操作日志
        /// </summary>
        /// <returns></returns>
        [Log(Title = "操作日志", BusinessType = BusinessType.EXPORT)]
        [HttpGet("export")]
        public IActionResult Export([FromQuery] SysOperLogQueryDto sysOperLog)
        {
            sysOperLog.PageSize = 100000;
            var list = sysOperLogService.SelectOperLogList(sysOperLog);
            var result = ExcelHelper.ExportExcelMini(list.Result, "操作日志", "操作日志");
            return ExportExcel(result.Item2, result.Item1);
        }

    }
}
