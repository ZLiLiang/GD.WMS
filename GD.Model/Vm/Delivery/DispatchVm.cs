namespace GD.Model.Vm.Delivery
{
    public class DispatchVm : BaseVm
    {
        public long DispatchId { get; set; }

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public string CustomerName { get; set; } = string.Empty;

        public int Qty { get; set; } = 0;

        public decimal Weight { get; set; } = 0;

        public decimal Volume { get; set; } = 0;

        public int DamageQty { get; set; } = 0;

        public int LockQty { get; set; } = 0;

        public int PickedQty { get; set; } = 0;

        public int UnpickedQty { get; set; } = 0;

        public int IntrasitQty { get; set; } = 0;

        public int PackageQty { get; set; } = 0;

        public int UnpackageQty { get; set; } = 0;

        public int WeighingQty { get; set; } = 0;

        public int UnweighingQty { get; set; } = 0;

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

        public string SkuName { get; set; } = string.Empty;

        public string SpuDescription { get; set; } = string.Empty;

        public string BarCode { get; set; } = string.Empty;

        /// <summary>
        /// 体积单位
        /// 0：立方厘米
        /// 1：立方分米
        /// 2：立方米
        /// </summary>
        public int VolumeUnit { get; set; } = 0;

        /// <summary>
        /// 重量单位
        /// 0：毫克
        /// 1：克
        /// 2：千克
        /// </summary>
        public int WeightUnit { get; set; } = 0;

        /// <summary>
        /// 长度单位
        /// 0：毫米
        /// 1：厘米
        /// 2：分米
        /// 3：米
        /// </summary>
        public int LengthUnit { get; set; } = 0;

        public string Unit { get; set; } = string.Empty;
    }
}
