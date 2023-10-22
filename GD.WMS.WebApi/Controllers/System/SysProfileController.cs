using GD.Infrastructure.Attribute;
using GD.Infrastructure.CustomException;
using GD.Infrastructure.Extensions;
using GD.Model.Dto.System;
using GD.Model.Enums;
using GD.Model.System;
using GD.Model;
using GD.Service.Interface.System;
using GD.WMS.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using GD.WMS.WebApi.Extensions;

namespace GD.WMS.WebApi.Controllers.System
{
    /// <summary>
    /// 个人中心
    /// </summary>
    [Verify]
    [Route("system/profile")]
    //[ApiExplorerSettings(GroupName = "sys")]
    public class SysProfileController : BaseController
    {
        private readonly ISysUserService UserService;
        private readonly ISysRoleService RoleService;
        private readonly ISysFileService FileService;
        private IWebHostEnvironment hostEnvironment;

        public SysProfileController(
            ISysUserService userService,
            ISysRoleService roleService,
            ISysFileService sysFileService,
            IWebHostEnvironment hostEnvironment)
        {
            UserService = userService;
            RoleService = roleService;
            FileService = sysFileService;
            this.hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// 个人中心用户信息获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Profile()
        {
            long userId = HttpContext.GetUId();
            var user = UserService.SelectUserById(userId);

            var roles = RoleService.SelectUserRoleNames(userId);

            return SUCCESS(new { user, roles}, TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [HttpPut()]
        [Log(Title = "修改信息", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateProfile([FromBody] SysUserDto userDto)
        {
            if (userDto == null)
            {
                throw new CustomException(ResultCode.PARAM_ERROR, "请求参数错误");
            }
            var user = userDto.Adapt<SysUser>().ToUpdate(HttpContext);

            int result = UserService.ChangeUser(user);
            return ToResponse(result);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPut("updatePwd")]
        [Log(Title = "修改密码", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdatePwd(string oldPassword, string newPassword)
        {
            long userId = HttpContext.GetUId();
            SysUser user = UserService.SelectUserById(userId);
            string oldMd5 = NETCore.Encrypt.EncryptProvider.Md5(oldPassword);
            string newMd5 = NETCore.Encrypt.EncryptProvider.Md5(newPassword);

            if (!user.Password.Equals(oldMd5, StringComparison.OrdinalIgnoreCase))
            {
                return ToResponse(ApiResult.Error("修改密码失败，旧密码错误"));
            }
            if (user.Password.Equals(newMd5, StringComparison.OrdinalIgnoreCase))
            {
                return ToResponse(ApiResult.Error("新密码不能和旧密码相同"));
            }
            if (UserService.ResetPwd(userId, newMd5) > 0)
            {
                //TODO 更新缓存

                return SUCCESS(1);
            }

            return ToResponse(ApiResult.Error("修改密码异常，请联系管理员"));
        }

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("Avatar")]
        [Log(Title = "修改头像", BusinessType = BusinessType.UPDATE, IsSaveRequestData = false)]
        public async Task<IActionResult> Avatar([FromForm(Name = "picture")] IFormFile formFile)
        {
            long userId = HttpContext.GetUId();
            if (formFile == null) throw new CustomException("请选择文件");

            SysFile file = await FileService.SaveFileToLocal(hostEnvironment.WebRootPath, "", "avatar", HttpContext.GetName(), formFile);

            UserService.UpdatePhoto(new SysUser() { Avatar = file.AccessUrl, UserId = userId });
            return SUCCESS(new { imgUrl = file.AccessUrl });
        }
    }
}
