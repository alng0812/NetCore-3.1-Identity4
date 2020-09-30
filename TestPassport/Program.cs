using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace TestPassport
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("0:获取Token 1：验证token 2：登录 3：注册 4：修改密码 5：修改基本资料 6：值比较");
                int Type = int.Parse(Console.ReadLine());
                string username = string.Empty;
                string password = string.Empty;
                string openid = string.Empty;
                string access_token = string.Empty;
                string appid = string.Empty;
                switch (Type)
                {
                    case 1:
                        TestIdentity4Async();
                        break;
                    case 2:
                        Console.WriteLine("模拟登录选择→ 0:账户密码 1：微信登录 2：QQ登录");
                        int loginType = int.Parse(Console.ReadLine());
                        switch (loginType)
                        {
                            case 1:
                                Console.WriteLine("请输入openId");
                                openid = Console.ReadLine();
                                Console.WriteLine("请输入access_token");
                                access_token = Console.ReadLine();
                                break;
                            case 2:
                                Console.WriteLine("请输入openId");
                                openid = Console.ReadLine();
                                Console.WriteLine("请输入access_token");
                                access_token = Console.ReadLine();
                                Console.WriteLine("请输入appid");
                                appid = Console.ReadLine();
                                break;
                            default:
                                Console.WriteLine("请输入账号");
                                username = Console.ReadLine();
                                Console.WriteLine("请输入密码");
                                password = Console.ReadLine();
                                break;
                        }
                        TestIdentity4Login(username, password);
                        break;
                    case 3:
                        TestIdentity4Register();
                        break;
                    case 4:
                        TestIdentity4EditPwd();
                        break;
                    case 5:
                        TestIdentity4EditAcountInfo();
                        break;
                    case 6:
                        TestValueEqual();
                        break;
                    default:
                        TestGetAccessToken();
                        break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("异常：" + e.Message);
            }
            Console.ReadKey();
        }

        public class TokenModel
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scope { get; set; }
        }

        /// <summary>
        /// 获取token
        /// </summary>
        private static async void TestGetAccessToken()
        {
            var client = new HttpClient();
            var responseJson = "";
            string url = "http://localhost:5001/api/Account/GetAccessToken";
            TokenModel tokenmodel = new TokenModel
            {
                ClientId = "Client",
                ClientSecret = "sercet",
                Scope = "api"
            };
            //string data = "{\"ClientId\" : \"Client\",\"ClientSecret\" : \"NewareAI\",\"Scope\" : \"api\"}";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(tokenmodel));
            client.Timeout = new TimeSpan(1, 0, 0, 0, 0);
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //await异步等待回应
            var response = await client.PostAsync(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseJson);
            }
        }

        /// <summary>
        /// 验证token
        /// </summary>
        private static async void TestIdentity4Async()
        {
            //获取到获取token的url
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine("获取到获取token的url失败：" + disco.Error);
                return;
            }
            Console.WriteLine("获取token的url为：" + disco.TokenEndpoint);
            Console.WriteLine();

            //获取token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,//就是我们postman请求token的地址
                ClientId = "Client",//客户端
                ClientSecret = "sercet",//秘钥
                Scope = "api"//请求的api
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine("获取token失败：" + tokenResponse.Error);
                return;
            }
            Console.WriteLine("获取token的response：");
            int index = 0;
            foreach (var proc in tokenResponse.GetType().GetProperties())
            {
                Console.WriteLine($"{++index}.{proc.Name}：{proc.GetValue(tokenResponse)}");
            }
            Console.WriteLine();

            //模拟客户端调用需要身份认证的api
            var apiClient = new HttpClient();
            //赋值token（携带token访问）
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var url = "http://localhost:5001/Identity";
            //var url = "http://localhost:5001/api/Account/Get";
            //发起请求
            var response = await apiClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //请求成功
                var content = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("请求成功，返回结果是：" + content);
                Console.WriteLine("请求成功，返回结果是：" + JArray.Parse(content));
            }
            else
            {
                //请求失败
                Console.WriteLine($"请求失败，状态码为：{(int)response.StatusCode}，描述：{response.StatusCode.ToString()}");
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        private static async void TestIdentity4Login(string username, string Password)
        {
            //获取到获取token的url
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine("获取到获取token的url失败：" + disco.Error);
                return;
            }
            Console.WriteLine("获取token的url为：" + disco.TokenEndpoint);
            Console.WriteLine();

            //获取token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,//
                ClientId = "Client",//客户端
                ClientSecret = "sercet",//秘钥
                Scope = "api"//请求的api
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine("获取token失败：" + tokenResponse.Error);
                return;
            }
            Console.WriteLine("获取token的response：");
            int index = 0;
            foreach (var proc in tokenResponse.GetType().GetProperties())
            {
                Console.WriteLine($"{++index}.{proc.Name}：{proc.GetValue(tokenResponse)}");
            }
            Console.WriteLine();

            //模拟客户端调用需要身份认证的api
            var apiClient = new HttpClient();
            //赋值token（携带token访问）
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var url = string.Format("http://localhost:5001/api/Login/Login?logintype=mobileOrEmail&username={0}&password={1}", username, Password);
            //发起请求
            var response = await apiClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //请求成功
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("请求成功，返回结果是：" + (JObject)JsonConvert.DeserializeObject(content));
            }
            else
            {
                //请求失败
                Console.WriteLine($"请求失败，状态码为：{(int)response.StatusCode}，描述：{response.StatusCode.ToString()}");
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        private static async void TestIdentity4Register()
        {

        }

        /// <summary>
        /// 修改密码
        /// </summary>
        private static async void TestIdentity4EditPwd()
        {

        }

        /// <summary>
        /// 修改基本资料
        /// </summary>
        private static async void TestIdentity4EditAcountInfo()
        {

        }

        private static async void TestValueEqual()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5, 7, 6, 8 };
            List<int> b = new List<int> { 9, 10, 11, 12, 13, 14, 15, 16 };
            List<int> c = new List<int> { 1, 2, 4, 3, 5, 6, 7, 8 };
            var aaa = a.OrderBy(a => a);
            var ccc = a.OrderBy(c => c);
            string aa = string.Join("", aaa);
            string bb = string.Join("", b);
            string cc = string.Join("", ccc);
            Console.WriteLine(aa.Equals(bb));
            Console.WriteLine(aa.Equals(cc));
            //Console.WriteLine(a.SequenceEqual(aaa));
            Console.WriteLine(aaa.SequenceEqual(ccc));
        }
    }
}
