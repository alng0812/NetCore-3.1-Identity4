// Creater aWave
//  Copyright(c) 2020

using Microsoft.AspNetCore.Mvc;
using NetCoreWebAPI.Business;
using NetCoreWebAPI.Common;
using NetCoreWebAPI.Models;

namespace NetCoreWebAPI.Controllers
{
    /// <summary>
    /// 博客
    /// </summary>
    public class BlogController : BaseController
    {
        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetArticles()
        {
            ApiResult apiResult = new ApiResult();
            var result = BlogApp.Instance.GetArticles();
            if (result.Code != 200)
            {
                apiResult.Error((ResultCode)result.Code);
                apiResult.ErrorMessage = result.Message;
                return new JsonResult(apiResult);
            }
            apiResult.Data = result.Data;
            return new JsonResult(apiResult);
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetArticleInfo(int id)
        {
            ApiResult apiResult = new ApiResult();
            var result = BlogApp.Instance.GetArticleInfo(id);
            if (result.Code != 200)
            {
                apiResult.Error((ResultCode)result.Code);
                apiResult.ErrorMessage = result.Message;
                return new JsonResult(apiResult);
            }
            apiResult.Data = result.Data;
            return new JsonResult(apiResult);
        }
    }
}
