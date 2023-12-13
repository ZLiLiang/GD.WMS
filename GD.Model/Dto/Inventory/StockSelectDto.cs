using GD.Model.Page;

namespace GD.Model.Dto.Inventory
{
    public class StockSelectDto : PagerInfo
    {
        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        /// <summary>
        /// Custom Classification
        /// </summary>
        public string SqlTitle { get; set; } = string.Empty;
    }
}
