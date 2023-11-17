using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Enums;
using GD.Model.System;
using GD.Model.WarehouseManagement;
using GD.Service.Interface.WarehouseManagement;
using GD.Service.WarehouseManagement;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace GD.WMS.WebApi.Controllers.WarehouseManagement
{
    /// <summary>
    /// 供应商信息
    /// </summary>
    [Verify]
    [Route("/warehousemanagement/supplier")]
    [ApiExplorerSettings(GroupName = "wm")]
    public class SupplierController : BaseController
    {
        private ISupplierService supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="supplierQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] SupplierQueryDto supplierQueryDto)
        {
            var result = supplierService.GetAllSupplier(supplierQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据Id查询供应商信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetSupplierInfo(long id)
        {
            var result = supplierService.GetSupplierById(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增供应商信息
        /// </summary>
        /// <param name="supplierDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "供应商信息", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromQuery] SupplierDto supplierDto)
        {
            var supplier = supplierDto.Adapt<Supplier>();
            var userName = HttpContext.GetName();
            var result = supplierService.AddSupplier(supplier, userName);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑供应商信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplierDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id}")]
        [Log(Title = "供应商信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit(long id, [FromQuery] SupplierDto supplierDto)
        {
            var supplier = supplierDto.Adapt<Supplier>();
            supplier.SupplierId = id;
            var userName = HttpContext.GetName();
            var result = supplierService.EditSupplier(supplier, userName);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据Id删除供应商信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Log(Title = "供应商信息", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var condition = supplierService.IsOtherUse(id);
            if (condition)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "该供应商存在商品错误");
            }
            else
            {
                var result = supplierService.DeleteBySupplierId(id);

                return SUCCESS(result);
            }
        }

        /// <summary>
        /// 导出供应商信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "供应商导出", BusinessType = BusinessType.EXPORT)]
        public IActionResult SupplierExport()
        {
            var suppliers = supplierService.GetAllSupplier();
            var supplierExcelDtos = suppliers.Select(supplier => supplier.Adapt<SupplierExcelDto>()).ToList();
            var result = ExcelHelper.ExportExcelMini(supplierExcelDtos, "company", "供应商导信息");

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("exportTemplate")]
        [Log(Title = "供应商模板", BusinessType = BusinessType.EXPORT, IsSaveRequestData = true, IsSaveResponseData = false)]
        public IActionResult DownloadSupplierTemplate()
        {
            var result = supplierService.DownloadImportTemplate();

            return ExportExcel(result.Item2, result.Item1);
        }

        [HttpPost("importData")]
        [Log(Title = "供应商导入", BusinessType = BusinessType.IMPORT, IsSaveRequestData = false, IsSaveResponseData = true)]
        public IActionResult ImportData([FromForm(Name = "file")] IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            //var supplierExcel = stream.Query<SupplierExcelDto>(startCell: "A2").ToList();
            var supplierExcel = stream.Query<SupplierExcelDto>().ToList();
            List<Supplier> supplier = new();
            foreach (var item in supplierExcel)
            {
                supplier.Add(item.Adapt<Supplier>());
            }
            var result = supplierService.ImportSupplers(supplier);

            return SUCCESS(result);
        }
    }
}
