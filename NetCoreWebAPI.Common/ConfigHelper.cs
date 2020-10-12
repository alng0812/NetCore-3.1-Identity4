using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;


namespace NetCoreWebAPI.Common
{
    /// <summary>
    /// 配置帮助类（获取json文件配置）
    /// </summary>
    public class ConfigHelper
    {
        private static IConfiguration _configuration;

        private static bool? _isDebugMode;

        static ConfigHelper()
        {
            //在当前目录或者根目录中寻找appsettings.json文件
            var fileName = "appsettings.json";

            var directory = AppContext.BaseDirectory;
            directory = directory.Replace("\\", "/");

            var filePath = $"{directory}/{fileName}";
            if (!File.Exists(filePath))
            {
                var length = directory.IndexOf("/bin");
                filePath = $"{directory.Substring(0, length)}/{fileName}";
            }

            var builder = new ConfigurationBuilder()
                .AddJsonFile(filePath, false, true);

            _configuration = builder.Build();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public static IConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// 获取指定节点（二级以上）要定义实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetAppsettings<T>(string key) where T : class, new()
        {
            var appconfig = new ServiceCollection()
               .AddOptions()
               .Configure<T>(Configuration.GetSection(key))
               .BuildServiceProvider()
               .GetService<IOptions<T>>()
               .Value;
            return appconfig;
        }


        /// <summary>
        /// 获取指定节点（只有一级节点适用）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSectionValue(string key)
        {
            return _configuration.GetSection(key).Value;
        }

        /// <summary>
        /// 获取配置文件名
        /// </summary>
        /// <returns></returns>
        public static string GetAppsettingsFileName()
        {
            if (IsDebug)
            {
                return "appsettings.Development.json";
            }
            else
            {
                return "appsettings.json";
            }
        }
        public DebuggableAttribute.DebuggingModes DebuggingFlags { get; }

        /// <summary>
        ///当前程序是否在调试中 
        /// </summary>
        public static bool IsDebug
        {
            get
            {
                if (_isDebugMode == null)
                {
                    var assembly = Assembly.GetEntryAssembly();
                    if (assembly == null)
                    {
                        //由于调用GetFrames的StackTrace实例没有跳过任何帧，所以GetFrames()一定不为null
                        assembly = new StackTrace().GetFrames().Last().GetMethod().Module.Assembly;
                    }
                    var debuggableAttribute = assembly.GetCustomAttribute<DebuggableAttribute>();
                    _isDebugMode = debuggableAttribute.DebuggingFlags
                        .HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue);
                }
                return _isDebugMode.Value;
            }
        }

        public static AppSettings GetAppSettings()
        {
            AppSettings appSettings = new AppSettings();
            appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
            return appSettings;
        }
    }
}
