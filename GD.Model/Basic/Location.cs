using GD.Model.Constant;

namespace GD.Model.Basic
{
    /// <summary>
    /// 库位设置表
    /// </summary>
    [SugarTable("wm_location", "库位设置表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Location : Base
    {
        /// <summary>
        /// 库位Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long LocationId { get; set; }

        /// <summary>
        /// 库位编码
        /// </summary>
        public string LocationCode { get; set; } = string.Empty;

        /// <summary>
        /// 仓库Id
        /// </summary>
        public long WarehouseId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; } = string.Empty;

        /// <summary>
        /// 库区Id
        /// </summary>
        public long RegionId { get; set; }

        /// <summary>
        /// 库区名称
        /// </summary>
        public string RegionName { get; set; } = string.Empty;

        /// <summary>
        /// 库区类型
        /// 0：拣货区，1：备货区，2：收货区，3：退货区，4：次品区，5：存货区
        /// </summary>
        public int RegionProperty { get; set; }

        /// <summary>
        /// 库位长(m)
        /// </summary>
        public decimal LocationLength { get; set; }

        /// <summary>
        /// 库位宽(m)
        /// </summary>
        public decimal LocationWidth { get; set; }

        /// <summary>
        /// 库位高(m)
        /// </summary>
        public decimal LocationHeight { get; set; }

        /// <summary>
        /// 库位容积(m³)
        /// </summary>
        public decimal LocationVolume { get; set; }

        /// <summary>
        /// 库位承重(kg)
        /// </summary>
        public decimal LocationLoad { get; set; }

        /// <summary>
        /// 巷道号
        /// </summary>
        public string RoadwayNumber { get; set; } = string.Empty;

        /// <summary>
        /// 货架号
        /// </summary>
        public string ShelfNumber { get; set; } = string.Empty;

        /// <summary>
        /// 层号
        /// </summary>
        public string LayerNumber { get; set; } = string.Empty;

        /// <summary>
        /// 位号
        /// </summary>
        public string TagNumber { get; set; } = string.Empty;

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
