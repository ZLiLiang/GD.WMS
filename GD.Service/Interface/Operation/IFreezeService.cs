using GD.Model.Dto.Operation;
using GD.Model.Operation;
using GD.Model.Page;
using GD.Model.Vm.Operation;

namespace GD.Service.Interface.Operation
{
    /// <summary>
    /// 仓内冻结服务
    /// </summary>
    public interface IFreezeService : IBaseService<Freeze>
    {
        /// <summary>
        /// 分页获取所有数据
        /// </summary>
        /// <param name="freezeQueryDto"></param>
        /// <returns></returns>
        PagedInfo<FreezeVm> GetAll(FreezeQueryDto freezeQueryDto);

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<FreezeVm> GetAll();

        /// <summary>
        /// 根据Id获取冻结作业信息
        /// </summary>
        /// <param name="freezeId"></param>
        /// <returns></returns>
        FreezeVm Get(long freezeId);

        /// <summary>
        /// 新增冻结作业
        /// </summary>
        /// <param name="freezeDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Add(FreezeDto freezeDto, string userName);

        /// <summary>
        /// 更新冻结作业
        /// </summary>
        /// <param name="freezeDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        (bool, string) Update(FreezeDto freezeDto, string userName);

        /// <summary>
        /// 删除冻结作业
        /// </summary>
        /// <param name="freezeId"></param>
        /// <returns></returns>
        (bool, string) Delete(long freezeId);

        /// <summary>
        /// 生成冻结作业编号
        /// </summary>
        /// <returns></returns>
        string GetFreezeJobCode();
    }
}
