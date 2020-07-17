using log4net;
using System;
using System.Reflection;

namespace NewarePassPort.Common
{
    public static class LogHelper
    {//log4net日志初始化 这里就读取多个配置节点

        private static readonly ILog _logError = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogError");
        private static readonly ILog _logNormal = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogNormal");
        private static readonly ILog _logAOP = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogAOP");
        private static readonly ILog _logDebug = LogManager.GetLogger(Assembly.GetCallingAssembly(), "LogDebug");
        public static void LogErrException(string info, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(info, ex);
            }
        }

        public static void LogControllerErr(string info, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error("控制器方法：" + info, ex);
            }
        }

        public static void LogDALErr(string info, Exception ex)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error("数据访问层方法：" + info, ex);
            }
        }

        public static void LogErr(string message)
        {
            if (_logError.IsErrorEnabled)
            {
                _logError.Error(message);
            }
        }

        public static void LogDebug(string message)
        {
            if (_logError.IsErrorEnabled)
            {
                _logDebug.Debug(message);
            }
        }

        public static void LogNormal(string info)
        {
            if (_logNormal.IsInfoEnabled)
            {
                _logNormal.Info(info);
            }
        }

        public static void LogAOP(string key, string info)
        {
            if (_logAOP.IsInfoEnabled)
            {
                _logAOP.Info($"{key}:{info}");
            }
        }
        public static void LogAOP(string info)
        {
            if (_logAOP.IsInfoEnabled)
            {
                _logAOP.Info(info);
            }
        }
    }
}
