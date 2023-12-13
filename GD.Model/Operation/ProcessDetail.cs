using GD.Model.Constant;

namespace GD.Model.Operation
{
    /// <summary>
    /// 仓内加工详细表
    /// </summary>
    [SugarTable("wm_processDetail", "仓内加工详细表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class ProcessDetail : Base
    {
        /// <summary>
        /// 加工详细Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long ProcessDetailId { get; set; }

        /// <summary>
        /// 加工Id
        /// </summary>
        public long ProcessId { get; set; } = 0;

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
        /// 是否为来源
        /// 0:是，来源
        /// 1:否，目标
        /// </summary>
        public int IsSource { get; set; } = 0;

        /// <summary>
        /// 是否更新
        /// 0:否
        /// 1:是
        /// </summary>
        public int IsUpate { get; set; } = 0;
    }
}
