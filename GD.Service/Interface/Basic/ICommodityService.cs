using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Page;

namespace GD.Service.Interface.Basic
{
    public interface ICommodityService : IBaseService<CommoditySPU>
    {
        /// <summary>
        /// 分页获取商品数据
        /// </summary>
        /// <param name="commodityQueryDto"></param>
        /// <returns></returns>
        PagedInfo<CommoditySPU> GetAllCommodities(CommodityQueryDto commodityQueryDto);

        /// <summary>
        /// 获取所有商品数据
        /// </summary>
        /// <returns></returns>
        List<CommoditySPU> GetAllCommodities();

        /// <summary>
        /// 根据Id获取单个商品数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CommoditySPU GetCommodityById(long id);

        /// <summary>
        /// 新增商品信息
        /// </summary>
        /// <param name="commoditySPU"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddCommodity(CommoditySPU commoditySPU, string userName);

        /// <summary>
        /// 编辑商品信息
        /// </summary>
        /// <param name="commoditySPU"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EditCommodity(CommoditySPU commoditySPU, string userName);

        /// <summary>
        /// 根据id删除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        long DeleteCommodityById(long id);

        /// <summary>
        /// 根据skuId删除商品sku
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        long DeleteSKUById(long id);
    }
}
