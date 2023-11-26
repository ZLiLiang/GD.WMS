using GD.Model.Page;
using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD.Model.Dto.Receive
{
    public class AsnDto
    {
        public string AsnNo { get; set; } = string.Empty;

        public int AsnStatus { get; set; } = 0;
        /// <summary>
        /// 供应商Id
        /// </summary>
        public long SupplierId { get; set; } = 0;

        /// <summary>
        /// 货主Id
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public long SpuId { get; set; } = 0;

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SpuCode { get; set; } = string.Empty;

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SpuName { get; set; } = string.Empty;

        /// <summary>
        /// 规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 规格编码
        /// </summary>
        public string SkuCode { get; set; } = string.Empty;

        /// <summary>
        /// 规格名称
        /// </summary>
        public string SkuName { get; set; } = string.Empty;

        /// <summary>
        /// 到货通知书数据
        /// </summary>
        public int AsnQty { get; set; } = 0;

        /// <summary>
        /// 上架数量
        /// </summary>
        public int ActualQty { get; set; } = 0;

        /// <summary>
        /// 分拣数量
        /// </summary>
        public int SortedQty { get; set; } = 0;

        /// <summary>
        /// 短少数量
        /// </summary>
        public int ShortageQty { get; set; } = 0;

        /// <summary>
        /// 超量数量
        /// </summary>
        public int MoreQty { get; set; } = 0;

        /// <summary>
        /// 破损数量
        /// </summary>
        public int DamageQty { get; set; } = 0;
    }

    public class AsnQueryDto : PagerInfo
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// 规格名称
        /// </summary>
        public string SkuName { get; set; } = string.Empty;

        /// <summary>
        /// 货物状态
        /// </summary>
        public int? AsnStatus { get; set; }

        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class AsnExcelDto
    {
        [ExcelColumn(Name = "到货通知书编号")]
        public string AsnNo { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品编码")]
        public string SpuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "商品名称")]
        public string SpuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格编码")]
        public string SkuCode { get; set; } = string.Empty;

        [ExcelColumn(Name = "规格名称")]
        public string SkuName { get; set; } = string.Empty;

        [ExcelColumn(Name = "货主名称")]
        public string OwnerName { get; set; } = string.Empty;

        [ExcelColumn(Name = "供应商名称")]
        public string SupplierName { get; set; } = string.Empty;

        [ExcelColumn(Name = "到货通知书数据")]
        public int AsnQty { get; set; } = 0;

        [ExcelColumn(Name = "总重量")]
        public decimal Weight { get; set; } = 0;

        [ExcelColumn(Name = "总体积")]
        public decimal Volume { get; set; } = 0;

        [ExcelColumn(Name = "上架数量")]
        public int ActualQty { get; set; } = 0;

        [ExcelColumn(Name = "分拣数量")]
        public int SortedQty { get; set; } = 0;

        [ExcelColumn(Name = "短少数量")]
        public int ShortageQty { get; set; } = 0;

        [ExcelColumn(Name = "超量数量")]
        public int MoreQty { get; set; } = 0;

        [ExcelColumn(Name = "破损数量")]
        public int DamageQty { get; set; } = 0;
    }
}
