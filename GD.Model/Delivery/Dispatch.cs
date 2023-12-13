using GD.Model.Constant;

namespace GD.Model.Delivery
{
    /// <summary>
    /// 发货管理表
    /// </summary>
    [SugarTable("wm_dispatch", "发货管理表")]
    [Tenant(DBConfigId.WarehouseManagement)]
    public class Dispatch : Base
    {
        /// <summary>
        /// 派工单id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long DispatchId { get; set; }

        /// <summary>
        /// 派工单编号
        /// </summary>
        public string DispatchNo { get; set; } = string.Empty;

        /// <summary>
        /// 派工单状态
        /// </summary>
        public int DispatchStatus { get; set; } = 0;

        /// <summary>
        /// 顾客Id
        /// </summary>
        public long CustomerId { get; set; } = 0;

        /// <summary>
        /// 顾客名称
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// 商品规格Id
        /// </summary>
        public long SkuId { get; set; } = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public int Qty { get; set; } = 0;

        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; } = 0;

        /// <summary>
        /// 体积
        /// </summary>
        public decimal Volume { get; set; } = 0;

        /// <summary>
        /// 破损数量
        /// </summary>
        public int DamageQty { get; set; } = 0;

        /// <summary>
        /// 锁定数量
        /// </summary>
        public int LockQty { get; set; } = 0;

        /// <summary>
        /// 已拣货数量
        /// </summary>
        public int PickedQty { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int IntrasitQty { get; set; } = 0;

        /// <summary>
        /// 已打包数量
        /// </summary>
        public int PackageQty { get; set; } = 0;

        /// <summary>
        /// 称重数量
        /// </summary>
        public int WeighingQty { get; set; } = 0;

        /// <summary>
        /// 出库数量
        /// </summary>
        public int ActualQty { get; set; } = 0;

        /// <summary>
        /// 签收数量
        /// </summary>
        public int SignQty { get; set; } = 0;

        /// <summary>
        /// 打包编号
        /// </summary>
        public string PackageNo { get; set; } = string.Empty;

        /// <summary>
        /// 打包人
        /// </summary>
        public string PackagePerson { get; set; } = string.Empty;

        /// <summary>
        /// 打包时间
        /// </summary>
        public DateTime PackageTime { get; set; }

        /// <summary>
        /// 称重编号
        /// </summary>
        public string WeighingNo { get; set; } = string.Empty;

        /// <summary>
        /// 称重人
        /// </summary>
        public string WeighingPerson { get; set; } = string.Empty;

        /// <summary>
        /// 称重重量
        /// </summary>
        public decimal WeighingWeight { get; set; } = 0;

        /// <summary>
        /// 运单号
        /// </summary>
        public string WayBillNo { get; set; } = string.Empty;

        /// <summary>
        /// 承运单位
        /// </summary>
        public string Carrier { get; set; } = string.Empty;

        /// <summary>
        /// 运费
        /// </summary>
        public decimal Freightfee { get; set; } = 0;

        [Navigate(NavigateType.OneToMany,nameof(Dispatchpick.DispatchpickId))]
        public List<Dispatchpick> Dispatchpicklists { get; set; } = new();
    }
}
