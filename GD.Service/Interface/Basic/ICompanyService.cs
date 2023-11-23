using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;

namespace GD.Service.Interface.Basic
{
    public interface ICompanyService : IBaseService<Company>
    {
        /// <summary>
        /// 分页获取所有公司信息
        /// </summary>
        /// <returns></returns>
        PagedInfo<Company> GetAllCompany(CompanyQueryDto companyQueryDto);

        /// <summary>
        /// 获取所有公司信息
        /// </summary>
        /// <returns></returns>
        List<Company> GetAllCompany();

        /// <summary>
        /// 根据ID获取公司信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Company GetCompanyById(long companyId);

        /// <summary>
        /// 新增公司信息
        /// </summary>
        /// <param name="company"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddCompany(Company company, string userName);

        /// <summary>
        /// 编辑公司信息
        /// </summary>
        /// <param name="company"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EditCompany(Company company, string userName);

        /// <summary>
        /// 根据主键删除公司信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        long DeleteByCompanyId(long companyId);
    }
}
