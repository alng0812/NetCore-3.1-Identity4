using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NewarePassPort.Common;
using NetCoreWebAPI.Business;

namespace NetCoreWebAPI.Controllers
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        #region 请求客户端的一些信息
        /// <summary>
        /// AppId（ClientId）
        /// </summary>
        public string ClientId
        {
            get
            {
                return User.FindFirstValue("client_id");
            }

        }

        /// <summary>
        /// apps表的Id（创建来源app_id）
        /// </summary>
        public int AppId
        {
            get
            {
                return Account.Instance.GetAuthAppId(User.FindFirstValue("client_id"));
            }

        }
        #endregion
    }
}
