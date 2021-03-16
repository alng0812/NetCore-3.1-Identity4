using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Business.SingalR
{
    public class MessageHub : Hub
    {
        private readonly ILogger Logger;
        public MessageHub(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<MessageHub>();
        }

        /// <summary>
        /// 服务端方法 发送消息--发送给所有连接的客户端
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            //ReceiveMessage 为客户端方法，让所有客户端调用这个方法
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        ///// <summary>
        ///// 服务端方法 发送消息--发送给指定的客户端
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="message"></param>
        ///// <returns></returns>
        //public async Task SendUserMessage(string userId, string message)
        //{
        //    //ReceiveMessage 为客户端方法，让所有客户端调用这个方法
        //    await Clients.User(userId).SendAsync("ShowMessage", message);
        //}
        ///// <summary>
        ///// 服务端方法 发送消息--发送给指定分组的客户端
        ///// </summary>
        ///// <param name="groupName"></param>
        ///// <param name="message"></param>
        ///// <returns></returns>
        //public async Task SendGroupUserMessage(string groupName, string message)
        //{
        //    //ReceiveMessage 为客户端方法，让所有客户端调用这个方法
        //    await Clients.Group(groupName).SendAsync("ShowMessage", message);
        //}

        /// <summary>
        /// 客户端连接的时候调用
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            Logger.LogWarning(Context.ConnectionId + "连接成功");
            await Clients.Caller.SendAsync("Connected");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 连接终止时调用。
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            Logger.LogWarning(Context.ConnectionId + "连接中止");
            return base.OnDisconnectedAsync(exception);
        }


        private static int count;

        private int GetLastCount()
        {
            return count++;
        }

        public async Task GetLastedCount(string random)
        {
            int count;
            do
            {
                count = GetLastCount();
                Thread.Sleep(1000);
                await Clients.All.SendAsync("ReceiveMessage", count);
            } while (count < 10);
            Thread.Sleep(2000);
            await Clients.All.SendAsync("Finished");
        }
    }
}
