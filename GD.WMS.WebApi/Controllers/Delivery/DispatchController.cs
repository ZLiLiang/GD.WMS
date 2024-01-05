using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.Delivery;
using GD.Model.Enums;
using GD.Service.Interface.Delivery;
using GD.WMS.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Delivery
{
    /// <summary>
    /// 发货控制器
    /// </summary>
    //[Verify]
    [Route("/inventory/dispatch")]
    //[ApiExplorerSettings(GroupName = "delivery")]
    public class DispatchController : BaseController
    {
        private readonly IDispatchService dispatchService;

        public DispatchController(IDispatchService dispatchService)
        {
            this.dispatchService = dispatchService;
        }

        [HttpGet("list")]
        public IActionResult GetAll([FromQuery] DispatchQueryDto dispatchQueryDto)
        {
            var result = dispatchService.GetDispatchAll(dispatchQueryDto);

            return SUCCESS(result);
        }

        [HttpGet("advancedList")]
        public IActionResult GetPreAll([FromQuery] PreDispatchQueryDto preDispatchQueryDto)
        {
            var result = dispatchService.GetPreDispatchAll(preDispatchQueryDto);

            return SUCCESS(result);
        }

        [HttpPost("add")]
        [Log(Title = "新增发货数据", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] List<DispatchAddDto> dispatchAddDtos)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.Add(dispatchAddDtos, username);

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

        [HttpPut("update")]
        [Log(Title = "编辑发货数据", BusinessType = BusinessType.UPDATE)]
        public IActionResult Update([FromBody] List<DispatchUpdateDto> dispatchUpdateDtos)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.Update(dispatchUpdateDtos, username);

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

        [HttpGet("getDispatch/{dispatchNo}")]
        public IActionResult GetDispatchByNo(string dispatchNo)
        {
            var result = dispatchService.GetByDispatchNo(dispatchNo);

            return SUCCESS(result);
        }

        [HttpGet("confirmCheck/{dispatchNo}")]
        public IActionResult ConfirmOrderCheck(string dispatchNo)
        {
            var reuslt = dispatchService.ConfirmOrderCheck(dispatchNo);

            return SUCCESS(reuslt);
        }

        [HttpGet("pickList/{dispatchId:long}")]
        public IActionResult GetPickListByDispatchId(long dispatchId)
        {
            var reuslt = dispatchService.GetPickListByDispatchId(dispatchId);

            return SUCCESS(reuslt);
        }

        [HttpDelete("delete/{dispatchNo}")]
        [Log(Title = "删除发货数据", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(string dispatchNo)
        {
            var result = dispatchService.Delete(dispatchNo);

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

        [HttpPost("confirmOrder")]
        [Log(Title = "确认与创建发货数据", BusinessType = BusinessType.UPDATE)]
        public IActionResult ConfirmOrder(List<DispatchConfirmDetailDto> dispatchConfirmDetailDtos)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.ConfirmOrder(dispatchConfirmDetailDtos, username);

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

        [HttpPut("confirmPick/{dispatchNo}")]
        [Log(Title = "确认拣货数据", BusinessType = BusinessType.UPDATE, IsSaveRequestData = true)]
        public IActionResult ConfirmPickByDispatchNo(string dispatchNo)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.ConfirmPickByDispatchNo(dispatchNo, username);

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

        [HttpPost("package")]
        [Log(Title = "进行打包操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult Package([FromBody] List<DispatchPackageDto> dispatchPackageDtos)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.Package(dispatchPackageDtos, username);

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

        [HttpPost("weight")]
        [Log(Title = "进行称重操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult Weight([FromBody] List<DispatchWeightDto> dispatchWeightDtos)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.Weight(dispatchWeightDtos, username);

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

        [HttpPost("delivery")]
        [Log(Title = "进行派送操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult Delivery([FromBody] List<DispatchDeliveryDto> dispatchDeliveryDtos)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.Delivery(dispatchDeliveryDtos, username);

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

        [HttpPost("freightfee")]
        [Log(Title = "设置运费操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult SetFreightfee([FromBody] List<DispatchFreightfeeDto> dispatchFreightfeeDtos)
        {
            var result = dispatchService.SetFreightfee(dispatchFreightfeeDtos);

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

        [HttpPost("sign")]
        [Log(Title = "进行签收操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult SignForArrival([FromBody] List<DispatchSignDto> dispatchSignDtos)
        {
            var result = dispatchService.SignForArrival(dispatchSignDtos);

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

        [HttpPost("cancelOrder")]
        [Log(Title = "取消确认操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult CancelOrderOpration([FromBody] CancelOrderOprationDto cancelOrderOprationDto)
        {
            var username = HttpContext.GetName();
            var result = dispatchService.CancelOrderOpration(cancelOrderOprationDto, username);

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

        [HttpPut("cancelOrder/{id:long}")]
        [Log(Title = "取消确认操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult CancelDispatchlistDetailOpration(long id)
        {
            var result = dispatchService.CancelDispatchDetailOpration(id);

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
    }
}
