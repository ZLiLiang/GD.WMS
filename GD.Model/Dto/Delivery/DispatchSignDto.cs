namespace GD.Model.Dto.Delivery
{
    public class DispatchSignDto
    {
        public long DispatchId { get; set; }

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public int DamageQty { get; set; } = 0;
    }
}
