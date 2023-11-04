using GD.Model.Constant;

namespace GD.Model.WarehouseManagement
{
    /// <summary>
    /// WM_company表
    /// </summary>
    [SugarTable("wm_company", "公司信息表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Company : Base
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int CompanyId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [SugarColumn(Length = 50, ExtendedAttribute = 0)]
        public string CompanyName { get; set; } = string.Empty;

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
        /// 联系方式
        /// </summary>
        public string ContactTel { get; set; } = string.Empty;
    }
}
