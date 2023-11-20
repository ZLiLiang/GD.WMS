using GD.Infrastructure.Attribute;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;
using GD.Repository;
using GD.Service.Interface.WarehouseManagement;
using SqlSugar;

namespace GD.Service.WarehouseManagement
{
    [AppService(ServiceType = typeof(IRegionService), ServiceLifetime = LifeTime.Transient)]
    public class RegionService : BaseService<Region>, IRegionService
    {
        public int AddRegion(Region region, string userName)
        {
            region.Create_by = userName;
            region.Create_time = DateTime.Now;
            region.WarehouseName = Context
                .Queryable<Warehouse>()
                .First(it => it.WarehouseId == region.WarehouseId)
                .WarehouseName;

            return Insert(region);
        }

        public int DeleteRegion(long regionId)
        {
            return Delete(regionId);
        }

        public int EidtRegion(Region region, string userName)
        {
            region.Update_by = userName;
            region.Update_time = DateTime.Now;
            region.WarehouseName = Context
                .Queryable<Warehouse>()
                .First(it => it.WarehouseId == region.WarehouseId)
                .WarehouseName;

            return Update(region);
        }

        public PagedInfo<Region> GetAllRegions(RegionQueryDto regionQueryDto)
        {
            var expression = Expressionable.Create<Region>()
                .AndIF(!string.IsNullOrEmpty(regionQueryDto.WarehouseName), region => region.WarehouseName.Contains(regionQueryDto.WarehouseName))
                .AndIF(!string.IsNullOrEmpty(regionQueryDto.RegionName), region => region.RegionName.Contains(regionQueryDto.RegionName))
                .AndIF(regionQueryDto.RegionProperty != null, region => region.RegionProperty == regionQueryDto.RegionProperty)
                .AndIF(regionQueryDto.BeginTime != DateTime.MinValue && regionQueryDto.BeginTime != null, exp => exp.Create_time >= regionQueryDto.BeginTime)
                .AndIF(regionQueryDto.EndTime != DateTime.MaxValue && regionQueryDto.EndTime != null, exp => exp.Create_time <= regionQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(regionQueryDto);
        }

        public List<Region> GetAllRegions()
        {
            return Queryable()
                .ToList();
        }

        public Region GetRegion(long regionId)
        {
            return Queryable()
                .Where(it => it.RegionId == regionId)
                .First();
        }

        public List<Region> GetRegionsByWarehouseId(long warehouseId)
        {
            return Queryable()
                .Where(it => it.WarehouseId == warehouseId)
                .ToList();
        }

        public bool IsOtherUse(long regionId)
        {
            return Context
                .Queryable<Location>()
                .Where(it => it.RegionId == regionId)
                .Any();
        }
    }
}
