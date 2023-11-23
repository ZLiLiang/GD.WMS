using GD.Common;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.Extensions;
using GD.Model.Basic;
using GD.Model.Dto.Basic;
using GD.Model.Enums;
using GD.Service.Interface.Basic;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GD.WMS.WebApi.Controllers.Basic
{
    [Verify]
    [Route("/basic/category")]
    [ApiExplorerSettings(GroupName = "basic")]
    public class CategoryController : BaseController
    {
        private ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] CategoryQueryDto categoryQueryDto)
        {
            var result = categoryService.GetAllCategory(categoryQueryDto);
            return SUCCESS(result);
        }

        [HttpGet("tree")]
        public IActionResult GetAllTree()
        {
            var result = categoryService.GetAllCategoryTree();
            return SUCCESS(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryInfo(long id)
        {
            var result = categoryService.GetCategoryById(id);

            return SUCCESS(result);
        }

        [HttpPost("add")]
        [Log(Title = "商品类别", BusinessType = BusinessType.INSERT)]
        public IActionResult Add([FromQuery] CategoryDto categoryDto)
        {
            var category = categoryDto.Adapt<Category>();
            var userName = HttpContext.GetName();
            var result = categoryService.AddCategory(category, userName);

            return SUCCESS(result);
        }

        [HttpPost("edit/{id}")]
        [Log(Title = "商品类别", BusinessType = BusinessType.UPDATE)]
        public IActionResult Edit(int id, [FromQuery] CategoryDto categoryDto)
        {
            var category = categoryDto.Adapt<Category>();
            category.CategoryId = id;
            var userName = HttpContext.GetName();
            var result = categoryService.EditCategory(category, userName);

            return SUCCESS(result);
        }

        [HttpDelete("{id}")]
        [Log(Title = "商品类别", BusinessType = BusinessType.DELETE)]
        public IActionResult Delete(long id)
        {
            if (categoryService.IsExistChild(id))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "存在子菜单,不允许删除");
            }
            if (categoryService.IsOtherUse(id))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "该类别存在商品，不允许删除");
            }
            var result = categoryService.DeleteByCategoryId(id);

            return SUCCESS(result);
        }

        [HttpGet("export")]
        [Log(Title = "商品类别", BusinessType = BusinessType.EXPORT)]
        public IActionResult CompanyExport()
        {
            var config = new TypeAdapterConfig();
            config.ForType<Category, CategoryExcelDto>()
                .Map(dest => dest.CreateBy, src => src.Create_by)
                .Map(dest => dest.Parent, src => src.ParentId.ToString());
            var categories = categoryService.GetAllCategory();
            var categoryxcelDtos = categories.Select(category => category.Adapt<CategoryExcelDto>(config)).ToList();
            categoryxcelDtos.ForEach(category =>
            {
                var parentId = int.Parse(category.Parent);
                category.Parent = categories
                .Where(exp => exp.CategoryId == parentId)
                .FirstOrDefault()?
                .CategoryName;
            });
            var result = ExcelHelper.ExportExcelMini(categoryxcelDtos, "category", "商品类别");

            return ExportExcel(result.Item2, result.Item1);
        }
    }
}
