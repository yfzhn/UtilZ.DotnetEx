using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public sealed class Loger : LogerBase
    {
        #region 静态成员
        private static readonly LogerBase _emptyLoger = new EmptyLoger();
        private static ILoger _defaultLoger;

        /// <summary>
        /// [key:LogerName;Value:Loger]
        /// </summary>
        private static readonly Dictionary<string, ILoger> _logerDic = new Dictionary<string, ILoger>();
        private static readonly object _logerDicLock = new object();

        /// <summary>
        /// 静态构造函数(初始化默认日志追加器)
        /// </summary>
        static Loger()
        {
            Base.ApplicationEx.Add(new Base.ApplicationExitNotify(Release));
            StaticStructLoadLogConfig();
        }


        private static void StaticStructLoadLogConfig()
        {
            try
            {
                const string logConfigFileName = LogConstant.DEFAULT_CONFIG_FILE_NAME;
                if (File.Exists(logConfigFileName))
                {
                    LoadConfig(logConfigFileName);
                }
                else
                {
                    string[] xmlFilePathArr = Directory.GetFiles(LogConstant.CurrentAssemblyDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                    if (xmlFilePathArr.Length == 0)
                    {
                        CreateDefaultLoger();
                    }
                    else
                    {
                        foreach (var xmlFilePath in xmlFilePathArr)
                        {
                            try
                            {
                                var xdoc = XDocument.Load(xmlFilePath);
                                if (PrimitiveLoadConfig(xdoc))
                                {
                                    return;
                                }
                            }
                            catch (Exception exi)
                            {
                                LogSysInnerLog.OnRaiseLog($"加载日志配置[{xmlFilePath}]异常", exi);
                            }
                        }

                        LogSysInnerLog.OnRaiseLog("没有可用的日志配置文件,使用默认配置", null);
                        CreateDefaultLoger();
                    }
                }
            }
            catch (Exception ex)
            {
                CreateDefaultLoger();
                LogSysInnerLog.OnRaiseLog("加载日志配置异常,使用默认配置", ex);
            }
        }

        private static void CreateDefaultLoger()
        {
            var defaultLoger = new Loger(false);
            defaultLoger._appenders.Add(new FileAppender(new FileAppenderConfig(null)));
            _defaultLoger = defaultLoger;
        }

        /// <summary>
        /// 清空所有配置,包括默认
        /// </summary>
        public static void Clear()
        {
            lock (_logerDicLock)
            {
                foreach (ILoger loger in _logerDic.Values)
                {
                    loger.Dispose();
                }

                if (_defaultLoger != _emptyLoger && _defaultLoger != null)
                {
                    _defaultLoger.Dispose();
                }

                _defaultLoger = _emptyLoger;
                _logerDic.Clear();
            }
        }

        /// <summary>
        /// 加载配置,加载前清空旧的配置
        /// </summary>
        /// <param name="configFilePath">配置文件路径</param>
        public static void LoadConfig(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                return;
            }

            try
            {
                var xdoc = XDocument.Load(configFilePath);
                PrimitiveLoadConfig(xdoc);
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog("加载配置文件异常", ex);
            }
        }

        /// <summary>
        /// 加载配置,加载前清空旧的配置
        /// </summary>
        /// <param name="xdoc">配置文件XDocument</param>
        private static bool PrimitiveLoadConfig(XDocument xdoc)
        {
            if (string.Equals(xdoc.Root.Name.LocalName, LogConstant.LOGCONFIG_ROOT_ELEMENT_NAME, StringComparison.OrdinalIgnoreCase))
            {
                var logerEles = xdoc.Root.Elements(LogConstant.LOGCONFIG_LOGER_ELEMENT_NAME);
                if (logerEles.Count() == 0)
                {
                    return true;
                }

                foreach (var logerEle in logerEles)
                {
                    ParseLogerConfig(logerEle);
                }

                return true;
            }
            else
            {
                LogSysInnerLog.OnRaiseLog("无效的载配置文件", null);
                return false;
            }
        }

        private static void ParseLogerConfig(XElement logerEle)
        {
            try
            {
                string name = LogUtil.GetAttributeValue(logerEle, "name");
                if (string.IsNullOrEmpty(name))
                {
                    if (_defaultLoger != _emptyLoger && _defaultLoger != null)
                    {
                        _defaultLoger.Dispose();
                    }

                    _defaultLoger = _emptyLoger;
                }

                bool thread;
                if (!bool.TryParse(LogUtil.GetAttributeValue(logerEle, "thread"), out thread))
                {
                    thread = false;
                }

                var loger = new Loger(thread);
                loger.Name = name;

                LogLevel level;
                if (Enum.TryParse<LogLevel>(LogUtil.GetAttributeValue(logerEle, "level"), true, out level))
                {
                    loger.Level = level;
                }

                bool enable;
                if (bool.TryParse(LogUtil.GetAttributeValue(logerEle, "enable"), out enable))
                {
                    loger.Enable = enable;
                }

                IEnumerable<XElement> appenderEles = logerEle.XPathSelectElements("appender");
                foreach (var appenderEle in appenderEles)
                {
                    try
                    {
                        CreateAppender(appenderEle, loger);
                    }
                    catch (Exception exi)
                    {
                        LogSysInnerLog.OnRaiseLog(null, exi);
                    }
                }

                if (string.IsNullOrEmpty(name))
                {
                    _defaultLoger = loger;
                }
                else
                {
                    lock (_logerDicLock)
                    {
                        if (_logerDic.ContainsKey(name))
                        {
                            ((ILoger)_logerDic[name]).Dispose();
                        }

                        _logerDic[name] = loger;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog("解析配置文件异常", ex);
            }
        }

        private static void CreateAppender(XElement appenderEle, Loger loger)
        {
            string appenderName = LogUtil.GetAttributeValue(appenderEle, "name");
            try
            {
                string appenderTypeName = LogUtil.GetAttributeValue(appenderEle, "type");
                if (string.IsNullOrWhiteSpace(appenderTypeName))
                {
                    return;
                }

                appenderTypeName = appenderTypeName.Trim();
                AppenderBase appender;
                if (appenderTypeName.Length == 1)
                {
                    appender = CreateAppenderByAppenderPattern(appenderTypeName, appenderEle);
                }
                else
                {
                    if (!appenderTypeName.Contains('.') && !appenderTypeName.Contains(','))
                    {
                        Type appenderBaseType = typeof(AppenderBase);
                        appenderTypeName = string.Format("{0}.{1},{2}", appenderBaseType.Namespace, appenderTypeName, Path.GetFileName(appenderBaseType.Assembly.Location));
                    }

                    Type appenderType = LogUtil.GetType(appenderTypeName);
                    if (appenderType == null)
                    {
                        return;
                    }

                    appender = Activator.CreateInstance(appenderType, new object[] { (object)appenderEle }) as AppenderBase;
                }

                if (appender == null)
                {
                    return;
                }

                appender.Name = appenderName;
                loger._appenders.Add(appender);
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(string.Format("解析:{0}日志追加器异常", appenderName), ex);
            }
        }

        private static AppenderBase CreateAppenderByAppenderPattern(string appenderTypeName, XElement appenderEle)
        {
            AppenderBase appender;
            switch (appenderTypeName[0])
            {
                case LogConstant.FILE_APPENDER_PATTERN_BREIF:
                    appender = new FileAppender(appenderEle);
                    break;
                case LogConstant.REDIRECT_APPENDER_PATTERN_BREIF:
                    appender = new RedirectAppender(appenderEle);
                    break;
                case LogConstant.CONSOLE_APPENDER_PATTERN_BREIF:
                    appender = new ConsoleAppender(appenderEle);
                    break;
                case LogConstant.DATABASE_APPENDER_PATTERN_BREIF:
                    appender = new DatabaseAppender(appenderEle);
                    break;
                case LogConstant.MAIL_APPENDER_PATTERN_BREIF:
                    appender = new MailAppender(appenderEle);
                    break;
                case LogConstant.SYSTEM_APPENDER_PATTERN_BREIF:
                    appender = new SystemLogAppender(appenderEle);
                    break;
                default:
                    appender = null;
                    break;
            }

            return appender;
        }

        /// <summary>
        /// 根据日志追加器名称获取指定日志记录器中的日志追加器
        /// </summary>
        /// <param name="logerName">日志记录器名称</param>
        /// <param name="appenderName">日志追加器名称</param>
        /// <returns>日志追加器</returns>
        public static AppenderBase GetAppenderByName(string logerName, string appenderName)
        {
            ILoger loger = GetLoger(logerName);
            if (loger == null)
            {
                return null;
            }

            return loger.GetAppenderByName(appenderName);
        }

        /// <summary>
        /// 获取指定日志记录器中的日志追加器
        /// </summary>
        /// <param name="logerName">日志记录器名称</param>
        /// <returns>日志追加器数组</returns>
        public static AppenderBase[] GetAppenders(string logerName)
        {
            ILoger loger = GetLoger(logerName);
            if (loger == null)
            {
                return null;
            }

            return loger.GetAppenders();
        }

        /// <summary>
        /// 获取日志记录器,如果日志记录器名称为空或null则返回默认日志记录器,否则当名称对应的日志记录器存在时返回配置的日志记录器,不存在则抛出异常
        /// </summary>
        /// <param name="logerName">日志记录器名称</param>
        /// <returns>日志记录器</returns>
        public static ILoger GetLoger(string logerName)
        {
            ILoger loger;
            if (string.IsNullOrEmpty(logerName))
            {
                loger = _defaultLoger;
            }
            else
            {
                lock (_logerDicLock)
                {
                    if (!_logerDic.TryGetValue(logerName, out loger))
                    {
                        throw new ApplicationException($"不存在名称为\"{logerName}\"的日志记录器");
                    }
                }
            }

            if (loger == null)
            {
                loger = _emptyLoger;
            }

            return loger;
        }

        /// <summary>
        /// 添加日志记录器
        /// </summary>
        /// <param name="loger">日志记录器</param>
        public static void AddLoger(ILoger loger)
        {
            if (loger == null)
            {
                return;
            }

            string logerName = loger.Name;
            if (string.IsNullOrEmpty(logerName))
            {
                if (_defaultLoger != null)
                {
                    _defaultLoger.Dispose();
                }

                _defaultLoger = loger;
            }
            else
            {
                lock (_logerDicLock)
                {
                    if (_logerDic.ContainsKey(logerName))
                    {
                        ((ILoger)_logerDic[logerName]).Dispose();
                    }

                    _logerDic[logerName] = loger;
                }
            }
        }
        #endregion

        #region 日志记录器实例成员
        private readonly bool _thread;
        private readonly object _lock = null;

        /// <summary>
        /// 日志分发线程队列
        /// </summary>
        private readonly LogAsynQueue<LogItem> _logDispatcherQueue;

        private Loger(bool thread)
            : base()
        {
            this._thread = thread;
            if (thread)
            {
                this._logDispatcherQueue = new LogAsynQueue<LogItem>(this.RecordLogCallback, "日志分发线程");
            }
            else
            {
                this._lock = new object();
            }
        }

        private void RecordLogCallback(LogItem item)
        {
            item.LogProcess();
            lock (base._appendersLock)
            {
                foreach (var appender in base._appenders)
                {
                    try
                    {
                        appender.WriteLog(item);
                    }
                    catch (Exception exi)
                    {
                        LogSysInnerLog.OnRaiseLog(this, exi);
                    }
                }
            }
        }

        #region ILoger接口
        /// <summary>
        /// 静态方法添加日志的方法
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        internal override void ObjectAddLog(LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.PrimitiveAddLog(5, level, eventId, tag, ex, format, args);
        }

        /// <summary>
        /// 实例添加日志
        /// </summary>
        /// <param name="skipFrames"></param>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        protected override void PrimitiveAddLog(int skipFrames, LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {
            try
            {
                if (!this.Enable || level < this.Level)
                {
                    return;
                }

                var item = new LogItem(DateTime.Now, Thread.CurrentThread, skipFrames, true, this.Name, level, eventId, tag, ex, format, args);
                if (this._thread)
                {
                    this._logDispatcherQueue.Enqueue(item);
                }
                else
                {
                    lock (this._lock)
                    {
                        this.RecordLogCallback(item);
                    }
                }
            }
            catch (Exception exi)
            {
                LogSysInnerLog.OnRaiseLog(this, exi);
            }
        }
        #endregion

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDisposing">是否释放标识</param>
        protected override void Dispose(bool isDisposing)
        {
            if (this._thread)
            {
                this._logDispatcherQueue.Dispose();
            }

            lock (this._appendersLock)
            {
                foreach (var appender in this._appenders)
                {
                    try
                    {
                        appender.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogSysInnerLog.OnRaiseLog(this, ex);
                    }
                }
            }
        }
        #endregion

        #region 静态记录日志方法,默认日志快捷方法
        private static void SAddLog(LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {
            try
            {
                var loger = _defaultLoger as LogerBase;
                if (loger == null)
                {
                    return;
                }

                loger.ObjectAddLog(level, eventId, tag, ex, format, args);
            }
            catch (Exception exi)
            {
                LogSysInnerLog.OnRaiseLog(null, exi);
            }
        }

        #region Trace
        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Trace(string format, params object[] args)
        {
            SAddLog(LogLevel.Trace, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Trace(int eventId, object tag, string format, params object[] args)
        {
            SAddLog(LogLevel.Trace, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        public static void Trace(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null)
        {
            SAddLog(LogLevel.Trace, eventId, tag, ex, null);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Trace(Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Trace, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Trace(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Trace, eventId, tag, ex, format, args);
        }
        #endregion

        #region Debug
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Debug(string format, params object[] args)
        {
            SAddLog(LogLevel.Debug, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Debug(int eventId, object tag, string format, params object[] args)
        {
            SAddLog(LogLevel.Debug, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        public static void Debug(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null)
        {
            SAddLog(LogLevel.Debug, eventId, tag, ex, null);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Debug(Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Debug, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Debug(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Debug, eventId, tag, ex, format, args);
        }
        #endregion

        #region Info
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Info(string format, params object[] args)
        {
            SAddLog(LogLevel.Info, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Info(int eventId, object tag, string format, params object[] args)
        {
            SAddLog(LogLevel.Info, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        public static void Info(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null)
        {
            SAddLog(LogLevel.Info, eventId, tag, ex, null);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Info(Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Info, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Info(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Info, eventId, tag, ex, format, args);
        }
        #endregion

        #region Warn
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Warn(string format, params object[] args)
        {
            SAddLog(LogLevel.Warn, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Warn(int eventId, object tag, string format, params object[] args)
        {
            SAddLog(LogLevel.Warn, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="ex">异常警告</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        public static void Warn(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null)
        {
            SAddLog(LogLevel.Warn, eventId, tag, ex, null);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="ex">异常警告</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Warn(Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Warn, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常警告</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Warn(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Warn, eventId, tag, ex, format, args);
        }
        #endregion

        #region Error
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Error(string format, params object[] args)
        {
            SAddLog(LogLevel.Error, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Error(int eventId, object tag, string format, params object[] args)
        {
            SAddLog(LogLevel.Error, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常错误</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        public static void Error(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null)
        {
            SAddLog(LogLevel.Error, eventId, tag, ex, null);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常错误</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Error(Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Error, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常错误</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Error(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Error, eventId, tag, ex, format, args);
        }
        #endregion

        #region Fatal
        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Fatal(string format, params object[] args)
        {
            SAddLog(LogLevel.Fatal, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Fatal(int eventId, object tag, string format, params object[] args)
        {
            SAddLog(LogLevel.Fatal, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex">异常致命</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        public static void Fatal(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null)
        {
            SAddLog(LogLevel.Fatal, eventId, tag, ex, null);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex">异常致命</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Fatal(Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Fatal, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常致命</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void Fatal(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            SAddLog(LogLevel.Fatal, eventId, tag, ex, format, args);
        }
        #endregion


        /// <summary>
        /// 释放日志资源
        /// </summary>
        public static void Release()
        {
            try
            {
                foreach (var loger in _logerDic.Values)
                {
                    loger.Dispose();
                }

                if (_defaultLoger != null)
                {
                    _defaultLoger.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(null, ex);
            }
        }
        #endregion
    }
}
