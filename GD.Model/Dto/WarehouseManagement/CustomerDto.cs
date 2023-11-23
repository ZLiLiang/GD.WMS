using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.WarehouseManagement
{
    public class CustomerDto
    {
        public string CustomerName { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Manager { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ContactTel { get; set; } = string.Empty;
    }

    public class CustomerQueryDto : PagerInfo
    {
        public string CustomerName { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class CustomerExcelDto
    {
        [ExcelColumn(Name = "客户名称")]
        public string CustomerName { get; set; } = string.Empty;

        [ExcelColumn(Name = "所在城市")]
        public string City { get; set; } = string.Empty;

        [ExcelColumn(Name = "详细地址")]
        public string Address { get; set; } = string.Empty;

        [ExcelColumn(Name = "负责人")]
        public string Manager { get; set; } = string.Empty;

        [ExcelColumn(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [ExcelColumn(Name = "联系方式")]
        public string ContactTel { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建时间")]
        public DateTime Create_time { get; set; }
    }
}
