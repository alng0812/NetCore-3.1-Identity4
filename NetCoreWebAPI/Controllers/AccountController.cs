using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NewarePassPort.Common;
using NewarePassPort.Common.Enums;
using NetCoreWebAPI.Business;
using NetCoreWebAPI.Entity.Models;
using NetCoreWebAPI.Models;
using System;
namespace NetCoreWebAPI.Controllers
{
    public class AccountController : BaseController
    {
        #region <<取配置文件的一些属性>>
        public readonly AppSettings _settings;

        public AccountController(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }
        #endregion

        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(ClientId);
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetConfig()
        {
            return Ok(_settings);
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAccessToken([FromBody] TokenInfo tokenModel)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                string AccessToken = TokenUtil.GetToken(tokenModel.ClientId, tokenModel.ClientSecret, tokenModel.Scope, _settings.IdentityServerUrl);
                if (string.IsNullOrEmpty(AccessToken))
                {
                    apiResult.ErrorMessage = "无法取得token";
                    apiResult.ResultCode = ResultCode.NotToken;
                    LogHelper.LogErr("无法取得token");
                    return new JsonResult(apiResult);
                }
                LogHelper.LogDebug("token：" + AccessToken + "客户端Id：" + tokenModel.ClientId + "客户端秘钥：" + tokenModel.ClientSecret + "访问资源：" + tokenModel.Scope);
                apiResult.Data = AccessToken;
            }
            catch (Exception ex)
            {
                LogHelper.LogDebug("GetAccessToken:" + ex.ToString());
                LogHelper.LogControllerErr("GetAccessToken", ex);
                apiResult.Error(ResultCode.API_Abnormal);
            }
            return new JsonResult(apiResult);
        }

        /// <summary>
        /// 修改用户资料(头像、昵称、姓名、性别)
        /// </summary>
        /// <param name="accountInfo">基本信息集合</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult EditAccountInfo([FromForm] Accounts accountInfo)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var editResult = Account.Instance.EditAccountInfo(accountInfo.PassportId, accountInfo.HeadImage, accountInfo.Nickname, accountInfo.Realname, accountInfo.Gender);
                apiResult.Data = editResult;
            }
            catch (Exception ex)
            {
                LogHelper.LogControllerErr("EditAccountInfo", ex);
                apiResult.Error(ResultCode.API_Abnormal);
            }
            return new JsonResult(apiResult);
        }

        /// <summary>
        /// 修改用户资料(手机)
        /// </summary>
        /// <param name="mobile">手机</param>
        /// <param name="passportid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult EditAccountMobile(string mobile, int passportid)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var editResult = Account.Instance.EditAccountMobile(passportid, mobile);
                apiResult.Data = editResult;
            }
            catch (Exception ex)
            {
                LogHelper.LogControllerErr("EditAccountMobile", ex);
                apiResult.Error(ResultCode.API_Abnormal);
            }
            return new JsonResult(apiResult);
        }

        /// <summary>
        /// 修改用户资料(邮箱)
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="passportid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult EditAccountEmail(string email, int passportid)
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var editResult = Account.Instance.EditAccountEmail(passportid, email);
                apiResult.Data = editResult;
            }
            catch (Exception ex)
            {
                LogHelper.LogControllerErr("EditAccountEmail", ex);
                apiResult.Error(ResultCode.API_Abnormal);
            }
            return new JsonResult(apiResult);
        }

    }
}
