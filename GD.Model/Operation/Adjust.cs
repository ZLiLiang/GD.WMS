using GD.Model.Constant;

namespace GD.Model.Operation
{
    /// <summary>
    /// 仓内调整表
    /// </summary>
    [SugarTable("wm_adjust", "仓内调整表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Adjust : Base
    {
        /// <summary>
        /// 调整id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long AdjustId { get; set; }

        /// <summary>
        /// 作业单号
        /// </summary>
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 作业类型
        /// 0：拆分,
        /// 1：组合,
        /// 2：盘点,
        /// 3：移动,
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
        /// 数量
        /// </summary>
        public int Qty { get; set; } = 0;

        /// <summary>
        /// 是否更新
        /// 0:否
        /// 1:是
        /// </summary>
        public int IsUpate { get; set; } = 0;

        /// <summary>
        /// 来源表Id
        /// </summary>
        public long SourceTableId { get; set; } = 0;
    }
}
