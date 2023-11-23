using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.Basic
{
    public class SupplierDto
    {
        public string SupplierName { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string Manager { get; set; }
        public string? Email { get; set; }
        public string ContactTel { get; set; }
    }

    public class SupplierQueryDto : PagerInfo
    {
        public string SupplierName { get; set; }
        public string Manager { get; set; }
        public string ContactTel { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class SupplierExcelDto
    {
        [ExcelColumn(Name = "供应商名称")]
        public string SupplierName { get; set; }

        [ExcelColumn(Name = "所在城市")]
        public string? City { get; set; }

        [ExcelColumn(Name = "详细地址")]
        public string? Address { get; set; }

        [ExcelColumn(Name = "负责人")]
        public string Manager { get; set; }

        [ExcelColumn(Name = "邮箱")]
        public string? Email { get; set; }

        [ExcelColumn(Name = "联系方式")]
        public string ContactTel { get; set; }

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; }

        [ExcelColumn(Name = "创建时间")]
        public DateTime CreateTime { get; set; }
    }
}
