using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;

namespace GD.Service.Interface.WarehouseManagement
{
    /// <summary>
    /// 运费服务
    /// </summary>
    public interface IFreightFeeService: IBaseService<FreightFee>
    {
        /// <summary>
        /// 分页获取所有运费信息
        /// </summary>
        /// <param name="freightFeeQueryDto"></param>
        /// <returns></returns>
        PagedInfo<FreightFee> GetAllFreightFees(FreightFeeQueryDto freightFeeQueryDto);

        /// <summary>
        /// 获取所有运费信息
        /// </summary>
        /// <returns></returns>
        List<FreightFee> GetAllFreightFees();

        /// <summary>
        /// 根据id获取运费信息
        /// </summary>
        /// <param name="freightFeeId"></param>
        /// <returns></returns>
        FreightFee GetFreightFee(long freightFeeId);

        /// <summary>
        /// 新增运费信息
        /// </summary>
        /// <param name="freightFee"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddFreightFee(FreightFee freightFee, string userName);

        /// <summary>
        /// 修改运费信息
        /// </summary>
        /// <param name="freightFee"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EidtFreightFee(FreightFee freightFee, string userName);

        /// <summary>
        /// 根据id删除运费信息
        /// </summary>
        /// <param name="freightFeeId"></param>
        /// <returns></returns>
        long DeleteFreightFee(long freightFeeId);

        /// <summary>
        /// 导入运费信息
        /// </summary>
        /// <returns></returns>
        (string, object) ImportFreightFees(List<FreightFee> freightFee);

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        (string, string) DownloadImportTemplate();
    }
}
