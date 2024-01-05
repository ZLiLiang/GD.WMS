namespace GD.Model.Dto.Delivery
{
    public class DispatchWeightDto
    {
        public long DispatchId { get; set; }

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public int WeighingQty { get; set; } = 0;

        public decimal WeighingWeight { get; set; } = 0;

        public int PickedQty { get; set; } = 0;
    }
}
