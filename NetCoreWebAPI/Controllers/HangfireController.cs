using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using NetCoreWebAPI.HangFire;
using System;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        [HttpGet]
        public async Task<string> GetAsync()
        {
            var jobId = await Task.Run(() => BackgroundJob.Enqueue(() => PostAsync()));
            return jobId;
        }

        [HttpPost]
        [Queue("default")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task PostAsync()
        {
            var a = await Task.FromResult(0);
            Console.WriteLine("正在执行中...");
        }
    }
}
