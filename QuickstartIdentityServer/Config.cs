using IdentityServer4.Models;
using IdentityServer4.Test;
using NetCoreWebAPI.Business;
using System.Collections.Generic;

namespace QuickstartIdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
         new IdentityResource[]
         {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
         };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
            new ApiScope("api")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
            new ApiResource("api","#api")
            {
                //!!!重要
                Scopes = { "api" }
            }
            };

        //测试用户
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser{
                    SubjectId="",
                    Username="cool",
                    Password=""
                }
            };
        }

        /// <summary>
        /// 获取可访问客户端进行配置
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClientConfigList()
        {
            //从数据库获取配置文件
            var appInfo = GetApps();
            List<Client> Clients = new List<Client>();
            foreach (var item in appInfo)
            {
                Client client = new Client();
                client.ClientId = item.AppId;
                client.AllowedGrantTypes = GrantTypes.ClientCredentials;//授权类型，这里使用的是客户端凭证模式
                client.ClientSecrets.Add(new Secret(item.AppKey.Sha256()));
                client.AllowedScopes.Add("api");
                client.AccessTokenLifetime = 36000;//配置Token 失效时间
                Clients.Add(client);
            }
            return Clients;
        }
    }
}
