using GD.Model.Constant;

namespace GD.Model.Inventory
{
    /// <summary>
    /// 库存表
    /// </summary>
    [SugarTable("wm_stock", "库存表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Stock : Base
    {
        /// <summary>
        /// 库存id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long StockId { get; set; }

        /// <summary>
        /// 规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 位置Id
        /// </summary>
        public long LocationId { get; set; } = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; } = 0;

        /// <summary>
        /// 货主id
        /// </summary>
        public long OwnerId { get; set; } = 0;

        /// <summary>
        /// 是否冻结
        /// 0：否
        /// 1：是
        /// </summary>
        public int IsFreeze { get; set; } = 0;
    }
}
