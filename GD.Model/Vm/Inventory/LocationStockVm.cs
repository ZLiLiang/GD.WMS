using MiniExcelLibs.Attributes;

namespace GD.Model.Vm.Inventory
{
    public class LocationStockVm : BaseVm
    {
        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string SkuName { get; set; } = string.Empty;

        public long SkuId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public int AvailableQty { get; set; } = 0;

        public int LockedQty { get; set; } = 0;

        public int FrozenQty { get; set; } = 0;
    }

    public class LocationStockExcelVm
    {
        [ExcelColumn(Name = "仓库名称")]
        public string WarehouseName { get; set; } = string.Empty;

        [ExcelColumn(Name = "库位名称")]
        public string LocationCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格名称")]
        public string SkuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "数量")]
        public int Qty { get; set; } = 0;

        [ExcelColumn(Name = "可用数量")]
        public int AvailableQty { get; set; } = 0;

        [ExcelColumn(Name = "锁定数量")]
        public int LockedQty { get; set; } = 0;

        [ExcelColumn(Name = "冻结数量")]
        public int FrozenQty { get; set; } = 0;
    }
}
