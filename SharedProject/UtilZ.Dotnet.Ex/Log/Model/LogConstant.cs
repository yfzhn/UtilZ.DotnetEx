using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志常量
    /// </summary>
    public class LogConstant
    {
        static LogConstant()
        {
            _currentAssemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        }

        private static string _currentAssemblyDirectory;
        /// <summary>
        /// 获取当前应用程序所在根目录
        /// </summary>
        public static string CurrentAssemblyDirectory
        {
            get { return _currentAssemblyDirectory; }
        }

        /// <summary>
        /// 默认事件ID
        /// </summary>
        public const int DEFAULT_EVENT_ID = -1;

        #region 日志级别中文字符串常量
        /// <summary>
        /// 追踪
        /// </summary>
        public const string TRACESTR = "追踪";

        /// <summary>
        /// 调试
        /// </summary>
        public const string DEBUGSTR = "调试";

        /// <summary>
        /// 信息
        /// </summary>
        public const string INFOSTR = "信息";

        /// <summary>
        /// 警告
        /// </summary>
        public const string WARNSTR = "警告";

        /// <summary>
        /// 错误
        /// </summary>
        public const string ERRORSTR = "错误";

        /// <summary>
        /// 致命
        /// </summary>
        public const string FATALSTR = "致命";

        /// <summary>
        /// 日志文件扩展名
        /// </summary>
        public const string LOGEXTENSION = @".log";

        /// <summary>
        /// 日志日期格式
        /// </summary>
        public const string LOGDATAFORMAT = @"yyyy-MM-dd";

        /// <summary>
        /// 日期格式字符串
        /// </summary>
        public const string DateTimeFormat = @"yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 等待重试时间,毫秒
        /// </summary>
        public const int WAITREPEATTIME = 200;

        /// <summary>
        /// 获取日志等级名称
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <returns>日志标题</returns>
        public static string GetLogLevelName(LogLevel level)
        {
            string title = null;
            switch (level)
            {
                case LogLevel.Trace:
                    title = TRACESTR;
                    break;
                case LogLevel.Debug:
                    title = DEBUGSTR;
                    break;
                case LogLevel.Info:
                    title = INFOSTR;
                    break;
                case LogLevel.Warn:
                    title = WARNSTR;
                    break;
                case LogLevel.Error:
                    title = ERRORSTR;
                    break;
                case LogLevel.Fatal:
                    title = FATALSTR;
                    break;
                default:
                    throw new Exception(string.Format("未知的日志级别:{0}", level));
            }

            return title;
        }
        #endregion

        #region 配置
        /// <summary>
        /// 默认配置文件名称
        /// </summary>
        public const string DEFAULT_CONFIG_FILE_NAME = "logConfig.xml";

        /// <summary>
        /// 日志配置根节点名称
        /// </summary>
        public const string LOGCONFIG_ROOT_ELEMENT_NAME = "logConfig";

        /// <summary>
        /// 日志记录器配置节点名称
        /// </summary>
        public const string LOGCONFIG_LOGER_ELEMENT_NAME = "loger";

        /// <summary>
        /// 通配符
        /// </summary>
        public const char PATTERN_FALG_CHAR = '*';
        #endregion

        #region 日志追加器简称通配符
        /// <summary>
        /// File
        /// </summary>
        public const char FILE_APPENDER_PATTERN_BREIF = 'F';

        /// <summary>
        /// Redirect
        /// </summary>
        public const char REDIRECT_APPENDER_PATTERN_BREIF = 'R';

        /// <summary>
        /// Console
        /// </summary>
        public const char CONSOLE_APPENDER_PATTERN_BREIF = 'C';

        /// <summary>
        /// Database
        /// </summary>
        public const char DATABASE_APPENDER_PATTERN_BREIF = 'D';

        /// <summary>
        /// Mail
        /// </summary>
        public const char MAIL_APPENDER_PATTERN_BREIF = 'M';

        /// <summary>
        /// System
        /// </summary>
        public const char SYSTEM_APPENDER_PATTERN_BREIF = 'S';
        #endregion

        #region 日志布局字段
        /// <summary>
        /// 时间
        /// </summary>
        public const string TIME = "%d";

        /// <summary>
        /// 日志级别
        /// </summary>
        public const string LEVEL = "%l";

        /// <summary>
        /// 事件ID
        /// </summary>
        public const string EVENT = "%e";

        /// <summary>
        /// 与对象关联的用户定义数据
        /// </summary>
        public const string TAG = "%g";

        /// <summary>
        /// 线程ID
        /// </summary>
        public const string THREAD = "%t";

        /// <summary>
        /// 内容
        /// </summary>
        public const string CONTENT = "%c";

        /// <summary>
        /// 堆栈
        /// </summary>
        public const string STACKTRACE = "%s";
        #endregion
    }
}
