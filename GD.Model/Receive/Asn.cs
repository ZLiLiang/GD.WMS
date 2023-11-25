using GD.Model.Constant;

namespace GD.Model.Receive
{
    /// <summary>
    /// 到货通知书表
    /// </summary>
    [SugarTable("wm_asn", "到货通知书表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Asn : Base
    {
        /// <summary>
        /// 到货通知书Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long AsnId { get; set; }

        /// <summary>
        /// 到货通知书编号
        /// </summary>
        public string AsnNo { get; set; } = string.Empty;

        /// <summary>
        /// 货物状态
        /// </summary>
        public int AsnStatus { get; set; }

        /// <summary>
        /// 规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 规格编码
        /// </summary>
        public string SkuCode { get; set; } = string.Empty;

        /// <summary>
        /// 规格名称
        /// </summary>
        public string SkuName { get; set; } = string.Empty;

        /// <summary>
        /// 商品Id
        /// </summary>
        public long SpuId { get; set; } = 0;

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SpuCode { get; set; } = string.Empty;

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SpuName { get; set; } = string.Empty;

        /// <summary>
        /// 长度单位（0=毫米、1=厘米、2=分米、3=米）
        /// </summary>
        public int LengthUnit { get; set; } = 0;

        /// <summary>
        /// 体积单位（0=立方厘米、1=立方分米、2=立方米）
        /// </summary>
        public int VolumeUnit { get; set; } = 0;

        /// <summary>
        /// 重量单位（0=毫克、1=克、2=千克）
        /// </summary>
        public int WeightUnit { get; set; } = 0;

        /// <summary>
        /// 到货通知书数据
        /// </summary>
        public int AsnQty { get; set; } = 0;

        /// <summary>
        /// 上架数量
        /// </summary>
        public int ActualQty { get; set; } = 0;

        /// <summary>
        /// 分拣数量
        /// </summary>
        public int SortedQty { get; set; } = 0;

        /// <summary>
        /// 短少数量
        /// </summary>
        public int ShortageQty { get; set; } = 0;

        /// <summary>
        /// 超量数量
        /// </summary>
        public int MoreQty { get; set; } = 0;

        /// <summary>
        /// 破损数量
        /// </summary>
        public int DamageQty { get; set; } = 0;

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; } = 0;

        /// <summary>
        /// 体积
        /// </summary>
        public decimal Volume { get; set; } = 0;

        /// <summary>
        /// 供应商Id
        /// </summary>
        public long SupplierId { get; set; } = 0;

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// 货主Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// 货主名称
        /// </summary>
        public string OwnerName { get; set; } = string.Empty;
    }
}
