using GD.Model.Constant;

namespace GD.Model.Operation
{
    /// <summary>
    /// 仓内盘点表
    /// </summary>
    [SugarTable("wm_taking", "仓内盘点表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Taking : Base
    {
        /// <summary>
        /// 盘点id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long TakingId { get; set; }

        /// <summary>
        /// 作业单号
        /// </summary>
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 盘点作业状态
        /// 0:待作业
        /// 1:已完成
        /// </summary>
        public long JobStatus { get; set; } = 0;

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
        /// 账面数量
        /// </summary>
        public int BookQty { get; set; } = 0;

        /// <summary>
        /// 盘点数量
        /// </summary>
        public int CountedQty { get; set; } = 0;

        /// <summary>
        /// 差异数量
        /// </summary>
        public int DifferenceQty { get; set; } = 0;

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
