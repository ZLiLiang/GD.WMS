using GD.Model.Page;

namespace GD.Model.Dto.Operation
{
    public class FreezeDto
    {
        public long FreezeId { get; set; } = 0;
        public int JobType { get; set; } = 0;

        public long SkuId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public long OwnerId { get; set; } = 0;
    }

    public class FreezeQueryDto : PagerInfo
    {
        public string JobCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
