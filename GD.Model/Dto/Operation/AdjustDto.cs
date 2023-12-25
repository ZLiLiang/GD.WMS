using GD.Model.Page;

namespace GD.Model.Dto.Operation
{
    public class AdjustDto
    {
        public long AdjustId { get; set; }

        public int JobType { get; set; } = 0;

        public long SkuId { get; set; } = 0;

        public long OwnerId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public int IsUpate { get; set; } = 0;

        public long SourceTableId { get; set; } = 0;
    }

    public class AdjustQueryDto : PagerInfo
    {
        public string JobCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
