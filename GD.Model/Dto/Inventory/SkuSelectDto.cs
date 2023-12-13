using GD.Model.Page;

namespace GD.Model.Dto.Inventory
{
    public class SkuSelectQueryDto : PagerInfo
    {
        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;
    }
}
