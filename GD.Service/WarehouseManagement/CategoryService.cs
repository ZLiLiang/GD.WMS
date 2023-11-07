﻿using GD.Infrastructure.Attribute;
using GD.Model.Dto.WarehouseManagement;
using GD.Model.Page;
using GD.Model.WarehouseManagement;
using GD.Repository;
using GD.Service.Interface.WarehouseManagement;
using SqlSugar;

namespace GD.Service.WarehouseManagement
{
    /// <summary>
    /// 商品类别
    /// </summary>
    [AppService(ServiceType = typeof(ICategoryService), ServiceLifetime = LifeTime.Transient)]
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        /// <summary>
        /// 新增商品类别
        /// </summary>
        /// <param name="category"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public long AddCategory(Category category, string userName)
        {
            category.Create_by = userName;
            category.Create_time = DateTime.Now;
            return InsertReturnBigIdentity(category);
        }

        /// <summary>
        /// 删除商品类别
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public long DeleteByCategoryId(long categoryId)
        {
            return Delete(categoryId);
        }

        /// <summary>
        /// 编辑商品类别
        /// </summary>
        /// <param name="category"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public long EditCategory(Category category, string userName)
        {
            category.Update_by = userName;
            category.Update_time = DateTime.Now;
            return Update(category);
        }

        /// <summary>
        /// 分页获取所有商品类别
        /// </summary>
        /// <param name="categoryQueryDto"></param>
        /// <returns></returns>
        public PagedInfo<Category> GetAllCategory(CategoryQueryDto categoryQueryDto)
        {
            var expression = Expressionable.Create<Category>()
                .AndIF(!string.IsNullOrEmpty(categoryQueryDto.CategoryName), exp => exp.CategoryName.Contains(categoryQueryDto.CategoryName))
                .AndIF(!string.IsNullOrEmpty(categoryQueryDto.CreateBy), exp => exp.Create_by.Contains(categoryQueryDto.CreateBy))
                .AndIF(categoryQueryDto.ParentId != 0, exp => exp.ParentId == categoryQueryDto.ParentId)
                .AndIF(categoryQueryDto.BeginTime != DateTime.MinValue && categoryQueryDto.BeginTime != null, exp => exp.Create_time >= categoryQueryDto.BeginTime)
                .AndIF(categoryQueryDto.EndTime != DateTime.MaxValue && categoryQueryDto.EndTime != null, exp => exp.Create_time <= categoryQueryDto.EndTime);

            var queryResult = Queryable()
                .Where(expression.ToExpression())
                .ToList();

            var buildResult = BuildCategoryTree(queryResult);
            int pageSize = categoryQueryDto.PageSize,
                pageNum = categoryQueryDto.PageNum,
                totalNum = buildResult.Count;
            var pagedResult = buildResult
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedInfo<Category>
            {
                PageIndex = pageNum,
                PageSize = pageSize,
                TotalNum = totalNum,
                Result = pagedResult
            };
        }

        public List<Category> GetAllCategory()
        {
            var queryResult = Queryable()
                .ToList();

            //return BuildCategoryTree(queryResult);
            return queryResult;
        }

        public List<Category> GetAllCategoryTree()
        {
            var queryResult = Queryable()
                .ToList();

            return BuildCategoryTree(queryResult);
        }

        public Category GetCategoryById(long categoryId)
        {
            return Queryable()
                .Where(epx => epx.CategoryId == categoryId)
                .Single();
        }

        public bool IsExistChild(long categoryId)
        {
            return Queryable()
                .Where(category => category.ParentId == categoryId)
                .Any();
        }


        /// <summary>
        /// 构建商品类别树
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        private List<Category> BuildCategoryTree(List<Category> categories)
        {
            var rootId = categories.Min(category => category.ParentId);
            var rootCategories = categories
                .Where(category => category.ParentId == rootId)
                .ToList();

            foreach (var item in rootCategories)
            {
                item.ChildCategories = RecursionSetSub(item, categories);
            }

            return rootCategories;
        }

        /// <summary>
        /// 递归构建商品类别树
        /// </summary>
        /// <param name="category">父商品类别</param>
        /// <param name="categories">商品类别列表</param>
        /// <returns></returns>
        private List<Category> RecursionSetSub(Category category, List<Category> categories)
        {
            var subCategories = categories
                .Where(predicate => predicate.ParentId == category.CategoryId)
                .ToList();

            foreach (var item in subCategories)
            {
                item.ChildCategories = RecursionSetSub(item, categories);
            }

            return subCategories;
        }
    }
}