using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
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
    /// 仓库盘点控制器
    /// </summary>
    [Verify]
    [Route("/operation/taking")]
    [ApiExplorerSettings(GroupName = "Operation")]
    public class TakingController : BaseController
    {
        private ITakingService takingService;

        public TakingController(ITakingService takingService)
        {
            this.takingService = takingService;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="takingQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] TakingQueryDto takingQueryDto)
        {
            var result = takingService.GetAll(takingQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取仓库盘点数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult Get(long id)
        {
            var result = takingService.Get(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增仓库盘点
        /// </summary>
        /// <param name="takingDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增仓库盘点", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] TakingDto takingDto)
        {
            var username = HttpContext.GetName();
            var result = takingService.Add(takingDto, username);

            if (result.Item1 == true)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, result.Item2);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 确认盘点作业
        /// </summary>
        /// <param name="takingPutDto"></param>
        /// <returns></returns>
        [HttpPut("put")]
        [Log(Title = "确认盘点作业", BusinessType = BusinessType.UPDATE)]
        public IActionResult Put([FromBody] TakingPutDto takingPutDto)
        {
            var username = HttpContext.GetName();
            var result = takingService.Put(takingPutDto, username);

            if (result.Item1 == true)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, result.Item2);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 确认调整
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("confirm/{id:long}")]
        [Log(Title = "确认调整", BusinessType = BusinessType.UPDATE)]
        public IActionResult Confirm(long id)
        {
            var username = HttpContext.GetName();
            var result = takingService.Confirm(id, username);

            if (result.Item1 == true)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, result.Item2);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 删除仓库盘点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除仓库盘点", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = takingService.Delete(id);

            if (result.Item1 == true)
            {
                return SUCCESS(true);
            }
            else
            {
                var apiResult = GetApiResult(ResultCode.FAIL, result.Item2);
                return ToResponse(apiResult);
            }
        }

        /// <summary>
        /// 导出仓库盘点
        /// </summary>
        /// <returns></returns>
        [HttpGet("takingExport")]
        [Log(Title = "导出仓库盘点", BusinessType = BusinessType.EXPORT)]
        public IActionResult ProcessExport()
        {
            var takings = takingService.GetAll();
            var takingExcelDto = takings
                .Select(it => it.Adapt<TakingExcelVm>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(takingExcelDto, "takings", "仓库盘点");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
