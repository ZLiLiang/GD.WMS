namespace GD.Model.Dto.Delivery
{
    public class DispatchConfirmPickDetailDto
    {
        public long StockId { get; set; } = 0;

        public long DispatchId { get; set; } = 0;

        public long OwnerId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public int AvailableQty { get; set; } = 0;
        public int PickQty { get; set; } = 0;
    }
}
