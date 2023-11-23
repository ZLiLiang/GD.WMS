using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;

namespace GD.Service.Interface.Basic
{
    /// <summary>
    /// 库区服务
    /// </summary>
    public interface IRegionService : IBaseService<Region>
    {
        /// <summary>
        /// 分页获取所有库区信息
        /// </summary>
        /// <param name="regionQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Region> GetAllRegions(RegionQueryDto regionQueryDto);

        /// <summary>
        /// 获取所有库区信息
        /// </summary>
        /// <returns></returns>
        List<Region> GetAllRegions();

        /// <summary>
        /// 根据id获取库区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        Region GetRegion(long regionId);

        /// <summary>
        /// 新增库区信息
        /// </summary>
        /// <param name="region"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int AddRegion(Region region, string userName);

        /// <summary>
        /// 修改库区信息
        /// </summary>
        /// <param name="region"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int EidtRegion(Region region, string userName);

        /// <summary>
        /// 根据id删除库区信息
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        int DeleteRegion(long regionId);

        /// <summary>
        /// 查询是否被其他表引用
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        bool IsOtherUse(long regionId);

        /// <summary>
        /// 根据仓库id获取库区信息
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        List<Region> GetRegionsByWarehouseId(long warehouseId);
    }
}
