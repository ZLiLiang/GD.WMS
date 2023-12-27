using GD.Infrastructure.App;
using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Delivery;
using GD.Model.Dto.Inventory;
using GD.Model.Inventory;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Receive;
using GD.Model.Vm.Inventory;
using GD.Model.Vm.Operation;
using GD.Repository;
using GD.Service.Interface.Inventory;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient.Server;
using SqlSugar;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;

namespace GD.Service.Inventory
{
    [AppService(ServiceType = typeof(IStockService), ServiceLifetime = LifeTime.Transient)]
    public class StockService : BaseService<Stock>, IStockService
    {
        public PagedInfo<StockVm> GetStock(StockQueryDto stockQueryDto)
        {
            var expression = Expressionable.Create<StockVm>()
                .AndIF(!string.IsNullOrEmpty(stockQueryDto.SpuName), entity => entity.SpuName.Contains(stockQueryDto.SpuName))
                .AndIF(stockQueryDto.BeginTime != DateTime.MinValue && stockQueryDto.BeginTime != null, exp => exp.Create_time >= stockQueryDto.BeginTime)
                .AndIF(stockQueryDto.EndTime != DateTime.MaxValue && stockQueryDto.EndTime != null, exp => exp.Create_time <= stockQueryDto.EndTime);

            var stockGroupDatas = Queryable()
                .LeftJoin<Location>((stock, location) => stock.LocationId == location.LocationId)
                .GroupBy((stock, location) => stock.SkuId)
                .Select((stock, location) => new
                {
                    skuId = stock.SkuId,
                    frozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(stock.IsFreeze == 1, stock.Qty, 0)),
                    qty = SqlFunc.AggregateSum(stock.Qty),
                    normalQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5, stock.Qty, 0)),
                    normalFrozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5 && stock.IsFreeze == 1, stock.Qty, 0))
                });

            var asnGroupDatas = Context.Queryable<Asn>()
                .GroupBy(asn => asn.SkuId)
                .Select(asn => new
                {
                    skuId = asn.SkuId,
                    asnQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 0, asn.AsnQty, 0)),
                    toUnloadQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 1, asn.AsnQty, 0)),
                    toSortQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 2, asn.AsnQty, 0)),
                    sortedQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 3, asn.SortedQty, 0)),
                    shortageQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 4, asn.ShortageQty, 0))
                });

            var dispatchGroupDatas = Context.Queryable<Dispatch>()
                .GroupBy(dispatch => dispatch.SkuId)
                .Select(dispatch => new
                {
                    skuId = dispatch.SkuId,
                    lockedQty = SqlFunc.AggregateSum(dispatch.Qty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .LeftJoin<Location>((processDetail, location) => processDetail.LocationId == location.LocationId)
                .Where((processDetail, location) => processDetail.IsUpate == 0 && processDetail.IsSource == 0)
                .GroupBy((processDetail, location) => processDetail.SkuId)
                .Select((processDetail, location) => new
                {
                    skuId = processDetail.SkuId,
                    lockedQty = SqlFunc.AggregateSum(processDetail.Qty),
                    normalLockedQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5, processDetail.Qty, 0))
                });

            var moveLockedGroupDatas = Context.Queryable<Move>()
                .LeftJoin<Location>((move, location) => move.OrigLocationId == location.LocationId)
                .Where((move, location) => move.MoveStatus == 0)
                .GroupBy((move, location) => move.SkuId)
                .Select((move, location) => new
                {
                    skuId = move.SkuId,
                    lockedQty = SqlFunc.AggregateSum(move.Qty),
                    normalLockedQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5, move.Qty, 0))
                });

            var query = Context.Queryable<CommoditySKU>()
                .LeftJoin(asnGroupDatas, (sku, asn) => sku.CommoditySKUId == asn.skuId)
                .LeftJoin(stockGroupDatas, (sku, asn, stock) => sku.CommoditySKUId == stock.skuId)
                .LeftJoin(dispatchGroupDatas, (sku, asn, stock, dispatch) => stock.skuId == dispatch.skuId)
                .LeftJoin(processLockedGroupDatas, (sku, asn, stock, dispatch, process) => sku.CommoditySKUId == process.skuId)
                .LeftJoin(moveLockedGroupDatas, (sku, asn, stock, dispatch, process, move) => sku.CommoditySKUId == move.skuId)
                .LeftJoin<CommoditySPU>((sku, asn, stock, dispatch, process, move, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Select((sku, asn, stock, dispatch, process, move, spu) => new StockVm
                {
                    SkuId = sku.CommoditySKUId,
                    SpuName = spu.CommoditySPUName,
                    SpuCode = spu.CommoditySPUCode,
                    SkuCode = sku.CommoditySKUCode,
                    AsnQty = asn.asnQty == null ? 0 : asn.asnQty,
                    AvailableQty = (stock.normalQty == null ? 0 : stock.normalQty) - (stock.normalFrozenQty == null ? 0 : stock.normalFrozenQty) - (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - (process.normalLockedQty == null ? 0 : process.normalLockedQty) - (move.normalLockedQty == null ? 0 : move.normalLockedQty),
                    FrozenQty = stock.frozenQty == null ? 0 : stock.frozenQty,
                    LockedQty = (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) + (process.lockedQty == null ? 0 : process.lockedQty) + (move.lockedQty == null ? 0 : move.lockedQty),
                    SortedQty = asn.sortedQty == null ? 0 : asn.sortedQty,
                    ToSortQty = asn.toSortQty == null ? 0 : asn.toSortQty,
                    ShortageQty = asn.shortageQty == null ? 0 : asn.shortageQty,
                    ToUnloadQty = asn.toUnloadQty == null ? 0 : asn.toUnloadQty,
                    Qty = stock.qty == null ? 0 : stock.qty
                }, true)
                .MergeTable()
                .Where(entity => entity.AsnQty > 0 || entity.Qty > 0);

            var result = query.Where(expression.ToExpression())
                .ToPage(stockQueryDto);

            return result;
        }
        public List<StockVm> GetStock()
        {
            var stockGroupDatas = Queryable()
                .LeftJoin<Location>((stock, location) => stock.LocationId == location.LocationId)
                .GroupBy((stock, location) => stock.SkuId)
                .Select((stock, location) => new
                {
                    skuId = stock.SkuId,
                    frozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(stock.IsFreeze == 1, stock.Qty, 0)),
                    qty = SqlFunc.AggregateSum(stock.Qty),
                    normalQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5, stock.Qty, 0)),
                    normalFrozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5 && stock.IsFreeze == 1, stock.Qty, 0))
                });

            var asnGroupDatas = Context.Queryable<Asn>()
                .GroupBy(asn => asn.SkuId)
                .Select(asn => new
                {
                    skuId = asn.SkuId,
                    asnQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 0, asn.AsnQty, 0)),
                    toUnloadQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 1, asn.AsnQty, 0)),
                    toSortQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 2, asn.AsnQty, 0)),
                    sortedQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 3, asn.SortedQty, 0)),
                    shortageQty = SqlFunc.AggregateSum(SqlFunc.IIF(asn.AsnStatus == 4, asn.ShortageQty, 0))
                });

            var dispatchGroupDatas = Context.Queryable<Dispatch>()
                .GroupBy(dispatch => dispatch.SkuId)
                .Select(dispatch => new
                {
                    skuId = dispatch.SkuId,
                    lockedQty = SqlFunc.AggregateSum(dispatch.Qty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .LeftJoin<Location>((processDetail, location) => processDetail.LocationId == location.LocationId)
                .Where((processDetail, location) => processDetail.IsUpate == 0 && processDetail.IsSource == 0)
                .GroupBy((processDetail, location) => processDetail.SkuId)
                .Select((processDetail, location) => new
                {
                    skuId = processDetail.SkuId,
                    lockedQty = SqlFunc.AggregateSum(processDetail.Qty),
                    normalLockedQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5, processDetail.Qty, 0))
                });

            var moveLockedGroupDatas = Context.Queryable<Move>()
                .LeftJoin<Location>((move, location) => move.OrigLocationId == location.LocationId)
                .Where((move, location) => move.MoveStatus == 0)
                .GroupBy((move, location) => move.SkuId)
                .Select((move, location) => new
                {
                    skuId = move.SkuId,
                    lockedQty = SqlFunc.AggregateSum(move.Qty),
                    normalLockedQty = SqlFunc.AggregateSum(SqlFunc.IIF(location.RegionProperty != 5, move.Qty, 0))
                });

            var query = Context.Queryable<CommoditySKU>()
                .LeftJoin(asnGroupDatas, (sku, asn) => sku.CommoditySKUId == asn.skuId)
                .LeftJoin(stockGroupDatas, (sku, asn, stock) => sku.CommoditySKUId == stock.skuId)
                .LeftJoin(dispatchGroupDatas, (sku, asn, stock, dispatch) => stock.skuId == dispatch.skuId)
                .LeftJoin(processLockedGroupDatas, (sku, asn, stock, dispatch, process) => sku.CommoditySKUId == process.skuId)
                .LeftJoin(moveLockedGroupDatas, (sku, asn, stock, dispatch, process, move) => sku.CommoditySKUId == move.skuId)
                .LeftJoin<CommoditySPU>((sku, asn, stock, dispatch, process, move, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Select((sku, asn, stock, dispatch, process, move, spu) => new StockVm
                {
                    SkuId = sku.CommoditySKUId,
                    SpuName = spu.CommoditySPUName,
                    SpuCode = spu.CommoditySPUCode,
                    SkuCode = sku.CommoditySKUCode,
                    AsnQty = asn.asnQty == null ? 0 : asn.asnQty,
                    AvailableQty = (stock.normalQty == null ? 0 : stock.normalQty) - (stock.normalFrozenQty == null ? 0 : stock.normalFrozenQty) - (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - (process.normalLockedQty == null ? 0 : process.normalLockedQty) - (move.normalLockedQty == null ? 0 : move.normalLockedQty),
                    FrozenQty = stock.frozenQty == null ? 0 : stock.frozenQty,
                    LockedQty = (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) + (process.lockedQty == null ? 0 : process.lockedQty) + (move.lockedQty == null ? 0 : move.lockedQty),
                    SortedQty = asn.sortedQty == null ? 0 : asn.sortedQty,
                    ToSortQty = asn.toSortQty == null ? 0 : asn.toSortQty,
                    ShortageQty = asn.shortageQty == null ? 0 : asn.shortageQty,
                    ToUnloadQty = asn.toUnloadQty == null ? 0 : asn.toUnloadQty,
                    Qty = stock.qty == null ? 0 : stock.qty
                }, true)
                .MergeTable()
                .Where(entity => entity.AsnQty > 0 || entity.Qty > 0);

            var result = query.ToList();

            return result;
        }

        public PagedInfo<LocationStockVm> GetLocationStock(LocationStockQueryDto locationStockQueryDto)
        {
            var expression = Expressionable.Create<LocationStockVm>()
                .AndIF(!string.IsNullOrEmpty(locationStockQueryDto.LocationCode), entity => entity.LocationCode.Contains(locationStockQueryDto.LocationCode))
                .AndIF(locationStockQueryDto.BeginTime != DateTime.MinValue && locationStockQueryDto.BeginTime != null, exp => exp.Create_time >= locationStockQueryDto.BeginTime)
                .AndIF(locationStockQueryDto.EndTime != DateTime.MaxValue && locationStockQueryDto.EndTime != null, exp => exp.Create_time <= locationStockQueryDto.EndTime);

            var stockGroupDatas = Queryable()
                .GroupBy(stock => new
                {
                    stock.SkuId,
                    stock.LocationId
                })
                .Select(stock => new
                {
                    skuId = stock.SkuId,
                    locationId = stock.LocationId,
                    frozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(stock.IsFreeze == 1, stock.Qty, 0)),
                    qty = SqlFunc.AggregateSum(stock.Qty)
                });

            var dispatchGroupDatas = Context.Queryable<Dispatch>()
                .LeftJoin<Dispatchpick>((dp, dpp) => dp.DispatchId == dpp.DispatchId)
                .Where((dp, dpp) => dp.DispatchStatus > 1 && dp.DispatchStatus < 6)
                .GroupBy((dp, dpp) => new
                {
                    dpp.SkuId,
                    dpp.LocationId,
                })
                .Select((dp, dpp) => new
                {
                    skuId = dpp.SkuId,
                    locationId = dpp.LocationId,
                    lockedQty = SqlFunc.AggregateSum(dpp.PickedQty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .Where(processDetail => processDetail.IsUpate == 0 && processDetail.IsSource == 0)
                .GroupBy(processDetail => new
                {
                    processDetail.SkuId,
                    processDetail.LocationId,
                })
                .Select(processDetail => new
                {
                    skuId = processDetail.SkuId,
                    locationId = processDetail.LocationId,
                    lockedQty = SqlFunc.AggregateSum(processDetail.Qty)
                });

            var moveLockedGroupDatas = Context.Queryable<Move>()
                .Where(move => move.MoveStatus == 0)
                .GroupBy(move => new
                {
                    move.SkuId,
                    move.OrigLocationId
                })
                .Select(move => new
                {
                    skuId = move.SkuId,
                    locationId = move.OrigLocationId,
                    lockedQty = SqlFunc.AggregateSum(move.Qty)
                });

            var query = stockGroupDatas.LeftJoin(dispatchGroupDatas, (stock, dispatch) => stock.skuId == dispatch.skuId && stock.locationId == dispatch.locationId)
                .LeftJoin(processLockedGroupDatas, (stock, dispatch, process) => stock.skuId == process.skuId && stock.locationId == process.locationId)
                .LeftJoin(moveLockedGroupDatas, (stock, dispatch, process, move) => stock.skuId == move.skuId && stock.locationId == move.locationId)
                .LeftJoin<CommoditySKU>((stock, dispatch, process, move, sku) => stock.skuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((stock, dispatch, process, move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((stock, dispatch, process, move, sku, spu, location) => stock.locationId == location.LocationId)
                .Select((stock, dispatch, process, move, sku, spu, location) => new LocationStockVm
                {
                    SkuId = stock.skuId,
                    SpuName = spu.CommoditySPUName,
                    SpuCode = spu.CommoditySPUCode,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    AvailableQty = location.RegionProperty == 5 ? 0 : (stock.qty - stock.frozenQty - (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - (process.lockedQty == null ? 0 : process.lockedQty) - (move.lockedQty == null ? 0 : move.lockedQty)),
                    FrozenQty = stock.frozenQty,
                    LockedQty = (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) + (process.lockedQty == null ? 0 : process.lockedQty) + (move.lockedQty == null ? 0 : move.lockedQty),
                    Qty = stock.qty,
                    LocationCode = location.LocationCode,
                    WarehouseName = location.WarehouseName
                }, true)
                .MergeTable()
                .Where(entity => entity.Qty > 0);

            var result = query.Where(expression.ToExpression())
                .ToPage(locationStockQueryDto);

            return result;
        }

        public List<LocationStockVm> GetLocationStock()
        {
            var stockGroupDatas = Queryable()
                .GroupBy(stock => new
                {
                    stock.SkuId,
                    stock.LocationId
                })
                .Select(stock => new
                {
                    skuId = stock.SkuId,
                    locationId = stock.LocationId,
                    frozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(stock.IsFreeze == 1, stock.Qty, 0)),
                    qty = SqlFunc.AggregateSum(stock.Qty)
                });

            var dispatchGroupDatas = Context.Queryable<Dispatch>()
                .LeftJoin<Dispatchpick>((dp, dpp) => dp.DispatchId == dpp.DispatchId)
                .Where((dp, dpp) => dp.DispatchStatus > 1 && dp.DispatchStatus < 6)
                .GroupBy((dp, dpp) => new
                {
                    dpp.SkuId,
                    dpp.LocationId,
                })
                .Select((dp, dpp) => new
                {
                    skuId = dpp.SkuId,
                    locationId = dpp.LocationId,
                    lockedQty = SqlFunc.AggregateSum(dpp.PickedQty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .Where(processDetail => processDetail.IsUpate == 0 && processDetail.IsSource == 0)
                .GroupBy(processDetail => new
                {
                    processDetail.SkuId,
                    processDetail.LocationId,
                })
                .Select(processDetail => new
                {
                    skuId = processDetail.SkuId,
                    locationId = processDetail.LocationId,
                    lockedQty = SqlFunc.AggregateSum(processDetail.Qty)
                });

            var moveLockedGroupDatas = Context.Queryable<Move>()
                .Where(move => move.MoveStatus == 0)
                .GroupBy(move => new
                {
                    move.SkuId,
                    move.OrigLocationId
                })
                .Select(move => new
                {
                    skuId = move.SkuId,
                    locationId = move.OrigLocationId,
                    lockedQty = SqlFunc.AggregateSum(move.Qty)
                });

            var query = stockGroupDatas.LeftJoin(dispatchGroupDatas, (stock, dispatch) => stock.skuId == dispatch.skuId && stock.locationId == dispatch.locationId)
                .LeftJoin(processLockedGroupDatas, (stock, dispatch, process) => stock.skuId == process.skuId && stock.locationId == process.locationId)
                .LeftJoin(moveLockedGroupDatas, (stock, dispatch, process, move) => stock.skuId == move.skuId && stock.locationId == move.locationId)
                .LeftJoin<CommoditySKU>((stock, dispatch, process, move, sku) => stock.skuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((stock, dispatch, process, move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((stock, dispatch, process, move, sku, spu, location) => stock.locationId == location.LocationId)
                .Select((stock, dispatch, process, move, sku, spu, location) => new LocationStockVm
                {
                    SkuId = stock.skuId,
                    SpuName = spu.CommoditySPUName,
                    SpuCode = spu.CommoditySPUCode,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    AvailableQty = location.RegionProperty == 5 ? 0 : (stock.qty - stock.frozenQty - (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - (process.lockedQty == null ? 0 : process.lockedQty) - (move.lockedQty == null ? 0 : move.lockedQty)),
                    FrozenQty = stock.frozenQty,
                    LockedQty = (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) + (process.lockedQty == null ? 0 : process.lockedQty) + (move.lockedQty == null ? 0 : move.lockedQty),
                    Qty = stock.qty,
                    LocationCode = location.LocationCode,
                    WarehouseName = location.WarehouseName
                }, true)
                .MergeTable()
                .Where(entity => entity.Qty > 0);

            var result = query.ToList();

            return result;
        }

        public PagedInfo<StockSelectVm> GetStockSelect(StockSelectQueryDto stockSelectQueryDto)
        {
            var dispatchGroupDatas = Context.Queryable<Dispatch>()
                .LeftJoin<Dispatchpick>((dp, dpp) => dp.DispatchId == dpp.DispatchId)
                .Where((dp, dpp) => dp.DispatchStatus > 1 && dp.DispatchStatus < 6)
                .GroupBy((dp, dpp) => new
                {
                    dpp.SkuId,
                    dpp.LocationId,
                    dpp.OwnerId
                })
                .Select((dp, dpp) => new
                {
                    ownerId = dpp.OwnerId,
                    skuId = dpp.SkuId,
                    locationId = dpp.LocationId,
                    lockedQty = SqlFunc.AggregateSum(dpp.PickQty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .Where(proDet => proDet.IsUpate == 0 && proDet.IsSource == 0)
                .GroupBy(proDet => new
                {
                    proDet.SkuId,
                    proDet.LocationId,
                    proDet.OwnerId
                })
                .Select(proDet => new
                {
                    ownerId = proDet.OwnerId,
                    skuId = proDet.SkuId,
                    locationId = proDet.LocationId,
                    lockedQty = SqlFunc.AggregateSum(proDet.Qty)
                });

            var moveLockedGroupDatas = Context.Queryable<Move>()
                .Where(mov => mov.MoveStatus == 0)
                .GroupBy(mov => new
                {
                    mov.SkuId,
                    mov.OrigLocationId,
                    mov.OwnerId
                })
                .Select(mov => new
                {
                    ownerId = mov.OwnerId,
                    skuId = mov.SkuId,
                    locationId = mov.OrigLocationId,
                    lockedQty = SqlFunc.AggregateSum(mov.Qty)
                });

            // 这段代码会转换为SQL代码，在代码层面可能不符合逻辑，但在SQL层面却符合逻辑
            var query = Queryable()
                .LeftJoin(dispatchGroupDatas, (stock, dispatch) => stock.SkuId == dispatch.skuId && stock.LocationId == dispatch.locationId && stock.OwnerId == dispatch.ownerId)
                .LeftJoin(processLockedGroupDatas, (stock, dispatch, process) => stock.SkuId == process.skuId && stock.LocationId == process.locationId && stock.OwnerId == process.ownerId)
                .LeftJoin(moveLockedGroupDatas, (stock, dispatch, process, move) => stock.SkuId == move.skuId && stock.LocationId == move.locationId && stock.OwnerId == move.ownerId)
                .LeftJoin<CommoditySKU>((stock, dispatch, process, move, sku) => stock.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((stock, dispatch, process, move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Location>((stock, dispatch, process, move, sku, spu, location) => stock.LocationId == location.LocationId)
                .LeftJoin<Owner>((stock, dispatch, process, move, sku, spu, location, owner) => stock.OwnerId == owner.OwnerId)
                .GroupBy((stock, dispatch, process, move, sku, spu, location, owner) => new
                {
                    skuId = stock.SkuId,
                    spuName = spu.CommoditySPUName,
                    spuCode = spu.CommoditySPUCode,
                    skuName = sku.CommoditySKUName,
                    skuCode = sku.CommoditySKUCode,
                    locationId = stock.LocationId,
                    ownerId = stock.OwnerId,
                    ownerName = owner.OwnerName,
                    qty = stock.Qty,
                    locationCode = location.LocationCode,
                    isFreeze = stock.IsFreeze,
                    warehouseName = location.WarehouseName,
                    stockId = stock.StockId,
                    unit = sku.Unit
                })
                .Select((stock, dispatch, process, move, sku, spu, location, owner) => new StockSelectVm
                {
                    SkuId = stock.SkuId,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    AvailableQty = stock.IsFreeze == 1 ? 0 : (stock.Qty - SqlFunc.AggregateSum(dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - SqlFunc.AggregateSum(process.lockedQty == null ? 0 : process.lockedQty) - SqlFunc.AggregateSum(move.lockedQty == null ? 0 : move.lockedQty)), //代码中可能不为空，但在SQL层面是会等于空的
                    Qty = stock.Qty,
                    LocationId = location.LocationId,
                    OwnerId = owner.OwnerId,
                    OwnerName = owner.OwnerName,
                    LocationCode = location.LocationCode,
                    WarehouseName = location.WarehouseName,
                    IsFreeze = stock.IsFreeze,
                    StockId = stock.StockId,
                    Unit = sku.Unit
                })
                .MergeTable();

            var result = query
                .WhereIF(!string.IsNullOrEmpty(stockSelectQueryDto.WarehouseName), entity => entity.WarehouseName.Contains(stockSelectQueryDto.WarehouseName))
                .WhereIF(!string.IsNullOrEmpty(stockSelectQueryDto.LocationCode), entity => entity.LocationCode.Contains(stockSelectQueryDto.LocationCode))
                .WhereIF(!string.IsNullOrEmpty(stockSelectQueryDto.SpuName), entity => entity.SpuName.Contains(stockSelectQueryDto.SpuName))
                .WhereIF(!string.IsNullOrEmpty(stockSelectQueryDto.SkuCode), entity => entity.SkuCode.Contains(stockSelectQueryDto.SkuCode))
                .WhereIF(stockSelectQueryDto.SqlTitle == null, entity => entity.AvailableQty > 0)
                //.WhereIF(stockSelectQueryDto.SqlTitle == string.Empty, entity => entity.AvailableQty > 0 && entity.IsFreeze == 0)
                .WhereIF(stockSelectQueryDto.SqlTitle == "all", entity => true)
                .WhereIF(stockSelectQueryDto.SqlTitle == "forzen", entity => entity.IsFreeze == 1)
                .ToPage(stockSelectQueryDto);

            return result;
        }

        public PagedInfo<SkuSelectVm> GetSkuSelect(SkuSelectQueryDto commoditySkuSelectQueryDto)
        {
            return Context.Queryable<CommoditySKU>()
                .LeftJoin<CommoditySPU>((sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .WhereIF(!string.IsNullOrEmpty(commoditySkuSelectQueryDto.SpuName), (sku, spu) => spu.CommoditySPUName.Contains(commoditySkuSelectQueryDto.SpuName))
                .WhereIF(!string.IsNullOrEmpty(commoditySkuSelectQueryDto.SpuName), (sku, spu) => sku.CommoditySKUCode.Contains(commoditySkuSelectQueryDto.SkuCode))
                .Select((sku, spu) => new SkuSelectVm
                {
                    SkuId = sku.CommoditySKUId,
                    SkuCode = sku.CommoditySKUCode,
                    SkuName = sku.CommoditySKUName,
                    SpuId = spu.CommoditySPUId,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    SupplierId = spu.SupplierId,
                    SupplierName = spu.SupplierName,
                    Brand = spu.Brand,
                    Unit = sku.Unit
                })
                .ToPage(commoditySkuSelectQueryDto);
        }

    }
}
