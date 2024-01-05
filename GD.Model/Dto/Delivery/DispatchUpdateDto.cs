namespace GD.Model.Dto.Delivery
{
    public class DispatchUpdateDto
    {
        public long DispatchId { get; set; } = 0;

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        //public long CustomerId { get; set; } = 0;

        public long SkuId { get; set; } = 0;

        public int Qty { get; set; } = 0;

    }
}
