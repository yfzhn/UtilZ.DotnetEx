using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 发送数据异常
    /// </summary>
    public class SendDataException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常</param>
        public SendDataException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
