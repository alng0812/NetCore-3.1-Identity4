using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Entity.ResponseModels
{
    public class ArticleResponse
    {
        public int Id { get; set; }

        public string PictureUrl { get; set; }

        public string Title { get; set; }

        public string Createdate { get; set; }

        public string Content { get; set; }

        public int Typeid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ViewCount { get; set; }

    }
}
