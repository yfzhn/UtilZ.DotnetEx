using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    /// <summary>
    /// 接收到的数据信息
    /// </summary>
    internal struct RevDataInfo
    {
        /// <summary>
        /// 接收缓存中的偏移位置
        /// </summary>
        public int Offset;

        /// <summary>
        /// 本次数据长度
        /// </summary>
        public int Length;

        /// <summary>
        /// 来源
        /// </summary>
        public IPEndPoint RemoteEP;

        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="offset">接收缓存中的偏移位置</param>
        /// <param name="length">本次数据长度</param>
        /// <param name="remoteEP">来源</param>
        public RevDataInfo(int offset, int length, IPEndPoint remoteEP)
        {
            this.Offset = offset;
            this.Length = length;
            this.RemoteEP = remoteEP;
        }
    }
}
