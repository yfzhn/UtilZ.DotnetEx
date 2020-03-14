using System;
using System.Collections.Generic;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILoger : IDisposable
    {
        /// <summary>
        /// 日志记录器名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取日志级别
        /// </summary>
        LogLevel Level { get; }

        /// <summary>
        /// 是否启用日志追加器
        /// </summary>
        bool Enable { get; }

        /// <summary>
        /// 添加日志追加器
        /// </summary>
        /// <param name="appender">日志追加器</param>
        void AddAppender(AppenderBase appender);

        /// <summary>
        /// 根据日志追加器名称获取日志追加器
        /// </summary>
        /// <param name="appenderName">日志追加器名称</param>
        /// <returns>日志追加器</returns>
        AppenderBase GetAppenderByName(string appenderName);

        /// <summary>
        /// 获取日志追加器
        /// </summary>
        /// <returns>日志追加器数组</returns>
        AppenderBase[] GetAppenders();

        #region 记录日志方法
        #region Trace
        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Trace(string format, params object[] args);

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Trace(int eventId, object tag, string format, params object[] args);

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void Trace(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null);

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Trace(Exception ex, string format, params object[] args);

        /// <summary>
        /// 追踪
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Trace(int eventId, object tag, Exception ex, string format, params object[] args);
        #endregion

        #region Debug
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Debug(string format, params object[] args);

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Debug(int eventId, object tag, string format, params object[] args);

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void Debug(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null);

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Debug(Exception ex, string format, params object[] args);

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Debug(int eventId, object tag, Exception ex, string format, params object[] args);
        #endregion

        #region Info
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Info(string format, params object[] args);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Info(int eventId, object tag, string format, params object[] args);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void Info(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Info(Exception ex, string format, params object[] args);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常信息</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Info(int eventId, object tag, Exception ex, string format, params object[] args);
        #endregion

        #region Warn
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Warn(string format, params object[] args);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Warn(int eventId, object tag, string format, params object[] args);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="ex">异常警告</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void Warn(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="ex">异常警告</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Warn(Exception ex, string format, params object[] args);

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常警告</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Warn(int eventId, object tag, Exception ex, string format, params object[] args);
        #endregion

        #region Error
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Error(string format, params object[] args);

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Error(int eventId, object tag, string format, params object[] args);

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常错误</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void Error(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null);

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常错误</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Error(Exception ex, string format, params object[] args);

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常错误</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Error(int eventId, object tag, Exception ex, string format, params object[] args);
        #endregion

        #region Fatal
        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Fatal(string format, params object[] args);

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Fatal(int eventId, object tag, string format, params object[] args);

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex">异常致命</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        void Fatal(Exception ex, int eventId = LogConstant.DEFAULT_EVENT_ID, object tag = null);

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="ex">异常致命</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Fatal(Exception ex, string format, params object[] args);

        /// <summary>
        /// 致命
        /// </summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="tag">与对象关联的用户定义数据</param>
        /// <param name="ex">异常致命</param>
        /// <param name="format">复合格式字符串,参数为空或null表示无格式化</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        void Fatal(int eventId, object tag, Exception ex, string format, params object[] args);
        #endregion
        #endregion
    }
}
