using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;

namespace GD.Service.Interface.WarehouseManagement
{
    public interface ICompanyService : IBaseService<Company>
    {
        PagedInfo<Company> GetAllCompany(CompanyQueryDto companyQueryDto);

        List<Company> GetAllCompany();

        Company GetCompanyById(long companyId);

        long AddCompany(Company company, string userName);

        long EditCompany(Company company, string userName);

        long DeleteByCompanyId(long companyId);
    }
}
