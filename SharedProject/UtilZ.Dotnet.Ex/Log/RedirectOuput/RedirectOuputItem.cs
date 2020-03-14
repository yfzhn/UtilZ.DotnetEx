using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 重定向输出项
    /// </summary>
    public class RedirectOuputItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appenderName">日志追加器名称</param>
        /// <param name="item">日志信息项</param>
        public RedirectOuputItem(string appenderName, LogItem item)
            : base()
        {
            this.AppenderName = appenderName;
            this.Item = item;
        }

        /// <summary>
        /// 日志追加器名称
        /// </summary>
        public string AppenderName { get; private set; }

        /// <summary>
        /// 日志信息项
        /// </summary>
        public LogItem Item { get; private set; }
    }
}
