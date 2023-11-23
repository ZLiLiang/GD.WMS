using GD.Model.Page;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.Dto.Basic
{
    public class FreightFeeDto
    {
        public string Carrier { get; set; } = string.Empty;

        public string DepartureCity { get; set; } = string.Empty;

        public string ArrivalCity { get; set; } = string.Empty;

        public decimal PricePerWeight { get; set; } = 0;

        public decimal PricePerVolume { get; set; } = 0;

        public decimal MinPayment { get; set; } = 0;

        public int IsValid { get; set; }
    }

    public class FreightFeeQueryDto : PagerInfo
    {
        public string Carrier { get; set; } = string.Empty;

        public string DepartureCity { get; set; } = string.Empty;

        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class FreightFeeExcelDto
    {
        private int _isValid;

        [ExcelColumn(Name = "承运商")]
        public string Carrier { get; set; } = string.Empty;

        [ExcelColumn(Name = "始发城市")]
        public string DepartureCity { get; set; } = string.Empty;

        [ExcelColumn(Name = "到货城市")]
        public string ArrivalCity { get; set; } = string.Empty;

        [ExcelColumn(Name = "单公斤运费")]
        public decimal PricePerWeight { get; set; } = 0;

        [ExcelColumn(Name = "单立方米运费")]
        public decimal PricePerVolume { get; set; } = 0;

        [ExcelColumn(Name = "最小运费")]
        public decimal MinPayment { get; set; } = 0;

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
