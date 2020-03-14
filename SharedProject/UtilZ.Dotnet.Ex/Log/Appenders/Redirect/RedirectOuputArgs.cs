using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 重定向输出项
    /// </summary>
    public class RedirectOuputArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="item">日志信息项</param>
        public RedirectOuputArgs(LogItem item)
            : base()
        {
            this.Item = item;
        }

        /// <summary>
        /// 日志信息项
        /// </summary>
        public LogItem Item { get; private set; }
    }
}
