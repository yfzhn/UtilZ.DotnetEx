using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.FileTransfer
{
    /// <summary>
    /// 传输文件异常
    /// </summary>
    [Serializable]
    public class TransferFileException : Exception
    {
        /// <summary>
        /// 已传输文件大小
        /// </summary>
        public long TransferLength { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="transferLength">已传输文件大小</param>
        /// <param name="message">信息</param>
        /// <param name="ex">内部异常</param>
        public TransferFileException(long transferLength, string message, Exception ex)
            : base(message, ex)
        {
            this.TransferLength = transferLength;
        }
    }
}
