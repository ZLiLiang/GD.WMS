using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;

namespace GD.Service.Interface.WarehouseManagement
{
    public interface ICategoryService : IBaseService<Category>
    {
        /// <summary>
        /// 分页获取所有商品类别
        /// </summary>
        /// <param name="categoryQueryDto"></param>
        /// <returns></returns>
        PagedInfo<Category> GetAllCategory(CategoryQueryDto categoryQueryDto);

        /// <summary>
        /// 获取所有商品类别,未构建成树
        /// </summary>
        /// <returns></returns>
        List<Category> GetAllCategory();

        /// <summary>
        /// 获取所有商品类别,构建成树
        /// </summary>
        /// <returns></returns>
        List<Category> GetAllCategoryTree();

        /// <summary>
        /// 查询单个商品类别
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Category GetCategoryById(long categoryId);

        /// <summary>
        /// 新增商品类别
        /// </summary>
        /// <param name="category"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long AddCategory(Category category, string userName);

        /// <summary>
        /// 编辑商品类别
        /// </summary>
        /// <param name="category"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        long EditCategory(Category category, string userName);

        /// <summary>
        /// 删除商品类别
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        long DeleteByCategoryId(long categoryId);

        /// <summary>
        /// 查询是否存在子树
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool IsExistChild(long categoryId);

        /// <summary>
        /// 查询是否被其他表引用
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public bool IsOtherUse(long categoryId);
    }
}
