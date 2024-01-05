namespace GD.Model.Vm.Delivery
{
    public class DispatchConfirmPickDetailVm
    {
        public long StockId { get; set; } = 0;

        public long DispatchId { get; set; } = 0;

        public long OwnerId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public string OwnerName { get; set; } = string.Empty;

        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public string RegionName { get; set; } = string.Empty;

        public int AvailableQty { get; set; } = 0;
        public int PickQty { get; set; } = 0;
    }
}
