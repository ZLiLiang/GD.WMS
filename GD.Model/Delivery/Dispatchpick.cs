using GD.Model.Constant;

namespace GD.Model.Delivery
{
    /// <summary>
    /// 发货管理详细表
    /// </summary>
    [SugarTable("wm_dispatchpick", "发货管理详细表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Dispatchpick : Base
    {
        /// <summary>
        /// 派工单详细id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long DispatchpickId { get; set; }

        /// <summary>
        /// 派工单id
        /// </summary>
        public long DispatchId { get; set; }

        /// <summary>
        /// 货主Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// 库位Id
        /// </summary>
        public long LocationId { get; set; }

        /// <summary>
        /// 规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 未拣货数量
        /// </summary>
        public int PickQty { get; set; } = 0;

        /// <summary>
        /// 已拣货数量
        /// </summary>
        public int PickedQty { get; set; } = 0;

        /// <summary>
        /// 是否更新
        /// 0:否
        /// 1:是
        /// </summary>
        public int IsUpate { get; set; } = 0;
    }
}
