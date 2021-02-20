using System;
using System.Collections.Generic;

#nullable disable

namespace NetCoreWebAPI.Entity.Models
{
    public partial class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? Typeid { get; set; }
        public string Content { get; set; }
        public int? ViewCount { get; set; }
        public string PictureUrl { get; set; }
        public int? Status { get; set; }
        public DateTime? Createdate { get; set; }
    }
}
