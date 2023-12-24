using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Delivery;
using GD.Model.Dto.Operation;
using GD.Model.Inventory;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;
using GD.Repository;
using GD.Service.Interface.Operation;
using Mapster;
using SqlSugar;

namespace GD.Service.Operation
{
    [AppService(ServiceType = typeof(IFreezeService), ServiceLifetime = LifeTime.Transient)]
    public class FreezeService : BaseService<Freeze>, IFreezeService
    {
        public (bool, string) Add(FreezeDto freezeDto, string userName)
        {
            var result = UseTran(() =>
            {
                var entity = freezeDto.Adapt<Freeze>();
                entity.Create_by = userName;
                entity.Handler = userName;
                entity.HandlerTime = DateTime.Now;
                entity.JobCode = GetFreezeJobCode();

                var stocks = Context.Queryable<Stock>()
                    .Where(it => it.LocationId == entity.LocationId && it.OwnerId == entity.OwnerId && it.SkuId == entity.SkuId)
                    .ToList();
                foreach (var item in stocks)
                {
                    if (entity.JobType == 0)
                    {
                        item.IsFreeze = 1;
                    }
                    else
                    {
                        item.IsFreeze = 0;
                    }
                    Context.Updateable(item).ExecuteCommand();
                }
                //Insertable(entity).ExecuteCommand();
                Insert(entity);

                var processNotConfirm = Context.Queryable<ProcessDetail>()
                    .Where(it => it.LocationId == entity.LocationId && it.OwnerId == entity.OwnerId && it.SkuId == entity.SkuId && it.IsUpate == 0)
                    .ToList()
                    .Any();
                var dispatchNotConfirm = Context.Queryable<Dispatchpick>()
                    .Where(it => it.LocationId == entity.LocationId && it.SkuId == entity.SkuId && it.IsUpate == 0)
                    .ToList()
                    .Any();
                var moveNotConfirm = Context.Queryable<Move>()
                    .Where(it => (it.OrigLocationId == entity.LocationId || it.DestLocationId == entity.LocationId) && it.SkuId == entity.SkuId && it.MoveStatus == 0)
                    .ToList()
                    .Any();

                if (processNotConfirm)
                {
                    throw new Exception("加工未确认");
                }
                else if (dispatchNotConfirm)
                {
                    throw new Exception("派送未确认");
                }
                else if (moveNotConfirm)
                {
                    throw new Exception("移动未确认");
                }
            });

            return (result.IsSuccess, result.ErrorMessage);

        }

        public (bool, string) Delete(long freezeId)
        {
            //var result = Deleteable()
            //    .Where(it => it.FreezeId == freezeId)
            //    .ExecuteCommandHasChange();

            var result = base.Delete(freezeId);

            if (result > 0)
            {
                return (true, "删除成功");
            }
            else
            {
                return (false, "删除失败");
            }
        }

        public FreezeVm Get(long freezeId)
        {
            var query = Queryable()
                .LeftJoin<CommoditySKU>((freeze, sku) => freeze.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((freeze, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((freeze, sku, spu, location) => freeze.LocationId == location.LocationId)
                .Where((freeze, sku, spu, location) => freeze.FreezeId == freezeId)
                .Select((freeze, sku, spu, location) => new FreezeVm
                {
                    FreezeId = freeze.FreezeId,
                    JobCode = freeze.JobCode,
                    JobType = freeze.JobType,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    LocationCode = location.LocationCode,
                    WarehouseName = location.WarehouseName,
                    Handler = freeze.Handler,
                    HandlerTime = freeze.HandlerTime
                }, true);

            return query.First();
        }

        public PagedInfo<FreezeVm> GetAll(FreezeQueryDto freezeQueryDto)
        {
            var expression = Expressionable.Create<FreezeVm>()
                .AndIF(!string.IsNullOrEmpty(freezeQueryDto.JobCode), entity => entity.JobCode.Contains(freezeQueryDto.JobCode))
                .AndIF(freezeQueryDto.BeginTime != DateTime.MinValue && freezeQueryDto.BeginTime != null, exp => exp.Create_time >= freezeQueryDto.BeginTime)
                .AndIF(freezeQueryDto.EndTime != DateTime.MaxValue && freezeQueryDto.EndTime != null, exp => exp.Create_time <= freezeQueryDto.EndTime);

            var query = Queryable()
                .LeftJoin<CommoditySKU>((freeze, sku) => freeze.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((freeze, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((freeze, sku, spu, location) => freeze.LocationId == location.LocationId)
                .Select((freeze, sku, spu, location) => new FreezeVm
                {
                    FreezeId = freeze.FreezeId,
                    JobCode = freeze.JobCode,
                    JobType = freeze.JobType,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    LocationCode = location.LocationCode,
                    WarehouseName = location.WarehouseName,
                    Handler = freeze.Handler,
                    HandlerTime = freeze.HandlerTime
                }, true)
                .MergeTable();

            return query.Where(expression.ToExpression())
                .ToPage(freezeQueryDto);

        }

        public List<FreezeVm> GetAll()
        {
            var query = Queryable()
                .LeftJoin<CommoditySKU>((freeze, sku) => freeze.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((freeze, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((freeze, sku, spu, location) => freeze.LocationId == location.LocationId)
                .Select((freeze, sku, spu, location) => new FreezeVm
                {
                    FreezeId = freeze.FreezeId,
                    JobCode = freeze.JobCode,
                    JobType = freeze.JobType,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    LocationCode = location.LocationCode,
                    WarehouseName = location.WarehouseName,
                    Handler = freeze.Handler,
                    HandlerTime = freeze.HandlerTime
                }, true);

            return query.ToList();
        }

        public string GetFreezeJobCode()
        {
            var date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var maxNo = Queryable()
                .OrderBy(it => it.JobCode, OrderByType.Desc)
                .First()?
                .JobCode
                .Split("-");

            if (maxNo != null && maxNo.First() == date)
            {
                return $"{date}-{int.Parse(maxNo.Last()) + 1:0000}";
            }
            else
            {
                return $"{date}-0001";
            }
        }

        public (bool, string) Update(FreezeDto freezeDto, string userName)
        {
            var entity = Queryable()
                .First(it => it.FreezeId == freezeDto.FreezeId);

            if (entity == null)
            {
                return (false, "数据不存在");
            }

            entity.JobType = freezeDto.JobType;
            entity.SkuId = freezeDto.SkuId;
            entity.OwnerId = freezeDto.OwnerId;
            entity.LocationId = freezeDto.LocationId;
            entity.Handler = userName;
            entity.HandlerTime = DateTime.Now;

            var result = Update(entity);

            if (result > 0)
            {
                return (true, "修改成功");
            }
            else
            {
                return (false, "修改失败");
            }
        }
    }
}
