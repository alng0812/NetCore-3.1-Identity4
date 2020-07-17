using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewarePassPort.Common;
using NewarePassPort.Common.Enums;
using NetCoreWebAPI.Business;
using NetCoreWebAPI.Models;
using System;
using System.Collections.Generic;
namespace NetCoreWebAPI.Controllers
{
    /// <summary>
    /// 登录控制器
    /// </summary>
    public class LoginController : BaseController
    {
        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("hello");
        }
    }
}
