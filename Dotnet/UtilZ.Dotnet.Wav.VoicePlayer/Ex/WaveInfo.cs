using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 音频信息
    /// </summary>
    public class WaveInfo
    {
        /// <summary>
        /// 采样率
        /// </summary>
        public int SampleRate { get; private set; }

        /// <summary>
        /// 声道数
        /// </summary>
        public int ChannelCount { get; private set; }

        /// <summary>
        /// 采样位数
        /// </summary>
        public SampleType SampleBit { get; private set; }

        /// <summary>
        /// 每秒数据长度
        /// </summary>
        public int SecondLength { get; private set; }



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sampleRate">采样率</param>
        /// <param name="channelCount">声道数[1-6]</param>
        /// <param name="sampleType">采样类型(采样位位)</param>
        public WaveInfo(int sampleRate, int channelCount, SampleType sampleType)
        {
            this.SampleRate = sampleRate;
            this.ChannelCount = channelCount;
            this.SampleBit = sampleType;
            this.SecondLength = channelCount * sampleRate * (((int)sampleType) / 8);
        }
    }
}
