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

//ע��HttpContextAccessor
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// ��������
builder.Services.AddCors(builder.Configuration);
//����Error unprotecting the session cookie����
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "DataProtection"));
//��ͨ��֤��
//builder.Services.AddCaptcha(builder.Configuration);
//IPRatelimit
//builder.Services.AddIPRate(builder.Configuration);
//builder.Services.AddSession();
//builder.Services.AddHttpContextAccessor();
//����������Model��
builder.Services.Configure<OptionsSetting>(builder.Configuration);

//jwt ��֤
builder.Services.AddJwt();
//�����ļ�
builder.Services.AddSingleton(new AppSettings(builder.Configuration));
//app����ע��
builder.Services.AddAppService();

//ע��REDIS ����
//var openRedis = builder.Configuration["RedisServer:open"];
//if (openRedis == "1")
//{
//    RedisServer.Initalize();
//}

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(GlobalActionMonitor));//ȫ��ע��
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.Converters.Add(new JsonConverterUtil.DateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonConverterUtil.DateTimeNullConverter());
    options.JsonSerializerOptions.Converters.Add(new StringConverter());
    //PropertyNamingPolicy��������ǰ�˴����������Եĸ�ʽ���ԣ�Ŀǰ���õĽ���һ�ֲ���CamelCase
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();
InternalApp.ServiceProvider = app.Services;
InternalApp.Configuration = builder.Configuration;
InternalApp.WebHostEnvironment = app.Environment;

//��ʼ��db
builder.Services.AddDb(app.Environment);

//ʹ��ȫ���쳣�м��
app.UseMiddleware<GlobalExceptionMiddleware>();

//����ͷת��
//ForwardedHeaders�м�����Զ��ѷ�����������ת��������X-Forwarded-For���ͻ�����ʵIP���Լ�X-Forwarded-Proto���ͻ��������Э�飩�Զ���䵽HttpContext.Connection.RemoteIPAddress��HttpContext.Request.Scheme�У�����Ӧ�ô����ж�ȡ���ľ�����ʵ��IP����ʵ��Э���ˣ�����ҪӦ�������⴦��
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
