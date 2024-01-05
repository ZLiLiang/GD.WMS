namespace GD.Model.Vm.Delivery
{
    public class DispatchlDetailVm
    {
        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public string CustomerName { get; set; } = string.Empty;

        public int Qty { get; set; } = 0;

        public decimal Weight { get; set; } = 0;

        public decimal Volume { get; set; } = 0;

        public int DamageQty { get; set; } = 0;

        public int LockQty { get; set; } = 0;

        public int PickedQty { get; set; } = 0;

        public int IntrasitQty { get; set; } = 0;

        public int PackageQty { get; set; } = 0;

        public int WeighingQty { get; set; } = 0;

        public int ActualQty { get; set; } = 0;

        public int SignQty { get; set; } = 0;

        public string PackageNo { get; set; } = string.Empty;

        public string PackagePerson { get; set; } = string.Empty;

        public DateTime PackageTime { get; set; }

        public string WeighingNo { get; set; } = string.Empty;

        public string WeighingPerson { get; set; } = string.Empty;

        public decimal WeighingWeight { get; set; } = 0;

        public string WayBillNo { get; set; } = string.Empty;

        public string Carrier { get; set; } = string.Empty;

        public decimal Freightfee { get; set; } = 0;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string SpuDescription { get; set; } = string.Empty;

        public string BarCode { get; set; } = string.Empty;
    }
}
