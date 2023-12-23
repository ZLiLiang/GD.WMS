using MiniExcelLibs.Attributes;

namespace GD.Model.Vm.Operation
{
    public class MoveVm : BaseVm
    {
        public long MoveId { get; set; }
        public string JobCode { get; set; } = string.Empty;

        /// <summary>
        /// 移动作业状态
        /// 0:未调整
        /// 1:已调整
        /// </summary>
        public int MoveStatus { get; set; } = 0;

        public string SpuCode { get; set; } = string.Empty;

        public string SpuName { get; set; } = string.Empty;

        public string SkuCode { get; set; } = string.Empty;

        public string SkuName { get; set; } = string.Empty;

        public int Qty { get; set; } = 0;

        public string OrigWarehousName { get; set; } = string.Empty;

        public string OrigLocationCode { get; set; } = string.Empty;

        public string DestWarehousName { get; set; } = string.Empty;

        public string DestLocationCode { get; set; } = string.Empty;
        public string Handler { get; set; } = string.Empty;

        public DateTime? HandlerTime { get; set; }
    }

    public class MoveExcelVm : BaseVm
    {
        private int moveStatus;

        [ExcelColumn(Name = "作业单号")]
        public string JobCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "作业状态")]
        public string MoveStatus
        {
            get
            {
                return moveStatus switch
                {
                    0 => "未调整",
                    1 => "已调整",
                    _ => "未知",
                };
            }
            set
            {
                moveStatus = int.Parse(value);
            }
        }

        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格名称")]
        public string SkuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "数量")]
        public int Qty { get; set; } = 0;

        [ExcelColumn(Name = "来源仓库")]
        public string OrigWarehousName { get; set; } = string.Empty;

        [ExcelColumn(Name = "来源库位")]
        public string OrigLocationCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "目标仓库")]
        public string DestWarehousName { get; set; } = string.Empty;

        [ExcelColumn(Name = "目标库位")]
        public string DestLocationCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "操作人")]
        public string Handler { get; set; } = string.Empty;

        [ExcelColumn(Name = "操作时间")]
        public DateTime? HandlerTime { get; set; }
    }
}
