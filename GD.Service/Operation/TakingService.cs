using GD.Infrastructure.Attribute;
using GD.Model.Basic;
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
    [AppService(ServiceType = typeof(ITakingService), ServiceLifetime = LifeTime.Transient)]
    public class TakingService : BaseService<Taking>, ITakingService
    {
        public (bool, string) Add(TakingDto takingDto, string userName)
        {
            var entity = takingDto.Adapt<Taking>();
            entity.JobCode = GetTakingJobCode();
            entity.Create_by = userName;

            var result = Insert(entity);

            if (result > 0)
            {
                return (true, "新增成功");
            }
            else
            {
                return (false, "新增失败");
            }
        }

        public (bool, string) Confirm(long takingId, string userName)
        {
            var result = UseTran(() =>
            {
                var taking = Queryable()
                .First(it => it.TakingId == takingId);
                if (taking == null) throw new Exception("实体不存在");

                var stock = Context.Queryable<Stock>()
                    .First(it => it.SkuId == taking.SkuId && it.OwnerId == taking.OwnerId && it.LocationId == taking.LocationId);
                if (stock == null)
                {
                    Context.Insertable(new Stock
                    {
                        SkuId = taking.SkuId,
                        LocationId = taking.LocationId,
                        Qty = taking.DifferenceQty,
                        OwnerId = taking.OwnerId,
                        Create_by = userName,
                        IsFreeze = 0
                    })
                    .ExecuteCommand();
                }
                else
                {
                    stock.Qty += taking.DifferenceQty;
                    Context.Updateable(stock).ExecuteCommand();
                }

                Context.Insertable(new Adjust
                {
                    JobCode = taking.JobCode,
                    SkuId = taking.SkuId,
                    LocationId = taking.LocationId,
                    OwnerId = taking.OwnerId,
                    Qty = taking.DifferenceQty,
                    Create_by = userName,
                    IsUpate = 1,
                    JobType = 1,
                    SourceTableId = taking.TakingId
                })
                .ExecuteCommand();
            });

            if (result.IsSuccess)
            {
                return (true, "确认成功");
            }
            else
            {
                return (false, result.ErrorMessage);
            }
        }

        public (bool, string) Delete(long takingId)
        {
            var result = base.Delete(takingId);

            if (result > 0)
            {
                return (true, "删除成功");
            }
            else
            {
                return (false, "删除失败");
            }
        }

        public TakingVm Get(long takingId)
        {
            var queryAdjust = Context.Queryable<Adjust>()
                .Where(it => it.JobType == 1)
                .Select(it => new
                {
                    adjustId = it.AdjustId,
                    sourceTableId = it.SourceTableId
                });

            var query = Queryable()
                .LeftJoin<CommoditySKU>((taking, sku) => taking.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((taking, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((taking, sku, spu, location) => taking.LocationId == location.LocationId)
                .LeftJoin<Owner>((taking, sku, spu, location, owner) => taking.OwnerId == owner.OwnerId)
                .LeftJoin(queryAdjust, (taking, sku, spu, location, owner, adjust) => taking.TakingId == adjust.sourceTableId)
                .Where((taking, sku, spu, location, owner, adjust) => taking.TakingId == takingId)
                .Select((taking, sku, spu, location, owner, adjust) => new TakingVm
                {
                    TakingId = taking.TakingId,
                    JobCode = taking.JobCode,
                    JobStatus = taking.JobStatus,
                    AdjustStatus = adjust.adjustId == null ? 0 : 1,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    WarehouseName = location.WarehouseName,
                    LocationCode = location.LocationCode,
                    OnwerName = owner.OwnerName,
                    BookQty = taking.BookQty,
                    CountedQty = taking.CountedQty,
                    DifferenceQty = taking.DifferenceQty,
                    Create_by = taking.Create_by,
                    Create_time = taking.Create_time,
                    Handler = taking.Handler,
                    HandlerTime = taking.HandlerTime,
                    Update_by = taking.Update_by,
                    Update_time = taking.Update_time
                });

            return query.First();
        }

        public PagedInfo<TakingVm> GetAll(TakingQueryDto takingQueryDto)
        {
            var expression = Expressionable.Create<TakingVm>()
                .AndIF(!string.IsNullOrEmpty(takingQueryDto.JobCode), entity => entity.JobCode.Contains(takingQueryDto.JobCode))
                .AndIF(takingQueryDto.BeginTime != DateTime.MinValue && takingQueryDto.BeginTime != null, exp => exp.Create_time >= takingQueryDto.BeginTime)
                .AndIF(takingQueryDto.EndTime != DateTime.MaxValue && takingQueryDto.EndTime != null, exp => exp.Create_time <= takingQueryDto.EndTime);

            var queryAdjust = Context.Queryable<Adjust>()
                .Where(it => it.JobType == 1)
                .Select(it => new
                {
                    adjustId = it.AdjustId,
                    sourceTableId = it.SourceTableId
                });

            var query = Queryable()
                .LeftJoin<CommoditySKU>((taking, sku) => taking.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((taking, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((taking, sku, spu, location) => taking.LocationId == location.LocationId)
                .LeftJoin<Owner>((taking, sku, spu, location, owner) => taking.OwnerId == owner.OwnerId)
                .LeftJoin(queryAdjust, (taking, sku, spu, location, owner, adjust) => taking.TakingId == adjust.sourceTableId)
                .Select((taking, sku, spu, location, owner, adjust) => new TakingVm
                {
                    TakingId = taking.TakingId,
                    JobCode = taking.JobCode,
                    JobStatus = taking.JobStatus,
                    AdjustStatus = adjust.adjustId == null ? 0 : 1,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    WarehouseName = location.WarehouseName,
                    LocationCode = location.LocationCode,
                    OnwerName = owner.OwnerName,
                    BookQty = taking.BookQty,
                    CountedQty = taking.CountedQty,
                    DifferenceQty = taking.DifferenceQty,
                    Handler = taking.Handler,
                    HandlerTime = taking.HandlerTime,
                    Create_by = taking.Create_by,
                    Create_time = taking.Create_time,
                    Update_by = taking.Update_by,
                    Update_time = taking.Update_time
                })
                .MergeTable();

            return query.Where(expression.ToExpression())
                .ToPage(takingQueryDto);
        }

        public List<TakingVm> GetAll()
        {
            var queryAdjust = Context.Queryable<Adjust>()
                .Where(it => it.JobType == 1)
                .Select(it => new
                {
                    adjustId = it.AdjustId,
                    sourceTableId = it.SourceTableId
                });

            var query = Queryable()
                .LeftJoin<CommoditySKU>((taking, sku) => taking.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((taking, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((taking, sku, spu, location) => taking.LocationId == location.LocationId)
                .LeftJoin<Owner>((taking, sku, spu, location, owner) => taking.OwnerId == owner.OwnerId)
                .LeftJoin(queryAdjust, (taking, sku, spu, location, owner, adjust) => taking.TakingId == adjust.sourceTableId)
                .Select((taking, sku, spu, location, owner, adjust) => new TakingVm
                {
                    TakingId = taking.TakingId,
                    JobCode = taking.JobCode,
                    JobStatus = taking.JobStatus,
                    AdjustStatus = adjust.adjustId == null ? 0 : 1,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    WarehouseName = location.WarehouseName,
                    LocationCode = location.LocationCode,
                    OnwerName = owner.OwnerName,
                    BookQty = taking.BookQty,
                    CountedQty = taking.CountedQty,
                    DifferenceQty = taking.DifferenceQty,
                    Handler = taking.Handler,
                    HandlerTime = taking.HandlerTime,
                    Create_by = taking.Create_by,
                    Create_time = taking.Create_time,
                    Update_by = taking.Update_by,
                    Update_time = taking.Update_time
                });

            return query.ToList();
        }

        public string GetTakingJobCode()
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

        public (bool, string) Put(TakingPutDto takingPutDto, string userName)
        {
            var entity = Queryable()
                .First(it => it.TakingId == takingPutDto.TakingId);
            if (entity == null) return (false, "实体不存在");

            entity.CountedQty = takingPutDto.CountedQty;
            entity.DifferenceQty = takingPutDto.CountedQty - entity.BookQty;
            entity.Handler = userName;
            entity.HandlerTime = DateTime.Now;
            entity.JobStatus = 1;

            var result = Update(entity);
            if (result > 0)
            {
                return (true, "盘点成功");
            }
            else
            {
                return (false, "盘点失败");
            }
        }
    }
}
