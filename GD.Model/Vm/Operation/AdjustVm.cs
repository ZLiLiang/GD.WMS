using MiniExcelLibs.Attributes;

namespace GD.Model.Vm.Operation
{
    public class AdjustVm : BaseVm
    {
        public long AdjustId { get; set; }


        /// <summary>
        /// 作业单号
        /// </summary>
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 作业类型
        /// 0：拆分,
        /// 1：组合,
        /// 2：盘点,
        /// 3：移动,
        /// </summary>
        public int JobType { get; set; } = 0;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string SkuName { get; set; } = string.Empty;

        public string OnwerName { get; set; } = string.Empty;

        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public int Qty { get; set; } = 0;

        public int IsUpate { get; set; } = 0;

        public long SourceTableId { get; set; } = 0;
    }

    public class AdjustExcelVm : BaseVm
    {
        private long jobType;

        [ExcelColumn(Name = "作业单号")]
        public string JobCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "作业状态")]
        public string JobType
        {
            get
            {
                return jobType switch
                {
                    0 => "拆分",
                    1 => "组合",
                    2 => "盘点",
                    3 => "移动",
                    _ => "未知",
                };
            }
            set
            {
                jobType = int.Parse(value);
            }
        }

        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "调整差异数量")]
        public int Qty { get; set; } = 0;

        [ExcelColumn(Name = "所在仓库")]
        public string WarehouseName { get; set; } = string.Empty;

        [ExcelColumn(Name = "所在库位")]
        public string LocationCode { get; set; } = string.Empty;
    }
}
