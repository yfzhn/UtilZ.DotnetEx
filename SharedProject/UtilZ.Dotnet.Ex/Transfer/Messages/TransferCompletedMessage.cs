using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class TransferCompletedMessage : MessageBase
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="sendDataNotifyMessage"></param>
        public TransferCompletedMessage(SendDataNotifyMessage sendDataNotifyMessage)
            : base(new CommonHeader(TransferCommands.TransferCompleted, TimeEx.GetTimestamp(), sendDataNotifyMessage.Header.Rid))
        {

        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="header"></param>
        /// <param name="br"></param>
        /// <param name="srcEndPoint"></param>
        public TransferCompletedMessage(CommonHeader header, BinaryReader br, System.Net.IPEndPoint srcEndPoint)
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
                //bw.Write(this.ContextId);
                bw.Flush();
            }

            ProtocolParser.FillValidCode(buffer);
            return buffer;
        }
    }
}
