using GD.Model.Constant;

namespace GD.Model.WarehouseManagement
{
    /// <summary>
    /// 仓库设置表
    /// </summary>
    [SugarTable("wm_warehouse", "仓库设置表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Warehouse : Base
    {
        /// <summary>
        /// 仓库Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long WarehouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 是否有效
        /// 0：无效
        /// 1：有效
        /// </summary>
        [SugarColumn(DefaultValue ="1")]
        public int IsValid { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [SugarColumn(DefaultValue ="0")]
        public long TenantId { get; set; }
    }
}
