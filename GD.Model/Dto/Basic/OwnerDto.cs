using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.Basic
{
    /// <summary>
    /// 新增、编辑实体
    /// </summary>
    public class OwnerDto
    {
        public string OwnerName { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string ContactTel { get; set; } = string.Empty;

        public string Manager { get; set; } = string.Empty;
    }

    /// <summary>
    /// 查询实体
    /// </summary>
    public class OwnerQueryDto : PagerInfo
    {
        public string OwnerName { get; set; } = string.Empty;

        public string ContactTel { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 导出实体
    /// </summary>
    public class OwnerExcelDto
    {
        [ExcelColumn(Name = "货主名称")]
        public string OwnerName { get; set; } = string.Empty;

        [ExcelColumn(Name = "所在城市")]
        public string City { get; set; } = string.Empty;

        [ExcelColumn(Name = "详细地址")]
        public string Address { get; set; } = string.Empty;

        [ExcelColumn(Name = "联系方式")]
        public string ContactTel { get; set; } = string.Empty;

        [ExcelColumn(Name = "负责人")]
        public string Manager { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建时间")]
        public DateTime Create_time { get; set; }
    }
}
