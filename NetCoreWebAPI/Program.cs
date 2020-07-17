using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NetCoreWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, builder) =>
            {
                //该方法需要引入Microsoft.Extensions.Logging名称空间
                builder.AddFilter("System", LogLevel.Error); //过滤掉系统默认的一些日志
                builder.AddFilter("Microsoft", LogLevel.Error);//过滤掉系统默认的一些日志

                //添加Log4Net
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Config/Log4net.config");
                //不带参数：表示log4net.config的配置文件就在应用程序根目录下，也可以指定配置文件的路径
                //需要添加nuget包：Microsoft.Extensions.Logging.Log4Net.AspNetCore
                builder.AddLog4Net(path);
            }).ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
    }
}
