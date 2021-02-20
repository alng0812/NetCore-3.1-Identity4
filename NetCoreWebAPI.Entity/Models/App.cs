using System;
using System.Collections.Generic;

#nullable disable

namespace NetCoreWebAPI.Entity.Models
{
    public partial class App
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
