using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 重采样数据段信息
    /// </summary>
    public class RepeatSampleDataSegmentInfo
    {
        /// <summary>
        /// 声道数
        /// </summary>
        public int ChannelCount { get; set; }

        /// <summary>
        /// 采样率,必须为声道数的倍数
        /// </summary>
        public long Sample { get; set; }

        /// <summary>
        /// 原始数据
        /// </summary>
        public short[] SrcData { get; set; }

        /// <summary>
        /// 起始位置
        /// </summary>
        public long BegeinIndex { get; set; }

        /// <summary>
        /// 结束位置
        /// </summary>
        public long EndIndex { get; set; }

        /// <summary>
        /// 重采样后的数据
        /// </summary>
        public short[] RepeatData { get; set; }
    }
}
