using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;

namespace GD.Service.Interface.Basic
{
    /// <summary>
    /// 客户服务
    /// </summary>
    public interface ICustomerService : IBaseService<Customer>
    {
        /// <summary>
        /// 分页获取所有客户信息
        /// </summary>
        /// <param name="customerQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Customer> GetAllCustomers(CustomerQueryDto customerQueryDto);

        /// <summary>
        /// 获取所有客户信息
        /// </summary>
        /// <returns></returns>
        List<Customer> GetAllCustomers();

        /// <summary>
        /// 根据id获取客户信息
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Customer GetCustomer(long customerId);

        /// <summary>
        /// 新增客户信息
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddCustomer(Customer customer, string userName);

        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EidtCustomer(Customer customer, string userName);

        /// <summary>
        /// 根据id删除客户信息
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        long DeleteCustomer(long customerId);

        /// <summary>
        /// 导入客户信息
        /// </summary>
        /// <returns></returns>
        (string, object) ImportCustomers(List<Customer> customer);

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        (string, string) DownloadImportTemplate();
    }
}
