using GD.Model.Constant;

namespace GD.Model.Basic
{

    [SugarTable("wm_commoditysku", "商品管理表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class CommoditySKU : Base
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]
        public long CommoditySKUId { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string CommoditySKUCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string CommoditySKUName { get; set; }

        /// <summary>
        /// SPU商品Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long CommoditySPUId { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 长
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        /// <summary>
        /// 成本
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}
