using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Transfer.Net
{
    internal class CommonHeader
    {
        public Int16 Ver { get; private set; }

        public Int16 Cmd { get; private set; }

        public Int64 Timestamp { get; set; }

        /// <summary>
        /// 资源标识
        /// </summary>
        public Int32 Rid { get; private set; }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="timestamp"></param>
        /// <param name="rid"></param>
        /// <param name="ver"></param>
        public CommonHeader(Int16 cmd, Int64 timestamp, Int32 rid, Int16 ver = TransferConstant.PROTOCOL_VER)
        {
            this.Ver = ver;
            this.Cmd = cmd;
            this.Timestamp = timestamp;
            this.Rid = rid;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="br"></param>
        public CommonHeader(BinaryReader br)
        {
            this.Ver = br.ReadInt16();
            this.Cmd = br.ReadInt16();
            this.Timestamp = br.ReadInt64();
            this.Rid = br.ReadInt32();
        }

        public void FillHeader(BinaryWriter bw, Int32 packageLen)
        {
            //公共头
            bw.Write(TransferConstant.SYNC);   // 同步字
            bw.Write(packageLen);   // 当前数据包传输数据总长度
            bw.Write(TransferConstant.VALID_CODE_FILL);    // 校验码
            bw.Write(this.Ver);   // 协议版本号
            bw.Write(this.Cmd);
            bw.Write(this.Timestamp);
            bw.Write(this.Rid);
        }
    }
}
