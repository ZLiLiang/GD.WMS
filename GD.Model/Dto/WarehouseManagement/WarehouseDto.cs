using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.WarehouseManagement
{
    public class WarehouseDto
    {
        public string WarehouseName { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string ContactTel { get; set; }

        public string Email { get; set; }

        public string Manager { get; set; }

        public int IsValid { get; set; }
    }

    public class WarehouseQueryDto : PagerInfo
    {
        public string WarehouseName { get; set; }

        public string City { get; set; }

        public string Manager { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class WarehouseExcelDto
    {
        [ExcelColumn(Name = "仓库名称")]
        public string WarehouseName { get; set; }

        [ExcelColumn(Name = "所在城市")]
        public string City { get; set; }

        [ExcelColumn(Name = "详细地址")]
        public string Address { get; set; }

        [ExcelColumn(Name = "联系方式")]
        public string ContactTel { get; set; }

        [ExcelColumn(Name = "Email")]
        public string Email { get; set; }

        [ExcelColumn(Name = "负责人")]
        public string Manager { get; set; }

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; }

        [ExcelColumn(Name = "创建时间")]
        public DateTime Create_time { get; set; }

        [ExcelColumn(Name = "是否有效")]
        public int IsValid { get; set; }
    }
}
