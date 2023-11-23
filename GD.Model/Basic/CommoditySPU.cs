using GD.Model.Constant;

namespace GD.Model.Basic
{
    [SugarTable("wm_commodityspu", "商品管理表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class CommoditySPU : Base
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
        public long CommoditySPUId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string CommoditySPUCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string CommoditySPUName { get; set; }

        /// <summary>
        /// 商品类别Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long CategoryId { get; set; }

        /// <summary>
        /// 商品类别名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string? CategoryName { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string? CommoditySPUDescription { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string? BarCode { get; set; }

        /// <summary>
        /// 供应商Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long SupplierId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string? SupplierName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string? Brand { get; set; }

        /// <summary>
        /// 起源
        /// </summary>
        public string? Origin { get; set; }

        /// <summary>
        /// 长度单位（0=毫米、1=厘米、2=分米、3=米）
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int LengthUnit { get; set; }

        /// <summary>
        /// 重量单位（0=毫克、1=克、2=千克）
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int WeightUnit { get; set; }

        /// <summary>
        /// 体积单位（0=立方厘米、1=立方分米、2=立方米）
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int VolumeUnit { get; set; }

        /// <summary>
        /// 商品sku列表
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(CommoditySKU.CommoditySPUId))]
        public List<CommoditySKU>? DetailList { get; set; }
    }
}
