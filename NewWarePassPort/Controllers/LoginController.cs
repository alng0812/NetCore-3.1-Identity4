using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewarePassPort.Common;
using NewarePassPort.Common.Enums;
using NewWarePassPort.Business;
using NewWarePassPort.Models;
using System;
using System.Collections.Generic;
namespace NewWarePassPort.Controllers
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

        ///// <summary>
        ///// 登录post回发处理
        ///// </summary>
        //[HttpPost]
        //public async Task<IActionResult> Login(string account, string password, string returnUrl = null)
        //{
        //    Accounts userInfo = Account.Instance.GetUserInfo(account, password);
        //    if (userInfo != null)
        //    {
        //        AuthenticationProperties props = new AuthenticationProperties
        //        {
        //            IsPersistent = true,
        //            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(1))
        //        };
        //        await HttpContext.SignInAsync(userInfo.PassportId.ToString(), userInfo.Account.ToString(), props);
        //        if (returnUrl != null)
        //        {
        //            return Redirect(returnUrl);
        //        }
        //    }
        //    return Redirect("Error");

        //}

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="logintype">验证类型</param>
        /// <param name="username">帐户</param>
        /// <param name="password">密码</param>
        /// <param name="openid">openId</param>
        /// <param name="access_token">access_token</param>
        /// <param name="appid">appid</param>
        /// <param name="code">code</param>
        /// <param name="encryptedData">encryptedData</param>
        /// <param name="iv">iv</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Login(string logintype, string username = "", string password = "", string openid = "", string access_token = "", string appid = "", string code = "", string encryptedData = "", string iv = "")
        {
            ApiResult apiResult = new ApiResult();
            var outDic = new Dictionary<string, object>() { ["data"] = null };
            try
            {
                switch (logintype)
                {
                    case "mobileOrEmail"://邮箱、手机登录
                        outDic = Account.Instance.ValidateByMobileOrEmail(username, password);
                        break;
                    case "wechat"://微信登录
                        outDic = Account.Instance.ValidateByWechat(openid, access_token, AppId);
                        break;
                    case "qq"://QQ登录
                        outDic = Account.Instance.ValidateByQQ(openid, appid, access_token, AppId);
                        break;
                    case "miniProg"://小程序登录
                        outDic = Account.Instance.ValidateByMiniProg(code, encryptedData, iv, AppId);
                        break;
                    default:
                        break;
                }
                if (outDic.ContainsKey("errmsg") && !string.IsNullOrWhiteSpace(outDic["errmsg"].ToString()))
                {
                    apiResult.ErrorMessage = outDic["errmsg"].ToString();
                    apiResult.ResultCode = ResultCode.NotLogin;
                    return new JsonResult(apiResult);
                }
                if (outDic["data"] == null)
                {
                    apiResult.ErrorMessage = "没有任何数据";
                    apiResult.ResultCode = ResultCode.NotLogin;
                    return new JsonResult(apiResult);
                }
                apiResult.Data = outDic["data"];
            }
            catch (Exception ex)
            {
                LogHelper.LogControllerErr("Login/login", ex);
                apiResult.Error(ResultCode.API_Abnormal);
            }
            return new JsonResult(apiResult);
        }
    }
}
