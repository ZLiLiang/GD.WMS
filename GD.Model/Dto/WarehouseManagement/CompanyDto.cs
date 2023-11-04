using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.WarehouseManagement
{
    public class CompanyDto
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 负责人
        /// </summary>
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactTel { get; set; } = string.Empty;
    }
    public class CompanyQueryDto : PagerInfo
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string ContactTel { get; set; } = string.Empty;
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class CompanyExcelDto
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [ExcelColumn(Name = "公司名称", Width = 20)]
        public string CompanyName { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        [ExcelColumn(Name = "所在城市", Width = 40)]

        public string City { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [ExcelColumn(Name = "详细地址", Width = 40)]
        public string Address { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        [ExcelColumn(Name = "负责人", Width = 20)]
        public string Manager { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [ExcelColumn(Name = "联系方式", Width = 20)]
        public string ContactTel { get; set; }

        [ExcelColumn(Format = "yyyy-MM-dd HH:mm:ss", Name = "创建时间", Width = 20)]
        public DateTime Create_time { get; set; }
    }
}
