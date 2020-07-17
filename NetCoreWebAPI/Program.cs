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
                //�÷�����Ҫ����Microsoft.Extensions.Logging���ƿռ�
                builder.AddFilter("System", LogLevel.Error); //���˵�ϵͳĬ�ϵ�һЩ��־
                builder.AddFilter("Microsoft", LogLevel.Error);//���˵�ϵͳĬ�ϵ�һЩ��־

                //���Log4Net
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Config/Log4net.config");
                //������������ʾlog4net.config�������ļ�����Ӧ�ó����Ŀ¼�£�Ҳ����ָ�������ļ���·��
                //��Ҫ���nuget����Microsoft.Extensions.Logging.Log4Net.AspNetCore
                builder.AddLog4Net(path);
            }).ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
    }
}
