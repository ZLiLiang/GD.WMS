using GD.Model.Dto.Operation;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;

namespace GD.Service.Interface.Operation
{
    /// <summary>
    /// 仓内盘点服务
    /// </summary>
    public interface ITakingService : IBaseService<Taking>
    {
        /// <summary>
        /// 分页获取所有数据
        /// </summary>
        /// <param name="takingQueryDto"></param>
        /// <returns></returns>
        PagedInfo<TakingVm> GetAll(TakingQueryDto takingQueryDto);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<TakingVm> GetAll();

        /// <summary>
        /// 根据Id获取盘点作业信息
        /// </summary>
        /// <param name="takingId"></param>
        /// <returns></returns>
        TakingVm Get(long takingId);

        /// <summary>
        /// 新增盘点作业
        /// </summary>
        /// <param name="takingDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Add(TakingDto takingDto, string userName);

        /// <summary>
        /// 确认盘点作业
        /// </summary>
        /// <param name="takingPutDto"></param>
        /// <returns></returns>
        (bool, string) Put(TakingPutDto takingPutDto, string userName);

        /// <summary>
        /// 确认调整
        /// </summary>
        /// <param name="takingId"></param>
        /// <returns></returns>
        (bool, string) Confirm(long takingId, string userName);

        /// <summary>
        /// 删除盘点作业
        /// </summary>
        /// <param name="takingId"></param>
        /// <returns></returns>
        (bool, string) Delete(long takingId);

        /// <summary>
        /// 生成盘点作业编号
        /// </summary>
        /// <returns></returns>
        string GetTakingJobCode();
    }
}
