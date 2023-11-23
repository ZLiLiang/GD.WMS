using GD.Model.Constant;

namespace GD.Model.Basic
{
    [SugarTable("wm_supplier", "供应商信息表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Supplier : Base
    {
        /// <summary>
        /// 供应商Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
        public long SupplierId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string SupplierName { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Manager { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string ContactTel { get; set; }
    }
}
