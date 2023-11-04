﻿using GD.Infrastructure.Attribute;
using GD.Infrastructure.CustomException;
using GD.Model.Enums;
using IPTools.Core;
using Microsoft.AspNetCore.Http.Features;
using NLog;
using System.Text.Encodings.Web;
using GD.Service.Interface.System;
using LogLevel = NLog.LogLevel;
using textJson = System.Text.Json;
using GD.Model.System;
using GD.Infrastructure.Extensions;
using GD.Model.Constant;

namespace GD.WMS.WebApi.Middleware
{
    /// <summary>
    /// 全局异常处理中间件
    /// 调用 app.UseMiddlewareGlobalExceptionMiddleware>();
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ISysOperLogService SysOperLogService;

        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public GlobalExceptionMiddleware(RequestDelegate next, ISysOperLogService sysOperLog)
        {
            this.next = next;
            this.SysOperLogService = sysOperLog;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            LogLevel logLevel = LogLevel.Info;
            int code = (int)ResultCode.GLOBAL_ERROR;
            string msg;
            string error = string.Empty;
            //自定义异常
            if (ex is CustomException customException)
            {
                code = customException.Code;
                msg = customException.Message;
                error = customException.LogMsg;
            }
            else if (ex is ArgumentException)//参数异常
            {
                code = (int)ResultCode.PARAM_ERROR;
                msg = ex.Message;
            }
            else
            {
                msg = "服务器好像出了点问题，请联系系统管理员...";
                error = $"{ex.Message}";
                logLevel = LogLevel.Error;
                context.Response.StatusCode = 500;
            }
            var options = new textJson.JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = textJson.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            ApiResult apiResult = new(code, msg);
            string responseResult = textJson.JsonSerializer.Serialize(apiResult, options);
            string ip = HttpContextExtension.GetClientUserIp(context);
            var ip_info = IpTool.Search(ip);

            SysOperLog sysOperLog = new()
            {
                Status = 1,
                OperIp = ip,
                OperUrl = HttpContextExtension.GetRequestUrl(context),
                RequestMethod = context.Request.Method,
                JsonResult = responseResult,
                ErrorMsg = string.IsNullOrEmpty(error) ? msg : error,
                OperName = HttpContextExtension.GetName(context),
                OperLocation = ip_info.Province + " " + ip_info.City,
                OperTime = DateTime.Now,
                OperParam = HttpContextExtension.GetRequestValue(context, context.Request.Method)
            };
            var endpoint = GetEndpoint(context);
            if (endpoint != null)
            {
                var logAttribute = endpoint.Metadata.GetMetadata<LogAttribute>();
                if (logAttribute != null)
                {
                    sysOperLog.BusinessType = (int)logAttribute.BusinessType;
                    sysOperLog.Title = logAttribute?.Title;
                    sysOperLog.OperParam = logAttribute.IsSaveRequestData ? sysOperLog.OperParam : "";
                    sysOperLog.JsonResult = logAttribute.IsSaveResponseData ? sysOperLog.JsonResult : "";
                }
            }
            LogEventInfo ei = new(logLevel, "GlobalExceptionMiddleware", error)
            {
                Exception = ex,
                Message = error
            };
            ei.Properties["status"] = 1;//走正常返回都是通过走GlobalExceptionFilter不通过
            ei.Properties["jsonResult"] = responseResult;
            ei.Properties["requestParam"] = sysOperLog.OperParam;
            ei.Properties["user"] = sysOperLog.OperName;

            Logger.Log(ei);
            context.Response.ContentType = "text/json;charset=utf-8";
            await context.Response.WriteAsync(responseResult, System.Text.Encoding.UTF8);

            SysOperLogService.InsertOperlog(sysOperLog);

        }

        public static Endpoint GetEndpoint(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Features.Get<IEndpointFeature>()?.Endpoint;
        }
    }
}
