namespace GD.Model.Vm.Delivery
{
    public class DispatchConfirmDetailVm
    {
        public long DispatchId { get; set; }

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public string CustomerName { get; set; } = string.Empty;

        public long CustomerId { get; set; } = 0;

        public long SkuId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SpuDescription { get; set; } = string.Empty;

        public string BarCode { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public int AvailableQty { get; set; } = 0;

        public bool Confirm { get; set; } = false;

        public List<DispatchConfirmPickDetailVm> PickList { get; set; } = new();
    }
}
