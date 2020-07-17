using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace NewarePassPort.Common
{
    public class TokenUtil
    {
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public static string GetToken(string ClientId, string ClientSecret, string Scope, string IdentityServerUrl)
        {
            string token = string.Empty;
            //获取到获取token的url
            var client = new HttpClient();
            LogHelper.LogDebug("IdentityServerUrl:" + IdentityServerUrl);
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
                LogHelper.LogErr("disco:" + disco.Error);
            }
            LogHelper.LogDebug("disco:" + disco.TokenEndpoint);
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
            LogHelper.LogDebug("tokenResponse:" + tokenResponse.ToString());
            token = tokenResponse.AccessToken;
            return token;
        }
    }
}
