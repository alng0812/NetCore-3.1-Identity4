using System;
using System.Collections.Generic;

namespace NetCoreWebAPI.Entity.Models
{
    public partial class AccountOauthMap
    {
        public uint Id { get; set; }
        public uint PassportId { get; set; }
        public sbyte OpenType { get; set; }
        public string Openid { get; set; }
        public string Unionid { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
