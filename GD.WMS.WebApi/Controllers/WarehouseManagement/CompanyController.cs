using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Enums;
using GD.Model.WarehouseManagement;
using GD.Service.Interface.WarehouseManagement;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.WarehouseManagement
{
    /// <summary>
    /// 公司信息
    /// </summary>
    [Verify]
    [Route("/warehousemanagement/company")]
    [ApiExplorerSettings(GroupName = "wm")]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService companyService;
        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] CompanyQueryDto companyQueryDto)
        {
            var result = companyService.GetAllCompany(companyQueryDto);

            return SUCCESS(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetCompanyInfo(long id)
        {
            var result = companyService.GetCompanyById(id);

            return SUCCESS(result);
        }

        [HttpPost("add")]
        [Log(Title = "公司信息", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromQuery] CompanyDto companyDto)
        {
            var company = companyDto.Adapt<Company>();
            var userName = HttpContext.GetName();
            var result = companyService.AddCompany(company, userName);

            return SUCCESS(result);
        }

        [HttpPost("edit/{id}")]
        [Log(Title = "公司信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit(int id,[FromQuery] CompanyDto companyDto)
        {
            var company = companyDto.Adapt<Company>();
            company.CompanyId = id;
            var userName = HttpContext.GetName();
            var result = companyService.EditCompany(company, userName);

            return SUCCESS(result);
        }

        [HttpDelete("{id}")]
        [Log(Title = "公司信息", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            var result = companyService.DeleteByCompanyId(id);

            return SUCCESS(result);
        }

        [HttpGet("export")]
        [Log(Title = "公司导出", BusinessType = BusinessType.EXPORT)]
        public IActionResult CompanyExport()
        {
            var companys = companyService.GetAllCompany();
            var companyExcelDtos = companys.Select(company => company.Adapt<CompanyExcelDto>()).ToList();
            var result = ExcelHelper.ExportExcelMini(companyExcelDtos, "company", "公司信息");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
