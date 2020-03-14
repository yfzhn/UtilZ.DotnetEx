using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志消息项
    /// </summary>
    public class LogMessageItem
    {
        /// <summary>
        /// 日志记录器名称
        /// </summary>
        public string LogerName { get; private set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string LogMsg { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logerName">日志记录器名称</param>
        /// <param name="logMsg">日志信息</param>
        public LogMessageItem(string logerName, string logMsg)
        {
            this.LogerName = logerName;
            this.LogMsg = logMsg;
        }
    }
}
