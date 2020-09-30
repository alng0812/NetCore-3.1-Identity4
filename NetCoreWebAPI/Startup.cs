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
            #region ��ȡ�����ļ�appsettings.json
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            #endregion

            #region Swagger
            if (Configuration.GetSection("UseSwagger").Value == "true")
            {
                //ע��swagger����,����1�����߶��swagger�ĵ�
                services.AddSwaggerGen(s =>
            {
                //����swagger�ĵ������Ϣ
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApi�ĵ�",
                    Description = "�ӿ��ĵ�",
                    Version = "v1.0"
                });
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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

                //��ȡxmlע���ļ���Ŀ¼
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ����xmlע��
                s.IncludeXmlComments(xmlPath, true); //��ӿ�������ע�ͣ�true��ʾ��ʾ������ע�ͣ�
                var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "");
                s.IncludeXmlComments(xmlModelPath, true);
            });
            }
            #endregion

            #region MySql���ݿ�
            //���� mysql ���ݿ⣬������ݿ�������
            services.AddDbContext<testdataContext>(options =>
        options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            #endregion

            #region �����֤
            //���������֤
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //�����֤ƽ̨��ַ�������֤ʱ��ȥ�����ַ��֤token�Ƿ���Ч��
                    options.Authority = Configuration.GetSection("AppSettings").Get<AppSettings>().IdentityServerUrl;
                    //һ��Ĭ�ϣ�true������������ʱ��������Ϊfalse
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters.RequireExpirationTime = true; //�Ƿ���Ҫ��ʱʱ�����
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
            //���������֤�м��
            app.UseAuthentication();
            //������Ȩ
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
            //����SwaggerUI�м����htlm css js�ȣ�������swagger json ���
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreWebapi�ĵ�v1");
                s.RoutePrefix = "";
                //ע�뺺���ļ�(3.0�汾֮��֧�ֺ���)
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
