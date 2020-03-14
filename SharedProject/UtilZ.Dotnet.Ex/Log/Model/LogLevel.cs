using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 追踪[1]
        /// </summary>
        Trace = 1,

        /// <summary>
        /// 调试[2]
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 提示[4]
        /// </summary>
        Info = 4,

        /// <summary>
        /// 警告[8]
        /// </summary>
        Warn = 8,

        /// <summary>
        /// 错误[16]
        /// </summary>
        Error = 16,

        /// <summary>
        /// 致命[32]
        /// </summary>
        Fatal = 32,

        /// <summary>
        /// 关闭[100]
        /// </summary>
        Off = 1024
    }
}
