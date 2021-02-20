using System;
using System.Collections.Generic;

#nullable disable

namespace NetCoreWebAPI.Entity.Models
{
    public partial class Articletype
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Createdate { get; set; }
    }
}
