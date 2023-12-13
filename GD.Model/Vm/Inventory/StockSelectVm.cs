namespace GD.Model.Vm.Inventory
{
    public class StockSelectVm : BaseVm
    {
        public long StockId { get; set; }

        public long SkuId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public long OwnerId { get; set; } = 0;

        public int IsFreeze { get; set; } = 0;

        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SpuCode { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string SkuName { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public int AvailableQty { get; set; } = 0;

        public string OwnerName { get; set; } = string.Empty;
    }
}
