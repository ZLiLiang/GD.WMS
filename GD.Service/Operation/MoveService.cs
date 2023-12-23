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
    [AppService(ServiceType = typeof(IMoveService), ServiceLifetime = LifeTime.Transient)]
    public class MoveService : BaseService<Move>, IMoveService
    {
        public (bool, string) Add(MoveDto moveDto, string userName)
        {
            var entity = moveDto.Adapt<Move>();

            var dispatchGroupDatas = Context.Queryable<Dispatch>()
                .LeftJoin<Dispatchpick>((dp, dpp) => dp.DispatchId == dpp.DispatchpickId)
                .Where((dp, dpp) => dp.DispatchStatus > 1 && dp.DispatchStatus < 6 && dpp.LocationId == entity.OrigLocationId && dpp.SkuId == entity.SkuId)
                .GroupBy((dp, dpp) => new
                {
                    dpp.SkuId,
                    dpp.LocationId,
                })
                .Select((dp, dpp) => new
                {
                    skuId = dpp.SkuId,
                    locationId = dpp.LocationId,
                    lockedQty = SqlFunc.AggregateSum(dpp.PickQty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .Where(pd => pd.IsUpate == 0 && pd.SkuId == entity.SkuId && pd.LocationId == entity.OrigLocationId)
                .GroupBy(pd => new
                {
                    pd.SkuId,
                    pd.LocationId
                })
                .Select(pd => new
                {
                    skuId = pd.SkuId,
                    locationId = pd.LocationId,
                    lockedQty = SqlFunc.AggregateSum(pd.Qty)
                });

            var moveLockedGroupDatas = Queryable()
                .Where(m => m.MoveStatus == 0 && m.SkuId == entity.SkuId && m.OrigLocationId == entity.OrigLocationId)
                .GroupBy(m => new
                {
                    m.SkuId,
                    m.OrigLocationId
                })
                .Select(m => new
                {
                    skuId = m.SkuId,
                    locationId = m.OrigLocationId,
                    lockedQty = SqlFunc.AggregateSum(m.Qty)
                });

            var origStock = Context.Queryable<Stock>()
                .LeftJoin(dispatchGroupDatas, (stock, dispatch) => stock.SkuId == dispatch.skuId && stock.LocationId == dispatch.locationId)
                .LeftJoin(processLockedGroupDatas, (stock, dispatch, process) => stock.SkuId == process.skuId && stock.LocationId == process.locationId)
                .LeftJoin(moveLockedGroupDatas, (stock, dispatch, process, move) => stock.SkuId == move.skuId && stock.LocationId == move.locationId)
                .Where((stock, dispatch, process, move) => stock.SkuId == entity.SkuId && stock.LocationId == entity.OrigLocationId)
                .Select((stock, dispatch, process, move) => new
                {
                    id = stock.StockId,
                    AvailableQty = stock.IsFreeze == 1 ? 0 : (stock.Qty - (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - (process.lockedQty == null ? 0 : process.lockedQty) - (move.lockedQty == null ? 0 : move.lockedQty))
                })
                .First();

            var destStock = Context.Queryable<Stock>()
                .First(it => it.LocationId == entity.DestLocationId && it.SkuId == entity.SkuId);

            if (origStock == null || origStock.AvailableQty < entity.Qty)
            {
                return (false, "数量非法");
            }
            if (destStock != null && destStock.IsFreeze == 1)
            {
                return (false, "目标库存被冻结");
            }

            entity.MoveStatus = 0;
            entity.Create_by = userName;
            entity.JobCode = GetMoveJobCode();
            var insertResult = Insertable(entity)
                .ExecuteCommandIdentityIntoEntity();

            if (insertResult)
            {
                return (insertResult, "新增成功");
            }
            else
            {
                return (insertResult, "新增失败");
            }
        }

        public (bool, string) Confirm(long moveId, string userName)
        {
            var result = UseTran(() =>
            {
                var entity = Queryable()
                .First(it => it.MoveId == moveId);

                if (entity == null)
                {
                    throw new Exception("移动作业不存在");
                }

                entity.Handler = userName;
                entity.Update_by = userName;
                entity.MoveStatus = 1;
                var origStock = Context.Queryable<Stock>()
                    .First(it => it.LocationId == entity.OrigLocationId && it.SkuId == entity.SkuId);
                var destStock = Context.Queryable<Stock>()
                    .First(it => it.LocationId == entity.DestLocationId && it.SkuId != entity.SkuId);

                if (origStock != null)
                {
                    if (origStock.Qty == entity.Qty)
                    {
                        Context.Deleteable(origStock).ExecuteCommand();
                    }
                    else
                    {
                        origStock.Qty -= entity.Qty;
                        Context.Updateable(origStock).ExecuteCommand();
                    }
                }

                if (destStock == null)
                {
                    destStock = new Stock
                    {
                        LocationId = entity.DestLocationId,
                        SkuId = entity.SkuId,
                        OwnerId = entity.OwnerId,
                        IsFreeze = 0,
                        Qty = entity.Qty
                    };
                    Context.Insertable(destStock).ExecuteCommand();
                }
                else
                {
                    destStock.Qty += entity.Qty;
                    Context.Updateable(destStock).ExecuteCommand();
                }

                Update(entity);
            });

            return (result.IsSuccess, result.ErrorMessage);
        }

        public (bool, string) Delete(long moveId)
        {
            int result = Deleteable()
                .Where(it => it.MoveId == moveId && it.MoveStatus == 0)
                .ExecuteCommand();

            if (result > 0)
            {
                return (true, "删除成功");
            }
            else
            {
                return (false, "删除失败");
            }
        }

        public MoveVm Get(long moveId)
        {
            var result = Queryable()
                .LeftJoin<CommoditySKU>((move, sku) => move.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((move, sku, spu, orig) => move.OrigLocationId == orig.LocationId)
                .LeftJoin<Location>((move, sku, spu, orig, dest) => move.DestLocationId == dest.LocationId)
                .Where((move, sku, spu, orig, dest) => move.MoveId == moveId)
                .Select((move, sku, spu, orig, dest) => new MoveVm
                {
                    MoveId = move.MoveId,
                    JobCode = move.JobCode,
                    MoveStatus = move.MoveStatus,
                    Qty = move.Qty,
                    Handler = move.Handler,
                    HandlerTime = move.HandlerTime,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUName,
                    SpuName = spu.CommoditySPUCode,
                    OrigLocationCode = orig.LocationCode,
                    OrigWarehousName = orig.WarehouseName,
                    DestLocationCode = dest.LocationCode,
                    DestWarehousName = dest.WarehouseName
                }, true)
                .First();

            return result;
        }

        public PagedInfo<MoveVm> GetAll(MoveQueryDto moveQueryDto)
        {
            var expression = Expressionable.Create<MoveVm>()
                .AndIF(!string.IsNullOrEmpty(moveQueryDto.JobCode), entity => entity.JobCode.Contains(moveQueryDto.JobCode))
                .AndIF(moveQueryDto.BeginTime != DateTime.MinValue && moveQueryDto.BeginTime != null, exp => exp.Create_time >= moveQueryDto.BeginTime)
                .AndIF(moveQueryDto.EndTime != DateTime.MaxValue && moveQueryDto.EndTime != null, exp => exp.Create_time <= moveQueryDto.EndTime);

            var query = Queryable()
                .LeftJoin<CommoditySKU>((move, sku) => move.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((move, sku, spu, orig) => move.OrigLocationId == orig.LocationId)
                .LeftJoin<Location>((move, sku, spu, orig, dest) => move.DestLocationId == dest.LocationId)
                .Select((move, sku, spu, orig, dest) => new MoveVm
                {
                    MoveId = move.MoveId,
                    JobCode = move.JobCode,
                    MoveStatus = move.MoveStatus,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    Qty = move.Qty,
                    OrigWarehousName = orig.WarehouseName,
                    OrigLocationCode = orig.LocationCode,
                    DestWarehousName = dest.WarehouseName,
                    DestLocationCode = dest.LocationCode,
                }, true)
                .MergeTable();

            return query.Where(expression.ToExpression())
                .ToPage(moveQueryDto);
        }

        public List<MoveVm> GetAll()
        {
            var query = Queryable()
                .LeftJoin<CommoditySKU>((move, sku) => move.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((move, sku, spu, orig) => move.OrigLocationId == orig.LocationId)
                .LeftJoin<Location>((move, sku, spu, orig, dest) => move.DestLocationId == dest.LocationId)
                .Select((move, sku, spu, orig, dest) => new MoveVm
                {
                    MoveId = move.MoveId,
                    JobCode = move.JobCode,
                    MoveStatus = move.MoveStatus,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    Qty = move.Qty,
                    OrigWarehousName = orig.WarehouseName,
                    OrigLocationCode = orig.LocationCode,
                    DestWarehousName = dest.WarehouseName,
                    DestLocationCode = dest.LocationCode
                }, true);

            return query.ToList();
        }

        public string GetMoveJobCode()
        {
            var date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var maxNo = Queryable()
                .OrderBy(it => it.JobCode, OrderByType.Desc)
                .First()?
                .JobCode
                .Split("-");

            if (maxNo != null && maxNo.First() == date)
            {
                //return $"{date}-{(int.Parse(maxNo.Last()) + 1).ToString("0000")}";
                return $"{date}-{int.Parse(maxNo.Last()) + 1:0000}";
            }
            else
            {
                return $"{date}-0001";
            }
        }
    }
}
