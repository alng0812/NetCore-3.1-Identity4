using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewWarePassPort.Models
{
    /// <summary>
    /// 获取Token参数
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 客户端Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端加密方式
        /// </summary>
        public string ClientSecret { get; set; }

        //授权范围
        public string Scope { get; set; }

        /// <summary>
        /// 加密Key
        /// </summary>
        public const string SecretKey = "NewWare2020";
    }
}
