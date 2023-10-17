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
