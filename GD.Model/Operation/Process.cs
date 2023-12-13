using GD.Model.Constant;

namespace GD.Model.Operation
{
    /// <summary>
    /// 仓内加工表
    /// </summary>
    [SugarTable("wm_process", "仓内加工表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Process : Base
    {
        /// <summary>
        /// 加工id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long ProcessId { get; set; }

        /// <summary>
        /// 作业单号
        /// </summary>
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 作业类型
        /// 0:拆分加工
        /// 1:组合加工
        /// </summary>
        public int JobType { get; set; } = 0;

        /// <summary>
        /// 是否已调整
        /// 0:否
        /// 1:是
        /// </summary>
        public int ProcessStatus { get; set; } = 0;

        /// <summary>
        /// 操作人
        /// </summary>
        public string Processor { get; set; } = string.Empty;

        /// <summary>
        /// 操作时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public DateTime ProcessTime { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(ProcessDetail.ProcessId))]
        public List<ProcessDetail> ProcessDetails { get; set; }
    }
}
