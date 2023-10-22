using GD.Infrastructure.App;
using GD.Infrastructure.Attribute;
using GD.Infrastructure.CustomException;
using GD.Infrastructure.Helper;
using GD.Infrastructure;
using GD.Model.Dto.System;
using GD.Model.Enums;
using GD.Model.System;
using GD.Model;
using GD.Service.Interface.System;
using GD.Service.System;
using GD.WMS.WebApi.Filters;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Lazy.Captcha.Core;
using GD.Infrastructure.Extensions;

namespace GD.WMS.WebApi.Controllers.System
{
    /// <summary>
    /// 登录
    /// </summary>
    //[ApiExplorerSettings(GroupName = "sys")]
    public class SysLoginController : BaseController
    {
        //static readonly NLog.Logger logger = NLog.LogManager.GetLogger("LoginController");
        private readonly ISysUserService sysUserService;
        private readonly ISysMenuService sysMenuService;
        private readonly ISysLoginService sysLoginService;
        private readonly ICaptcha SecurityCodeHelper;
        private readonly ISysRoleService roleService;

        public SysLoginController(
            ISysMenuService sysMenuService,
            ISysUserService sysUserService,
            ISysLoginService sysLoginService,
            ISysRoleService sysRoleService,
            ICaptcha captcha,
            IOptions<OptionsSetting> optionSettings)
        {
            SecurityCodeHelper = captcha;
            this.sysMenuService = sysMenuService;
            this.sysUserService = sysUserService;
            this.sysLoginService = sysLoginService;
            roleService = sysRoleService;
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginBody">登录对象</param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        [Log(Title = "登录")]
        public IActionResult Login([FromBody] LoginBodyDto loginBody)
        {
            if (loginBody == null) { throw new CustomException("请求参数错误"); }
            //SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
            //if (sysConfig?.ConfigValue != "off" && !SecurityCodeHelper.Validate(loginBody.Uuid, loginBody.Code))
            //{
            //    return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
            //}

            if (!SecurityCodeHelper.Validate(loginBody.Uuid, loginBody.Code))
            {
                return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
            }

            sysLoginService.CheckLockUser(loginBody.Username);
            var user = sysLoginService.Login(loginBody, new SysLogininfor());

            List<SysRole> roles = roleService.SelectUserRoleListByUserId(user.UserId);

            TokenModel loginUser = new(user.Adapt<TokenModel>(), roles.Adapt<List<Roles>>());
            return SUCCESS(JwtUtil.GenerateJwtToken(JwtUtil.AddClaims(loginUser)));
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [Log(Title = "注销")]
        [HttpPost("logout")]
        public IActionResult LogOut()
        {
            //Task.Run(async () =>
            //{
            //    //注销登录的用户，相当于ASP.NET中的FormsAuthentication.SignOut  
            //    await HttpContext.SignOutAsync();
            //}).Wait();
            var userid = HttpContext.GetUId();
            var name = HttpContext.GetName();

            return SUCCESS(new { name, id = userid });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Verify]
        [HttpGet("getInfo")]
        public IActionResult GetUserInfo()
        {
            long userid = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userid);

            user.WelcomeContent = GlobalConstant.WelcomeMessages[new Random().Next(0, GlobalConstant.WelcomeMessages.Length)];

            return SUCCESS(new { user });
        }

        /// <summary>
        /// 获取路由信息
        /// </summary>
        /// <returns></returns>
        [Verify]
        [HttpGet("getRouters")]
        public IActionResult GetRouters()
        {
            long uid = HttpContext.GetUId();
            var menus = sysMenuService.SelectMenuTreeByUserId(uid);

            return SUCCESS(sysMenuService.BuildMenus(menus));
        }

        [HttpGet("getMockRouters")]
        public IActionResult GetMockRouters()
        {
            long uid = 1;
            var menus = sysMenuService.SelectMenuTreeByUserId(uid);

            return SUCCESS(sysMenuService.BuildMenus(menus));
        }


        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("captchaImage")]
        public IActionResult CaptchaImage()
        {
            string uuid = Guid.NewGuid().ToString().Replace("-", "");

            //SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
            //var captchaOff = sysConfig?.ConfigValue ?? "0";
            var captchaOff = "1";
            var info = SecurityCodeHelper.Generate(uuid, 60);
            var obj = new { captchaOff, uuid, img = info.Base64 };// File(stream, "image/png")

            return SUCCESS(obj);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("/register")]
        [AllowAnonymous]
        [Log(Title = "注册", BusinessType = BusinessType.INSERT)]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (!SecurityCodeHelper.Validate(dto.Uuid, dto.Code))
            {
                return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
            }
            dto.UserIP = HttpContext.GetClientUserIp();
            SysUser user = sysUserService.Register(dto);
            if (user.UserId > 0)
            {
                return SUCCESS(user);
            }
            return ToResponse(ResultCode.CUSTOM_ERROR, "注册失败，请联系管理员");
        }

    }
}
