namespace GD.Model.Vm.Delivery
{
    public class DispatchpickVm : BaseVm
    {
        public long DispatchpickId { get; set; }

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SpuDescription { get; set; } = string.Empty;

        public string BarCode { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string OwnerName { get; set; } = string.Empty;

        public string WarehouseName { get; set; } = string.Empty;

        public string RegionName { get; set; } = string.Empty;

        /// <summary>
        /// 库区类型
        /// 0：拣货区，1：备货区，2：收货区，3：退货区，4：次品区，5：存货区
        /// </summary>
        public int RegionProperty { get; set; }

        public string LocationCode { get; set; } = string.Empty;

        public int PickQty { get; set; } = 0;

        public int PickedQty { get; set; } = 0;
    }
}
