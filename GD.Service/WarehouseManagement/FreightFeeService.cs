using GD.Infrastructure.App;
using GD.Infrastructure.Attribute;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;
using GD.Repository;
using GD.Service.Interface.WarehouseManagement;
using Microsoft.AspNetCore.Hosting;
using SqlSugar;

namespace GD.Service.WarehouseManagement
{
    [AppService(ServiceType = typeof(IFreightFeeService), ServiceLifetime = LifeTime.Transient)]
    public class FreightFeeService : BaseService<FreightFee>, IFreightFeeService
    {
        public long AddFreightFee(FreightFee freightFee, string userName)
        {
            freightFee.Create_by = userName;
            freightFee.Create_time = DateTime.Now;

            return Insert(freightFee);
        }

        public long DeleteFreightFee(long freightFeeId)
        {
            return Delete(freightFeeId);
        }

        public (string, string) DownloadImportTemplate()
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = "运费信息.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            return (sFileName, fullPath);
        }

        public long EidtFreightFee(FreightFee freightFee, string userName)
        {
            freightFee.Create_by = userName;
            freightFee.Create_time = DateTime.Now;

            return Update(freightFee);
        }

        public PagedInfo<FreightFee> GetAllFreightFees(FreightFeeQueryDto freightFeeQueryDto)
        {
            var expression = Expressionable.Create<FreightFee>()
                .AndIF(!string.IsNullOrEmpty(freightFeeQueryDto.Carrier), freightFee => freightFee.Carrier.Contains(freightFeeQueryDto.Carrier))
                .AndIF(!string.IsNullOrEmpty(freightFeeQueryDto.DepartureCity), freightFee => freightFee.DepartureCity.Contains(freightFeeQueryDto.DepartureCity))
                .AndIF(!string.IsNullOrEmpty(freightFeeQueryDto.ArrivalCity), freightFee => freightFee.ArrivalCity.Contains(freightFeeQueryDto.ArrivalCity))
                .AndIF(freightFeeQueryDto.BeginTime != DateTime.MinValue && freightFeeQueryDto.BeginTime != null, exp => exp.Create_time >= freightFeeQueryDto.BeginTime)
                .AndIF(freightFeeQueryDto.EndTime != DateTime.MaxValue && freightFeeQueryDto.EndTime != null, exp => exp.Create_time <= freightFeeQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(freightFeeQueryDto);
        }

        public List<FreightFee> GetAllFreightFees()
        {
            return Queryable()
                .ToList();
        }

        public FreightFee GetFreightFee(long freightFeeId)
        {
            return Queryable()
                .Where(it => it.FreightFeeId == freightFeeId)
                .First();
        }

        public (string, object) ImportFreightFees(List<FreightFee> freightFee)
        {
            var storage = Context.Storageable(freightFee)
                .SplitUpdate(it => it.Any()) //存在更新
                .SplitInsert(it => true) //否则插入（更新优先级大于插入）
                .SplitError(it => string.IsNullOrEmpty(it.Item.Carrier), "承运商不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.DepartureCity), "始发城市不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.ArrivalCity), "到货城市不能为空")
                .WhereColumns(it => it.Carrier) //如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
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
                Console.WriteLine("userName为" + item.Item.Carrier + " : " + item.StorageMessage);
            }

            return (msg, storage.ErrorList);
        }
    }
}
