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
                .AddInMemoryApiResources(Config.ApiResources)//把受保护的Api资源添加到内存中
                .AddInMemoryClients(Config.GetClientConfigList())//客户端配置添加到内存中
                .AddSigningCredential(new X509Certificate2(Path.Combine(basePath,
                 Configuration["Certificates:CertPath"]),
                 Configuration["Certificates:Password"]));
            //services.AddIdentityServer().AddDeveloperSigningCredential();//添加证书加密方式，执行该方法，会先判断tempkey.rsa证书文件是否存在，如果不存在的话，就创建一个新的tempkey.rsa证书文件，如果存在的话，就使用此证书文件。
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
            //启用id4，将id4中间件添加到管道中
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
