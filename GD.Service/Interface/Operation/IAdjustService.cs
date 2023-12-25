using GD.Model.Dto.Operation;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;

namespace GD.Service.Interface.Operation
{
    /// <summary>
    /// 仓内调整服务
    /// </summary>
    public interface IAdjustService : IBaseService<Adjust>
    {
        /// <summary>
        /// 分页获取所有数据
        /// </summary>
        /// <param name="adjustQueryDto"></param>
        /// <returns></returns>
        PagedInfo<AdjustVm> GetAll(AdjustQueryDto adjustQueryDto);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<AdjustVm> GetAll();

        /// <summary>
        /// 根据Id获取调整作业信息
        /// </summary>
        /// <param name="adjustId"></param>
        /// <returns></returns>
        AdjustVm Get(long adjustId);

        /// <summary>
        /// 新增调整作业
        /// </summary>
        /// <param name="adjustDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Add(AdjustDto adjustDto, string userName);

        /// <summary>
        /// 更新调整作业
        /// </summary>
        /// <param name="adjustDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Update(AdjustDto adjustDto, string userName);

        /// <summary>
        /// 删除调整作业
        /// </summary>
        /// <param name="adjustId"></param>
        /// <returns></returns>
        (bool, string) Delete(long adjustId);

        /// <summary>
        /// 确认调整作业
        /// </summary>
        /// <param name="adjustId"></param>
        /// <returns></returns>
        (bool, string) ConfirmAdjustmen(long adjustId, string userName);
    }
}
