using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.Basic
{
    public class RegionDto
    {
        public long WarehouseId { get; set; }

        public string RegionName { get; set; }

        public int RegionProperty { get; set; }

        public int IsValid { get; set; }
    }

    public class RegionQueryDto : PagerInfo
    {
        public string WarehouseName { get; set; }

        public string RegionName { get; set; }

        public int? RegionProperty { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class RegionExcelDto
    {
        private int _regionProperty;
        private int _isValid;

        [ExcelColumn(Name = "仓库名称")]
        public string WarehouseName { get; set; }

        [ExcelColumn(Name = "库区名称")]
        public string RegionName { get; set; }

        [ExcelColumn(Name = "库区类型")]
        public string RegionProperty
        {
            set => _regionProperty = int.Parse(value);
            get => _regionProperty switch
            {
                0 => "拣货区",
                1 => "备货区",
                2 => "收货区",
                3 => "退货区",
                4 => "次品区",
                5 => "存货区"
            };
        }

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; }

        [ExcelColumn(Name = "创建时间")]
        public DateTime Create_time { get; set; }

        [ExcelColumn(Name = "是否有效")]
        public string IsValid
        {
            set => _isValid = int.Parse(value);
            get => _isValid switch
            {
                0 => "否",
                1 => "是"
            };
        }
    }
}
