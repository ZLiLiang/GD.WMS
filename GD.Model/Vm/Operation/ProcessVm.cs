namespace GD.Model.Vm.Operation
{
    public class ProcessVm : BaseVm
    {
        public long ProcessId { get; set; }

        public string JobCode { get; set; } = string.Empty;

        public int JobType { get; set; } = 0;

        public int ProcessStatus { get; set; } = 0;

        public string Processor { get; set; } = string.Empty;

        public DateTime? ProcessTime { get; set; }

        /// <summary>
        /// 是否已调整
        /// 0:否
        /// 1:是
        /// </summary>
        public int AdjustStatus { get; set; } = 0;

        public List<ProcessDetailVm> SourceDetailList { get; set; } = new();

        public List<ProcessDetailVm> TargetDetailList { get; set; } = new();
    }

    public class ProcessDetailVm : BaseVm
    {
        public long ProcessDetailId { get; set; }

        public long ProcessId { get; set; } = 0;

        public long SkuId { get; set; } = 0;

        public long OwnerId { get; set; } = 0;

        public long LocationId { get; set; } = 0;

        public string LocationCode { get; set; } = string.Empty;

        public int Qty { get; set; } = 0;

        public int IsSoucre { get; set; } = 0;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public int IsUpate { get; set; } = 0;
    }
}
