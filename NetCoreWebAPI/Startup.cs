using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCoreWebAPI.Business.SingalR;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Common.AutoMapper;
using NetCoreWebAPI.Entity.Models;
using NetCoreWebAPI.HangFire;
using NetCoreWebAPI.Service;
using NewarePassPort.Common;

namespace NetCoreWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置autoMapper
            services.AddAutoMapper();

            #region 获取配置文件appsettings.json
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            #endregion

            #region Swagger
            if (Configuration.GetSection("UseSwagger").Value == "true")
            {
                //注册swagger服务,定义1个或者多个swagger文档
                services.AddSwaggerGen(s =>
            {
                //设置swagger文档相关信息
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApi文档",
                    Description = "接口文档",
                    Version = "v1.0"
                });
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                                                              }
                        },
                        new string[] { }
                    }
            });

                foreach (var name in Directory.GetFiles(AppContext.BaseDirectory, "*.*",
                   SearchOption.AllDirectories).Where(f => Path.GetExtension(f).ToLower() == ".xml"))
                {
                    s.IncludeXmlComments(name, true);
                }

                ////获取xml注释文件的目录
                //var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                //// 启用xml注释
                //s.IncludeXmlComments(xmlPath, true); //添加控制器层注释（true表示显示控制器注释）
                //var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "NetCoreWebAPI.Entity.xml");
                //s.IncludeXmlComments(xmlModelPath, true);
            });
            }
            #endregion

            #region MySql数据库
            //连接 mysql 数据库，添加数据库上下文
            //    services.AddDbContext<TestDataContext>(options =>
            //options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContext<blogContext>(options =>
                options.UseMySql(connectionString, serverVersion));
            #endregion

            #region 身份认证
            //配置身份认证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //身份认证平台地址（身份认证时会去这个地址验证token是否有效）
                    options.Authority = Configuration.GetSection("AppSettings").Get<AppSettings>().IdentityServerUrl;
                    //一般默认（true），开发环境时可以设置为false
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters.RequireExpirationTime = true; //是否需要超时时间参数
                    //apiResource
                    options.Audience = "api";
                    options.SaveToken = true;
                });
            #endregion

            #region  Hangfire定时任务
            services.AddHangfireService();
            #endregion

            #region Redis分布式缓存
            //redis缓存
            var section = Configuration.GetSection("Redis:Default");
            //连接字符串
            string _connectionString = section.GetSection("Connection").Value;
            //实例名称
            string _instanceName = section.GetSection("InstanceName").Value;
            //默认数据库 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _defaultDB));
            #endregion

            #region 跨域请求
            //配置跨域处理，允许所有来源          
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            #endregion

            #region SignalR
            services.AddSignalRExtension(Configuration);
            #endregion
            services.AddHttpClient();
            //services.AddHttpClient("TestClient", c =>
            //{
            //    var appServerUrl = Configuration.GetValue<string>("AppSetting:TestServerUrl");
            //    c.BaseAddress = new Uri(appServerUrl);
            //    c.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            //});
            //services.AddSingleton<HttpClienService>();

            services.AddMvc();
            services.AddControllers();
            services.AddTransient<LoggerHelper>();
            services.AddScoped<IScopeService, scopeTest>();
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            //允许所有跨域，CorsPolicy是在ConfigureServices方法中配置的跨域策略名称
            app.UseCors("CorsPolicy");
            //启用身份认证中间件
            app.UseAuthentication();
            //启用授权
            app.UseAuthorization();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("cors");
            app.UseStaticFiles();

            #region Hangfire定时任务
            app.UseHangfireMiddleware();
            #endregion

            #region Swagger
            app.UseSwagger(opt =>
        {
            //opt.RouteTemplate = "api/{controller=Home}/{action=Index}/{id?}";
        });
            //启用SwaggerUI中间件（htlm css js等），定义swagger json 入口
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreWebapi文档v1");
                s.RoutePrefix = "";
                //注入汉化文件(3.0版本之后不支持汉化)
                //s.InjectJavascript($"/Scripts/Swagger-zhCN.js");
            });
            #endregion

            #region 异常状态码
            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "application/json;charset=utf-8";

                int code = context.HttpContext.Response.StatusCode;
                string message =
                 code switch
                 {
                     401 => "未登录",
                     403 => "访问拒绝",
                     404 => "未找到",
                     _ => "未知错误",
                 };

                context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
                await context.HttpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    isSuccess = false,
                    code,
                    message
                }));

            });
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/[action]");
                endpoints.MapMessageHub();
            });
        }
    }
}
