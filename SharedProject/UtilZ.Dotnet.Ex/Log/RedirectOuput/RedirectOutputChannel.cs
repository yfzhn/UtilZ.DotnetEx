using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 输出日志订阅项
    /// </summary>
    [Serializable]
    public class RedirectOutputChannel
    {
        /// <summary>
        /// 日志输出回调
        /// </summary>
        private readonly Action<RedirectOuputItem> _logOutput;

        /// <summary>
        /// 过滤日志追加器名称,忽略大小写[空或null不作验证,其它值需要有匹配的日志追加器验证]
        /// </summary>
        private readonly string _appenderName = null;

        /// <summary>
        /// 日志记录器名称
        /// </summary>
        private readonly string _loggerName = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logOutput">日志输出回调</param>
        /// <param name="appenderName">过滤日志追加器名称,忽略大小写[空或null不作验证,其它值需要有匹配的日志追加器验证]</param>
        /// <param name="loggerName">日志记录器名称</param>
        public RedirectOutputChannel(Action<RedirectOuputItem> logOutput, string appenderName = null, string loggerName = null)
        {
            this._logOutput = logOutput;
            this._appenderName = appenderName;
            this._loggerName = loggerName;
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="logItem">要输出的日志项</param>
        internal void OnRaiseLogOutput(RedirectOuputItem logItem)
        {
            try
            {
                if (!string.IsNullOrEmpty(this._loggerName) &&
                    !string.Equals(this._loggerName, logItem.Item.LogerName))
                {
                    //日志记录器名称不为空或null,且与产生日志的日志记录器名称不匹配，直接返回
                    return;
                }

                if (!string.IsNullOrEmpty(this._appenderName) &&
                    !string.Equals(this._appenderName, logItem.AppenderName))
                {
                    //日志重定制追加器名称不为空或null,且与重定向日志追加器名称不匹配，直接返回
                    return;
                }

                var handler = this._logOutput;
                if (handler == null || logItem == null)
                {
                    //重定向委托为空，直接返回
                    return;
                }

                handler(logItem);
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }
    }
}
