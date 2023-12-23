using GD.Model.Page;

namespace GD.Model.Dto.Operation
{
    public class MoveDto
    {
        public long SkuId { get; set; } = 0;

        public long OrigLocationId { get; set; } = 0;

        public long DestLocationId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public long OwnerId { get; set; } = 0;
    }

    public class MoveQueryDto : PagerInfo
    {
        public string JobCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
