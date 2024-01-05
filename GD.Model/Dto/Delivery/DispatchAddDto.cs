namespace GD.Model.Dto.Delivery
{
    public class DispatchAddDto
    {
        public long CustomerId { get; set; } = 0;

        public string CustomerName { get; set; } = string.Empty;

        public long SkuId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public decimal Weight { get; set; } = 0;

        public decimal Volume { get; set; } = 0;
    }
}
