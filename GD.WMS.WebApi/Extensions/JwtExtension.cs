﻿using GD.Infrastructure.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace GD.WMS.WebApi.Extensions
{
    public static class JwtExtension
    {
        public static void AddJwt(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie()
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = JwtUtil.ValidParameters();
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // 如果过期，把过期信息添加到头部
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            Console.WriteLine("jwt过期了");
                            context.Response.Headers.Add("Token-Expired", "true");
                        }

                        return Task.CompletedTask;
                    },
                };
            });
        }
    }
}
