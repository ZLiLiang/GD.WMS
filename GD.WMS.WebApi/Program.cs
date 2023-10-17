using GD.Infrastructure;
using GD.Infrastructure.App;
using GD.Infrastructure.Cache;
using GD.Infrastructure.Converter;
using GD.Model;
using GD.WMS.WebApi.Extensions;
using GD.WMS.WebApi.Filters;
using GD.WMS.WebApi.Middleware;
using Microsoft.AspNetCore.DataProtection;
using NLog.Web;
using System.Text.Json;

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

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(GlobalActionMonitor));//全局注册
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Converters.Add(new JsonConverterUtil.DateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonConverterUtil.DateTimeNullConverter());
    options.JsonSerializerOptions.Converters.Add(new StringConverter());
    //PropertyNamingPolicy属性用于前端传过来的属性的格式策略，目前内置的仅有一种策略CamelCase
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();
InternalApp.ServiceProvider = app.Services;
InternalApp.Configuration = builder.Configuration;
InternalApp.WebHostEnvironment = app.Environment;

//初始化db
builder.Services.AddDb(app.Environment);

//使用全局异常中间件
app.UseMiddleware<GlobalExceptionMiddleware>();

//请求头转发
//ForwardedHeaders中间件会自动把反向代理服务器转发过来的X-Forwarded-For（客户端真实IP）以及X-Forwarded-Proto（客户端请求的协议）自动填充到HttpContext.Connection.RemoteIPAddress和HttpContext.Request.Scheme中，这样应用代码中读取到的就是真实的IP和真实的协议了，不需要应用做特殊处理。
//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
//});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
