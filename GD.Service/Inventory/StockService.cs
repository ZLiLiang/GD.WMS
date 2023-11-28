using GD.Infrastructure.Attribute;
using GD.Model.Basic;
using GD.Model.Dto.Inventory;
using GD.Model.Page;
using GD.Model.Vm.Inventory;
using GD.Repository;
using GD.Service.Interface.Inventory;
using SqlSugar;

namespace GD.Service.Inventory
{
    [AppService(ServiceType = typeof(IStockService), ServiceLifetime = LifeTime.Transient)]
    public class StockService : BaseService<CommoditySKU>, IStockService
    {
        public PagedInfo<CommoditySkuSelect> GetCommoditySkuSelect(CommoditySkuSelectQueryDto commoditySkuSelectQueryDto)
        {
            return Queryable()
                .LeftJoin<CommoditySPU>((sku, spu) => sku.CommoditySPUId == spu.CommoditySPUId)
                .WhereIF(!string.IsNullOrEmpty(commoditySkuSelectQueryDto.SpuName), (sku, spu) => spu.CommoditySPUName.Contains(commoditySkuSelectQueryDto.SpuName))
                .WhereIF(!string.IsNullOrEmpty(commoditySkuSelectQueryDto.SpuName), (sku, spu) => sku.CommoditySKUCode.Contains(commoditySkuSelectQueryDto.SkuCode))
                .Select((sku, spu) => new CommoditySkuSelect
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
