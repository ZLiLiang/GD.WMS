using GD.Model.Page;

namespace GD.Model.Dto.Inventory
{
    public class LocationStockQueryDto : PagerInfo
    {
        public string LocationCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
