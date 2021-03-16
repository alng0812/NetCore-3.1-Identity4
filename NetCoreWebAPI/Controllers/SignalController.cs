// Creater aWave
//  Copyright(c) 2020

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetCoreWebAPI.Business.SingalR;

namespace NetCoreWebAPI.Controllers
{
    public class SignalController : BaseController
    {
        private readonly IHubContext<MessageHub> _messageHub;
        public SignalController(IHubContext<MessageHub> hubContext)
        {
            _messageHub = hubContext;
        }


        [HttpPost]
        public async Task<IActionResult> SignalServerStart()
        {
            await _messageHub.Clients.All.SendAsync("signalServer", new { message = "Server is Start" });

            Thread.Sleep(2000);
            return Accepted(1); //202: 请求已被接受并处理，但还没有处理完成
        }

        [HttpPost]
        public async Task<IActionResult> SignalRPost([FromBody] string message)
        {
            await _messageHub.Clients.All.SendAsync("signalTest", new { message = message });

            Thread.Sleep(2000);
            return Accepted(1); //202: 请求已被接受并处理，但还没有处理完成
        }
    }
}
