﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetCoreWebAPI.Business;
using NetCoreWebAPI.Business.Request;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Models;
using NewarePassPort.Common;
using NewarePassPort.Common.Enums;

namespace NetCoreWebAPI.Controllers
{
    public class AccountController : BaseController
    {
        #region <<取配置文件的一些属性>>
        public readonly AppSettings _settings;
        private readonly LoggerHelper _logger;
        public AccountController(IOptions<AppSettings> settings, LoggerHelper logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
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
                TokenUtil client = new TokenUtil(_logger);
                string AccessToken = client.GetToken(tokenModel.ClientId, tokenModel.ClientSecret, tokenModel.Scope, _settings.IdentityServerUrl);
                if (string.IsNullOrEmpty(AccessToken))
                {
                    apiResult.ErrorMessage = "无法取得token";
                    apiResult.ResultCode = ResultCode.NotToken;
                    _logger.LogErr("无法取得token");
                    return new JsonResult(apiResult);
                }
                _logger.LogDebug("token：" + AccessToken + "客户端Id：" + tokenModel.ClientId + "客户端秘钥：" + tokenModel.ClientSecret + "访问资源：" + tokenModel.Scope);
                apiResult.Data = AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("GetAccessToken:" + ex.ToString());
                _logger.LogControllerErr("GetAccessToken", "", ex);
                apiResult.Error(ResultCode.API_Abnormal);
            }
            return new JsonResult(apiResult);
        }

        [HttpGet]
        public IActionResult GetApps()
        {
            ApiResult apiResult = new ApiResult();
            apiResult.Data = Account.Instance.GetApps();
            return new JsonResult(apiResult);
        }

        /// <summary>
        /// 配置访问权限
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ConfigApps(ConfigAppRequest req)
        {
            ApiResult apiResult = new ApiResult();
            apiResult.Data = Account.Instance.ConfigApps(req);
            return new JsonResult(apiResult);
        }

    }
}
