using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreWebAPI.Controllers
{
    /// <summary>
    /// 身份验证
    /// </summary>
    [Route("Identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// 获取令牌信息
        /// </summary>
        ///<remarks>
        ///<para> 参数说明:</para> 
        ///<para> iss：发行人</para> 
        ///<para> exp：到期时间</para> 
        ///<para> sub：主题</para> 
        ///<para> aud：用户</para> 
        ///<para> nbf：在此之前不可用</para> 
        ///<para> iat：发布时间</para> 
        ///<para> jti：JWT ID用于标识该JWT</para> 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
