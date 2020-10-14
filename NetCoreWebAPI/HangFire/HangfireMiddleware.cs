using Hangfire;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAPI.HangFire
{
    public static class HangfireMiddleware
    {
        public static void UseHangfireMiddleware(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            var options =
                new DashboardOptions
                {
                    AppPath = "#",//返回时跳转的地址
                    DisplayStorageConnectionString = false,//是否显示数据库连接信息
                    Authorization = new[] { new HangfireAuthorizationFilter() }
                };
            app.UseHangfireServer(ConfigureOptions());//配置服务 
            app.UseHangfireDashboard("/hangfire", options);//配置面板
            HangfireService();//配置各个任务
        }

        /// <summary>
        /// 配置启动
        /// </summary>
        /// <returns></returns>
        public static BackgroundJobServerOptions ConfigureOptions()
        {
            return new BackgroundJobServerOptions
            {
                Queues = new[] { "push", "default" },//队列名称，只能为小写
                WorkerCount = Environment.ProcessorCount * 5, //并发任务
                ServerName = "Hangfire", //代表服务名称

            };
        }

        #region 配置服务
        public static void HangfireService()
        {
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));//直接将任务加入到待执行任务队列
            //BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromDays(7));//在当前时间后的某个时间将任务加入到待执行任务队列
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring!"), Cron.Minutely);//周期性任务，每一个周期就将任务加入到待执行任务队列
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation!"));//继续执行任务

            //这里呢就是需要触发的方法 "0/10 * * * * ? " 可以自行搜索cron表达式 代表循环的规律很简单
            //GameLenthPush代表你要触发的类 UserGameLenthPush代表你要触发的方法

            //RecurringJob.AddOrUpdate<IHandFireTestService>(s => s.SendMsg("发送消息biubiubiu"), Cron.Minutely);//一分钟执行
            //RecurringJob.AddOrUpdate<IHandFireTestService>(s => s.ReceiveMsg("已接收到您的消息"), Cron.Minutely);//一分钟执行
            //RecurringJob.AddOrUpdate<ITestServices>(s => s.TestHangfireMyqueue(), "0/5 * * * * ? ", TimeZoneInfo.Local, HangfireConfigureQueue.Myqueue);

        }
        #endregion
    }
}
