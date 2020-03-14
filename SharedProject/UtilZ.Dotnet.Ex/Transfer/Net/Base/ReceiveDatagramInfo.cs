using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 接收到的数据报信息
    /// </summary>
    public class ReceiveDatagramInfo
    {
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public IPEndPoint SrcEndPoint { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="remoteEP">发送方信息</param>
        public ReceiveDatagramInfo(byte[] data, IPEndPoint remoteEP)
        {
            this.Data = data;
            this.SrcEndPoint = new IPEndPoint(new IPAddress(remoteEP.Address.GetAddressBytes()), remoteEP.Port);
        }
    }
}
