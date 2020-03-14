using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志记录器基类
    /// </summary>
    public abstract class LogerBase : ILoger
    {
        /// <summary>
        /// 日志追加器集合
        /// </summary>
        protected readonly List<AppenderBase> _appenders = new List<AppenderBase>();

        /// <summary>
        /// 日志追加器集合线程锁
        /// </summary>
        protected readonly object _appendersLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogerBase()
        {

        }

        #region ILoger接口
        /// <summary>
        /// 获取日志记录器名称
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// 获取获取日志级别
        /// </summary>
        public LogLevel Level { get; protected set; } = LogLevel.Trace;

        /// <summary>
        /// 是否启用日志追加器
        /// </summary>
        public bool Enable { get; protected set; } = true;

        /// <summary>
        /// 添加日志追加器
        /// </summary>
        /// <param name="appender">日志追加器</param>
        void ILoger.AddAppender(AppenderBase appender)
        {
            if (appender == null)
            {
                return;
            }

            lock (this._appendersLock)
            {
                this._appenders.Add(appender);
            }
        }

        /// <summary>
        /// 根据日志追加器名称获取日志追加器
        /// </summary>
        /// <param name="appenderName">日志追加器名称</param>
        /// <returns>日志追加器</returns>
        AppenderBase ILoger.GetAppenderByName(string appenderName)
        {
            lock (this._appendersLock)
            {
                return this._appenders.Where(t =>
                {
                    return appenderName == null && string.IsNullOrEmpty(t.Name) ||
                    t.Name == null && string.IsNullOrEmpty(appenderName) ||
                    string.Equals(t.Name, appenderName);
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取日志追加器
        /// </summary>
        /// <returns>日志追加器数组</returns>
        AppenderBase[] ILoger.GetAppenders()
        {
            lock (this._appendersLock)
            {
                return this._appenders.ToArray();
            }
        }

        /// <summary>
        /// 实例添加日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        protected void InsAddLog(LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.PrimitiveAddLog(4, level, eventId, tag, ex, format, args);
        }

        /// <summary>
        /// 静态方法添加日志的方法
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        internal abstract void ObjectAddLog(LogLevel level, int eventId, object tag, Exception ex, string format, params object[] args);

        /// <summary>
        /// 实例添加日志
        /// </summary>
        /// <param name="skipFrames"></param>
        /// <param name="level">日志级别</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常</param>
        /// <param name="msg">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        protected abstract void PrimitiveAddLog(int skipFrames, LogLevel level, int eventId, object tag, Exception ex, string msg, params object[] args);

        #region 记录日志方法
        #region Trace
        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Trace(string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Trace, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Trace(int eventId, object tag, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Trace, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void ILoger.Trace(Exception ex, int eventId, object tag)
        {
            this.InsAddLog(LogLevel.Trace, eventId, tag, ex, null);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Trace(Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Trace, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Trace(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Trace, eventId, tag, ex, format, args);
        }
        #endregion

        #region Debug
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Debug(string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Debug, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Debug(int eventId, object tag, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Debug, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void ILoger.Debug(Exception ex, int eventId, object tag)
        {
            this.InsAddLog(LogLevel.Debug, eventId, tag, ex, null);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Debug(Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Debug, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Debug(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Debug, eventId, tag, ex, format, args);
        }
        #endregion

        #region Info
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Info(string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Info, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Info(int eventId, object tag, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Info, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void ILoger.Info(Exception ex, int eventId, object tag)
        {
            this.InsAddLog(LogLevel.Info, eventId, tag, ex, null);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Info(Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Info, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Info(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Info, eventId, tag, ex, format, args);
        }
        #endregion

        #region Warn
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Warn(string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Warn, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Warn(int eventId, object tag, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Warn, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="ex">异常警告</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void ILoger.Warn(Exception ex, int eventId, object tag)
        {
            this.InsAddLog(LogLevel.Warn, eventId, tag, ex, null);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="ex">异常警告</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Warn(Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Warn, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常警告</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Warn(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Warn, eventId, tag, ex, format, args);
        }
        #endregion

        #region Error
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Error(string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Error, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Error(int eventId, object tag, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Error, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常错误</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void ILoger.Error(Exception ex, int eventId, object tag)
        {
            this.InsAddLog(LogLevel.Error, eventId, tag, ex, null);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常错误</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Error(Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Error, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常错误</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Error(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Error, eventId, tag, ex, format, args);
        }
        #endregion

        #region Fatal
        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Fatal(string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Fatal, LogConstant.DEFAULT_EVENT_ID, null, null, format, args);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Fatal(int eventId, object tag, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Fatal, eventId, tag, null, format, args);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex">异常致命</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void ILoger.Fatal(Exception ex, int eventId, object tag)
        {
            this.InsAddLog(LogLevel.Fatal, eventId, tag, ex, null);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex">异常致命</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Fatal(Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Fatal, LogConstant.DEFAULT_EVENT_ID, null, ex, format, args);
        }

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常致命</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void ILoger.Fatal(int eventId, object tag, Exception ex, string format, params object[] args)
        {
            this.InsAddLog(LogLevel.Fatal, eventId, tag, ex, format, args);
        }
        #endregion
        #endregion
        #endregion

        #region IDisposable接口
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDisposing">是否释放标识</param>
        protected virtual void Dispose(bool isDisposing)
        {

        }
        #endregion
    }
}
