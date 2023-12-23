using GD.Model.Dto.Operation;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;

namespace GD.Service.Interface.Operation
{
    /// <summary>
    /// 仓内移动服务
    /// </summary>
    public interface IMoveService : IBaseService<Move>
    {
        /// <summary>
        /// 分页获取所有数据
        /// </summary>
        /// <param name="moveQueryDto"></param>
        /// <returns></returns>
        PagedInfo<MoveVm> GetAll(MoveQueryDto moveQueryDto);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<MoveVm> GetAll();

        /// <summary>
        /// 根据Id获取移动作业信息
        /// </summary>
        /// <param name="moveId"></param>
        /// <returns></returns>
        MoveVm Get(long moveId);

        /// <summary>
        /// 新增移动作业
        /// </summary>
        /// <param name="moveDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool,string) Add(MoveDto moveDto, string userName);

        /// <summary>
        /// 确定移动作业
        /// </summary>
        /// <param name="moveId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Confirm(long moveId, string userName);

        /// <summary>
        /// 删除移动作业
        /// </summary>
        /// <param name="moveId"></param>
        /// <returns></returns>
        (bool, string) Delete(long moveId);

        /// <summary>
        /// 生成移动作业编号
        /// </summary>
        /// <returns></returns>
        string GetMoveJobCode();
    }
}
