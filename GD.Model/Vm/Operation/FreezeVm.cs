using MiniExcelLibs.Attributes;

namespace GD.Model.Vm.Operation
{
    public class FreezeVm : BaseVm
    {
        public long FreezeId { get; set; } = 0;

        /// <summary>
        /// 作业单号
        /// </summary>
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 作业类型
        /// 0:冻结
        /// 1:解冻
        /// </summary>
        public int JobType { get; set; } = 0;

        public string Handler { get; set; } = string.Empty;

        public DateTime HandlerTime { get; set; }

        public string WarehouseName { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;
    }

    public class FreezeExcelVm : BaseVm
    {
        private int jobType;

        [ExcelColumn(Name = "作业单号")]
        public string JobCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "作业类型")]
        public string JobType
        {
            get
            {
                return jobType switch
                {
                    0 => "冻结",
                    1 => "解冻",
                    _ => "未知",
                };
            }
            set
            {
                jobType = int.Parse(value);
            }
        }

        [ExcelColumn(Name = "所在仓库")]
        public string WarehouseName { get; set; } = string.Empty;

        [ExcelColumn(Name = "所在库位")]
        public string LocationCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "操作人")]
        public string Handler { get; set; } = string.Empty;

        [ExcelColumn(Name = "操作时间")]
        public DateTime HandlerTime { get; set; }

    }
}
