using GD.Model.Page;

namespace GD.Model.Dto.Delivery
{
    public class PreDispatchQueryDto : PagerInfo
    {
        public string DispatchNo { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
