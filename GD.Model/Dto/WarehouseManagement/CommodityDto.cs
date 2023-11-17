using GD.Model.Page;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.Dto.WarehouseManagement
{
    public class CommoditySPUDto
    {
        public string CommoditySPUCode { get; set; }
        public string CommoditySPUName { get; set; }
        public long CategoryId { get; set; }
        public string? CommoditySPUDescription { get; set; }
        public string? BarCode { get; set; }
        public string? Brand { get; set; }
        public long SupplierId { get; set; }
        public int LengthUnit { get; set; }
        public int WeightUnit { get; set; }
        public int VolumeUnit { get; set; }

        public List<CommoditySKUDto> DetailList { get; set; }
    }

    public class CommoditySKUDto
    {
        public string CommoditySKUCode { get; set; }
        public string CommoditySKUName { get; set; }
        public string CommoditySPUId { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Volume { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
    }

    public class CommodityQueryDto : PagerInfo
    {
        public string CommoditySPUCode { get; set; }
        public string CommoditySPUName { get; set; }
        public long? CategoryId { get; set; }

    }

    public class CommoditySPUExcelDto
    {
        [ExcelColumn(Name = "商品编码")]
        public string CommoditySPUCode { get; set; }

        [ExcelColumn(Name = "商品名称")]
        public string CommoditySPUName { get; set; }

        [ExcelColumn(Name = "商品类别名称")]
        public string CategoryName { get; set; }

        [ExcelColumn(Name = "商品描述")]
        public string? CommoditySPUDescription { get; set; }

        [ExcelColumn(Name = "商品条码")]
        public string? BarCode { get; set; }

        [ExcelColumn(Name = "供应商名称")]
        public string SupplierName { get; set; }

        [ExcelColumn(Name = "品牌")]
        public string Brand { get; set; }

        private int lengthUniy;

        [ExcelColumn(Name = "长度单位")]
        public string LengthUnit
        {
            get
            {
                switch (lengthUniy)
                {
                    case 0:
                        return "毫米";
                    case 1:
                        return "厘米";
                    case 2:
                        return "分米";
                    case 3:
                        return "米";
                    default:
                        return "无";
                }
            }
            set => lengthUniy = int.Parse(value);
        }

        private int weightUnit;

        [ExcelColumn(Name = "重量单位")]
        public string WeightUnit
        {
            get
            {
                switch (weightUnit)
                {
                    case 0:
                        return "毫克";
                    case 1:
                        return "克";
                    case 2:
                        return "千克";
                    default:
                        return "无";
                }
            }
            set => weightUnit = int.Parse(value);
        }

        private int volumeUnit;

        [ExcelColumn(Name = "体积单位")]
        public string VolumeUnit
        {
            get
            {
                switch (volumeUnit)
                {
                    case 0:
                        return "立方厘米";
                    case 1:
                        return "立方分米";
                    case 2:
                        return "立方米";
                    default:
                        return "无";
                }
            }
            set => volumeUnit = int.Parse(value);
        }

        [ExcelColumn(Ignore = true)]
        public List<CommoditySKUExcelDto> DetailList { get; set; }
    }

    public class CommoditySKUExcelDto
    {
        [ExcelColumn(Name = "商品sku编码", Index = 9)]
        public string CommoditySKUCode { get; set; }

        [ExcelColumn(Name = "商品sku名称", Index = 10)]
        public string CommoditySKUName { get; set; }

        [ExcelColumn(Name = "单位", Index = 11)]
        public string Unit { get; set; }

        [ExcelColumn(Name = "重量", Index = 12)]
        public decimal Weight { get; set; }

        [ExcelColumn(Name = "长度", Index = 13)]
        public decimal Length { get; set; }

        [ExcelColumn(Name = "宽度", Index = 14)]
        public decimal Width { get; set; }

        [ExcelColumn(Name = "高度", Index = 15)]
        public decimal Height { get; set; }

        [ExcelColumn(Name = "体积", Index = 16)]
        public decimal Volume { get; set; }

        [ExcelColumn(Name = "成本", Index = 17)]
        public decimal Cost { get; set; }

        [ExcelColumn(Name = "价格", Index = 18)]
        public decimal Price { get; set; }
    }
}
