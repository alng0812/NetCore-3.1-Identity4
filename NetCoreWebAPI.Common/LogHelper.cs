using log4net;
using log4net.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace NewarePassPort.Common
{
    public class LoggerHelper
    {
        //log4net日志初始化 这里就读取多个配置节点
        private readonly ILog _logError = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogError");
        private readonly ILog _logNormal = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogNormal");
        private readonly ILog _logAOP = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogAOP");
        private readonly ILog _logDebug = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogDebug");
        public void LogErrException(string info, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(info, ex);
            }
        }

        public void LogControllerErr(string info, string parameters, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error($"控制器方法：{info}:参数为：{parameters}", ex);
            }
        }

        public void LogDALErr(string info, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error("数据访问层方法：" + info, ex);
            }
        }

        public void LogErr(string message)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(message);
            }
        }

        public void LogDebug(string message)
        {
            if (_logError.IsErrorEnabled)
            {
                _logDebug.Debug(message);
            }
        }

        public void LogControllerInfo(string info, string para)
        {
            if (_logNormal.IsInfoEnabled)
            {
                _logNormal.Info($"控制器方法：{info}:参数为：{para}");
            }
        }

        public void LogNormal(string info)
        {
            if (_logNormal.IsInfoEnabled)
            {
                _logNormal.Info(info);
            }
        }

        public void LogAOP(string key, string info)
        {
            if (_logAOP.IsInfoEnabled)
            {
                _logAOP.Info($"{key}:{info}");
            }
        }
        public void LogAOP(string info)
        {
            if (_logAOP.IsInfoEnabled)
            {
                _logAOP.Info(info);
            }
        }
    }
}
