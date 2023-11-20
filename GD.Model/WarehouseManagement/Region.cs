using GD.Model.Constant;

namespace GD.Model.WarehouseManagement
{
    /// <summary>
    /// 库区设置表
    /// </summary>
    [SugarTable("wm_region", "库区设置表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Region : Base
    {
        /// <summary>
        /// 库区Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long RegionId { get; set; }

        /// <summary>
        /// 库区名称
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 仓库Id
        /// </summary>
        public long WarehouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 库区类型
        /// 0：拣货区，1：备货区，2：收货区，3：退货区，4：次品区，5：存货区
        /// </summary>
        public int RegionProperty { get; set; }

        /// <summary>
        /// 是否有效
        /// 0：否
        /// 1：是
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
