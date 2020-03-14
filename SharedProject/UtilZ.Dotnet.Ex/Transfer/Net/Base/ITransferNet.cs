using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 收发接口网络数据
    /// </summary>
    public interface ITransferNet : IDisposable
    {
        /// <summary>
        /// 状态[true:正常false:停止]
        /// </summary>
        bool Status { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config">传输配置</param>
        /// <param name="rev">接收数据回调</param>
        void Init(NetConfig config, Action<ReceiveDatagramInfo> rev);

        /// <summary>
        /// 启动接收
        /// </summary>
        void Start();

        /// <summary>
        /// 停止接收
        /// </summary>
        void Stop();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">要发送的数据</param>
        /// <param name="remoteEP">接收数据EndPoint</param>
        void Send(byte[] data, IPEndPoint remoteEP);
    }
}