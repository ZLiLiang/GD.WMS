using GD.Model.Constant;

namespace GD.Model.Basic
{
    /// <summary>
    /// 货主信息表
    /// </summary>
    [SugarTable("wm_owner", "货主信息表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Owner : Base
    {
        /// <summary>
        /// 货主Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long OwnerId { get; set; }

        /// <summary>
        /// 货主名称
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactTel { get; set; } = string.Empty;

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; } = string.Empty;
    }
}
