﻿using Microsoft.AspNetCore.Authorization;
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

    }
}
