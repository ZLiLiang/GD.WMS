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
    [AppService(ServiceType = typeof(IAdjustService), ServiceLifetime = LifeTime.Transient)]
    public class AdjustService : BaseService<Adjust>, IAdjustService
    {
        public (bool, string) Add(AdjustDto adjustDto, string userName)
        {
            var entity = adjustDto.Adapt<Adjust>();
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

        public (bool, string) ConfirmAdjustmen(long adjustId, string userName)
        {
            var result = UseTran(() =>
            {
                var entity = Queryable()
                    .First(it => it.AdjustId == adjustId);
                if (entity == null) throw new Exception("不存在");

                if (entity.JobType == 2)
                {
                    var processDetail = Context.Queryable<ProcessDetail>()
                        .First(it => it.ProcessDetailId == entity.SourceTableId);
                    if (processDetail != null)
                    {
                        processDetail.Update_by = userName;
                        processDetail.Update_time = DateTime.Now;
                        processDetail.IsUpate = 1;
                        Context.Updateable(processDetail).ExecuteCommand();
                    }
                }

                var stock = Context.Queryable<Stock>()
                    .First(it => it.LocationId == entity.LocationId && it.SkuId == entity.SkuId);
                if (stock == null)
                {
                    Context.Insertable(new Stock
                    {
                        StockId = entity.AdjustId,
                        SkuId = entity.SkuId,
                        LocationId = entity.LocationId,
                        Qty = entity.Qty,
                        OwnerId = entity.OwnerId,
                        IsFreeze = 0
                    })
                    .ExecuteCommand();
                }
                else
                {
                    stock.Qty += entity.Qty;
                    stock.OwnerId = entity.OwnerId;
                    stock.Update_by = userName;
                    Context.Updateable(stock).ExecuteCommand();
                }

                entity.IsUpate = 1;
                Update(entity);
            });

            return (result.IsSuccess, result.ErrorMessage);
        }

        public (bool, string) Delete(long adjustId)
        {
            var entity = Queryable()
                .First(it => it.AdjustId == adjustId);
            if (entity == null) return (false, "不存在");

            var result = base.Delete(entity.AdjustId);
            if (result > 0)
            {
                return (true, "删除成功");
            }
            else
            {
                return (false, "删除失败");
            }
        }

        public AdjustVm Get(long adjustId)
        {
            var query = Queryable()
                .LeftJoin<CommoditySKU>((adjust, sku) => adjust.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((adjust, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((adjust, sku, spu, location) => adjust.LocationId == location.LocationId)
                .LeftJoin<Owner>((adjust, sku, spu, location, owner) => adjust.OwnerId == owner.OwnerId)
                .Where((adjust, sku, spu, location, owner) => adjust.AdjustId == adjustId)
                .Select((adjust, sku, spu, location, owner) => new AdjustVm
                {
                    AdjustId = adjust.AdjustId,
                    JobCode = adjust.JobCode,
                    IsUpate = adjust.IsUpate,
                    JobType = adjust.JobType,
                    Qty = adjust.Qty,
                    SourceTableId = adjust.SourceTableId,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    WarehouseName = location.WarehouseName,
                    LocationCode = location.LocationCode,
                    OnwerName = owner.OwnerName
                }, true);

            return query.First();

        }

        public PagedInfo<AdjustVm> GetAll(AdjustQueryDto adjustQueryDto)
        {
            var expression = Expressionable.Create<AdjustVm>()
                .AndIF(!string.IsNullOrEmpty(adjustQueryDto.JobCode), entity => entity.JobCode.Contains(adjustQueryDto.JobCode))
                .AndIF(adjustQueryDto.BeginTime != DateTime.MinValue && adjustQueryDto.BeginTime != null, exp => exp.Create_time >= adjustQueryDto.BeginTime)
                .AndIF(adjustQueryDto.EndTime != DateTime.MaxValue && adjustQueryDto.EndTime != null, exp => exp.Create_time <= adjustQueryDto.EndTime);

            var query = Queryable()
                .LeftJoin<CommoditySKU>((adjust, sku) => adjust.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((adjust, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((adjust, sku, spu, location) => adjust.LocationId == location.LocationId)
                .LeftJoin<Owner>((adjust, sku, spu, location, owner) => adjust.OwnerId == owner.OwnerId)
                .Select((adjust, sku, spu, location, owner) => new AdjustVm
                {
                    AdjustId = adjust.AdjustId,
                    JobCode = adjust.JobCode,
                    IsUpate = adjust.IsUpate,
                    JobType = adjust.JobType,
                    Qty = adjust.Qty,
                    SourceTableId = adjust.SourceTableId,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    WarehouseName = location.WarehouseName,
                    LocationCode = location.LocationCode,
                    OnwerName = owner.OwnerName
                }, true)
                .MergeTable();

            var result = query.Where(expression.ToExpression())
                .ToPage(adjustQueryDto);

            return result;
        }

        public List<AdjustVm> GetAll()
        {
            var query = Queryable()
                .LeftJoin<CommoditySKU>((adjust, sku) => adjust.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((adjust, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((adjust, sku, spu, location) => adjust.LocationId == location.LocationId)
                .LeftJoin<Owner>((adjust, sku, spu, location, owner) => adjust.OwnerId == owner.OwnerId)
                .Select((adjust, sku, spu, location, owner) => new AdjustVm
                {
                    AdjustId = adjust.AdjustId,
                    JobCode = adjust.JobCode,
                    IsUpate = adjust.IsUpate,
                    JobType = adjust.JobType,
                    Qty = adjust.Qty,
                    SourceTableId = adjust.SourceTableId,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    WarehouseName = location.WarehouseName,
                    LocationCode = location.LocationCode,
                    OnwerName = owner.OwnerName
                }, true);

            var result = query.ToList();

            return result;
        }

        public (bool, string) Update(AdjustDto adjustDto, string userName)
        {
            var entity = Queryable()
                .First(it => it.AdjustId == adjustDto.AdjustId);
            if (entity == null) return (false, "不存在");

            entity.JobType = adjustDto.JobType;
            entity.SkuId = adjustDto.SkuId;
            entity.OwnerId = adjustDto.OwnerId;
            entity.LocationId = adjustDto.LocationId;
            entity.Qty = adjustDto.Qty;
            entity.IsUpate = adjustDto.IsUpate;
            entity.SourceTableId = adjustDto.SourceTableId;

            var result = Update(entity);
            if (result > 0)
            {
                return (true, "更新成功");
            }
            else
            {
                return (false, "更新失败");
            }
        }
    }
}
