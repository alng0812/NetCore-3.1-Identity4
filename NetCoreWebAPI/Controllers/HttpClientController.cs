// Creater aWave
//  Copyright(c) 2020

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebAPI.Business;
using NetCoreWebAPI.Common;
using Newtonsoft.Json;

namespace NetCoreWebAPI.Controllers
{
    public class HttpClientController : Controller
    {
        private readonly HttpClienService _httpClienService;
        public HttpClientController(HttpClienService httpClienService)
        {
            _httpClienService = httpClienService;
        }

        /// <summary>
        /// Httpclient调用
        /// </summary>
        [HttpGet]
        public async Task<TableData> Get(int parameter1)
        {
            var result = new TableData();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("parameter1", parameter1);
            var r = await _httpClienService.Get(parameters, "api/test");
            result = JsonConvert.DeserializeObject<TableData>(r);
            //result = await _app.GetBindInfo(AppUserId);
            return result;
        }
    }
}
