using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.Operation
{
    public class ProcessDto
    {
        public long ProcessId { get; set; }

        public string JobCode { get; set; } = string.Empty;

        public int JobType { get; set; } = 0;

        public int ProcessStatus { get; set; } = 0;

        public List<ProcessDetailDto> DetailList { get; set; } = new();

    }

    public class ProcessDetailDto
    {
        public long ProcessDetailId { get; set; }

        public long ProcessId { get; set; } = 0;

        public long SkuId { get; set; } = 0;

        public long OwnerId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public int Qty { get; set; } = 0;

        public int IsSource { get; set; } = 0;

        public int IsUpate { get; set; } = 0;
    }

    public class ProcessQueryDto : PagerInfo
    {
        public string JobCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class ProcessExcelDto
    {
        private int _jobType;
        public int _processStatus;

        [ExcelColumn(Name = "作业单号")]
        public string JobCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "作业类型")]
        public string JobType
        {
            get => _jobType switch
            {
                0 => "拆分加工",
                1 => "组合加工"
            };
            set => _jobType = int.Parse(value);
        }

        [ExcelColumn(Name = "是否已调整")]
        public string ProcessStatus
        {
            get => _processStatus switch
            {
                0 => "否",
                1 => "是"
            };
            set => _processStatus = int.Parse(value);
        }

        [ExcelColumn(Name = "操作人")]
        public string Processor { get; set; } = string.Empty;

        [ExcelColumn(Name = "操作时间")]
        public DateTime ProcessTime { get; set; }

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建时间")]
        public DateTime Create_time { get; set; }
    }
}
