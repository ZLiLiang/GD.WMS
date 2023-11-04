﻿using GD.Infrastructure.CustomException;
using GD.Infrastructure.Extensions;
using GD.Model.Constant;
using GD.Model.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web;
using FileAlias = System.IO.File;

namespace GD.WMS.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        public static string TIME_FORMAT_FULL = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 返回成功封装
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeFormatStr"></param>
        /// <returns></returns>
        protected IActionResult SUCCESS(object data, string timeFormatStr = "yyyy-MM-dd HH:mm:ss")
        {
            string jsonStr = GetJsonStr(GetApiResult(data != null ? ResultCode.SUCCESS : ResultCode.NO_DATA, data), timeFormatStr);
            return Content(jsonStr, "application/json");
        }

        /// <summary>
        /// json输出带时间格式的
        /// </summary>
        /// <param name="apiResult"></param>
        /// <returns></returns>
        protected IActionResult ToResponse(ApiResult apiResult)
        {
            string jsonStr = GetJsonStr(apiResult, TIME_FORMAT_FULL);

            return Content(jsonStr, "application/json");
        }

        protected IActionResult ToResponse(long rows, string timeFormatStr = "yyyy-MM-dd HH:mm:ss")
        {
            string jsonStr = GetJsonStr(ToJson(rows), timeFormatStr);

            return Content(jsonStr, "application/json");
        }

        protected IActionResult ToResponse(ResultCode resultCode, string msg = "")
        {
            return ToResponse(new ApiResult((int)resultCode, msg));
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="path">完整文件路径</param>
        /// <param name="fileName">带扩展文件名</param>
        /// <returns></returns>
        protected IActionResult ExportExcel(string path, string fileName)
        {
            //var webHostEnvironment = App.WebHostEnvironment;
            
            if (!FileAlias.Exists(path))
            {
                
                throw new CustomException(fileName + "文件不存在");
            }
            var stream = FileAlias.OpenRead(path);  //创建文件流

            Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            return base.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", HttpUtility.UrlEncode(fileName));
        }

        #region 方法

        /// <summary>
        /// 响应返回结果
        /// </summary>
        /// <param name="rows">受影响行数</param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ApiResult ToJson(long rows, object? data = null)
        {
            return rows > 0 ? ApiResult.Success("success", data) : GetApiResult(ResultCode.FAIL);
        }

        /// <summary>
        /// 全局Code使用
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ApiResult GetApiResult(ResultCode resultCode, object? data = null)
        {
            var msg = resultCode.GetDescription();

            return new ApiResult((int)resultCode, msg, data);
        }
        protected ApiResult Success()
        {
            return GetApiResult(ResultCode.SUCCESS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiResult"></param>
        /// <param name="timeFormatStr"></param>
        /// <returns></returns>
        private static string GetJsonStr(ApiResult apiResult, string timeFormatStr)
        {
            if (string.IsNullOrEmpty(timeFormatStr))
            {
                timeFormatStr = TIME_FORMAT_FULL;
            }
            var serializerSettings = new JsonSerializerSettings
            {
                // 设置为驼峰命名
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = timeFormatStr
            };

            return JsonConvert.SerializeObject(apiResult, Formatting.Indented, serializerSettings);
        }
        #endregion
    }
}
