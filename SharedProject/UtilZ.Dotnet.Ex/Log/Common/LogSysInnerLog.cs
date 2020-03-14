using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志系统内部日志输出类
    /// </summary>
    public class LogSysInnerLog
    {
        /// <summary>
        /// 内部日志事件
        /// </summary>
        public static event EventHandler<InnerLogOutputArgs> Log;

        /// <summary>
        /// 触发内部日志事件
        /// </summary>
        /// <param name="sender">事件产生者</param>
        /// <param name="ex">异常</param>
        public static void OnRaiseLog(object sender, Exception ex)
        {
            try
            {
                var handler = Log;
                if (handler != null)
                {
                    handler(sender, new InnerLogOutputArgs(ex));
                }
            }
            catch (Exception exi)
            {
                Console.WriteLine(exi.Message);
            }
        }
    }
}
