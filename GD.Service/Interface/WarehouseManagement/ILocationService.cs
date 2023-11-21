using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;

namespace GD.Service.Interface.WarehouseManagement
{
    /// <summary>
    /// 库位服务
    /// </summary>
    public interface ILocationService : IBaseService<Location>
    {
        /// <summary>
        /// 分页获取所有库位信息
        /// </summary>
        /// <param name="locationQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Location> GetAllLocations(LocationQueryDto locationQueryDto);

        /// <summary>
        /// 获取所有库位信息
        /// </summary>
        /// <returns></returns>
        List<Location> GetAllLocations();

        /// <summary>
        /// 根据id获取库位信息
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        Location GetLocation(long locationId);

        /// <summary>
        /// 新增库位信息
        /// </summary>
        /// <param name="location"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int AddLocation(Location location, string userName);

        /// <summary>
        /// 修改库位信息
        /// </summary>
        /// <param name="location"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int EditLocation(Location location, string userName);

        /// <summary>
        /// 根据id删除库位信息
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        int DeleteLocation(long locationId);
    }
}
