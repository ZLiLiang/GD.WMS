using GD.Model.Constant;

namespace GD.Model.Basic
{
    [SugarTable("wm_category", "商品类别表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Category : Base
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long CategoryId { get; set; }

        /// <summary>
        /// 商品类别id
        /// </summary>
        [SugarColumn(Length = 50, ExtendedAttribute = 0)]
        public string CategoryName { get; set; }

        /// <summary>
        /// 上级类别id
        /// </summary>
        public long ParentId { get; set; }

        [SugarColumn(IsIgnore = true)]
        [JsonProperty("children")]
        public List<Category> ChildCategories { get; set; }
    }
}
