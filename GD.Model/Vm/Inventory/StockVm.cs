using MiniExcelLibs.Attributes;

namespace GD.Model.Vm.Inventory
{
    public class StockVm : BaseVm
    {
        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public long SkuId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public int AvailableQty { get; set; } = 0;

        public int LockedQty { get; set; } = 0;

        public int FrozenQty { get; set; } = 0;

        public int AsnQty { get; set; } = 0;

        public int ToUnloadQty { get; set; } = 0;

        public int ToSortQty { get; set; } = 0;

        public int SortedQty { get; set; } = 0;

        public int ShortageQty { get; set; } = 0;
    }

    public class StockExcelVm
    {
        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "数量")]
        public int Qty { get; set; } = 0;

        [ExcelColumn(Name = "可用数量")]
        public int AvailableQty { get; set; } = 0;

        [ExcelColumn(Name = "锁定数量")]
        public int LockedQty { get; set; } = 0;

        [ExcelColumn(Name = "冻结数量")]
        public int FrozenQty { get; set; } = 0;

        [ExcelColumn(Name = "到货通知书数量")]
        public int AsnQty { get; set; } = 0;

        [ExcelColumn(Name = "待卸货数量")]
        public int ToUnloadQty { get; set; } = 0;

        [ExcelColumn(Name = "待分拣数量")]
        public int ToSortQty { get; set; } = 0;

        [ExcelColumn(Name = "已分拣数量")]
        public int SortedQty { get; set; } = 0;

        [ExcelColumn(Name = "欠货数量")]
        public int ShortageQty { get; set; } = 0;
    }
}
