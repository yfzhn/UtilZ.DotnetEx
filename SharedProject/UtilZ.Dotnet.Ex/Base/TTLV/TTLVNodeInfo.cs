using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTLV节点信息
    /// </summary>
    public class TTLVNodeInfo
    {
        /// <summary>
        /// 节点Tag
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// 节点值类型
        /// </summary>
        public TypeCode Type { get; set; }

        /// <summary>
        /// 节点值数据长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TTLVNodeInfo()
        {

        }
    }
}
