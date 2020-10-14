using Hangfire;
using Hangfire.MySql.Core;
using Hangfire.Redis;
using Microsoft.Extensions.DependencyInjection;
using NetCoreWebAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAPI.HangFire
{
    public static class HangfireService
    {
        private static string _redisConnect = ConfigHelper.Configuration["RedisConfig:ReadWriteHosts"] ?? "127.0.0.1:6379";
        public static void AddHangfireService(this IServiceCollection services)
        {
            // Add Hangfire services.
            if (services == null) throw new ArgumentNullException(nameof(services));
            //1.使用Mysql存储
            services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_170) //- 用于设置MS SQL Server的兼容级别
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseStorage(new MySqlStorage(ConfigHelper.GetAppSettings().MysqlServerUrl, new MySqlStorageOptions
       {
           TablePrefix = "hangfire",
           JobExpirationCheckInterval = TimeSpan.FromHours(1), //- 作业到期检查间隔（管理过期记录）。默认值为1小时。
           CountersAggregateInterval = TimeSpan.FromMinutes(5) //- 聚合计数器的间隔。默认为5分钟。
       })));
            //2.使用Redis存储

            //services.AddHangfire(config =>
            //{
            //    config
            //    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseRecommendedSerializerSettings()
            //    .UseRedisStorage(_redisConnect,
            //                new RedisStorageOptions
            //                {
            //                    Db = 7,
            //                    Prefix = "hangfire",//在Redis存储中Hangfire使用的Key前缀
            //                    InvisibilityTimeout = TimeSpan.FromHours(3),//任务转移间隔, 在这段间隔内，后台任务任为同一个worker处理；超时后将转移到另一个worker处理
            //                    ExpiryCheckInterval = TimeSpan.FromHours(1),//任务过期检查频率
            //                    DeletedListSize = 10000,
            //                    SucceededListSize = 10000
            //                });
            //});
            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }
    }
}
