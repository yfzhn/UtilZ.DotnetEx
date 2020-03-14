using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTLV编解码接口
    /// </summary>
    public interface ITTLVNodeCodec
    {
        /// <summary>
        /// 顺序编码节点数据
        /// </summary>
        /// <param name="buffer">存储数据列表</param>
        /// <param name="tag">Tag</param>
        /// <param name="typeCode">Type</param>
        /// <param name="valueBuffer">Value</param>
        void WriteNode(List<byte> buffer, int tag, TypeCode typeCode, byte[] valueBuffer);

        ///// <summary>
        ///// 读取节点信息
        ///// </summary>
        ///// <param name="br"></param>
        ///// <returns></returns>
        //TTLVNodeInfo ReadNodeInfo(BinaryReader br);

        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <param name="br">BinaryReader</param>
        /// <param name="nodeInfo">存储节点信息对象</param>
        void ReadNodeInfo(BinaryReader br, TTLVNodeInfo nodeInfo);
    }
}
