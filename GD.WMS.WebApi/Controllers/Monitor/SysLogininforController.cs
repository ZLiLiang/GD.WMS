using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Infrastructure.Helper;
using GD.Model.Enums;
using GD.Model.System;
using GD.Service.Interface.System;
using GD.WMS.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using GD.Model.Constant;
using GD.Model.Page;

namespace GD.WMS.WebApi.Controllers.Monitor
{
    /// <summary>
    /// 系统访问记录
    /// </summary>
    [Verify]
    [Route("/monitor/logininfor")]
    [ApiExplorerSettings(GroupName = "sys")]
    public class SysLogininforController : BaseController
    {
        private ISysLoginService sysLoginService;

        public SysLogininforController(ISysLoginService sysLoginService)
        {
            this.sysLoginService = sysLoginService;
        }

        /// <summary>
        /// 查询登录日志
        /// </summary>
        /// <param name="sysLogininfoDto"></param>
        /// <param name="pagerInfo"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public IActionResult LoignLogList([FromQuery] SysLogininfor sysLogininfoDto, [FromQuery] PagerInfo pagerInfo)
        {
            var list = sysLoginService.GetLoginLog(sysLogininfoDto, pagerInfo);

            return SUCCESS(list);
        }

        /// <summary>
        /// 清空登录日志
        /// </summary>
        /// <returns></returns>
        [Log(Title = "清空登录日志", BusinessType = BusinessType.CLEAN)]
        [HttpDelete("clean")]
        public IActionResult CleanLoginInfo()
        {
            if (!HttpContextExtension.IsAdmin(HttpContext))
            {
                return ToResponse(ApiResult.Error("操作失败"));
            }
            sysLoginService.TruncateLogininfo();
            return SUCCESS(1);
        }

        /// <summary>
        /// </summary>
        /// <param name="infoIds"></param>
        /// <returns></returns>
        [Log(Title = "删除登录日志", BusinessType = BusinessType.DELETE)]
        [HttpDelete("{infoIds}")]
        public IActionResult Remove(string infoIds)
        {
            if (!HttpContextExtension.IsAdmin(HttpContext))
            {
                return ToResponse(ApiResult.Error("操作失败"));
            }
            long[] infoIdss = Tools.SpitLongArrary(infoIds);
            return SUCCESS(sysLoginService.DeleteLogininforByIds(infoIdss));
        }

        /// <summary>
        /// 登录日志导出
        /// </summary>
        /// <returns></returns>
        [Log(BusinessType = BusinessType.EXPORT, IsSaveResponseData = false, Title = "登录日志导出")]
        [HttpGet("export")]
        public IActionResult Export([FromQuery] SysLogininfor logininfoDto)
        {
            logininfoDto.BeginTime = DateTimeHelper.GetBeginTime(logininfoDto.BeginTime, -1);
            logininfoDto.EndTime = DateTimeHelper.GetBeginTime(logininfoDto.EndTime, 1);
            var exp = Expressionable.Create<SysLogininfor>()
                .And(it => it.LoginTime >= logininfoDto.BeginTime && it.LoginTime <= logininfoDto.EndTime);

            var list = sysLoginService.Queryable().Where(exp.ToExpression())
                .ToList();

            var result = ExcelHelper.ExportExcelMini(list, "loginlog", "登录日志");
            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
