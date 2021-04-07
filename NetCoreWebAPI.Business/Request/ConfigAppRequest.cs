using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Business.Request
{
    public class ConfigAppRequest
    {
        public string AppId { get; set; }

        public string AppKey { get; set; }

        public string Remark { get; set; }
    }
}
