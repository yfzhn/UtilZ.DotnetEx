using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 传输策略
    /// </summary>
    public class TransferPolicy
    {
        /// <summary>
        /// 接收数据EndPoint
        /// </summary>
        public IPEndPoint RemoteEP { get; protected set; }

        /// <summary>
        /// 发送数据优先级[值越小,优先级越高]
        /// </summary>
        public short Priority { get; protected set; }

        /// <summary>
        /// 单次发送超时时长,单位毫秒
        /// </summary>
        public int MillisecondsTimeout { get; protected set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RepeatCount { get; protected set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remoteEP">接收数据EndPoint</param> 
        /// <param name="priority">发送数据优先级[值越小,优先级越高]</param>
        /// <param name="millisecondsTimeout">发送超时时长,单位毫秒[此值不应过大,否则当发方消息发送到一半时宕机,则可能出现超时时长的网络风暴]</param>
        /// <param name="repeatCount">重试次数,小于1不重试</param>
        public TransferPolicy(IPEndPoint remoteEP, short priority, int millisecondsTimeout, int repeatCount = 0)
        {
            if (remoteEP == null)
            {
                throw new ArgumentNullException(nameof(remoteEP));
            }

            if (millisecondsTimeout < 1)
            {
                throw new ArgumentOutOfRangeException($"发送超时时长值:{millisecondsTimeout}无效");
            }

            this.RemoteEP = remoteEP;
            this.Priority = priority;
            this.MillisecondsTimeout = millisecondsTimeout;

            if (repeatCount < 1)
            {
                repeatCount = 1;
            }

            this.RepeatCount = repeatCount;
        }
    }
}
