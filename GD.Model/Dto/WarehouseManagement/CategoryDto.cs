using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.WarehouseManagement
{
    public class CategoryDto
    {
        /// <summary>
        /// 商品类别id
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 上级类别id
        /// </summary>
        public long ParentId { get; set; }
    }

    public class CategoryQueryDto : PagerInfo
    {
        public string CategoryName { get; set; }
        public string CreateBy { get; set; }
        public long ParentId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class CategoryExcelDto
    {
        [ExcelColumn(Name = "商品类别名称")]
        public string CategoryName { get; set; }

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; }

        [ExcelColumn(Name = "创建时间")]
        public DateTime Create_time { get; set; }

        [ExcelColumn(Name = "归属")]
        public string? Parent { get; set; }

        [ExcelColumn(Ignore = true)]
        public List<CategoryExcelDto> ChildCategories { get; set; }
    }
}
