using Microsoft.EntityFrameworkCore;
using NewarePassPort.Common;
using Newtonsoft.Json;
using NetCoreWebAPI.Entity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace NetCoreWebAPI.Business
{
    public class Account
    {
        #region 单例

        private static volatile Account mInstance = null;
        private static readonly object syncLock = new Object();
        private Account() { }
        public static Account Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (syncLock)
                    {
                        if (mInstance == null)
                            mInstance = new Account();
                    }
                }
                return mInstance;
            }
        }

        #endregion
        /// <summary>
        /// 获取授权客户端列表
        /// </summary>
        /// <returns></returns>
        public List<App> GetApps()
        {
            using (blogContext entity = new blogContext())
            {
                return entity.Apps.ToList();
            }
        }
    }
}
