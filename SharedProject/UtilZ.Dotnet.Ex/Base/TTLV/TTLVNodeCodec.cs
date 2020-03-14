using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    internal class TTLVNodeCodec : ITTLVNodeCodec
    {
        public TTLVNodeCodec()
        {

        }

        /// <summary>
        /// 顺序编码节点数据
        /// </summary>
        /// <param name="buffer">存储数据列表</param>
        /// <param name="tag">Tag</param>
        /// <param name="typeCode">Type</param>
        /// <param name="valueBuffer">Value</param>
        public void WriteNode(List<byte> buffer, int tag, TypeCode typeCode, byte[] valueBuffer)
        {
            buffer.AddRange(BitConverter.GetBytes(tag));
            buffer.Add((byte)typeCode);
            buffer.AddRange(BitConverter.GetBytes(valueBuffer.Length));
            buffer.AddRange(valueBuffer);
        }

        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <param name="nodeInfo">存储节点信息对象</param>
        public void ReadNodeInfo(BinaryReader br, TTLVNodeInfo nodeInfo)
        {
            nodeInfo.Tag = br.ReadInt32();
            nodeInfo.Type = (TypeCode)br.ReadByte();
            nodeInfo.Length = br.ReadInt32();
        }
    }
}
