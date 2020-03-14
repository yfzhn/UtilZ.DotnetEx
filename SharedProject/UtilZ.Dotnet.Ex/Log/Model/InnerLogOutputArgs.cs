using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 内部日志输出事件参数
    /// </summary>
    public class InnerLogOutputArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex">异常</param>
        public InnerLogOutputArgs(Exception ex)
        {
            this.Ex = ex;
        }

        /// <summary>
        /// 异常
        /// </summary>
        public Exception Ex { get; private set; }
    }
}
