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
    [Route("/operation/move")]
    [ApiExplorerSettings(GroupName = "Operation")]
    public class MoveController : BaseController
    {
        private IMoveService moveService;

        public MoveController(IMoveService moveService)
        {
            this.moveService = moveService;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="moveQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] MoveQueryDto moveQueryDto)
        {
            var result = moveService.GetAll(moveQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取仓库移动数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult Get(long id)
        {
            var result = moveService.Get(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增仓库移动
        /// </summary>
        /// <param name="processDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增仓库移动", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] MoveDto moveDto)
        {
            var username = HttpContext.GetName();
            var result = moveService.Add(moveDto, username);

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
        /// 确定仓库移动
        /// </summary>
        /// <param name="processDto"></param>
        /// <returns></returns>
        [HttpPut("confirm/{id:long}")]
        [Log(Title = "确定仓库移动", BusinessType = BusinessType.UPDATE)]
        public IActionResult Confirm(long id)
        {
            var username = HttpContext.GetName();
            var result = moveService.Confirm(id, username);

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
        /// 删除仓库移动
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除仓库移动", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = moveService.Delete(id);

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
        /// 导出仓库移动
        /// </summary>
        /// <returns></returns>
        [HttpGet("moveExport")]
        [Log(Title = "导出仓库移动", BusinessType = BusinessType.EXPORT)]
        public IActionResult ProcessExport()
        {
            var moves = moveService.GetAll();
            var moveExcelDto = moves
                .Select(it => it.Adapt<MoveExcelVm>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(moveExcelDto, "moves", "仓库移动");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
