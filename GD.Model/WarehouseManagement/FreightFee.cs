using GD.Model.Constant;

namespace GD.Model.WarehouseManagement
{
    /// <summary>
    /// 运费设置表
    /// </summary>
    [SugarTable("wm_freightFee", "运费设置表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class FreightFee : Base
    {
        /// <summary>
        /// 运费Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long FreightFeeId { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string Carrier { get; set; } = string.Empty;

        /// <summary>
        /// 始发城市
        /// </summary>
        public string DepartureCity { get; set; } = string.Empty;

        /// <summary>
        /// 到货城市
        /// </summary>
        public string ArrivalCity { get; set; } = string.Empty;

        /// <summary>
        /// 单公斤运费
        /// </summary>
        public decimal PricePerWeight { get; set; } = 0;

        /// <summary>
        /// 单立方米运费
        /// </summary>
        public decimal PricePerVolume { get; set; } = 0;

        /// <summary>
        /// 最小运费
        /// </summary>
        public decimal MinPayment { get; set; } = 0;

        /// <summary>
        /// 是否有效
        /// 0：无效
        /// 1：有效
        /// </summary>
        [SugarColumn(DefaultValue = "1")]
        public int IsValid { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [SugarColumn(DefaultValue = "0")]
        public long TenantId { get; set; }
    }
}
