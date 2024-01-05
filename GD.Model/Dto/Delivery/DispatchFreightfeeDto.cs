namespace GD.Model.Dto.Delivery
{
    public class DispatchFreightfeeDto
    {
        public long DispatchId { get; set; }

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public long FreightFeeId { get; set; } = 0;

        public string Carrier { get; set; } = string.Empty;

        public string WayBillNo { get; set; } = string.Empty;
    }
}
