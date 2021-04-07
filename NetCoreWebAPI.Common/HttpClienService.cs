using Microsoft.AspNetCore.Http;
using NetCoreWebAPI.Common;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Common
{
    public class HttpClienService
    {
        /// <summary>
        /// 注入http请求
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpClienService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // <summary>
        // Get请求数据
        // <para>最终以url参数的方式提交</para>
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> Get(Dictionary<string, object> parameters, string requestUri)
        {
            //从工厂获取请求对象   声明自己创建哪一个httpClient客户端
            var client = _httpClientFactory.CreateClient("TestClient");
            var token = _httpContextAccessor.HttpContext.Request.Headers["JwtToken"];
            ////添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("X-Token", token.ToString());
            }
            //拼接地址
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                requestUri = string.Concat(requestUri, '?', strParam);
            }
            return client.GetStringAsync(requestUri).Result;
        }

        /// <summary>
        /// 以json的方式Post数据 返回string类型
        /// <para>最终以json的方式放置在http体中</para>
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public async Task<string> Post(object entity, string requestUri)
        {
            //从工厂获取请求对象   声明自己创建哪一个httpClient客户端
            var client = _httpClientFactory.CreateClient("TestClient");
            string request = string.Empty;
            if (entity != null)
                request = JsonHelper.Instance.Serialize(entity);
            var token = _httpContextAccessor.HttpContext.Request.Headers["JwtToken"];
            ////添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("X-Token", token.ToString());
            }
            HttpContent httpContent = new StringContent(request);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = client.PostAsync(requestUri, httpContent);
            return result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}
