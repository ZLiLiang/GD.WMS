using GD.Model.Page;

namespace GD.Model.Dto.Operation
{
    public class TakingDto
    {
        public long SkuId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public int BookQty { get; set; } = 0;
    }

    public class TakingQueryDto : PagerInfo
    {
        public string JobCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class TakingPutDto
    {
        public long TakingId { get; set; }

        public int CountedQty { get; set; } = 0;
    }
}
