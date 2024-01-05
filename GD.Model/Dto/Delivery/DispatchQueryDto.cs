using GD.Model.Page;

namespace GD.Model.Dto.Delivery
{
    public class DispatchQueryDto : PagerInfo
    {
        public string DispatchNo { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public int DispatchStatus { get; set; } = 0;

        /// <summary>
        /// Custom Classification
        /// </summary>
        public string SqlTitle { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
