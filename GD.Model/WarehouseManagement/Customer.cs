using GD.Model.Constant;

namespace GD.Model.WarehouseManagement
{
    /// <summary>
    /// 客户信息表
    /// </summary>
    [SugarTable("wm_customer", "客户信息表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Customer : Base
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long CustomerId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactTel { get; set; } = string.Empty;
    }
}
