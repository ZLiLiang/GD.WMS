namespace GD.Model.Dto.Delivery
{
    public class DispatchPackageDto
    {
        public long DispatchId { get; set; }

        public string DispatchNo { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public int PackageQty { get; set; } = 0;

        public int PickedQty { get; set; } = 0;
    }
}
