using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 声道数据段信息
    /// </summary>
    public class ChannelDataSegment
    {
        /// <summary>
        /// 是否是立体声
        /// </summary>
        public bool IsStereo { get; set; }

        /// <summary>
        /// 原始左声道数据
        /// </summary>
        public short[] SrcLeftData { get; set; }

        /// <summary>
        /// 原始右声道数据
        /// </summary>
        public short[] SrcRightData { get; set; }

        /// <summary>
        /// 起始位置
        /// </summary>
        public long BegeinIndex { get; set; }

        /// <summary>
        /// 结束位置
        /// </summary>
        public long EndIndex { get; set; }

        /// <summary>
        /// 最后一段数据是否特殊处理
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public long Sample { get; set; }

        /// <summary>
        /// 左声道数据
        /// </summary>
        public short[] LeftData { get; set; }

        /// <summary>
        /// 右声道数据
        /// </summary>
        public short[] RightData { get; set; }
    }
}
