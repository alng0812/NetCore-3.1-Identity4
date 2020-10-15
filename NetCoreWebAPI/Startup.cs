using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Entity.Models;
using NetCoreWebAPI.HangFire;
using NewarePassPort.Common;
using System;
using System.IO;

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
                var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "NetCoreWebAPI.Entity.xml");
                s.IncludeXmlComments(xmlModelPath, true);
            });
            }
            #endregion

            #region MySql���ݿ�
            //���� mysql ���ݿ⣬������ݿ�������
            services.AddDbContext<TestDataContext>(options =>
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

            #region  Hangfire��ʱ����
            services.AddHangfireService();
            #endregion

            #region Redis�ֲ�ʽ����
            //redis����
            var section = Configuration.GetSection("Redis:Default");
            //�����ַ���
            string _connectionString = section.GetSection("Connection").Value;
            //ʵ������
            string _instanceName = section.GetSection("InstanceName").Value;
            //Ĭ�����ݿ� 
            int _defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
            services.AddSingleton(new RedisHelper(_connectionString, _instanceName, _defaultDB));
            #endregion

            services.AddMvc();

            services.AddControllers();
            services.AddTransient<LoggerHelper>();
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

            #region Hangfire��ʱ����
            app.UseHangfireMiddleware();
            #endregion

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

            #region �쳣״̬��
            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "application/json;charset=utf-8";

                int code = context.HttpContext.Response.StatusCode;
                string message =
                 code switch
                 {
                     401 => "δ��¼",
                     403 => "���ʾܾ�",
                     404 => "δ�ҵ�",
                     _ => "δ֪����",
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
            });
        }
    }
}
