using GD.Model.Dto.Operation;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;

namespace GD.Service.Interface.Operation
{
    /// <summary>
    /// 仓内加工服务
    /// </summary>
    public interface IProcessService : IBaseService<Process>
    {
        /// <summary>
        /// 分页获取所有数据
        /// </summary>
        /// <param name="processQueryDto"></param>
        /// <returns></returns>
        PagedInfo<ProcessVm> GetAll(ProcessQueryDto processQueryDto);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<ProcessVm> GetAll();

        /// <summary>
        /// 获取加工详细
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        ProcessVm Get(long processId);

        /// <summary>
        /// 新增加工
        /// </summary>
        /// <param name="processDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int Add(ProcessDto processDto, string userName);

        /// <summary>
        /// 编辑加工
        /// </summary>
        /// <param name="processDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int Edit(ProcessDto processDto, string userName);

        /// <summary>
        /// 删除加工
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        int Delete(long processId);

        /// <summary>
        /// 确认调整
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int ConfirmAdjustment(long processId, string userName);

        /// <summary>
        /// 确认加工
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int ConfirmProcess(long processId,string userName);

        /// <summary>
        /// 生成加工作业编号
        /// </summary>
        /// <returns></returns>
        string GetProcessJobCode();

        /// <summary>
        /// 生成调整作业编号
        /// </summary>
        /// <returns></returns>
        string GetAdjustJobCode();
    }
}
