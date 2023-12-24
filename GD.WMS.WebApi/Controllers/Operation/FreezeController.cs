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
    /// 仓库移动控制器
    /// </summary>
    [Verify]
    [Route("/operation/freeze")]
    [ApiExplorerSettings(GroupName = "Operation")]
    public class FreezeController : BaseController
    {
        private IFreezeService freezeService;

        public FreezeController(IFreezeService freezeService)
        {
            this.freezeService = freezeService;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="freezeQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] FreezeQueryDto freezeQueryDto)
        {
            var result = freezeService.GetAll(freezeQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取仓库冻结数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult Get(long id)
        {
            var result = freezeService.Get(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增仓库冻结
        /// </summary>
        /// <param name="freezeDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增仓库冻结", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] FreezeDto freezeDto)
        {
            var username = HttpContext.GetName();
            var result = freezeService.Add(freezeDto, username);

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
        /// 编辑仓库冻结
        /// </summary>
        /// <param name="freezeDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑仓库冻结", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit([FromBody] FreezeDto freezeDto)
        {
            var username = HttpContext.GetName();
            var result = freezeService.Update(freezeDto, username);

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
        /// 删除仓库冻结
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除仓库冻结", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = freezeService.Delete(id);

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
        /// 导出仓库冻结
        /// </summary>
        /// <returns></returns>
        [HttpGet("freezeExport")]
        [Log(Title = "导出仓库冻结", BusinessType = BusinessType.EXPORT)]
        public IActionResult ProcessExport()
        {
            var freezes = freezeService.GetAll();
            var freezeExcelDto = freezes
                .Select(it => it.Adapt<FreezeExcelVm>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(freezeExcelDto, "freezes", "仓库冻结");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
