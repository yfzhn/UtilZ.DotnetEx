using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class ResourceResponseMessage : MessageBase
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
        /// 本次请求到资源部分大小
        /// </summary>
        public Int32 Size { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>
        private byte[] _data;

        public const int HEAD_SIZE = TransferConstant.COMMON_HEADER_SIZE + 16;
        public const int RESOURCE_NOT_EXIST = 0;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="req"></param>
        /// <param name="data"></param>
        public ResourceResponseMessage(ResourceRequestMessage req, byte[] data)
            : base(new CommonHeader(TransferCommands.ResourceResponse, req.Header.Timestamp, req.Header.Rid))
        {
            this.ContextId = req.ContextId;
            this.Position = req.Position;
            if (data != null)
            {
                this.Size = data.Length;
            }
            else
            {
                this.Size = RESOURCE_NOT_EXIST;
            }

            this._data = data;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="header"></param>
        /// <param name="br"></param>
        /// <param name="srcEndPoint"></param>
        public ResourceResponseMessage(CommonHeader header, BinaryReader br, System.Net.IPEndPoint srcEndPoint)
            : base(header, srcEndPoint)
        {
            this.ContextId = br.ReadInt32();
            this.Position = br.ReadInt64();
            this.Size = br.ReadInt32();
            //此处不再拷贝数据,在写数据时指定位置写,减少拷贝次数
            //if (this.Size > 0)
            //{
            //    this.Data = new byte[this.Size];
            //    br.Read(this.Data, 0, this.Data.Length);
            //}
        }

        public override byte[] GenerateBuffer()
        {
            Int32 packageLen = ResourceResponseMessage.HEAD_SIZE + this.Size;
            Byte[] buffer = new Byte[packageLen];
            using (var ms = new MemoryStream(buffer))
            {
                var bw = new BinaryWriter(ms);
                base.Header.FillHeader(bw, packageLen);
                bw.Write(this.ContextId);
                bw.Write(this.Position);
                bw.Write(this.Size);
                if (this._data != null)
                {
                    bw.Write(this._data, 0, this._data.Length);
                }

                bw.Flush();
            }

            ProtocolParser.FillValidCode(buffer);
            return buffer;
        }
    }
}
