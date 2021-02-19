using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;

namespace NewarePassPort.Common
{
    public class TokenUtil
    {
        private readonly LoggerHelper _logger;

        public TokenUtil(LoggerHelper logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetToken(string ClientId, string ClientSecret, string Scope, string IdentityServerUrl)
        {
            string token = string.Empty;
            //获取到获取token的url
            var client = new HttpClient();
            _logger.LogDebug("IdentityServerUrl:" + IdentityServerUrl);
            //var disco = client.GetDiscoveryDocumentAsync(IdentityServerUrl).Result;
            var disco = client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = IdentityServerUrl,
                Policy =
                {
                    RequireHttps = false
                }
            }).Result;
            if (disco.IsError)
            {
                _logger.LogErr("disco:" + disco.Error);
            }
            _logger.LogDebug("disco:" + disco.TokenEndpoint);
            //当停掉IdentityServer服务时
            //Error connecting to http://localhost:5000/.well-known/openid-configuration: 由于目标计算机积极拒绝，无法连接。

            //获取token
            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = ClientId,//Client
                ClientSecret = ClientSecret,//secret
                Scope = Scope//api
            }).Result;
            _logger.LogDebug("tokenResponse:" + JsonConvert.SerializeObject(tokenResponse).ToString());
            token = tokenResponse.AccessToken;
            return token;
        }
    }
}
