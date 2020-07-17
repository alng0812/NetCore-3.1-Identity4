using System;
using System.Collections.Generic;
using System.Text;

namespace NewarePassPort.Common.Enums
{
    /// <summary>
    /// 接口返回状态
    /// </summary>
    public enum ResultCode : int
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 失败
        /// </summary>
        API_Abnormal = 201,
        /// <summary>
        /// 未登录
        /// </summary>
        NotLogin = 202,
        /// <summary>
        /// 获取Token失败
        /// </summary>
        NotToken = 203
    }
}
