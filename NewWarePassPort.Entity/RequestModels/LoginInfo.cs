using System;
using System.Collections.Generic;
using System.Text;

namespace NewWarePassPort.Entity.Models
{
    /// <summary>
    /// 登录信息
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// 加密Key
        /// </summary>
        public const string Salt = "NewWare2020";
        /// <summary>
        /// 用户帐号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 用户openId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///开放平台 1-weixin,2-微信小程序,3-qq,4-facebook,5-twitter
        /// </summary>
        public int OpenType { get; set; }

        /// <summary>
        /// 用户唯一UnionId
        /// </summary>
        public int UnionId { get; set; }
    }
}
