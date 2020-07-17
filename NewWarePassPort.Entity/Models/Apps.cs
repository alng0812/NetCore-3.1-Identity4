using System;
using System.Collections.Generic;

namespace NewWarePassPort.Entity.Models
{
    public partial class Apps
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
