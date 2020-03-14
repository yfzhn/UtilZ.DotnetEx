using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 环境扩展类
    /// </summary>
    public class EnvironmentEx
    {
        static EnvironmentEx()
        {
            var mfileName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).ToLower();
            if (mfileName.Contains("w3wp.exe") || mfileName.Contains("iisexpress.exe"))
            {
                _appType = AppType.WebApp;
            }
        }

        private static AppType _appType = AppType.Exe;
        /// <summary>
        /// 获取当前环境是否是WebApp环境[true:WebApp;false:exe]
        /// </summary>
        public static AppType AppType
        {
            get
            {
                return _appType;
            }
        }

        ///// <summary>
        ///// 使用指定的应用程序配置文件作为 System.Configuration.Configuration 对象打开以允许读或写操作。
        ///// </summary>
        ///// <returns>System.Configuration.Configuration</returns>
        //public static System.Configuration.Configuration OpenWebConfiguration()
        //{
        //    System.Configuration.Configuration config;
        //    if (_appType == AppType.WebApp)
        //    {
        //        config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        //    }
        //    else
        //    {
        //        config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
        //    }

        //    return config;
        //}
    }

    /// <summary>
    /// 应用类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// exe可执行程序
        /// </summary>
        Exe,

        /// <summary>
        /// 服务
        /// </summary>
        Service,

        /// <summary>
        /// Web应用
        /// </summary>
        WebApp
    }
}
