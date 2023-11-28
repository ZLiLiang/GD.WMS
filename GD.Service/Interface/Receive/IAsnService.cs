using GD.Model.Dto.Receive;
using GD.Model.Enums;
using GD.Model.Page;
using GD.Model.Receive;

namespace GD.Service.Interface.Receive
{
    /// <summary>
    /// 收货服务
    /// </summary>
    public interface IAsnService : IBaseService<Asn>
    {
        /// <summary>
        /// 分页获取asn数据
        /// </summary>
        /// <param name="asnQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Asn> GetAll(AsnQueryDto asnQueryDto);

        /// <summary>
        /// 获取所有asn数据
        /// </summary>
        /// <returns></returns>
        List<Asn> GetAll(AsnStatus? asnStatus = null);

        /// <summary>
        /// 获取单个asn数据
        /// </summary>
        /// <param name="asnId"></param>
        /// <returns></returns>
        Asn Get(long asnId);

        /// <summary>
        /// 到货通知模块
        /// 新增Asn数据
        /// </summary>
        /// <param name="asn"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int Add(Asn asn, string userName);

        /// <summary>
        /// 到货通知模块
        /// 编辑Asn数据
        /// </summary>
        /// <param name="asn"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int Edit(Asn asn, string userName);

        /// <summary>
        /// 到货通知模块
        /// 根据id删除asn数据
        /// </summary>
        /// <param name="asnId"></param>
        /// <returns></returns>
        int Delete(long asnId);

        /// <summary>
        /// 更新ans的状态
        /// </summary>
        /// <param name="asnId"></param>
        /// <param name="asnStatus"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        int Operate(long asnId, AsnStatus asnStatus, string userName);

        /// <summary>
        /// 上架数量
        /// </summary>
        /// <param name="asnPutAwayDto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool PutAway(AsnPutAwayDto asnPutAwayDto, string userName);
    }
}
