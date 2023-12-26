using MiniExcelLibs.Attributes;

namespace GD.Model.Vm.Operation
{
    public class TakingVm : BaseVm
    {
        public long TakingId { get; set; }

        public string JobCode { get; set; } = string.Empty;

        public long JobStatus { get; set; } = 0;

        /// <summary>
        /// 是否已调整
        /// 0:否
        /// 1:是
        /// </summary>
        public long AdjustStatus { get; set; } = 0;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string SkuName { get; set; } = string.Empty;

        public string OnwerName { get; set; } = string.Empty;

        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public int BookQty { get; set; } = 0;

        public int CountedQty { get; set; } = 0;

        public int DifferenceQty { get; set; } = 0;

        public string Handler { get; set; } = string.Empty;

        public DateTime? HandlerTime { get; set; }
    }

    public class TakingExcelVm : BaseVm
    {
        private long jobStatus;
        private long adjustStatus;

        [ExcelColumn(Name = "作业单号")]
        public string JobCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "作业状态")]
        public string JobStatus
        {
            get
            {
                return (jobStatus == 0 || adjustStatus == 0) switch
                {
                    true => "待作业",
                    false => "已完成",
                };
            }
            set
            {
                jobStatus = int.Parse(value);
            }
        }

        [ExcelColumn(Ignore = true)]
        public long AdjustStatus { get => adjustStatus; set => adjustStatus = value; }

        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "所在仓库")]
        public string WarehouseName { get; set; } = string.Empty;

        [ExcelColumn(Name = "所在库位")]
        public string LocationCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "账面数量")]
        public int BookQty { get; set; } = 0;

        [ExcelColumn(Name = "盘点数量")]
        public int CountedQty { get; set; } = 0;

        [ExcelColumn(Name = "差异数量")]
        public int DifferenceQty { get; set; } = 0;

        [ExcelColumn(Name = "操作人")]
        public string Handler { get; set; } = string.Empty;

        [ExcelColumn(Name = "操作时间")]
        public DateTime? HandlerTime { get; set; }
    }
}
