using GD.Model.Delivery;
using GD.Model.Dto.Delivery;
using GD.Model.Page;
using GD.Model.Vm.Delivery;

namespace GD.Service.Interface.Delivery
{
    /// <summary>
    /// 发货服务
    /// </summary>
    public interface IDispatchService : IBaseService<Dispatch>
    {
        /// <summary>
        /// 分页获取所有发货数据
        /// </summary>
        /// <param name="dispatchQueryDto"></param>
        /// <returns></returns>
        PagedInfo<DispatchVm> GetDispatchAll(DispatchQueryDto dispatchQueryDto);

        /// <summary>
        /// 获取所有发货数据
        /// </summary>
        /// <param name="sqlTitle"></param>
        /// <returns></returns>
        List<DispatchVm> GetDispatchAll(string sqlTitle);

        /// <summary>
        /// 分页获取所有预发货数据
        /// </summary>
        /// <param name="preDispatchQueryDto"></param>
        /// <returns></returns>
        PagedInfo<PreDispatchVm> GetPreDispatchAll(PreDispatchQueryDto preDispatchQueryDto);

        /// <summary>
        /// 获取所有预发货数据
        /// </summary>
        /// <param name="dispatchStatus"></param>
        /// <returns></returns>
        List<PreDispatchVm> GetPreDispatchAll(int dispatchStatus);

        /// <summary>
        /// 新增发货数据
        /// </summary>
        /// <param name="dispatchAddDtos"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Add(List<DispatchAddDto> dispatchAddDtos, string userName);

        /// <summary>
        /// 获取发货详细数据和可用数量
        /// </summary>
        /// <param name="dispatchNo"></param>
        /// <returns></returns>
        List<DispatchConfirmDetailVm> ConfirmOrderCheck(string dispatchNo);

        /// <summary>
        /// 根据发货编号获取发货数据
        /// </summary>
        /// <param name="dispatchNo"></param>
        /// <returns></returns>
        List<DispatchVm> GetByDispatchNo(string dispatchNo);

        /// <summary>
        /// 根据发货编号删除数据
        /// </summary>
        /// <param name="dispatchNo"></param>
        /// <returns></returns>
        (bool, string) Delete(string dispatchNo);

        /// <summary>
        /// 根据发货单号更新发货数据
        /// </summary>
        /// <param name="dispatchUpdateDtos"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Update(List<DispatchUpdateDto> dispatchUpdateDtos, string userName);

        /// <summary>
        /// 确认流程并创建发货挑选
        /// </summary>
        /// <param name="dispatchConfirmDetailDtos"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) ConfirmOrder(List<DispatchConfirmDetailDto> dispatchConfirmDetailDtos, string userName);

        /// <summary>
        /// 根据发货编号确认发货拣货
        /// </summary>
        /// <param name="dispatchNo"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) ConfirmPickByDispatchNo(string dispatchNo, string userName);

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="dispatchPackageDtos"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Package(List<DispatchPackageDto> dispatchPackageDtos, string userName);

        /// <summary>
        /// 称重
        /// </summary>
        /// <param name="dispatchWeightDtos"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Weight(List<DispatchWeightDto> dispatchWeightDtos, string userName);

        /// <summary>
        /// 进行出库操作
        /// </summary>
        /// <param name="dispatchDeliveryDtos"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Delivery(List<DispatchDeliveryDto> dispatchDeliveryDtos, string userName);

        /// <summary>
        /// 设置运费
        /// </summary>
        /// <param name="dispatchFreightfeeDtos"></param>
        /// <returns></returns>
        (bool, string) SetFreightfee(List<DispatchFreightfeeDto> dispatchFreightfeeDtos);

        /// <summary>
        /// 进行签收操作
        /// </summary>
        /// <param name="dispatchSignDtos"></param>
        /// <returns></returns>
        (bool, string) SignForArrival(List<DispatchSignDto> dispatchSignDtos);

        /// <summary>
        /// 根据发货Id获取打包数据
        /// </summary>
        /// <param name="dispatchId"></param>
        /// <returns></returns>
        List<DispatchpickVm> GetPickListByDispatchId(long dispatchId);

        /// <summary>
        /// 根据实体撤销返回到上一个流程
        /// </summary>
        /// <param name="cancelOrderOpration"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) CancelOrderOpration(CancelOrderOprationDto cancelOrderOpration, string userName);

        /// <summary>
        /// 根据发货id撤销返回到上一个流程
        /// </summary>
        /// <param name="dispatchId"></param>
        /// <returns></returns>
        (bool, string) CancelDispatchDetailOpration(long dispatchId);
    }
}
