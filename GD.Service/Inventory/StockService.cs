using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Delivery;
using GD.Model.Dto.Inventory;
using GD.Model.Inventory;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Inventory;
using GD.Repository;
using GD.Service.Interface.Inventory;
using Microsoft.AspNetCore.Components;
using SqlSugar;
using System.Diagnostics;

namespace GD.Service.Inventory
{
    [AppService(ServiceType = typeof(IStockService), ServiceLifetime = LifeTime.Transient)]
    public class StockService : BaseService<Stock>, IStockService
    {
        public PagedInfo<SkuSelectVm> GetCommoditySkuSelect(SkuSelectQueryDto commoditySkuSelectQueryDto)
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

        public PagedInfo<StockSelectVm> GetStockSelect(StockSelectDto stockSelectDto)
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
                    mov.OriginLocationId,
                    mov.OwnerId
                })
                .Select(mov => new
                {
                    ownerId = mov.OwnerId,
                    skuId = mov.SkuId,
                    locationId = mov.OriginLocationId,
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
                    AvailableQty = stock.IsFreeze == 1 ? 0 : (stock.Qty - SqlFunc.AggregateSum(dispatch.lockedQty==null?0: dispatch.lockedQty) - SqlFunc.AggregateSum(process.lockedQty == null ? 0 : process.lockedQty) - SqlFunc.AggregateSum(move.lockedQty == null ? 0 : move.lockedQty)), //代码中可能不为空，但在SQL层面是会等于空的
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
                .WhereIF(!string.IsNullOrEmpty(stockSelectDto.LocationCode), entity => entity.LocationCode.Contains(stockSelectDto.LocationCode))
                .WhereIF(!string.IsNullOrEmpty(stockSelectDto.SpuName), entity => entity.SpuName.Contains(stockSelectDto.SpuName))
                .WhereIF(!string.IsNullOrEmpty(stockSelectDto.SkuCode), entity => entity.SkuCode.Contains(stockSelectDto.SkuCode))
                .WhereIF(stockSelectDto.SqlTitle == string.Empty, entity => entity.AvailableQty > 0)
                .WhereIF(stockSelectDto.SqlTitle == "all", entity => true)
                .WhereIF(stockSelectDto.SqlTitle == "forzen", entity => entity.IsFreeze == 1)
                .ToPage(stockSelectDto);

            return result;
        }
    }
}
