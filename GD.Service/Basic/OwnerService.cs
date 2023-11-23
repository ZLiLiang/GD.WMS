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
    [AppService(ServiceType = typeof(IOwnerService), ServiceLifetime = LifeTime.Transient)]
    public class OwnerService : BaseService<Owner>, IOwnerService
    {
        public int AddOwner(Owner owner, string userName)
        {
            owner.Create_by = userName;
            owner.Create_time = DateTime.Now;

            return Insert(owner);
        }

        public int DeleteOwner(long ownerId)
        {
            return Delete(ownerId);
        }

        public (string, string) DownloadImportTemplate()
        {
            IWebHostEnvironment webHostEnvironment = (IWebHostEnvironment)App.ServiceProvider.GetService(typeof(IWebHostEnvironment));
            string sFileName = "货主信息.xlsx";
            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "ImportTemplate", sFileName);

            return (sFileName, fullPath);
        }

        public int EditOwner(Owner owner, string userName)
        {
            owner.Update_by = userName;
            owner.Update_time = DateTime.Now;

            return Update(owner);
        }

        public PagedInfo<Owner> GetAllOwners(OwnerQueryDto ownerQueryDto)
        {
            var expression = Expressionable.Create<Owner>()
                .AndIF(!string.IsNullOrEmpty(ownerQueryDto.OwnerName), owner => owner.OwnerName.Contains(ownerQueryDto.OwnerName))
                .AndIF(!string.IsNullOrEmpty(ownerQueryDto.ContactTel), owner => owner.ContactTel.Contains(ownerQueryDto.ContactTel))
                .AndIF(ownerQueryDto.BeginTime != DateTime.MinValue && ownerQueryDto.BeginTime != null, exp => exp.Create_time >= ownerQueryDto.BeginTime)
                .AndIF(ownerQueryDto.EndTime != DateTime.MaxValue && ownerQueryDto.EndTime != null, exp => exp.Create_time <= ownerQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(ownerQueryDto);
        }

        public List<Owner> GetAllOwners()
        {
            return Queryable()
                .ToList();
        }

        public Owner GetOwner(long ownerId)
        {
            return Queryable()
                .First(it => it.OwnerId == ownerId);
        }

        public (string, object) ImportOwners(List<Owner> owner)
        {
            var storage = Context.Storageable(owner)
                .SplitUpdate(it => it.Any()) //存在更新
                .SplitInsert(it => true) //否则插入（更新优先级大于插入）
                .SplitError(it => string.IsNullOrEmpty(it.Item.OwnerName), "货主名称不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.Manager), "负责人不能为空")
                .SplitError(it => string.IsNullOrEmpty(it.Item.ContactTel), "联系方式不能为空")
                .WhereColumns(it => it.OwnerName) //如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
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
                Console.WriteLine("userName为" + item.Item.OwnerName + " : " + item.StorageMessage);
            }

            return (msg, storage.ErrorList);
        }
    }
}
