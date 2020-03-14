using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class ResourceRequestMessage : MessageBase
    {
        /// <summary>
        /// 请求上下文标识
        /// </summary>
        public Int32 ContextId { get; private set; }

        /// <summary>
        /// 请求资源起始位置
        /// </summary>
        public Int64 Position { get; private set; }

        /// <summary>
        /// 本次请求资源部分大小
        /// </summary>
        public Int32 Size { get; private set; }

        private const int _HEAD_SIZE = 16;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public ResourceRequestMessage(Int32 rid, Int64 position, Int32 size)
            : base(new CommonHeader(TransferCommands.ResourceRequest, TimeEx.GetTimestamp(), rid))
        {
            this.ContextId = GUIDEx.GetGUIDHashCode();

            this.Position = position;
            this.Size = size;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="header"></param>
        /// <param name="br"></param>
        /// <param name="srcEndPoint"></param>
        public ResourceRequestMessage(CommonHeader header, BinaryReader br, System.Net.IPEndPoint srcEndPoint)
            : base(header, srcEndPoint)
        {
            this.ContextId = br.ReadInt32();
            this.Position = br.ReadInt64();
            this.Size = br.ReadInt32();
        }

        public override byte[] GenerateBuffer()
        {
            Int32 packageLen = TransferConstant.COMMON_HEADER_SIZE + _HEAD_SIZE;
            Byte[] buffer = new Byte[packageLen];
            using (var ms = new MemoryStream(buffer))
            {
                var bw = new BinaryWriter(ms);
                base.Header.FillHeader(bw, packageLen);
                bw.Write(this.ContextId);
                bw.Write(this.Position);
                bw.Write(this.Size);
                bw.Flush();
            }

            ProtocolParser.FillValidCode(buffer);
            return buffer;
        }
    }
}
