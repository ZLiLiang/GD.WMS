using GD.Infrastructure;
using GD.Infrastructure.Cache;
using GD.Model;
using GD.WMS.WebApi.Extensions;
using Microsoft.AspNetCore.DataProtection;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//注入HttpContextAccessor
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// 跨域配置
builder.Services.AddCors(builder.Configuration);
//消除Error unprotecting the session cookie警告
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "DataProtection"));
//普通验证码
//builder.Services.AddCaptcha(builder.Configuration);
//IPRatelimit
//builder.Services.AddIPRate(builder.Configuration);
//builder.Services.AddSession();
//builder.Services.AddHttpContextAccessor();
//绑定整个对象到Model上
builder.Services.Configure<OptionsSetting>(builder.Configuration);

//jwt 认证
builder.Services.AddJwt();
//配置文件
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
//app服务注册
builder.Services.AddAppService();

//注册REDIS 服务
//var openRedis = builder.Configuration["RedisServer:open"];
//if (openRedis == "1")
//{
//    RedisServer.Initalize();
//}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
