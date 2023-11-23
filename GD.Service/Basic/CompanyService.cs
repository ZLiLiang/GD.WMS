using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;
using GD.Repository;
using GD.Service.Interface.Basic;
using SqlSugar;

namespace GD.Service.Basic
{
    /// <summary>
    /// 公司信息
    /// </summary>
    [AppService(ServiceType = typeof(ICompanyService), ServiceLifetime = LifeTime.Transient)]
    public class CompanyService : BaseService<Company>, ICompanyService
    {
        /// <summary>
        /// 新增公司信息
        /// </summary>
        /// <param name="company"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public long AddCompany(Company company, string userName)
        {
            company.Create_time = DateTime.Now;
            company.Create_by = userName;
            return InsertReturnBigIdentity(company);
        }

        /// <summary>
        /// 根据主键删除公司信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public long DeleteByCompanyId(long companyId)
        {
            return Delete(companyId);
        }

        /// <summary>
        /// 编辑公司信息
        /// </summary>
        /// <param name="company"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public long EditCompany(Company company, string userName)
        {
            company.Update_time = DateTime.Now;
            company.Create_by = userName;
            return Update(company);
        }

        /// <summary>
        /// 分页获取所有公司信息
        /// </summary>
        /// <returns></returns>
        public PagedInfo<Company> GetAllCompany(CompanyQueryDto companyQueryDto)
        {
            var expression = Expressionable.Create<Company>()
                .AndIF(!string.IsNullOrEmpty(companyQueryDto.CompanyName), exp => exp.CompanyName.Contains(companyQueryDto.CompanyName))
                .AndIF(!string.IsNullOrEmpty(companyQueryDto.Manager), exp => exp.Manager.Contains(companyQueryDto.Manager))
                .AndIF(!string.IsNullOrEmpty(companyQueryDto.ContactTel), exp => exp.ContactTel.Contains(companyQueryDto.ContactTel))
                .AndIF(companyQueryDto.BeginTime != DateTime.MinValue && companyQueryDto.BeginTime != null, exp => exp.Create_time >= companyQueryDto.BeginTime)
                .AndIF(companyQueryDto.EndTime != DateTime.MaxValue && companyQueryDto.EndTime != null, exp => exp.Create_time <= companyQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(companyQueryDto);
        }

        /// <summary>
        /// 获取所有公司信息
        /// </summary>
        /// <returns></returns>
        public List<Company> GetAllCompany()
        {
            return Queryable()
                .ToList();
        }
        /// <summary>
        /// 根据ID获取公司信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Company GetCompanyById(long companyId)
        {
            return Queryable()
                .Where(company => company.CompanyId == companyId)
                .Single();
        }
    }
}
