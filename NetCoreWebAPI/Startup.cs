using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NewarePassPort.Common;
using NetCoreWebAPI.Entity.Models;
using ILoggerFactory = log4net.Repository.Hierarchy.ILoggerFactory;

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

                //获取xml注释文件的目录
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 启用xml注释
                s.IncludeXmlComments(xmlPath, true); //添加控制器层注释（true表示显示控制器注释）
                var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "");
                s.IncludeXmlComments(xmlModelPath, true);
            });
            }
            #endregion

            #region MySql数据库
            //连接 mysql 数据库，添加数据库上下文
            services.AddDbContext<testdataContext>(options =>
        options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
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

            services.AddMvc();

            services.AddControllers();
            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            //启用身份认证中间件
            app.UseAuthentication();
            //启用授权
            app.UseAuthorization();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/[action]");
            });
        }
    }
}
