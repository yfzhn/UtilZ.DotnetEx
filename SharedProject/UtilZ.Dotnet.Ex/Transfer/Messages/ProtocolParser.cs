using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class ProtocolParser
    {
        /*
        public static MessageBase Parse(ReceiveDatagramInfo receiveDatagramInfo)
        {
            var buffer = receiveDatagramInfo.Data;
            if (buffer == null)
            {
                Loger.Warn("收到的数据为null,忽略...");
                return null;
            }

            if (buffer.Length < NetTransferConstant.COMMON_HEADER_SIZE)
            {
                Loger.Warn($"收到的数据长度{buffer.Length}小于公共头{NetTransferConstant.COMMON_HEADER_SIZE},忽略...");
                return null;
            }

            var srcEndPoint = receiveDatagramInfo.SrcEndPoint;
            using (var ms = new MemoryStream(buffer))
            {
                var br = new BinaryReader(ms);
                var header = ParseCmd(br, buffer);
                MessageBase message;
                switch (header.Cmd)
                {
                    case TransferCommands.SendNotify:
                        message = new SendDataNotifyMessage(header, br, srcEndPoint);
                        break;
                    case TransferCommands.ResourceResponse:
                        message = new ResourceResponseMessage(header, br, srcEndPoint);
                        break;
                    case TransferCommands.TransferCompletedAck:
                        message = new TransferCompletedAckMessage(header, br, srcEndPoint);
                        break;
                    case TransferCommands.ResourceRequest:
                        message = new ResourceRequestMessage(header, br, srcEndPoint);
                        break;
                    case TransferCommands.TransferCompleted:
                        message = new TransferCompletedMessage(header, br, srcEndPoint);
                        break;
                    default:
                        throw new Exception($"未知命令{header.Cmd}");
                }

                return message;
            }
        }

        private static CommonHeader ParseCmd(BinaryReader br, byte[] buffer)
        {
            Int32 syc = br.ReadInt32();// 同步字
            if (syc != NetTransferConstant.SYNC)
            {
                throw new Exception($"收到数据的同步头:{syc}与期望的同步头:{NetTransferConstant.SYNC}不一致,忽略");
            }

            Int32 packageLen = br.ReadInt32();// 本次传输数据总长度
            if (packageLen != buffer.Length)
            {
                throw new Exception($"收到数据的长度:{buffer.Length}与期望的数据长度:{packageLen}不一致,忽略");
            }

            var p = br.BaseStream.Position;
            Int32 validCode = br.ReadInt32();//校验码,以后再验证

            //重置校验码为填充值
            byte[] validCodeBuf = BitConverter.GetBytes(NetTransferConstant.VALID_CODE_FILL);
            br.BaseStream.Seek(p, SeekOrigin.Begin);
            br.BaseStream.Write(validCodeBuf, 0, validCodeBuf.Length);

            //校验数据传输正确性
            bool parseResult = ProtocolParser.ValidData(validCode, buffer);
            if (!parseResult)
            {
                throw new Exception("校验不通过");
            }

            return new CommonHeader(br);
        }
        */
        internal static bool ValidData(int validCode, byte[] buffer)
        {
            return true;
        }

        internal static void FillValidCode(byte[] buffer)
        {

        }

        internal static int CalResourceValidCode(byte[] buffer)
        {
            return 0;
        }
    }
}
