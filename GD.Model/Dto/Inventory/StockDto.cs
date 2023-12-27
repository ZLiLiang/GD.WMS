using GD.Model.Page;

namespace GD.Model.Dto.Inventory
{
    public class StockQueryDto : PagerInfo
    {
        public string SpuName { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
