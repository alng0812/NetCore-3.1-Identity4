using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NewarePassPort.Common;

namespace NetCoreWebAPI.Controllers
{
    /// <summary>
    /// 日志Log4Net
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        private readonly LoggerHelper _logger;

        public LoggerController(LoggerHelper logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void LogTest()
        {
            _logger.LogDebug("测试Debug:" + DateTime.Now);
            _logger.LogErr("测试Error");
        }
    }
}
