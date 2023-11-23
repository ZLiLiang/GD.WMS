using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Enums;
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
    /// 客户信息
    /// </summary>
    [Verify]
    [Route("/warehousemanagement/customer")]
    [ApiExplorerSettings(GroupName = "wm")]
    public class CustomerController : BaseController
    {
        private ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        /// <summary>
        /// 分页查询所有数据
        /// </summary>
        /// <param name="customerQueryDto"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] CustomerQueryDto customerQueryDto)
        {
            var result = customerService.GetAllCustomers(customerQueryDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 根据id获取客户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get/{id:long}")]
        public IActionResult GetCustomer(long id)
        {
            var result = customerService.GetCustomer(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 新增客户信息
        /// </summary>
        /// <param name="customerDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Log(Title = "新增客户信息", BusinessType = BusinessType.INSERT)]
        public IActionResult AddCustomer([FromBody] CustomerDto customerDto)
        {
            var customer = customerDto.Adapt<Customer>();
            var username = HttpContext.GetName();
            var result = customerService.AddCustomer(customer, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 编辑客户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerDto"></param>
        /// <returns></returns>
        [HttpPost("edit/{id:long}")]
        [Log(Title = "编辑客户信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult EditCustomer(long id, [FromBody] CustomerDto customerDto)
        {
            var customer = customerDto.Adapt<Customer>();
            customer.CustomerId = id;
            var username = HttpContext.GetName();
            var result = customerService.EidtCustomer(customer, username);

            return SUCCESS(result);
        }

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id:long}")]
        [Log(Title = "删除客户信息", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteCustomer(long id)
        {
            var result = customerService.DeleteCustomer(id);

            return SUCCESS(result);
        }

        /// <summary>
        /// 导出客户信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("export")]
        [Log(Title = "导出客户信息", BusinessType = BusinessType.EXPORT)]
        public IActionResult FreightFeeExport()
        {
            var customers = customerService.GetAllCustomers();
            var customerExcelDtos = customers
                .Select(customer => customer.Adapt<CustomerExcelDto>())
                .ToList();
            var result = ExcelHelper.ExportExcelMini(customerExcelDtos, "customer", "客户信息");

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [HttpGet("exportTemplate")]
        [Log(Title = "客户模板", BusinessType = BusinessType.EXPORT, IsSaveRequestData = true, IsSaveResponseData = false)]
        public IActionResult DownloadSupplierTemplate()
        {
            var result = customerService.DownloadImportTemplate();

            return ExportExcel(result.Item2, result.Item1);
        }

        /// <summary>
        /// 从excel导入信息
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("importData")]
        [Log(Title = "客户信息导入", BusinessType = BusinessType.IMPORT, IsSaveRequestData = false, IsSaveResponseData = true)]
        public IActionResult ImportData([FromForm(Name = "file")] IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            var customers = stream
                .Query<CustomerExcelDto>()
                .Select(it => it.Adapt<Customer>())
                .ToList();
            var result = customerService.ImportCustomers(customers);

            return SUCCESS(result);
        }
    }
}
