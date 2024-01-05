namespace GD.Model.Vm.Delivery
{
    public class PreDispatchVm : BaseVm
    {
        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public string CustomerName { get; set; } = string.Empty;

        public int Qty { get; set; } = 0;

        public decimal Weight { get; set; } = 0;

        public decimal Volume { get; set; } = 0;
    }
}
