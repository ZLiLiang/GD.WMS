using GD.Model.Constant;

namespace GD.Model.Operation
{
    /// <summary>
    /// 仓内移动表
    /// </summary>
    [SugarTable("wm_move", "仓内移动表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Move : Base
    {
        /// <summary>
        /// 移动id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long MoveId { get; set; }

        /// <summary>
        /// 作业单号
        /// </summary>
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 移动作业状态
        /// 0:未调整
        /// 1:已调整
        /// </summary>
        public int MoveStatus { get; set; } = 0;

        /// <summary>
        /// 商品规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 来源库位id
        /// </summary>
        public long OrigLocationId { get; set; } = 0;

        /// <summary>
        /// 目标库位id
        /// </summary>
        public long DestLocationId { get; set; } = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; } = 0;

        /// <summary>
        /// 货主Id
        /// </summary>
        public long OwnerId { get; set; } = 0;

        /// <summary>
        /// 处理人
        /// </summary>
        public string Handler { get; set; } = string.Empty;

        /// <summary>
        /// 处理时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public DateTime HandlerTime { get; set; } = DateTime.Now;
    }
}
