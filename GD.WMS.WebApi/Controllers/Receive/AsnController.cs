using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.Receive;
using GD.Model.Enums;
using GD.Model.Receive;
using GD.Service.Interface.Basic;
using GD.Service.Interface.Receive;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;

namespace GD.WMS.WebApi.Controllers.Receive
{
    /// <summary>
    /// asn控制器
    /// </summary>
    [Verify]
    [Route("/receive/asn")]
    [ApiExplorerSettings(GroupName = "receive")]
    public class AsnController : BaseController
    {
        private IAsnService asnService;
        private ISupplierService supplierService;
        private IOwnerService ownerService;

        public AsnController(IAsnService asnService, ISupplierService supplierService, IOwnerService ownerService)
        {
            this.asnService = asnService;
            this.supplierService = supplierService;
            this.ownerService = ownerService;
        }


        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="asnQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] AsnQueryDto asnQueryDto)
        {
            var result = asnService.GetAll(asnQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取到货通知
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult Get(long id)
        {
            var result = asnService.Get(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取供应商列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/supplierOptions")]
        public IActionResult GetSupplierOptions()
        {
            var result = supplierService
                .GetAll()
                .Select(it => new { it.SupplierId, it.SupplierName })
                .ToList();

            return SUCCESS(result);
        }

        /// <summary>
        /// 获取货主列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/ownerOptions")]
        public IActionResult GetOwnerOptions()
        {
            var result = ownerService
                .GetAllOwners()
                .Select(it => new { it.OwnerId, it.OwnerName })
                .ToList();

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增到货通知
        /// </summary>
        /// <param name="asnDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增到货通知", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromBody] AsnDto asnDto)
        {
            var asn = asnDto.Adapt<Asn>();
            var username = HttpContext.GetName();
            var result = asnService.Add(asn, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑asn信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="asnDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑asn信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit(long id, [FromBody] AsnDto asnDto)
        {
            var asn = asnDto.Adapt<Asn>();
            asn.AsnId = id;
            var username = HttpContext.GetName();
            var result = asnService.Edit(asn, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除到货通知
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除到货通知", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = asnService.Delete(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 确认到货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("confirmArrive/{id:long}")]
        [Log(Title = "确认到货", BusinessType = BusinessType.UPDATE)]
        public IActionResult ConfirmArrive(long id)
        {
            var username = HttpContext.GetName();
            var result = asnService.Operate(id, AsnStatus.Unload, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 进行卸货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("unload/{id:long}")]
        [Log(Title = "进行卸货", BusinessType = BusinessType.UPDATE)]
        public IActionResult Unload(long id)
        {
            var username = HttpContext.GetName();
            var result = asnService.Operate(id, AsnStatus.Sort, username);

            return SUCCESS(result);
        }


        /// <summary>
        /// 取消确认到货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("cancelArrive/{id:long}")]
        [Log(Title = "取消到货", BusinessType = BusinessType.UPDATE)]
        public IActionResult CancelArrive(long id)
        {
            var username = HttpContext.GetName();
            var result = asnService.Operate(id, AsnStatus.Arrive, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 进行分拣
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("sorted/{id:long}")]
        [Log(Title = "进行分拣", BusinessType = BusinessType.UPDATE)]
        public IActionResult Sorted(long id)
        {
            var sortQty = asnService.Get(id).SortedQty;
            if (sortQty == 0)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "分拣数量不能为0");
            }

            var username = HttpContext.GetName();
            var result = asnService.Operate(id, AsnStatus.Actual, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 取消卸货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("cancelUnload/{id:long}")]
        [Log(Title = "取消卸货", BusinessType = BusinessType.UPDATE)]
        public IActionResult CancelUnload(long id)
        {
            var username = HttpContext.GetName();
            var result = asnService.Operate(id, AsnStatus.Unload, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 取消分拣
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("cancelSort/{id:long}")]
        [Log(Title = "取消分拣", BusinessType = BusinessType.UPDATE)]
        public IActionResult CancelSort(long id)
        {
            var username = HttpContext.GetName();
            var result = asnService.Operate(id, AsnStatus.Sort, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 上架操作
        /// </summary>
        /// <returns></returns>
        [HttpPut("putAway")]
        [Log(Title = "上架操作", BusinessType = BusinessType.UPDATE)]
        public IActionResult PutAway([FromBody] AsnPutAwayDto putAwayDto)
        {
            var username = HttpContext.GetName();
            var result = asnService.PutAway(putAwayDto, username);

            if (result == true)
            {
                return SUCCESS(result);
            }
            else
            {
                return ToResponse(ResultCode.FAIL,"上架数量超出分拣数量");
            }
        }

        /// <summary>
        /// 导出到货通知
        /// </summary>
        /// <returns></returns>
        [HttpGet("noticeExport")]
        [Log(Title = "导出到货通知", BusinessType = BusinessType.EXPORT)]
        public IActionResult NoticeExport()
        {
            var asns = asnService.GetAll();
            var asnExcelDtos = asns
                .Select(asn => asn.Adapt<AsnExcelDto>())
                .ToList();
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = new DynamicExcelColumn[]
                {
                    new DynamicExcelColumn("ActualQty"){Ignore=true},
                    new DynamicExcelColumn("SortedQty"){Ignore=true},
                    new DynamicExcelColumn("ShortageQty"){Ignore=true},
                    new DynamicExcelColumn("MoreQty"){Ignore=true},
                    new DynamicExcelColumn("DamageQty"){Ignore=true}
                }
            };
            var result = ExcelHelper.ExportExcelMini(asnExcelDtos, "asn", "到货通知", config);

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出待到货
        /// </summary>
        /// <returns></returns>
        [HttpGet("arriveExport")]
        [Log(Title = "导出待到货", BusinessType = BusinessType.EXPORT)]
        public IActionResult ArriveExport()
        {
            var arrives = asnService.GetAll(AsnStatus.Arrive);
            var arriveExcelDtos = arrives
                .Select(arrive => arrive.Adapt<AsnExcelDto>())
                .ToList();
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = new DynamicExcelColumn[]
                {
                    new DynamicExcelColumn("ActualQty"){Ignore=true},
                    new DynamicExcelColumn("SortedQty"){Ignore=true},
                    new DynamicExcelColumn("ShortageQty"){Ignore=true},
                    new DynamicExcelColumn("MoreQty"){Ignore=true},
                    new DynamicExcelColumn("DamageQty"){Ignore=true}
                }
            };
            var result = ExcelHelper.ExportExcelMini(arriveExcelDtos, "arrive", "待到货", config);

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出待卸货
        /// </summary>
        /// <returns></returns>
        [HttpGet("unloadExport")]
        [Log(Title = "导出待卸货", BusinessType = BusinessType.EXPORT)]
        public IActionResult UnloadExport()
        {
            var unloads = asnService.GetAll(AsnStatus.Unload);
            var unloadExcelDtos = unloads
                .Select(unload => unload.Adapt<AsnExcelDto>())
                .ToList();
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = new DynamicExcelColumn[]
                {
                    new DynamicExcelColumn("ActualQty"){Ignore=true},
                    new DynamicExcelColumn("SortedQty"){Ignore=true},
                    new DynamicExcelColumn("ShortageQty"){Ignore=true},
                    new DynamicExcelColumn("MoreQty"){Ignore=true},
                    new DynamicExcelColumn("DamageQty"){Ignore=true}
                }
            };
            var result = ExcelHelper.ExportExcelMini(unloadExcelDtos, "unload", "待卸货", config);

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出待分拣
        /// </summary>
        /// <returns></returns>
        [HttpGet("sortExport")]
        [Log(Title = "导出待分拣", BusinessType = BusinessType.EXPORT)]
        public IActionResult SortExport()
        {
            var sorts = asnService.GetAll(AsnStatus.Sort);
            var sortExcelDtos = sorts
                .Select(sort => sort.Adapt<AsnExcelDto>())
                .ToList();
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = new DynamicExcelColumn[]
                {
                    new DynamicExcelColumn("ActualQty"){Ignore=true},
                    //new DynamicExcelColumn("SortedQty"){Ignore=true},
                    new DynamicExcelColumn("ShortageQty"){Ignore=true},
                    new DynamicExcelColumn("MoreQty"){Ignore=true},
                    new DynamicExcelColumn("DamageQty"){Ignore=true}
                }
            };
            var result = ExcelHelper.ExportExcelMini(sortExcelDtos, "sort", "待分拣", config);

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出待上架
        /// </summary>
        /// <returns></returns>
        [HttpGet("actualExport")]
        [Log(Title = "导出待上架", BusinessType = BusinessType.EXPORT)]
        public IActionResult ActualExport()
        {
            var actuals = asnService.GetAll(AsnStatus.Actual);
            var actualExcelDtos = actuals
                .Select(actual => actual.Adapt<AsnExcelDto>())
                .ToList();
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = new DynamicExcelColumn[]
                {
                    //new DynamicExcelColumn("ActualQty"){Ignore=true},
                    //new DynamicExcelColumn("SortedQty"){Ignore=true},
                    new DynamicExcelColumn("ShortageQty"){Ignore=true},
                    new DynamicExcelColumn("MoreQty"){Ignore=true},
                    new DynamicExcelColumn("DamageQty"){Ignore=true}
                }
            };
            var result = ExcelHelper.ExportExcelMini(actualExcelDtos, "actual", "待上架", config);

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出收货细明
        /// </summary>
        /// <returns></returns>
        [HttpGet("detailExport")]
        [Log(Title = "导出收货细明", BusinessType = BusinessType.EXPORT)]
        public IActionResult DetailExport()
        {
            var details = asnService.GetAll();
            var detailExcelDtos = details
                .Select(detail => detail.Adapt<AsnExcelDto>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(detailExcelDtos, "detail", "收货细明");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
