using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferCommands
    {
        /// <summary>
        /// 数据发送通知
        /// </summary>
        public const Int16 SendNotify = 1;

        /// <summary>
        /// 资源请求
        /// </summary>
        public const Int16 ResourceRequest = 2;

        /// <summary>
        /// 资源响应
        /// </summary>
        public const Int16 ResourceResponse = 3;

        /// <summary>
        /// 传输完成通知
        /// </summary>
        public const Int16 TransferCompleted = 4;

        /// <summary>
        /// 传输完成确认
        /// </summary>
        public const Int16 TransferCompletedAck = 5;
    }
}
