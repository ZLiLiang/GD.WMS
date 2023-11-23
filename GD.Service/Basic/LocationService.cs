using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;
using GD.Repository;
using GD.Service.Interface.Basic;
using SqlSugar;

namespace GD.Service.Basic
{
    [AppService(ServiceType = typeof(ILocationService), ServiceLifetime = LifeTime.Transient)]
    public class LocationService : BaseService<Location>, ILocationService
    {
        public int AddLocation(Location location, string userName)
        {
            location.Create_by = userName;
            location.Create_time = DateTime.Now;
            location.WarehouseName = Context
                .Queryable<Warehouse>()
                .First(it => it.WarehouseId == location.WarehouseId)
                .WarehouseName;
            location.RegionName = Context
                .Queryable<Region>()
                .First(it => it.RegionId == location.RegionId)
                .RegionName;

            return Insert(location);
        }

        public int DeleteLocation(long locationId)
        {
            return Delete(locationId);
        }

        public int EditLocation(Location location, string userName)
        {
            location.Update_by = userName;
            location.Update_time = DateTime.Now;
            location.WarehouseName = Context
                .Queryable<Warehouse>()
                .First(it => it.WarehouseId == location.WarehouseId)
                .WarehouseName;
            location.RegionName = Context
                .Queryable<Region>()
                .First(it => it.RegionId == location.RegionId)
                .RegionName;

            return Update(location);
        }

        public PagedInfo<Location> GetAllLocations(LocationQueryDto locationQueryDto)
        {
            var expression = Expressionable.Create<Location>()
                .AndIF(!string.IsNullOrEmpty(locationQueryDto.WarehouseName), location => location.WarehouseName.Contains(locationQueryDto.WarehouseName))
                .AndIF(!string.IsNullOrEmpty(locationQueryDto.RegionName), location => location.RegionName.Contains(locationQueryDto.RegionName))
                .AndIF(!string.IsNullOrEmpty(locationQueryDto.LocationCode), location => location.LocationCode.Contains(locationQueryDto.LocationCode))
                .AndIF(locationQueryDto.BeginTime != DateTime.MinValue && locationQueryDto.BeginTime != null, exp => exp.Create_time >= locationQueryDto.BeginTime)
                .AndIF(locationQueryDto.EndTime != DateTime.MaxValue && locationQueryDto.EndTime != null, exp => exp.Create_time <= locationQueryDto.EndTime);

            return Queryable()
                .Where(expression.ToExpression())
                .ToPage(locationQueryDto);
        }

        public List<Location> GetAllLocations()
        {
            return Queryable()
                .ToList();
        }

        public Location GetLocation(long locationId)
        {
            return Queryable()
                .Where(it => it.LocationId == locationId)
                .First();
        }
    }
}
