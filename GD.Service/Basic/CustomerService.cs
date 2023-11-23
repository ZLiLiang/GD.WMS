using GD.Infrastructure.App;
using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;
using GD.Repository;
using GD.Service.Interface.Basic;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;

namespace GD.Service.Basic
{
    [AppService(ServiceType = typeof(ICustomerService), ServiceLifetime = LifeTime.Transient)]
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        public long AddCustomer(Customer customer, string userName)
        {
            customer.Create_by = userName;
            customer.Create_time = DateTime.Now;

            return Insert(customer);
        }

        public long DeleteCustomer(long customerId)
        {
            return Delete(customerId);
        }

        public (string, string) DownloadImportTemplate()
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = "客户信息.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            return (sFileName, fullPath);
        }

        public long EidtCustomer(Customer customer, string userName)
        {
            customer.Create_by = userName;
            customer.Create_time = DateTime.Now;

            return Update(customer);
        }

        public PagedInfo<Customer> GetAllCustomers(CustomerQueryDto customerQueryDto)
        {
            var expression = Expressionable.Create<Customer>()
                .AndIF(!string.IsNullOrEmpty(customerQueryDto.CustomerName), customer => customer.CustomerName.Contains(customerQueryDto.CustomerName))
                .AndIF(customerQueryDto.BeginTime != DateTime.MinValue && customerQueryDto.BeginTime != null, exp => exp.Create_time >= customerQueryDto.BeginTime)
                .AndIF(customerQueryDto.EndTime != DateTime.MaxValue && customerQueryDto.EndTime != null, exp => exp.Create_time <= customerQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(customerQueryDto);
        }

        public List<Customer> GetAllCustomers()
        {
            return Queryable()
                .ToList();
        }

        public Customer GetCustomer(long customerId)
        {
            return Queryable()
                .Where(it => it.CustomerId == customerId)
                .First();
        }

        public (string, object) ImportCustomers(List<Customer> customer)
        {
            var storage = Context.Storageable(customer)
                .SplitUpdate(it => it.Any()) //存在更新
                .SplitInsert(it => true) //否则插入（更新优先级大于插入）
                .SplitError(it => string.IsNullOrEmpty(it.Item.CustomerName), "客户名称不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.City), "所在城市不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.Address), "详细地址不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.Manager), "负责人不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.ContactTel), "联系方式不能为空")
                .WhereColumns(it => it.CustomerName) //如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
                .ToStorage();

            storage.AsInsertable.ExecuteCommand(); //执行插入
            storage.AsUpdateable.ExecuteCommand(); //执行更新
            //storage.AsDeleteable.ExecuteCommand(); //执行删除　

            string msg = string.Format(" 插入{0} 更新{1} 错误{2} 不计算{3} 删除{4} 总共{5}",
                               storage.InsertList.Count,
                               storage.UpdateList.Count,
                               storage.ErrorList.Count,
                               storage.IgnoreList.Count,
                               storage.DeleteList.Count,
                               storage.TotalList.Count);
            //输出统计                      
            Console.WriteLine(msg);

            //输出错误信息               
            foreach (var item in storage.ErrorList)
            {
                Console.WriteLine("userName为" + item.Item.CustomerName + " : " + item.StorageMessage);
            }

            return (msg, storage.ErrorList);
        }
    }
}
