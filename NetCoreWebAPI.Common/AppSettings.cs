﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreWebAPI.Common
{
    public class AppSettings
    {
        /// <summary>
        ///  认证服务地址 
        /// </summary>
        public string IdentityServerUrl { get; set; }
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string MysqlServerUrl { get; set; }

        /// <summary>
        /// 文件保存地址
        /// </summary>

        public string UploadPath { get; set; }

        /// <summary>
        /// signalR Redis地址
        /// </summary>
        public string SignalRRedis { get; set; }
    }
}
