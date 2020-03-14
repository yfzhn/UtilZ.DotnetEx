using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal abstract class MessageBase
    {
        public CommonHeader Header { get; private set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public IPEndPoint SrcEndPoint { get; private set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="header"></param>
        public MessageBase(CommonHeader header)
        {
            this.Header = header;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="header"></param>
        /// <param name="srcEndPoint"></param>
        public MessageBase(CommonHeader header, IPEndPoint srcEndPoint)
        {
            this.Header = header;
            this.SrcEndPoint = srcEndPoint;
        }

        public abstract byte[] GenerateBuffer();
    }
}
