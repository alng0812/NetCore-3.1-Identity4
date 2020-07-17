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
        //新威小程序
        private const string appid = "wx5e5160d848afd3f5";
        private const string secret = "5a4ce417253967b1a1009c362b3de27a";

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


        #region<<登录注册>>
        public int GetAuthAppId(string appid)
        {
            using (neware_passportContext entity = new neware_passportContext())
            {
                var app = entity.Apps.Where(a => a.AppId == appid).FirstOrDefault();
                return app.Id;
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Dictionary<string, object> Register(string mobile, string password, string email)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            using (neware_passportContext entity = new neware_passportContext())
            {
                try
                {

                }
                catch (Exception ex)
                {
                    outDic["errmsg"] = "用户注册失败," + ex.ToString();
                    return outDic;
                }
                return outDic;
            }
        }

        /// <summary>
        /// 验证用户的账户密码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Accounts GetUserInfo(string code, string password)
        {
            using (neware_passportContext entity = new neware_passportContext())
            {
                return entity.Accounts.Where(a => a.Account == code && a.Password == password).SingleOrDefault();
            }
        }

        /// <summary>
        /// 验证用户身份（手机/邮箱 密码登录）
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Dictionary<string, object> ValidateByMobileOrEmail(string username, string password)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            var outData = new List<object>();
            //var passWordAndSaltBytes = Encoding.UTF8.GetBytes(password + LoginInfo.Salt);
            //var hashBytes = new SHA256Managed().ComputeHash(passWordAndSaltBytes);
            //string hashString = Convert.ToBase64String(hashBytes);
            using (neware_passportContext entity = new neware_passportContext())
            {
                try
                {
                    var userInfo = entity.Accounts.Where(a => a.Account == username && a.Password == password).SingleOrDefault();
                    if (userInfo == null)
                    {
                        outDic["errmsg"] = "验证用户身份失败,账户或密码不正确";
                        return outDic;
                    }
                    var status = userInfo.Status;
                    if (status != 1)
                    {
                        outDic["errmsg"] = "验证用户身份失败,该账户已被停用";
                        return outDic;
                    }
                    outData.Add(new
                    {
                        userInfo.PassportId,
                        userInfo.UserId,
                        userInfo.Nickname,
                        userInfo.Realname,
                        userInfo.HeadImage,
                        userInfo.Mobile,
                        userInfo.Email,
                        userInfo.Gender,
                        userInfo.EmailVerify
                    });
                    outDic["data"] = outData;
                }
                catch (Exception ex)
                {
                    outDic["errmsg"] = "验证用户身份失败";
                    LogHelper.LogDALErr("Account/ValidateByMobileOrEmail", ex);
                    return outDic;
                }
            }
            return outDic;
        }


        /// <summary>
        /// 第三方登录（微信）
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="access_token"></param>
        /// <param name="createby">创建来源app_id</param>
        /// <returns></returns>
        public Dictionary<string, object> ValidateByWechat(string openid, string access_token, int createby)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            using (neware_passportContext entity = new neware_passportContext())
            {
                try
                {
                    uint passPortId = 0;
                    var dateTime = DateTime.Now;
                    //获取微信登陆用户数据
                    var getUserinfo = getAppUserinfo(access_token, openid);
                    if (!string.IsNullOrWhiteSpace(getUserinfo["errmsg"].ToString()))
                    {
                        outDic["errmsg"] = getUserinfo["errmsg"];
                        return outDic;
                    }
                    var unionid = getUserinfo["unionid"].ToString();
                    var userInfo = entity.AccountOauthMap.Where(a => a.OpenType == 1 && a.Unionid == unionid && a.Openid == openid).FirstOrDefault();
                    if (userInfo != null)
                    {
                        passPortId = userInfo.PassportId;
                        var accountInfo = entity.Accounts.Where(a => a.PassportId == passPortId).ToList();
                        if (accountInfo == null || accountInfo.Count == 0)
                        {
                            var accountLog = new Accounts
                            {
                                Nickname = getUserinfo["nickname"].ToString(),
                                Realname = getUserinfo["nickname"].ToString(),//真实姓名暂时用昵称替代
                                HeadImage = getUserinfo["headimgurl"].ToString(),
                                Gender = (sbyte)int.Parse(getUserinfo["sex"].ToString()),
                                CreatedBy = createby,
                                CreatedOn = dateTime
                            };
                            entity.Accounts.Add(accountLog);
                            entity.SaveChanges();
                        }
                    }
                    else
                    {
                        using (var transaction = entity.Database.BeginTransaction())
                        {
                            try
                            {
                                var accountLog = new Accounts
                                {
                                    Nickname = getUserinfo["nickname"].ToString(),
                                    Realname = getUserinfo["nickname"].ToString(),//真实姓名暂时用昵称替代
                                    HeadImage = getUserinfo["headimgurl"].ToString(),
                                    Gender = (sbyte)int.Parse(getUserinfo["sex"].ToString()),
                                    CreatedBy = createby,
                                    CreatedOn = dateTime
                                };
                                entity.Accounts.Add(accountLog);
                                entity.SaveChanges();
                                //写入用户信息后，对应account_outh_map表写入对应open_id数据
                                passPortId = (uint)accountLog.PassportId;
                                AccountOauthMap map = new AccountOauthMap
                                {
                                    PassportId = passPortId,
                                    OpenType = 1,
                                    Openid = getUserinfo["openid"].ToString(),
                                    Unionid = getUserinfo["unionid"].ToString(),
                                    CreatedOn = dateTime
                                };
                                entity.AccountOauthMap.Add(map);
                                entity.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                outDic["errmsg"] = "注册失败";
                                LogHelper.LogDALErr("Account/ValidateByWechat(transaction)", ex);
                            }
                        }
                    }
                    outDic["data"] = entity.Accounts.Where(x => x.PassportId == passPortId).Select(s => new { s.Nickname, s.HeadImage, s.Gender, s.PassportId, s.Realname, s.Email, s.EmailVerify, s.Mobile, s.Account }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    outDic["errmsg"] = "验证用户身份失败," + ex.ToString();
                    LogHelper.LogDALErr("Account/ValidateByWechat", ex);
                    return outDic;
                }
            }
            return outDic;
        }

        /// <summary>
        ///  //获取用户个人信息（UnionID机制）
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public Dictionary<string, object> getAppUserinfo(string access_token, string openid)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            try
            {
                if (string.IsNullOrWhiteSpace(openid))
                {
                    outDic["errmsg"] = "缺少必填参数openid";
                    return outDic;
                }
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    outDic["errmsg"] = "缺少必填参数access_token";
                    return outDic;
                }
                //获取用户个人信息（UnionID机制）
                string userinfo = NewarePassPort.Common.Http.Get($"https://api.weixin.qq.com/sns/userinfo?access_token={access_token}&openid={openid}");
                //反序列化返回值
                var userinfoDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(userinfo);
                if (userinfoDic.ContainsKey("errcode") && userinfoDic["errcode"].ToString() == "40003")
                {
                    outDic["errmsg"] = userinfoDic["errmsg"].ToString();
                    return outDic;
                }
                outDic = outDic.Union(userinfoDic).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            catch (Exception ex)
            {
                outDic["errmsg"] = ex.ToString();
            }
            return outDic;
        }

        /// <summary>
        /// 第三方登录（QQ）
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="appId"></param>
        /// <param name="access_token"></param>
        /// <param name="createby">创建来源app_id</param>
        /// <returns></returns>
        public Dictionary<string, object> ValidateByQQ(string openid, string appId, string access_token, int createby)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            using (neware_passportContext entity = new neware_passportContext())
            {
                try
                {
                    uint passPortId = 0;
                    var dateTime = DateTime.Now;
                    //获取QQ登陆用户数据
                    var getUserInfo = getQQUserInfo(openid, appId, access_token);
                    if (!string.IsNullOrWhiteSpace(getUserInfo["errmsg"].ToString()))
                    {
                        outDic["errmsg"] = getUserInfo["errmsg"];
                        return outDic;
                    }
                    var userInfo = entity.AccountOauthMap.Where(a => a.OpenType == 3 && a.Openid == openid).FirstOrDefault();
                    if (userInfo != null)
                    {
                        passPortId = userInfo.PassportId;
                        var accountInfo = entity.Accounts.Where(a => a.PassportId == passPortId).ToList();
                        if (accountInfo == null || accountInfo.Count == 0)
                        {
                            var accountLog = new Accounts
                            {
                                Nickname = getUserInfo["nickname"].ToString().Length > 32 ? getUserInfo["nickname"].ToString().Substring(0, 32) : getUserInfo["nickname"].ToString(),
                                Realname = getUserInfo["nickname"].ToString().Length > 32 ? getUserInfo["nickname"].ToString().Substring(0, 32) : getUserInfo["nickname"].ToString(),//真实姓名暂时用昵称替代
                                HeadImage = getUserInfo["figureurl_qq_2"] == null ? getUserInfo["figureurl_qq_1"].ToString() : getUserInfo["figureurl_qq_2"].ToString(),
                                Gender = (sbyte)(getUserInfo["gender"].ToString() == "男" ? 1 : 2),
                                CreatedBy = createby,
                                CreatedOn = dateTime
                            };
                            entity.Accounts.Add(accountLog);
                            entity.SaveChanges();
                        }
                    }
                    else
                    {
                        using (var transaction = entity.Database.BeginTransaction())
                        {
                            try
                            {
                                var accountLog = new Accounts
                                {
                                    Nickname = getUserInfo["nickname"].ToString().Length > 32 ? getUserInfo["nickname"].ToString().Substring(0, 32) : getUserInfo["nickname"].ToString(),
                                    Realname = getUserInfo["nickname"].ToString().Length > 32 ? getUserInfo["nickname"].ToString().Substring(0, 32) : getUserInfo["nickname"].ToString(),//真实姓名暂时用昵称替代
                                    HeadImage = getUserInfo["figureurl_qq_2"] == null ? getUserInfo["figureurl_qq_1"].ToString() : getUserInfo["figureurl_qq_2"].ToString(),
                                    Gender = (sbyte)(getUserInfo["gender"].ToString() == "男" ? 1 : 2),
                                    CreatedBy = createby,
                                    CreatedOn = dateTime
                                };
                                entity.Accounts.Add(accountLog);
                                entity.SaveChanges();
                                //写入用户信息后，对应account_outh_map表写入对应open_id数据
                                passPortId = (uint)accountLog.PassportId;
                                AccountOauthMap map = new AccountOauthMap
                                {
                                    PassportId = passPortId,
                                    OpenType = 3,
                                    Openid = openid,
                                    CreatedOn = dateTime
                                };
                                entity.AccountOauthMap.Add(map);
                                entity.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                outDic["errmsg"] = "注册失败";
                                LogHelper.LogDALErr("Account/ValidateByQQ(transaction)", ex);
                            }
                        }
                    }
                    outDic["data"] = entity.Accounts.Where(x => x.PassportId == passPortId).Select(s => new { s.Nickname, s.HeadImage, s.Gender, s.PassportId, s.Realname, s.Email, s.EmailVerify, s.Mobile, s.Account }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    outDic["errmsg"] = "验证用户身份失败," + ex.ToString();
                    LogHelper.LogDALErr("Account/ValidateByQQ", ex);
                    return outDic;
                }
            }
            return outDic;
        }

        /// <summary>
        /// 获取QQ用户个人信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="appId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public Dictionary<string, object> getQQUserInfo(string openid, string appId, string access_token)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            try
            {
                if (string.IsNullOrWhiteSpace(openid))
                {
                    outDic["errmsg"] = "缺少必填参数openid";
                    return outDic;
                }
                if (string.IsNullOrWhiteSpace(access_token))
                {
                    outDic["errmsg"] = "缺少必填参数access_token";
                    return outDic;
                }
                //获取QQ用户个人信息
                string userinfo = NewarePassPort.Common.Http.Get($"https://graph.qq.com/user/get_user_info?access_token={access_token}&oauth_consumer_key={appId}&openid={openid}&format=json");
                //反序列化返回值
                var userinfoDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(userinfo);
                if (userinfoDic.ContainsKey("ret") && int.Parse(userinfoDic["ret"].ToString()) < 0)
                {
                    outDic["errmsg"] = userinfoDic["msg"].ToString();
                    return outDic;
                }
                outDic = outDic.Union(userinfoDic).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            catch (Exception ex)
            {
                outDic["errmsg"] = ex.ToString();
            }
            return outDic;
        }

        /// <summary>
        /// 第三方登录（小程序）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <param name="createby">创建来源app_id</param>
        /// <returns></returns>
        public Dictionary<string, object> ValidateByMiniProg(string code, string encryptedData, string iv, int createby)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            using (neware_passportContext entity = new neware_passportContext())
            {
                try
                {
                    uint passPortId = 0;
                    var dateTime = DateTime.Now;
                    var openidDic = getSmallProceduresByOpenid(code);
                    if (!string.IsNullOrWhiteSpace(openidDic["errmsg"]))
                    {
                        outDic["errmsg"] = openidDic["errmsg"];
                        return outDic;
                    }
                    var Dic = DecodeEncryptedData(encryptedData, iv, openidDic["session_key"]);
                    if (!Dic.ContainsKey("unionId"))
                    {
                        outDic["errmsg"] = "授权失败,请点击授权重新授权";
                        return outDic;
                    }
                    var unionid = Dic["unionId"].ToString();
                    var userInfo = entity.AccountOauthMap.Where(a => a.OpenType == 2 && a.Unionid == unionid).FirstOrDefault();
                    if (userInfo != null)
                    {
                        passPortId = userInfo.PassportId;
                        var accountInfo = entity.Accounts.Where(a => a.PassportId == passPortId).ToList();
                        if (accountInfo == null || accountInfo.Count == 0)
                        {
                            var accountLog = new Accounts
                            {
                                Nickname = Dic["nickName"].ToString(),
                                Realname = Dic["nickName"].ToString(),//真实姓名暂时用昵称替代
                                HeadImage = Dic["avatarUrl"].ToString(),
                                Gender = (sbyte)int.Parse(Dic["gender"].ToString()),
                                CreatedBy = createby,
                                CreatedOn = dateTime
                            };
                            entity.Accounts.Add(accountLog);
                            entity.SaveChanges();
                        }
                    }
                    else
                    {
                        using (var transaction = entity.Database.BeginTransaction())
                        {
                            try
                            {
                                var accountLog = new Accounts
                                {
                                    Nickname = Dic["nickName"].ToString(),
                                    Realname = Dic["nickName"].ToString(),//真实姓名暂时用昵称替代
                                    HeadImage = Dic["avatarUrl"].ToString(),
                                    Gender = (sbyte)int.Parse(Dic["gender"].ToString()),
                                    CreatedBy = createby,
                                    CreatedOn = dateTime
                                };
                                entity.Accounts.Add(accountLog);
                                entity.SaveChanges();
                                //写入用户信息后，对应account_outh_map表写入对应open_id数据
                                passPortId = (uint)accountLog.PassportId;
                                AccountOauthMap map = new AccountOauthMap
                                {
                                    PassportId = passPortId,
                                    OpenType = 2,
                                    Openid = Dic["openId"].ToString(),
                                    Unionid = Dic["unionId"].ToString(),
                                    CreatedOn = dateTime
                                };
                                entity.AccountOauthMap.Add(map);
                                entity.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                outDic["errmsg"] = "注册失败";
                                LogHelper.LogDALErr("Account/ValidateByMiniProg(transaction)", ex);
                            }
                        }
                    }
                    outDic["data"] = entity.Accounts.Where(x => x.PassportId == passPortId).Select(s => new { s.Nickname, s.HeadImage, s.Gender, s.PassportId, s.Realname, s.Email, s.EmailVerify, s.Mobile, s.Account }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    outDic["errmsg"] = "验证用户身份失败," + ex.ToString();
                    LogHelper.LogDALErr("Account/ValidateByMiniProg", ex);
                    return outDic;
                }
            }
            return outDic;
        }

        /// <summary>
        /// 获取code获取小程序openid session_key
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Dictionary<string, string> getSmallProceduresByOpenid(string code)
        {
            var outDic = new Dictionary<string, string>() { ["errmsg"] = "" };
            try
            {
                string WeixinByOpenid = NewarePassPort.Common.Http.Get($"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={secret}&js_code={code}&grant_type=authorization_code");

                //反序列化微信返回值
                var WeixinOpenidByDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(WeixinByOpenid);
                if (WeixinOpenidByDic.ContainsKey("openid"))
                {
                    // apiResult.Data = WeixinOpenidByDic["openid"].ToString();
                    //apiResult.Data = new { openid = WeixinOpenidByDic["openid"].ToString(), session_key = WeixinOpenidByDic["session_key"].ToString() };
                    outDic["openid"] = WeixinOpenidByDic["openid"];
                    outDic["session_key"] = WeixinOpenidByDic["session_key"];
                }
                else
                {
                    outDic["errmsg"] = "微信接口异常";
                    //apiResult.Error("换取openid失败：" + WeixinOpenidByDic["errmsg"].ToString());
                }
            }
            catch (Exception ex)
            {
                outDic["errmsg"] = "微信接口异常";
            }
            return outDic;
        }

        /// <summary>
        /// 微信解码
        /// </summary>
        /// <param name="encryptedDatad"></param>
        /// <param name="iv"></param>
        /// <param name="session_key"></param>
        /// <returns></returns>
        public Dictionary<string, object> DecodeEncryptedData(string encryptedDatad, string iv, string session_key)
        {
            var DicByEncryptedData = new Dictionary<string, object>();
            try
            {


                RijndaelManaged rijalg = new RijndaelManaged();
                //-----------------    
                //设置 cipher 格式 AES-128-CBC                    
                rijalg.KeySize = 128;

                rijalg.Padding = PaddingMode.PKCS7;
                rijalg.Mode = CipherMode.CBC;

                rijalg.Key = System.Convert.FromBase64String(session_key);
                rijalg.IV = System.Convert.FromBase64String(iv);


                byte[] encryptedData = System.Convert.FromBase64String(encryptedDatad);
                //解密    
                ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

                string result;

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }
                DicByEncryptedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
            }
            catch (Exception ex)
            {
                var Path = $"{AppDomain.CurrentDomain.BaseDirectory}/Log/WeChatError/{DateTime.Now.ToString("yyyyMMdd")}.txt";
                IO.TextWrite(Path, DateTime.Now + "：Method：Mobile/DecodeEncryptedData-------Message：" + ex.ToString(), true);
            }
            return DicByEncryptedData;
        }
        #endregion

        #region<<修改基本资料>>
        /// <summary>
        /// 修改基本资料
        /// </summary>
        /// <param name="passportid"></param>
        /// <param name="headimage"></param>
        /// <param name="nickname"></param>
        /// <param name="realname"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public Dictionary<string, object> EditAccountInfo(int passportid, string headimage, string nickname, string realname, int gender)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            try
            {
                using (neware_passportContext entity = new neware_passportContext())
                {
                    var accountInfo = entity.Accounts.Where(a => a.PassportId == passportid).FirstOrDefault();
                    if (accountInfo == null)
                    {
                        outDic["errmsg"] = "未找到当前用户信息";
                        return outDic;
                    }
                    accountInfo.HeadImage = headimage;
                    accountInfo.Nickname = nickname;
                    accountInfo.Realname = realname;
                    accountInfo.Gender = (sbyte)gender;
                    entity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                outDic["errmsg"] = "修改资料异常";
                LogHelper.LogDALErr("Account/EditAccountInfo", ex);
                return outDic;
            }
            return outDic;
        }

        /// <summary>
        /// 修改手机号
        /// </summary>
        /// <param name="passportid"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public Dictionary<string, object> EditAccountMobile(int passportid, string mobile)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            try
            {
                using (neware_passportContext entity = new neware_passportContext())
                {
                    var accountInfo = entity.Accounts.Where(a => a.PassportId == passportid).FirstOrDefault();
                    if (accountInfo == null)
                    {
                        outDic["errmsg"] = "未找到当前用户信息";
                        return outDic;
                    }
                    accountInfo.Mobile = mobile;
                    entity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                outDic["errmsg"] = "手机绑定异常";
                LogHelper.LogDALErr("Account/EditAccountMobile", ex);
                return outDic;
            }
            return outDic;
        }

        /// <summary>
        /// 修改邮箱
        /// </summary>
        /// <param name="passportid"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public Dictionary<string, object> EditAccountEmail(int passportid, string email)
        {
            var outDic = new Dictionary<string, object>() { ["errmsg"] = string.Empty };
            try
            {
                using (neware_passportContext entity = new neware_passportContext())
                {
                    var accountInfo = entity.Accounts.Where(a => a.PassportId == passportid).FirstOrDefault();
                    if (accountInfo == null)
                    {
                        outDic["errmsg"] = "未找到当前用户信息";
                        return outDic;
                    }
                    accountInfo.Email = email;
                    accountInfo.EmailVerify = 1;
                    entity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                outDic["errmsg"] = "手机绑定异常";
                LogHelper.LogDALErr("Account/EditAccountEmail", ex);
                return outDic;
            }
            return outDic;
        }
        #endregion

        /// <summary>
        /// 获取授权客户端列表
        /// </summary>
        /// <returns></returns>
        public List<Apps> GetApps()
        {
            using (neware_passportContext entity = new neware_passportContext())
            {
                return entity.Apps.ToList();
            }
        }
    }
}
