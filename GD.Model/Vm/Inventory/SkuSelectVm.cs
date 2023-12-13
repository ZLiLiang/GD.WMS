namespace GD.Model.Vm.Inventory
{
    public class SkuSelectVm
    {
        /// <summary>
        /// 规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

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
        /// 规格编码
        /// </summary>
        public string SkuCode { get; set; } = string.Empty;

        /// <summary>
        /// 规格名称
        /// </summary>
        public string SkuName { get; set; } = string.Empty;

        /// <summary>
        /// 货主Id
        /// </summary>
        public long SupplierId { get; set; } = 0;

        /// <summary>
        /// 货主名称
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; } = string.Empty;
    }
}
