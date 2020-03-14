using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class SendDataNotifyMessage : MessageBase
    {
        /// <summary>
        /// 优先级[值越小, 优先级越高]
        /// </summary>
        public Int16 Priority { get; private set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        public Byte ResourceType { get; private set; }

        /// <summary>
        /// 发送超时时长,单位毫秒
        /// </summary>
        public Int32 Timeout { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 资源大小
        /// </summary>
        public Int64 Size { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; private set; }

        public const int HEAD_SIZE = 21;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="resoucesInfo"></param>
        public SendDataNotifyMessage(ResoucesInfo resoucesInfo)
            : base(new CommonHeader(TransferCommands.SendNotify, TimeEx.GetTimestamp(), resoucesInfo.Rid))
        {
            this.Priority = resoucesInfo.Policy.Priority;
            byte sendMode;
            if (resoucesInfo.Length <= TransferConstant.MESSAGE_MAX_SIZE)
            {
                var data = new byte[resoucesInfo.Length];
                Array.Copy(resoucesInfo.Data, resoucesInfo.Postion, data, 0, data.Length);
                this.Data = data;
                sendMode = ResourceTypeConstant.Message;
            }
            else
            {
                switch (resoucesInfo.ResoucesType)
                {
                    case TransferDataType.Data:
                        sendMode = ResourceTypeConstant.ResourceData;
                        break;
                    case TransferDataType.Stream:
                        sendMode = ResourceTypeConstant.ResourceStream;
                        break;
                    case TransferDataType.File:
                        sendMode = ResourceTypeConstant.ResourceFile;
                        this.FileName = Path.GetFileName(resoucesInfo.FilePath);
                        break;
                    default:
                        throw new NotImplementedException($"未实现的资源类型:{resoucesInfo.ResoucesType.ToString()}");
                }
            }

            this.ResourceType = sendMode;
            this.Timeout = resoucesInfo.Policy.MillisecondsTimeout;
            this.Size = resoucesInfo.Length;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="header"></param>
        /// <param name="br"></param>
        /// <param name="srcEndPoint"></param>
        public SendDataNotifyMessage(CommonHeader header, BinaryReader br, System.Net.IPEndPoint srcEndPoint)
            : base(header, srcEndPoint)
        {
            this.Priority = br.ReadInt16();
            this.ResourceType = br.ReadByte();
            this.Timeout = br.ReadInt32();
            Int16 fileNameLen = br.ReadInt16();
            if (fileNameLen > 0)
            {
                byte[] fileNameBuf = new byte[fileNameLen];
                br.Read(fileNameBuf, 0, fileNameBuf.Length);
                this.FileName = Encoding.UTF8.GetString(fileNameBuf);
            }

            this.Size = br.ReadInt64();
            if (this.ResourceType == ResourceTypeConstant.Message)
            {
                this.Data = new byte[this.Size];
                br.Read(this.Data, 0, this.Data.Length);
            }
        }

        public override byte[] GenerateBuffer()
        {
            Int32 packageLen = TransferConstant.COMMON_HEADER_SIZE + HEAD_SIZE;
            Int16 fileNameLen = 0;
            byte[] fileNameBuf = null;
            if (this.ResourceType == ResourceTypeConstant.Message)
            {
                packageLen = packageLen + (Int32)this.Size;
            }
            else if (this.ResourceType == ResourceTypeConstant.ResourceFile)
            {
                fileNameBuf = Encoding.UTF8.GetBytes(this.FileName);
                fileNameLen = (Int16)fileNameBuf.Length;
                packageLen = packageLen + fileNameLen;
            }

            Byte[] buffer = new Byte[packageLen];
            using (var ms = new MemoryStream(buffer))
            {
                var bw = new BinaryWriter(ms);
                base.Header.FillHeader(bw, packageLen);
                bw.Write(this.Priority);
                bw.Write(this.ResourceType);
                bw.Write(this.Timeout);
                bw.Write(fileNameLen);
                if (this.ResourceType == ResourceTypeConstant.ResourceFile)
                {
                    bw.Write(fileNameBuf, 0, fileNameBuf.Length);
                }

                bw.Write(this.Size);
                bw.Flush();

                if (this.ResourceType == ResourceTypeConstant.Message)
                {
                    bw.Write(this.Data, 0, this.Data.Length);
                }

                bw.Flush();
            }

            ProtocolParser.FillValidCode(buffer);
            return buffer;
        }

        public bool IsTimeout(long beginTs, long endTs)
        {
            return endTs - beginTs >= this.Timeout;
        }
    }
}
