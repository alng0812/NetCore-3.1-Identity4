using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Entity.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace QuickstartIdentityServer
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)//���ܱ�����Api��Դ��ӵ��ڴ���
                .AddInMemoryClients(Config.GetClientConfigList())//�ͻ���������ӵ��ڴ���
                .AddSigningCredential(new X509Certificate2(Path.Combine(basePath,
                 Configuration["Certificates:CertPath"]),
                 Configuration["Certificates:Password"]));
            //services.AddIdentityServer().AddDeveloperSigningCredential();//���֤����ܷ�ʽ��ִ�и÷����������ж�tempkey.rsa֤���ļ��Ƿ���ڣ���������ڵĻ����ʹ���һ���µ�tempkey.rsa֤���ļ���������ڵĻ�����ʹ�ô�֤���ļ���
            services.AddMvc();
            services.AddDbContextPool<TestDataContext>(options =>
      options.UseMySql(ConfigHelper.GetAppSettings().MysqlServerUrl,
              mySqlOptions =>
              {
                  mySqlOptions.ServerVersion(new Version(5, 7, 23), ServerType.MySql)
                  .EnableRetryOnFailure(
                  maxRetryCount: 10,
                  maxRetryDelay: TimeSpan.FromSeconds(30),
                  errorNumbersToAdd: null);
              }
          ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            //����id4����id4�м����ӵ��ܵ���
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("IdentityServer is Already Start!");
                });
            });
        }
    }
}
