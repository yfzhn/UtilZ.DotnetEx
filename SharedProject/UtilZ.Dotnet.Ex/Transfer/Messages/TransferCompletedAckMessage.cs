using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferCompletedAckMessage : MessageBase
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="transferCompletedMessage"></param>
        public TransferCompletedAckMessage(TransferCompletedMessage transferCompletedMessage)
            : base(new CommonHeader(TransferCommands.TransferCompletedAck, transferCompletedMessage.Header.Timestamp, transferCompletedMessage.Header.Rid))
        {

        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="header"></param>
        /// <param name="br"></param>
        /// <param name="srcEndPoint"></param>
        public TransferCompletedAckMessage(CommonHeader header, BinaryReader br, System.Net.IPEndPoint srcEndPoint)
            : base(header, srcEndPoint)
        {

        }

        public override byte[] GenerateBuffer()
        {
            Int32 packageLen = TransferConstant.COMMON_HEADER_SIZE;
            Byte[] buffer = new Byte[packageLen];
            using (var ms = new MemoryStream(buffer))
            {
                var bw = new BinaryWriter(ms);
                base.Header.FillHeader(bw, packageLen);
                bw.Flush();
            }

            ProtocolParser.FillValidCode(buffer);
            return buffer;
        }
    }
}
