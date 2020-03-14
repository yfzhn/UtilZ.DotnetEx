using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.LRPC
{
    /// <summary>
    /// 通道不能找到异常
    /// </summary>
    [Serializable]
    public class NotFoundLRPCChannelException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotFoundLRPCChannelException() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        public NotFoundLRPCChannelException(string message) : base(message)
        {

        }
    }
}
