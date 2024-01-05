using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Delivery;
using GD.Model.Dto.Delivery;
using GD.Model.Inventory;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Delivery;
using GD.Model.Vm.Inventory;
using GD.Repository;
using GD.Service.Interface.Delivery;
using Mapster;
using SqlSugar;

namespace GD.Service.Delivery
{
    [AppService(ServiceType = typeof(IDispatchService), ServiceLifetime = LifeTime.Transient)]
    public class DispatchService : BaseService<Dispatch>, IDispatchService
    {
        public (bool, string) Add(List<DispatchAddDto> dispatchAddDtos, string userName)
        {
            var entities = dispatchAddDtos.Select(it => it.Adapt<Dispatch>()).ToList();
            var skuIdList = entities.Select(it => it.SkuId).ToList();
            var skus = Context.Queryable<CommoditySKU>()
                .Where(it => skuIdList.Contains(it.CommoditySKUId))
                .ToList();
            var dispatchNo = GetDispatchNo();

            foreach (var entity in entities)
            {
                var sku = skus.FirstOrDefault(it => it.CommoditySKUId == entity.SkuId);
                entity.Create_by = userName;
                if (sku != null)
                {
                    entity.Volume = entity.Qty * sku.Volume;
                    entity.Weight = entity.Qty * sku.Weight;
                }
                entity.DispatchNo = dispatchNo;
            }
            var result = Insert(entities);
            if (result > 0)
            {
                return (true, "新增成功");
            }
            else
            {
                return (false, "新增失败");
            }
        }

        public (bool, string) CancelDispatchDetailOpration(long dispatchId)
        {
            var entity = Queryable().First(it => it.DispatchId == dispatchId);
            if (entity == null)
            {
                return (false, "数据不存在");
            }
            if (entity.DispatchStatus == 4)
            {
                if (entity.WeighingNo == "")
                {
                    entity.DispatchStatus = 3;
                }
                else
                {
                    entity.DispatchStatus = 5;
                }
                entity.PackageNo = "";
                entity.PackageQty = 0;
                entity.PackageTime = DateTime.Now;
                entity.PackagePerson = "";
            }
            else if (entity.DispatchStatus == 5)
            {
                if (entity.PackageNo == "")
                {
                    entity.DispatchStatus = 3;
                }
                else
                {
                    entity.DispatchStatus = 4;
                }
                entity.WeighingNo = "";
                entity.WeighingQty = 0;
                entity.WeighingWeight = 0;
                entity.WeighingPerson = "";
            }
            else
            {
                return (false, "状态发生改变");
            }
            var result = Update(entity);

            if (result > 0)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public (bool, string) CancelOrderOpration(CancelOrderOprationDto cancelOrderOpration, string userName)
        {
            var result = UseTran(() =>
            {
                var entities = Queryable()
                    .Where(it => it.DispatchNo == cancelOrderOpration.DispatchNo && it.DispatchStatus == cancelOrderOpration.DispatchStatus)
                    .ToList();
                if (entities.Count == 0)
                {
                    throw new Exception("状态发送改变");
                }
                var dispatchIdList = entities.Select(it => it.DispatchId).ToList();
                var pickEntities = Context.Queryable<Dispatchpick>()
                    .Where(it => dispatchIdList.Contains(it.DispatchId))
                    .ToList();
                if (cancelOrderOpration.DispatchStatus == 3)
                {
                    foreach (var pick in pickEntities)
                    {
                        pick.PickedQty = 0;
                    }
                    Context.Insertable(pickEntities).ExecuteCommand();

                    foreach (var entity in entities)
                    {
                        entity.PickedQty = 0;
                        entity.DispatchStatus = 2;
                    }
                }
                else if (cancelOrderOpration.DispatchStatus == 2)
                {
                    Context.Deleteable(pickEntities).ExecuteCommand();

                    foreach (var entity in entities)
                    {
                        entity.LockQty = 0;
                        entity.DispatchStatus = 1;
                    }
                }

                Insert(entities);
            });

            if (result.IsSuccess == true)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public (bool, string) ConfirmOrder(List<DispatchConfirmDetailDto> dispatchConfirmDetailDtos, string userName)
        {
            var result = UseTran(() =>
            {
                var dispatchIdList = dispatchConfirmDetailDtos.Select(it => it.DispatchId).ToList();
                var dispatchDatas = Queryable().Where(it => dispatchIdList.Contains(it.DispatchId)).ToList();
                var pickDatas = new List<Dispatchpick>();
                var stockIdList = new List<long>();
                var newDispatchs = new List<Dispatch>();
                var topickVms = new List<StockVm>();
                var skuIdList = dispatchConfirmDetailDtos.Select(it => it.SkuId).ToList();
                foreach (var item in dispatchConfirmDetailDtos.Where(it => it.Confirm == true).ToList())
                {
                    var collection = item.PickList.Where(it => it.PickQty > 0)
                        .Select(it => it.StockId)
                        .ToList();
                    skuIdList.AddRange(collection);
                }

                foreach (var item in dispatchConfirmDetailDtos)
                {
                    var dispatch = dispatchDatas.Where(it => it.DispatchId == item.DispatchId)
                        .FirstOrDefault() ?? throw new Exception("数据发生改变");
                    if (item.Confirm == true)
                    {
                        dispatch.DispatchStatus = 2;
                        dispatch.Update_by = userName;
                        dispatch.LockQty = item.PickList.Sum(it => it.PickQty);
                        foreach (var detail in item.PickList.Where(it => it.PickQty > 0).ToList())
                        {
                            pickDatas.Add(new Dispatchpick
                            {
                                SkuId = item.SkuId,
                                IsUpate = 0,
                                DispatchId = detail.DispatchId,
                                LocationId = detail.LocationId,
                                OwnerId = detail.OwnerId,
                                Create_by = userName,
                                PickQty = detail.PickQty
                            });
                            topickVms.Add(new StockVm
                            {
                                StockId = detail.StockId,
                                Qty = detail.PickQty,
                            });
                        }
                        if (dispatch.LockQty < dispatch.Qty)
                        {
                            newDispatchs.Add(new Dispatch
                            {
                                SkuId = item.SkuId,
                                DispatchStatus = 1,
                                Qty = dispatch.Qty - dispatch.LockQty,
                            });
                            dispatch.Qty = dispatch.LockQty;
                        }
                        Update(dispatch);
                    }
                    else
                    {
                        newDispatchs.Add(new Dispatch
                        {
                            SkuId = item.SkuId,
                            DispatchStatus = 1,
                            Qty = item.Qty
                        });
                        Delete(dispatch.DispatchId);
                    }
                }

                var stockGroupDatas = Context.Queryable<Stock>()
                    .Where(stock => stockIdList.Contains(stock.StockId))
                    .GroupBy(stock => new
                    {
                        stock.StockId,
                        stock.SkuId,
                        stock.LocationId,
                        stock.OwnerId
                    })
                    .Select(stock => new
                    {
                        stockId = stock.StockId,
                        ownerId = stock.OwnerId,
                        skuId = stock.SkuId,
                        locationId = stock.LocationId,
                        frozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(stock.IsFreeze == 1, stock.Qty, 0)),
                        qty = SqlFunc.AggregateSum(stock.Qty)
                    });

                var dispatchGroupDatas = Queryable()
                    .LeftJoin<Dispatchpick>((dispatch, dispatchpick) => dispatch.DispatchId == dispatchpick.DispatchId)
                    .Where((dispatch, dispatchpick) => dispatch.DispatchStatus > 1 && dispatch.DispatchStatus < 6)
                    .GroupBy((dispatch, dispatchpick) => new
                    {
                        dispatchpick.SkuId,
                        dispatchpick.LocationId,
                        dispatchpick.OwnerId
                    })
                    .Select((dispatch, dispatchpick) => new
                    {
                        ownerId = dispatchpick.OwnerId,
                        skuId = dispatchpick.SkuId,
                        locationId = dispatchpick.LocationId,
                        lockedQty = SqlFunc.AggregateSum(dispatchpick.PickQty)
                    });

                var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                    .Where(processDetail => processDetail.IsUpate == 0)
                    .GroupBy(processDetail => new
                    {
                        processDetail.SkuId,
                        processDetail.LocationId,
                        processDetail.OwnerId
                    })
                    .Select(processDetail => new
                    {
                        ownerId = processDetail.OwnerId,
                        skuId = processDetail.SkuId,
                        locationId = processDetail.LocationId,
                        lockedQty = SqlFunc.AggregateSum(processDetail.Qty)
                    });

                var moveLockedGroupDatas = Context.Queryable<Move>()
                    .Where(move => move.MoveStatus == 0)
                    .GroupBy(move => new
                    {
                        move.SkuId,
                        move.OrigLocationId,
                        move.OwnerId
                    })
                    .Select(move => new
                    {
                        ownerId = move.OwnerId,
                        skuId = move.SkuId,
                        locationId = move.OrigLocationId,
                        lockedQty = SqlFunc.AggregateSum(move.Qty)
                    });

                var stockDatas = stockGroupDatas.LeftJoin(dispatchGroupDatas, (stock, dispatch) => stock.skuId == dispatch.skuId && stock.locationId == dispatch.locationId && stock.ownerId == dispatch.ownerId)
                    .LeftJoin(processLockedGroupDatas, (stock, dispatch, process) => stock.skuId == process.skuId && stock.locationId == process.locationId && stock.ownerId == process.ownerId)
                    .LeftJoin(moveLockedGroupDatas, (stock, dispatch, process, move) => stock.skuId == move.skuId && stock.locationId == move.locationId && stock.ownerId == move.ownerId)
                    .Select((stock, dispatch, process, move) => new
                    {
                        stockId = stock.stockId,
                        availableQty = (stock.qty == null ? 0 : stock.qty) - (stock.frozenQty == null ? 0 : stock.frozenQty) - (dispatch.lockedQty == null ? 0 : dispatch.lockedQty) - (process.lockedQty == null ? 0 : process.lockedQty) - (move.lockedQty == null ? 0 : move.lockedQty)
                    })
                    .ToList();

                var ifNotStock = topickVms.Join(stockDatas, topickVms => topickVms.StockId, stockDatas => stockDatas.stockId, (topickVms, stockDatas) => topickVms.Qty > stockDatas.availableQty ? topickVms : null)
                    .Any(result => result != null);
                if (ifNotStock == true)
                {
                    throw new Exception("数据发生改变");
                }

                Context.Insertable(pickDatas).ExecuteCommand();

                var dispatchNo = GetDispatchNo();
                var skuDatas = Context.Queryable<CommoditySKU>()
                    .Where(it => skuIdList.Contains(it.CommoditySKUId))
                    .ToList();
                foreach (var nb in newDispatchs)
                {
                    nb.DispatchNo = dispatchNo;
                    nb.Create_by = userName;
                    var sku = skuDatas.FirstOrDefault(it => it.CommoditySKUId == nb.SkuId);
                    if (sku != null)
                    {
                        nb.Weight = nb.Qty * sku.Weight;
                        nb.Volume = nb.Qty * sku.Volume;
                    }
                }
                Insert(newDispatchs);
            });

            if (result.IsSuccess == true)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public List<DispatchConfirmDetailVm> ConfirmOrderCheck(string dispatchNo)
        {
            var stockGroupDatas = Context.Queryable<Stock>()
                .LeftJoin<Location>((stock, location) => stock.LocationId == location.LocationId)
                .GroupBy((stock, location) => new
                {
                    stock.StockId,
                    stock.SkuId,
                    stock.LocationId,
                    stock.OwnerId
                })
                .Select((stock, location) => new
                {
                    stockId = stock.StockId,
                    ownerId = stock.OwnerId,
                    skuId = stock.SkuId,
                    locationId = location.LocationId,
                    frozenQty = SqlFunc.AggregateSum(SqlFunc.IIF(stock.IsFreeze == 1, stock.Qty, 0)),
                    qty = SqlFunc.AggregateSum(stock.Qty)
                });

            var dispatchGroupDatas = Queryable()
                .LeftJoin<Dispatchpick>((dispatch, dispatchpick) => dispatch.DispatchId == dispatchpick.DispatchId)
                .Where((dispatch, dispatchpick) => dispatch.DispatchStatus > 1 && dispatch.DispatchStatus < 6)
                .GroupBy((dispatch, dispatchpick) => new
                {
                    dispatchpick.SkuId,
                    dispatchpick.LocationId,
                    dispatchpick.OwnerId
                })
                .Select((dispatch, dispatchpick) => new
                {
                    ownerId = dispatchpick.OwnerId,
                    skuId = dispatchpick.SkuId,
                    locationId = dispatchpick.LocationId,
                    lockedQty = SqlFunc.AggregateSum(dispatchpick.PickQty)
                });

            var processLockedGroupDatas = Context.Queryable<ProcessDetail>()
                .Where(processdetail => processdetail.IsUpate == 0)
                .GroupBy(processdetail => new
                {
                    processdetail.SkuId,
                    processdetail.LocationId,
                    processdetail.OwnerId
                })
                .Select(processdetail => new
                {
                    ownerId = processdetail.OwnerId,
                    skuId = processdetail.SkuId,
                    locationId = processdetail.LocationId,
                    lockedQty = SqlFunc.AggregateSum(processdetail.Qty)
                });

            var moveLockedGroupDatas = Context.Queryable<Move>()
                .Where(move => move.MoveStatus == 0)
                .GroupBy(move => new
                {
                    move.SkuId,
                    move.OrigLocationId,
                    move.OwnerId
                })
                .Select(move => new
                {
                    ownerId = move.OwnerId,
                    skuId = move.SkuId,
                    locationId = move.OrigLocationId,
                    lockedQty = SqlFunc.AggregateSum(move.Qty)
                });

            var query = Queryable()
                .LeftJoin(stockGroupDatas, (dispatch, stock) => dispatch.SkuId == stock.skuId)
                .LeftJoin(dispatchGroupDatas, (dispatch, stock, dispatchgroup) => stock.skuId == dispatchgroup.skuId && stock.locationId == dispatchgroup.locationId && stock.ownerId == dispatchgroup.ownerId)
                .LeftJoin(processLockedGroupDatas, (dispatch, stock, dispatchgroup, process) => stock.skuId == process.skuId && stock.locationId == process.locationId && stock.ownerId == process.ownerId)
                .LeftJoin(moveLockedGroupDatas, (dispatch, stock, dispatchgroup, process, move) => stock.skuId == move.skuId && stock.locationId == move.locationId && stock.ownerId == process.ownerId)
                .LeftJoin<CommoditySKU>((dispatch, stock, dispatchgroup, process, move, sku) => dispatch.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatch, stock, dispatchgroup, process, move, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Owner>((dispatch, stock, dispatchgroup, process, move, sku, spu, owner) => stock.ownerId == owner.OwnerId)
                .LeftJoin(Context.Queryable<Location>().Where(it => it.RegionProperty != 5),
                (dispatch, stock, dispatchgroup, process, move, sku, spu, owner, location) => stock.locationId == location.LocationId)
                .Where((dispatch, stock, dispatchgroup, process, move, sku, spu, owner, location) => dispatch.DispatchNo == dispatchNo && (dispatch.DispatchStatus == 0 || dispatch.DispatchStatus == 1))
                .Select((dispatch, stock, dispatchgroup, process, move, sku, spu, owner, location) => new
                {
                    stockId = stock.stockId == null ? 0 : stock.stockId,
                    ownerName = owner.OwnerName == null ? "" : owner.OwnerName,
                    locationId = stock.locationId == null ? 0 : stock.locationId,
                    ownerId = stock.ownerId == null ? 0 : stock.ownerId,
                    spuName = spu.CommoditySPUName,
                    spuCode = spu.CommoditySPUCode,
                    skuCode = sku.CommoditySKUCode,
                    availableQty = (stock.qty == null ? 0 : stock.qty) - (stock.frozenQty == null ? 0 : stock.frozenQty) - (dispatchgroup.lockedQty == null ? 0 : dispatchgroup.lockedQty) - (process.lockedQty == null ? 0 : process.lockedQty) - (move.lockedQty == null ? 0 : move.lockedQty),
                    qty = dispatch.Qty,
                    skuId = dispatch.SkuId,
                    id = dispatch.DispatchId,
                    spuDescription = spu.CommoditySPUDescription,
                    dispatchStatus = dispatch.DispatchStatus,
                    barCode = spu.BarCode,
                    customerId = dispatch.CustomerId,
                    customerName = dispatch.CustomerName,
                    dispatchNo = dispatch.DispatchNo,
                    locationCode = location.LocationCode == null ? "" : location.LocationCode,
                    regionName = location.RegionName == null ? "" : location.RegionName,
                    warehouseName = location.WarehouseName == null ? "" : location.WarehouseName
                })
                .ToList();

            var result = query
                .GroupBy(it => new
                {
                    it.spuName,
                    it.spuCode,
                    it.skuCode,
                    it.qty,
                    it.skuId,
                    it.id,
                    it.spuDescription,
                    it.dispatchStatus,
                    it.barCode,
                    it.customerId,
                    it.customerName,
                    it.dispatchNo
                })
                .Select(it => new DispatchConfirmDetailVm
                {
                    DispatchId = it.Key.id,
                    SkuId = it.Key.skuId,
                    DispatchNo = it.Key.dispatchNo,
                    SkuCode = it.Key.skuCode,
                    SpuCode = it.Key.spuCode,
                    DispatchStatus = it.Key.dispatchStatus,
                    SpuDescription = it.Key.spuDescription,
                    SpuName = it.Key.spuName,
                    BarCode = it.Key.barCode,
                    CustomerId = it.Key.customerId,
                    CustomerName = it.Key.customerName,
                    Qty = it.Key.qty,
                    AvailableQty = it.Sum(it => it.availableQty),
                    Confirm = it.Key.qty > it.Sum(it => it.availableQty) ? false : true
                })
                .ToList();

            foreach (var item in result)
            {
                var pickList = query.Where(it => it.skuId == item.SkuId && it.stockId > 0)
                    .OrderBy(it => it.availableQty)
                    .Select(it => new DispatchConfirmPickDetailVm
                    {
                        StockId = it.stockId,
                        DispatchId = item.DispatchId,
                        LocationId = it.locationId,
                        AvailableQty = it.availableQty,
                        OwnerId = it.ownerId,
                        OwnerName = it.ownerName,
                        LocationCode = it.locationCode,
                        RegionName = it.regionName,
                        WarehouseName = it.warehouseName,
                        PickQty = 0
                    })
                    .ToList();
                int pickQty = 0;
                foreach (var pick in pickList)
                {
                    if (pickQty >= item.Qty)
                    {
                        break;
                    }
                    pick.PickQty = (item.Qty <= (pickQty + pick.AvailableQty)) ? (item.Qty - pickQty) : pick.AvailableQty;
                    pickQty += pick.PickQty;
                }
                item.PickList = pickList.Where(it => it.AvailableQty > 0).ToList();
            }

            return result;
        }

        public (bool, string) ConfirmPickByDispatchNo(string dispatchNo, string userName)
        {
            var entities = Queryable()
                .Includes(it => it.Dispatchpicklists)
                .Where(it => it.DispatchStatus == 2 && it.DispatchNo == dispatchNo)
                .ToList();

            entities.ForEach(it =>
            {
                it.PickedQty = it.LockQty;
                it.DispatchStatus = 3;
                it.Update_by = userName;
                it.Dispatchpicklists.ForEach(pick =>
                {
                    pick.PickedQty = pick.PickQty;
                    pick.Update_by = userName;
                });
            });

            var result = Context.UpdateNav(entities)
                .Include(it => it.Dispatchpicklists)
                .ExecuteCommand();

            if (result == true)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public (bool, string) Delete(string dispatchNo)
        {
            var entities = Queryable()
                .Where(it => it.DispatchNo.Equals(dispatchNo))
                .ToList();
            if (entities.Any(it => it.DispatchStatus > 1))
            {
                return (false, "该状态不允许删除");
            }

            var result = Delete(entities.ToArray());
            if (result > 0)
            {
                return (true, "删除成功");
            }
            else
            {
                return (false, "删除失败");
            }
        }

        public (bool, string) Delivery(List<DispatchDeliveryDto> dispatchDeliveryDtos, string userName)
        {
            var result = UseTran(() =>
            {
                var dispatchIdList = dispatchDeliveryDtos.Select(it => it.DispatchId)
                .ToList();
                var entities = Queryable()
                    .Where(it => dispatchIdList.Contains(it.DispatchId))
                    .ToList();
                foreach (var entity in entities)
                {
                    if (entity.DispatchStatus != 3 && entity.DispatchStatus != 4 && entity.DispatchStatus != 5)
                    {
                        throw new Exception("数据发生改变");
                    }
                    entity.DispatchStatus = 6;
                    entity.LockQty = 0;
                    entity.ActualQty = entity.PickedQty;
                    entity.IntrasitQty = entity.PickedQty;

                    Update(entity);
                }
                var pickSql = Context.Queryable<Dispatchpick>()
                    .Where(it => dispatchIdList.Contains(it.DispatchId));
                var pickDatas = pickSql
                    .ToList();
                var picksG = pickSql
                    .GroupBy(it => new
                    {
                        it.LocationId,
                        it.SkuId,
                        it.OwnerId
                    })
                    .Select(it => new
                    {
                        locationId = it.LocationId,
                        skuId = it.SkuId,
                        ownerId = it.OwnerId,
                        pickedQty = SqlFunc.AggregateSum(it.PickedQty)
                    });
                var picks = picksG
                    .ToList();
                var stocks = Context.Queryable<Stock>()
                    .Where(stock => picksG.Any(it => it.locationId == stock.LocationId && it.skuId == stock.SkuId && it.ownerId == stock.OwnerId))
                    .ToList();

                foreach (var pick in picks)
                {
                    var s = stocks.FirstOrDefault(it => it.LocationId == pick.locationId && it.SkuId == pick.skuId && it.OwnerId == pick.ownerId) ?? throw new Exception("数据发生改变");
                    s.Qty -= pick.pickedQty;
                    s.Update_by = userName;
                    Context.Updateable(s).ExecuteCommand();
                }

                foreach (var pick in pickDatas)
                {
                    pick.IsUpate = 1;
                    pick.Update_by = userName;

                    Context.Updateable(pick).ExecuteCommand();
                }
            });

            if (result.IsSuccess == true)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public List<DispatchVm> GetByDispatchNo(string dispatchNo)
        {
            var result = Queryable()
                .LeftJoin<CommoditySKU>((dispatch, sku) => dispatch.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatch, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Where((dispatch, sku, spu) => dispatch.DispatchNo.Equals(dispatchNo))
                .Select((dispatch, sku, spu) => new DispatchVm
                {
                    DispatchId = dispatch.DispatchId,
                    DispatchNo = dispatch.DispatchNo,
                    CustomerName = dispatch.CustomerName,
                    Qty = dispatch.Qty,
                    Weight = dispatch.Weight,
                    Volume = dispatch.Volume,
                    DamageQty = dispatch.DamageQty,
                    LockQty = dispatch.LockQty,
                    PickedQty = dispatch.PickedQty,
                    IntrasitQty = dispatch.IntrasitQty,
                    PackageQty = dispatch.PackageQty,
                    UnpackageQty = dispatch.PickedQty - dispatch.PackageQty,
                    WeighingQty = dispatch.WeighingQty,
                    UnweighingQty = dispatch.PickedQty - dispatch.WeighingQty,
                    ActualQty = dispatch.ActualQty,
                    SignQty = dispatch.SignQty,
                    PackageNo = dispatch.PackageNo,
                    PackagePerson = dispatch.PackagePerson,
                    PackageTime = dispatch.PackageTime,
                    WeighingNo = dispatch.WeighingNo,
                    WeighingPerson = dispatch.WeighingPerson,
                    WeighingWeight = dispatch.WeighingWeight,
                    WayBillNo = dispatch.WayBillNo,
                    Carrier = dispatch.Carrier,
                    Freightfee = dispatch.Freightfee,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuDescription = spu.CommoditySPUDescription,
                    SpuName = spu.CommoditySPUName,
                    BarCode = spu.BarCode,
                    UnpickedQty = dispatch.Qty - dispatch.PickedQty,
                    SkuName = sku.CommoditySKUName,
                    Unit = sku.Unit
                })
                .ToList();

            return result;
        }

        public PagedInfo<DispatchVm> GetDispatchAll(DispatchQueryDto dispatchQueryDto)
        {
            var expression = Expressionable.Create<DispatchVm>()
                .AndIF(!string.IsNullOrEmpty(dispatchQueryDto.DispatchNo), it => it.DispatchNo == dispatchQueryDto.DispatchNo)
                .AndIF(!string.IsNullOrEmpty(dispatchQueryDto.CustomerName), it => it.CustomerName == dispatchQueryDto.CustomerName)
                .AndIF(!string.IsNullOrEmpty(dispatchQueryDto.SpuName), it => it.SpuName == dispatchQueryDto.SpuName)
                .And(it => it.DispatchStatus == dispatchQueryDto.DispatchStatus)
                .AndIF(dispatchQueryDto.BeginTime != DateTime.MinValue && dispatchQueryDto.BeginTime != null, exp => exp.Create_time >= dispatchQueryDto.BeginTime)
                .AndIF(dispatchQueryDto.EndTime != DateTime.MaxValue && dispatchQueryDto.EndTime != null, exp => exp.Create_time <= dispatchQueryDto.EndTime);

            expression = expression.AndIF(dispatchQueryDto.SqlTitle.Equals("to_package"), it => it.PickedQty == it.Qty && it.DispatchStatus == 3 || (it.PackageQty < it.PickedQty && it.DispatchStatus == 5))
                .AndIF(dispatchQueryDto.SqlTitle.Equals("to_weight"), it => it.PickedQty == it.Qty && (it.DispatchStatus == 3 || (it.WeighingQty < it.PickedQty && it.DispatchStatus == 4)))
                .AndIF(dispatchQueryDto.SqlTitle.Equals("to_delivery"), it => it.PickedQty == it.Qty && (it.DispatchStatus == 3 || it.DispatchStatus == 4 || it.DispatchStatus == 5));

            var query = Queryable()
                .LeftJoin<CommoditySKU>((dispatch, sku) => dispatch.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatch, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Select((dispatch, sku, spu) => new DispatchVm
                {
                    DispatchId = dispatch.DispatchId,
                    DispatchNo = dispatch.DispatchNo,
                    DispatchStatus = dispatch.DispatchStatus,
                    CustomerName = dispatch.CustomerName,
                    Qty = dispatch.Qty,
                    Weight = dispatch.Weight,
                    Volume = dispatch.Volume,
                    DamageQty = dispatch.DamageQty,
                    LockQty = dispatch.LockQty,
                    PickedQty = dispatch.PickedQty,
                    IntrasitQty = dispatch.IntrasitQty,
                    PackageQty = dispatch.PackageQty,
                    UnpackageQty = dispatch.PickedQty - dispatch.PackageQty,
                    WeighingQty = dispatch.WeighingQty,
                    UnweighingQty = dispatch.PickedQty - dispatch.WeighingQty,
                    ActualQty = dispatch.ActualQty,
                    SignQty = dispatch.SignQty,
                    PackageNo = dispatch.PackageNo,
                    PackagePerson = dispatch.PackagePerson,
                    PackageTime = dispatch.PackageTime,
                    WeighingNo = dispatch.WeighingNo,
                    WeighingPerson = dispatch.WeighingPerson,
                    WeighingWeight = dispatch.WeighingWeight,
                    WayBillNo = dispatch.WeighingNo,
                    Carrier = dispatch.Carrier,
                    Freightfee = dispatch.Freightfee,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuDescription = spu.CommoditySPUDescription,
                    SpuName = spu.CommoditySPUName,
                    BarCode = spu.BarCode,
                    UnpickedQty = dispatch.Qty - dispatch.PickedQty,
                    LengthUnit = spu.LengthUnit,
                    VolumeUnit = spu.VolumeUnit,
                    WeightUnit = spu.WeightUnit
                }, true)
                .MergeTable();

            var result = query.Where(expression.ToExpression())
                .ToPage(dispatchQueryDto);

            return result;
        }

        public List<DispatchVm> GetDispatchAll(string sqlTitle)
        {
            var expression = Expressionable.Create<DispatchVm>()
                .AndIF(sqlTitle.Equals("to_package"), it => it.PickedQty == it.Qty && it.DispatchStatus == 3 || (it.PackageQty < it.PickedQty && it.DispatchStatus == 5))
                .AndIF(sqlTitle.Equals("to_weight"), it => it.PickedQty == it.Qty && (it.DispatchStatus == 3 || (it.WeighingQty < it.PickedQty && it.DispatchStatus == 4)))
                .AndIF(sqlTitle.Equals("to_delivery"), it => it.PickedQty == it.Qty && (it.DispatchStatus == 3 || it.DispatchStatus == 4 || it.DispatchStatus == 5));

            var query = Queryable()
                .LeftJoin<CommoditySKU>((dispatch, sku) => dispatch.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatch, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Select((dispatch, sku, spu) => new DispatchVm
                {
                    DispatchId = dispatch.DispatchId,
                    DispatchNo = dispatch.DispatchNo,
                    DispatchStatus = dispatch.DispatchStatus,
                    CustomerName = dispatch.CustomerName,
                    Qty = dispatch.Qty,
                    Weight = dispatch.Weight,
                    Volume = dispatch.Volume,
                    DamageQty = dispatch.DamageQty,
                    LockQty = dispatch.LockQty,
                    PickedQty = dispatch.PickedQty,
                    IntrasitQty = dispatch.IntrasitQty,
                    PackageQty = dispatch.PackageQty,
                    UnpackageQty = dispatch.PickedQty - dispatch.PackageQty,
                    WeighingQty = dispatch.WeighingQty,
                    UnweighingQty = dispatch.PickedQty - dispatch.WeighingQty,
                    ActualQty = dispatch.ActualQty,
                    SignQty = dispatch.SignQty,
                    PackageNo = dispatch.PackageNo,
                    PackagePerson = dispatch.PackagePerson,
                    PackageTime = dispatch.PackageTime,
                    WeighingNo = dispatch.WeighingNo,
                    WeighingPerson = dispatch.WeighingPerson,
                    WeighingWeight = dispatch.WeighingWeight,
                    WayBillNo = dispatch.WeighingNo,
                    Carrier = dispatch.Carrier,
                    Freightfee = dispatch.Freightfee,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuDescription = spu.CommoditySPUDescription,
                    SpuName = spu.CommoditySPUName,
                    BarCode = spu.BarCode,
                    UnpickedQty = dispatch.Qty - dispatch.PickedQty,
                    LengthUnit = spu.LengthUnit,
                    VolumeUnit = spu.VolumeUnit,
                    WeightUnit = spu.WeightUnit
                }, true)
                .MergeTable();

            var result = query.Where(expression.ToExpression())
                .ToList();

            return result;
        }

        public List<DispatchpickVm> GetPickListByDispatchId(long dispatchId)
        {
            var result = Context.Queryable<Dispatchpick>()
                .LeftJoin<CommoditySKU>((dispatchpick, sku) => dispatchpick.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatchpick, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .LeftJoin<Owner>((dispatchpick, sku, spu, owner) => dispatchpick.OwnerId == owner.OwnerId)
                .LeftJoin<Location>((dispatchpick, sku, spu, owner, location) => dispatchpick.LocationId == location.LocationId)
                .Where((dispatchpick, sku, spu, owner, location) => dispatchpick.DispatchId == dispatchId)
                .Select((dispatchpick, sku, spu, owner, location) => new DispatchpickVm
                {
                    DispatchpickId = dispatchpick.DispatchpickId,
                    PickQty = dispatchpick.PickQty,
                    PickedQty = dispatchpick.PickedQty,
                    OwnerName = owner.OwnerName == null ? "" : owner.OwnerName,
                    SkuCode = sku.CommoditySKUCode,
                    SpuCode = spu.CommoditySPUCode,
                    SpuName = spu.CommoditySPUName,
                    SpuDescription = spu.CommoditySPUDescription,
                    BarCode = spu.BarCode,
                    LocationCode = location.LocationCode,
                    RegionName = location.RegionName,
                    RegionProperty = location.RegionProperty,
                    WarehouseName = location.WarehouseName
                }, true)
                .ToList();

            return result;
        }

        public PagedInfo<PreDispatchVm> GetPreDispatchAll(PreDispatchQueryDto preDispatchQueryDto)
        {
            var expression = Expressionable.Create<PreDispatchVm>()
                .AndIF(!string.IsNullOrEmpty(preDispatchQueryDto.DispatchNo), it => it.DispatchNo == preDispatchQueryDto.DispatchNo)
                .AndIF(!string.IsNullOrEmpty(preDispatchQueryDto.CustomerName), it => it.CustomerName == preDispatchQueryDto.CustomerName)
                .And(it => it.DispatchStatus == preDispatchQueryDto.DispatchStatus)
                .AndIF(preDispatchQueryDto.BeginTime != DateTime.MinValue && preDispatchQueryDto.BeginTime != null, exp => exp.Create_time >= preDispatchQueryDto.BeginTime)
                .AndIF(preDispatchQueryDto.EndTime != DateTime.MaxValue && preDispatchQueryDto.EndTime != null, exp => exp.Create_time <= preDispatchQueryDto.EndTime);

            var query = Queryable()
                .GroupBy(dispatch => new
                {
                    dispatch.DispatchNo,
                    dispatch.DispatchStatus,
                    dispatch.CustomerId,
                    dispatch.CustomerName,
                    dispatch.Create_by
                })
                .Select(dispatch => new PreDispatchVm
                {
                    DispatchNo = dispatch.DispatchNo,
                    DispatchStatus = dispatch.DispatchStatus,
                    CustomerName = dispatch.CustomerName,
                    Qty = SqlFunc.AggregateSum(dispatch.Qty),
                    Create_by = dispatch.Create_by,
                }, true);

            var result = query.Where(expression.ToExpression())
                .ToPage(preDispatchQueryDto);

            var dispatchNoList = result.Result.Select(it => it.DispatchNo)
                .Distinct()
                .ToList();
            var datas = Queryable()
                .LeftJoin<CommoditySKU>((dispatch, sku) => dispatch.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatch, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Where((dispatch, sku, spu) => dispatchNoList.Contains(dispatch.DispatchNo))
                .Select((dispatch, sku, spu) => new
                {
                    dispatchNo = dispatch.DispatchNo,
                    volume = spu.VolumeUnit == 1 ? dispatch.Volume : (spu.VolumeUnit == 0 ? dispatch.Volume / 1000 : dispatch.Volume * 1000),
                    weight = spu.WeightUnit == 0 ? dispatch.Weight / 1000000 : (spu.WeightUnit == 1 ? dispatch.Weight / 100 : dispatch.Weight)
                })
                .ToList();

            result.Result.ForEach(it =>
            {
                it.Volume = datas.Where(d => d.dispatchNo == it.DispatchNo).Sum(d => d.volume);
                it.Weight = datas.Where(d => d.dispatchNo == it.DispatchNo).Sum(d => d.weight);
            });

            return result;
        }

        public List<PreDispatchVm> GetPreDispatchAll(int dispatchStatus)
        {
            var result = Queryable()
                .GroupBy(dispatch => new
                {
                    dispatch.DispatchNo,
                    dispatch.DispatchStatus,
                    dispatch.CustomerId,
                    dispatch.CustomerName,
                    dispatch.Create_by
                })
                .Where(it => it.DispatchStatus == dispatchStatus)
                .Select(dispatch => new PreDispatchVm
                {
                    DispatchNo = dispatch.DispatchNo,
                    DispatchStatus = dispatch.DispatchStatus,
                    CustomerName = dispatch.CustomerName,
                    Qty = SqlFunc.AggregateSum(dispatch.Qty),
                    Create_by = dispatch.Create_by,
                }, true)
                .ToList();

            var dispatchNoList = result.Select(it => it.DispatchNo)
                .Distinct()
                .ToList();

            var datas = Queryable()
                .LeftJoin<CommoditySKU>((dispatch, sku) => dispatch.SkuId == sku.CommoditySKUId)
                .LeftJoin<CommoditySPU>((dispatch, sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .Where((dispatch, sku, spu) => dispatchNoList.Contains(dispatch.DispatchNo))
                .Select((dispatch, sku, spu) => new
                {
                    dispatchNo = dispatch.DispatchNo,
                    volume = spu.VolumeUnit == 1 ? dispatch.Volume : (spu.VolumeUnit == 0 ? dispatch.Volume / 1000 : dispatch.Volume * 1000),
                    weight = spu.WeightUnit == 0 ? dispatch.Weight / 1000000 : (spu.WeightUnit == 1 ? dispatch.Weight / 100 : dispatch.Weight)
                })
                .ToList();

            result.ForEach(it =>
            {
                it.Volume = datas.Where(d => d.dispatchNo == it.DispatchNo).Sum(d => d.volume);
                it.Weight = datas.Where(d => d.dispatchNo == it.DispatchNo).Sum(d => d.weight);
            });

            return result;
        }

        public (bool, string) Package(List<DispatchPackageDto> dispatchPackageDtos, string userName)
        {
            var dispatchIdList = dispatchPackageDtos.Select(it => it.DispatchId)
                .ToList();
            var entities = Queryable()
                .Where(it => dispatchIdList.Contains(it.DispatchId))
                .ToList();
            var code = GetPackageOrWeightCode();

            foreach (var vm in dispatchPackageDtos)
            {
                var entity = entities.FirstOrDefault(it => it.DispatchId == vm.DispatchId && it.DispatchNo == vm.DispatchNo);
                if (entity == null)
                {
                    return (false, "数据发生改变");
                }
                if ((entity.PackageQty + vm.PackageQty) > entity.PickedQty)
                {
                    return (false, "超出拣货数量");
                }
                entity.PackagePerson = userName;
                entity.PackageQty += vm.PackageQty;
                entity.PackageTime = DateTime.Now;
                entity.DispatchNo = code;
                entity.DispatchStatus = 4;
            }

            var result = Context.Updateable(entities).ExecuteCommand();

            if (result > 0)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public (bool, string) SetFreightfee(List<DispatchFreightfeeDto> dispatchFreightfeeDtos)
        {
            var dispatchIdList = dispatchFreightfeeDtos.Select(it => it.DispatchId).ToList();
            var freightfeeIdList = dispatchFreightfeeDtos.Select(it => it.FreightFeeId).ToList();
            var entities = Queryable()
                .Where(it => dispatchIdList.Contains(it.DispatchId))
                .ToList();
            var freightfees = Context.Queryable<FreightFee>()
                .Where(it => freightfeeIdList.Contains(it.FreightFeeId))
                .ToList();

            foreach (var entity in entities)
            {
                var vm = dispatchFreightfeeDtos.FirstOrDefault(it => it.DispatchId == entity.DispatchId);
                if (vm != null)
                {
                    var freightfee = freightfees.FirstOrDefault(it => it.FreightFeeId == vm.FreightFeeId);
                    if (freightfee != null)
                    {
                        entity.Carrier = freightfee.Carrier;
                        entity.WayBillNo = vm.WayBillNo;
                        if (entity.WeighingNo != "")
                        {
                            entity.Freightfee = entity.WeighingWeight * freightfee.PricePerWeight > freightfee.MinPayment ? entity.WeighingWeight * freightfee.PricePerWeight : freightfee.MinPayment;
                        }
                        else
                        {
                            entity.Freightfee = Math.Max(Math.Max(entity.Weight * freightfee.PricePerWeight, entity.Volume * freightfee.PricePerVolume), freightfee.MinPayment);
                        }
                    }
                }
            }

            var result = Context.Updateable(entities).ExecuteCommand();

            if (result > 0)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public (bool, string) SignForArrival(List<DispatchSignDto> dispatchSignDtos)
        {
            var dispatchIdList = dispatchSignDtos.Select(it => it.DispatchId)
                .ToList();
            var entities = Queryable()
                .Where(it => dispatchIdList.Contains(it.DispatchId))
                .ToList();
            foreach (var entity in entities)
            {
                var vm = dispatchSignDtos.FirstOrDefault(it => it.DispatchId == entity.DispatchId && it.DispatchStatus == entity.DispatchStatus) ?? throw new Exception("数据发生改变");
                entity.SignQty = entity.ActualQty - entity.DamageQty;
                entity.DamageQty = vm.DamageQty;
                entity.DispatchStatus = 7;
            }
            var result = Context.Updateable(entities).ExecuteCommand();

            if (result > 0)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        public (bool, string) Update(List<DispatchUpdateDto> dispatchUpdateDtos, string userName)
        {
            var result = UseTran(() =>
            {
                var dispatchNo = dispatchUpdateDtos.FirstOrDefault()?.DispatchNo;
                var dispatchStatus = dispatchUpdateDtos.FirstOrDefault()?.DispatchStatus;
                var entities = Queryable()
                .Where(it => it.DispatchNo == dispatchNo)
                .ToList();
                var deleteIdList = new List<long>();
                var skuIdList = dispatchUpdateDtos.Select(it => it.SkuId).ToList();
                var skus = Context.Queryable<CommoditySKU>()
                .Where(it => skuIdList.Contains(it.CommoditySKUId))
                .ToList();
                if (entities.Any(it => it.DispatchStatus != 1 && it.DispatchStatus != 0))
                {
                    throw new Exception("数据发生改变");
                }
                foreach (var item in dispatchUpdateDtos)
                {
                    if (item.DispatchId < 0)
                    {
                        var entity = entities.FirstOrDefault(it => it.DispatchId == -item.DispatchId) ?? throw new Exception("数据发生改变");
                        Delete(entity.DispatchId);
                        deleteIdList.Add(entity.DispatchId);
                    }
                    else if (item.DispatchId > 0)
                    {
                        var entity = entities.FirstOrDefault(it => it.DispatchId == item.DispatchId) ?? throw new Exception("数据发生改变");
                        entity.SkuId = item.SkuId;
                        entity.Qty = item.Qty;
                        entity.Update_by = userName;
                        entity.Update_time = DateTime.Now;
                        var sku = skus.FirstOrDefault(it => it.CommoditySKUId == entity.SkuId);
                        if (sku != null)
                        {
                            entity.Volume = sku.Volume * entity.Qty;
                            entity.Weight = sku.Weight * entity.Qty;
                        }
                        Update(entity);
                    }
                    else if (item.DispatchId == 0)
                    {
                        var entity = new Dispatch
                        {
                            DispatchId = 0,
                            DispatchNo = dispatchNo,
                            Create_by = userName,
                            DispatchStatus = (int)dispatchStatus,
                            SkuId = item.SkuId,
                            Qty = item.Qty,
                        };
                        var sku = skus.FirstOrDefault(it => it.CommoditySKUId == entity.SkuId);
                        if (sku != null)
                        {
                            entity.Volume = sku.Volume * entity.Qty;
                            entity.Weight = sku.Weight * entity.Qty;
                        }
                        entities.Add(entity);
                        Insert(entity);
                    }

                    var repeatSkusIdList = entities.Where(it => !deleteIdList.Contains(it.DispatchId))
                    .GroupBy(it => it.SkuId)
                    .Select(it => new
                    {
                        key = it.Key,
                        count = it.Count()
                    })
                    .Where(it => it.count > 1)
                    .Select(it => it.key)
                    .ToList();
                    if (repeatSkusIdList.Count > 0)
                    {
                        var repeatSkus = skus.Where(it => repeatSkusIdList.Contains(it.CommoditySKUId))
                        .Select(it => it.CommoditySKUCode)
                        .ToList();
                        var msg = "";
                        foreach (var sku in repeatSkus)
                        {
                            msg += $"存在重复实体，规格编码为：{sku}";
                        }
                        throw new Exception(msg);
                    }
                }
            });

            return (result.IsSuccess, result.ErrorMessage);
        }

        public (bool, string) Weight(List<DispatchWeightDto> dispatchWeightDtos, string userName)
        {
            var dispatchIdList = dispatchWeightDtos.Select(it => it.DispatchId)
                .ToList();
            var entities = Queryable()
                .Where(it => dispatchIdList.Contains(it.DispatchId))
                .ToList();
            var code = GetPackageOrWeightCode();

            foreach (var vm in dispatchWeightDtos)
            {
                var entity = entities.FirstOrDefault(it => it.DispatchId == vm.DispatchId && it.DispatchStatus == vm.DispatchStatus);
                if (entity == null)
                {
                    return (false, "数据发生改变");
                }
                if ((entity.WeighingQty + vm.WeighingQty) > entity.PickedQty)
                {
                    return (false, "未称重数量小于当前操作称重数量");
                }
                entity.WeighingPerson = userName;
                entity.WeighingQty += vm.WeighingQty;
                entity.WeighingWeight += vm.WeighingWeight;
                entity.WeighingNo = code;
                entity.DispatchStatus = 5;
            }

            var result = Context.Updateable(entities).ExecuteCommand();

            if (result > 0)
            {
                return (true, "操作成功");
            }
            else
            {
                return (false, "操作失败");
            }
        }

        /// <summary>
        /// 生成发货序号
        /// </summary>
        /// <returns></returns>
        private string GetDispatchNo()
        {
            string code = "";
            string date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var maxNo = Queryable()
                .OrderBy(it => it.DispatchNo, OrderByType.Desc)
                .First()?
                .DispatchNo
                .Split("-");

            if (maxNo != null && maxNo.First().Equals(date))
            {
                var newNo = int.Parse(maxNo.Last()) + 1;
                code = $"{date}-{newNo:0000}";
            }
            else
            {
                code = $"{date}-0001";
            }

            return code;
        }

        /// <summary>
        /// 生成打包或称重编码
        /// </summary>
        /// <returns></returns>
        private string GetPackageOrWeightCode()
        {
            string date = DateTime.Now.ToString("yyyy" + "MM" + "dd");
            var dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
            var timeStamp = Convert.ToInt32(DateTime.Now.Subtract(dtStart).TotalSeconds);

            return date + timeStamp.ToString();
        }
    }
}
