using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;

namespace GD.Service.Interface.WarehouseManagement
{
    /// <summary>
    /// 货主信息服务
    /// </summary>
    public interface IOwnerService : IBaseService<Owner>
    {
        /// <summary>
        /// 分页获取所有货主信息
        /// </summary>
        /// <param name="ownerQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Owner> GetAllOwners(OwnerQueryDto ownerQueryDto);

        /// <summary>
        /// 获取所有货主信息
        /// </summary>
        /// <returns></returns>
        List<Owner> GetAllOwners();

        /// <summary>
        /// 根据id获取货主信息
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        Owner GetOwner(long ownerId);

        /// <summary>
        /// 新增货主信息
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int AddOwner(Owner owner, string userName);

        /// <summary>
        /// 修改货主信息
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int EditOwner(Owner owner, string userName);

        /// <summary>
        /// 根据id删除货主信息
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        int DeleteOwner(long ownerId);

        /// <summary>
        /// 导入供货主信息
        /// </summary>
        /// <returns></returns>
        (string, object) ImportOwners(List<Owner> owner);

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        (string, string) DownloadImportTemplate();
    }
}
