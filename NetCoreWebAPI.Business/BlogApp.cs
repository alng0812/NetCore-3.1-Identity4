using NetCoreWebAPI.Entity.Models;
using NetCoreWebAPI.Entity.ResponseModels;
using System;
using System.Linq;

namespace NetCoreWebAPI.Business
{
    public class BlogApp
    {
        #region 单例

        private static volatile BlogApp mInstance = null;
        private static readonly object syncLock = new Object();
        private BlogApp() { }
        public static BlogApp Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (syncLock)
                    {
                        if (mInstance == null)
                            mInstance = new BlogApp();
                    }
                }
                return mInstance;
            }
        }
        #endregion

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <returns></returns>
        public TableData GetArticles()
        {
            var result = new TableData();
            using (blogContext entity = new blogContext())
            {
                var articles = (from a in entity.Articles
                                join b in entity.Articletypes on a.Typeid equals b.Id
                                where a.Status == 1
                                select new { a.Id, a.PictureUrl, a.Title, a.Createdate, a.Content, a.Typeid, b.Name, a.Description, a.ViewCount }).ToList().OrderByDescending(o => o.Createdate)
                                .ToList().Select(s => new { s.Id, s.PictureUrl, s.Title, Createdate = s.Createdate?.ToString("yyyy-MM-dd"), s.Content, s.Typeid, s.Name, s.Description, s.ViewCount }).ToList();
                result.Data = articles;
            }
            return result;
        }

        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TableData GetArticleInfo(int Id)
        {
            var result = new TableData();
            using (blogContext entity = new blogContext())
            {
                var article = (from a in entity.Articles
                               join b in entity.Articletypes on a.Typeid equals b.Id
                               where a.Id == Id
                               select new { a.Id, a.PictureUrl, a.Title, a.Createdate, a.Content, a.Typeid, b.Name, a.Description, a.ViewCount }).FirstOrDefault();
                var atticleInfo = new ArticleResponse
                {
                    Id = article.Id,
                    PictureUrl = article.PictureUrl,
                    Title = article.Title,
                    Createdate = article.Createdate?.ToString("yyy-MM-dd"),
                    Content = article.Content,
                    Typeid = (int)article.Typeid,
                    Name = article.Name,
                    Description = article.Description,
                    ViewCount = (int)article.ViewCount
                };
                result.Data = atticleInfo;
            }
            return result;
        }



    }
}
