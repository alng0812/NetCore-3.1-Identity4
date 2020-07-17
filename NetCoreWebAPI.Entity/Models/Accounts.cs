using System;
using System.Collections.Generic;

namespace NetCoreWebAPI.Entity.Models
{
    public partial class Accounts
    {
        public int PassportId { get; set; }
        public int? UserId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string Realname { get; set; }
        public string HeadImage { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public sbyte EmailVerify { get; set; }
        public sbyte Gender { get; set; }
        public sbyte Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
