using Microsoft.AspNetCore.Mvc;
using NetCoreWebAPI.Common;
using StackExchange.Redis;
using System;

namespace NetCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _redisHelper;

        public RedisController(RedisHelper client)
        {
            _redisHelper = client.GetDatabase();
        }

        /// <summary>
        /// redis测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RedisTest()
        {
            var a = _redisHelper.StringSet("redis", "redis" + DateTime.Now, TimeSpan.FromSeconds(1000000));
            var b = _redisHelper.StringGet("redis");
            //var c = _redisHelper.KeyDelete("redis");
            return Ok("success");
        }
    }
}
