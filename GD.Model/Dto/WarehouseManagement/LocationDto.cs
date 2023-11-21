using GD.Model.Page;
using MiniExcelLibs.Attributes;

namespace GD.Model.Dto.WarehouseManagement
{
    /// <summary>
    /// 新增、编辑实体
    /// </summary>
    public class LocationDto
    {
        public long WarehouseId { get; set; }

        public long RegionId { get; set; }

        public int RegionProperty { get; set; }

        public string LocationCode { get; set; } = string.Empty;

        public decimal LocationLength { get; set; }

        public decimal LocationWidth { get; set; }

        public decimal LocationHeight { get; set; }

        public decimal LocationVolume { get; set; }

        public decimal LocationLoad { get; set; }

        public string RoadwayNumber { get; set; } = string.Empty;

        public string ShelfNumber { get; set; } = string.Empty;

        public string LayerNumber { get; set; } = string.Empty;

        public string TagNumber { get; set; } = string.Empty;

        public int IsValid { get; set; }
    }

    /// <summary>
    /// 查询实体
    /// </summary>
    public class LocationQueryDto : PagerInfo
    {
        public string WarehouseName { get; set; } = string.Empty;

        public string RegionName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 导出实体
    /// </summary>
    public class LocationExcelDto
    {
        private int _regionProperty;
        private int _isValid;

        [ExcelColumn(Name = "仓库名称")]
        public string WarehouseName { get; set; } = string.Empty;

        [ExcelColumn(Name = "库区名称")]
        public string RegionName { get; set; } = string.Empty;

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

        [ExcelColumn(Name = "库位编码")]
        public string LocationCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "库位长(m)")]
        public decimal LocationLength { get; set; }

        [ExcelColumn(Name = "库位宽(m)")]
        public decimal LocationWidth { get; set; }

        [ExcelColumn(Name = "库位高(m)")]
        public decimal LocationHeight { get; set; }

        [ExcelColumn(Name = "库位容积（m³）")]
        public decimal LocationVolume { get; set; }

        [ExcelColumn(Name = "库位承重（kg）")]
        public decimal LocationLoad { get; set; }

        [ExcelColumn(Name = "巷道号")]
        public string RoadwayNumber { get; set; } = string.Empty;

        [ExcelColumn(Name = "货架号")]
        public string ShelfNumber { get; set; } = string.Empty;

        [ExcelColumn(Name = "层号")]
        public string LayerNumber { get; set; } = string.Empty;

        [ExcelColumn(Name = "位号")]
        public string TagNumber { get; set; } = string.Empty;

        [ExcelColumn(Name = "创建者")]
        public string CreateBy { get; set; } = string.Empty;

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
