using GD.Model.Constant;

namespace GD.Model.WarehouseManagement
{
    [SugarTable("wm_commodity", "商品管理表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Commodity : Base
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
        public long CommodityId { get; set; }

        [SugarColumn(IsNullable = false)]
        public string CommodityName { get; set; }

        [SugarColumn(IsNullable = false)]
        public long CategoryId { get; set; }

        [SugarColumn(IsNullable = false)]
        public string CategoryName { get; set; }

        public string? Description { get; set; }

        public string? BarCode { get; set; }

        [SugarColumn(IsNullable = false)]
        public long SupplierId { get; set; }

        [SugarColumn(IsNullable = false)]
        public string SupplierName { get; set; }
    }
}
