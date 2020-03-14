using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 网络配置
    /// </summary>
    public class NetConfig
    {
        /// <summary>
        /// 接收数据临时存放空间大小,默认10MB
        /// </summary>
        public int TempBufferSize { get; set; } = 10485760;

        /// <summary>
        /// 收缓存大小,默认10MB
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 10485760;

        /// <summary>
        /// 发送缓存大小,默认10MB
        /// </summary>
        public int SendBufferSize { get; set; } = 10485760;

        /// <summary>
        /// 接收数据监听地址
        /// </summary>
        public IPEndPoint ListenEP { get; set; }

        /// <summary>
        /// 传输协议
        /// </summary>
        public TransferProtocal Protocal { get; set; } = TransferProtocal.Udp;

        /// <summary>
        /// 构造函数
        /// </summary>
        public NetConfig()
        {

        }

        /// <summary>
        /// 验证配置
        /// </summary>
        public void Validate()
        {
            if (this.ListenEP == null)
            {
                throw new ArgumentNullException(nameof(ListenEP));
            }
        }
    }
}
