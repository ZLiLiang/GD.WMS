using GD.Model.Constant;

namespace GD.Model.Operation
{
    /// <summary>
    /// 仓内冻结表
    /// </summary>
    [SugarTable("wm_freeze", "仓内冻结表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Freeze : Base
    {
        /// <summary>
        /// 冻结id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long FreezeId { get; set; } = 0;

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
        /// 商品规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 货主Id
        /// </summary>
        public long OwnerId { get; set; } = 0;

        /// <summary>
        /// 库位Id
        /// </summary>
        public long LocationId { get; set; } = 0;

        /// <summary>
        /// 处理人
        /// </summary>
        public string Handler { get; set; } = string.Empty;

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime HandlerTime { get; set; }
    }
}
