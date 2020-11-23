using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 线性数据信息
    /// </summary>
    public class PcmDataInfo
    {
        /// <summary>
        /// 声道数
        /// </summary>
        public int ChanelCount { get; private set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public int SampleRate { get; private set; }

        /// <summary>
        /// 采样位数
        /// </summary>
        public int SampleBit { get; private set; }

        /// <summary>
        /// 线性数据播放持续时长,单位/秒
        /// </summary>
        public double DurationSeconds { get; private set; }

        /// <summary>
        /// 线性数据
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// 线性数据
        /// </summary>
        public short[] Data2 { get; private set; }











        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="chanelCount">声道数</param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="sampleBit">采样位数</param>
        /// <param name="durationSeconds">线性数据播放持续时长,单位/秒</param>
        /// <param name="data">线性数据</param>
        public PcmDataInfo(int chanelCount, int sampleRate, int sampleBit, double durationSeconds, byte[] data)
            : this(chanelCount, sampleRate, sampleBit, durationSeconds)
        {
            this.Data = data;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="chanelCount">声道数</param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="sampleBit">采样位数</param>
        /// <param name="durationSeconds">线性数据播放持续时长,单位/秒</param>
        /// <param name="data">线性数据</param>
        public PcmDataInfo(int chanelCount, int sampleRate, int sampleBit, double durationSeconds, short[] data)
            : this(chanelCount, sampleRate, sampleBit, durationSeconds)
        {
            this.Data2 = data;
        }



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="chanelCount">声道数</param>
        /// <param name="freq">采样率</param>
        /// <param name="sampleBit">采样位数</param>
        /// <param name="durationSeconds">线性数据播放持续时长,单位/秒</param>
        private PcmDataInfo(int chanelCount, int freq, int sampleBit, double durationSeconds)
        {
            this.ChanelCount = chanelCount;
            this.SampleRate = freq;
            this.SampleBit = sampleBit;
            this.DurationSeconds = durationSeconds;
        }

        private static int CalSampleBit(BASS_CHANNELINFO_INTERNAL wavInfo, double durationSeconds, int dataLength, bool dataType)
        {
            int sampleBit = wavInfo.origres;
            if (sampleBit == 0)
            {
                //1秒内播放的数据 sd=data.Length / durationSeconds=fs*chans*bits
                sampleBit = (int)(dataLength / durationSeconds) / wavInfo.freq / wavInfo.chans * 8;
                if (dataType)
                {
                    sampleBit = sampleBit * 2;
                }

                if (sampleBit <= 8)
                {
                    sampleBit = 8;
                }
                else if (sampleBit <= 16)
                {
                    sampleBit = 16;
                }
                else if (sampleBit <= 24)
                {
                    sampleBit = 24;
                }
                else
                {
                    sampleBit = 32;
                }
            }

            return sampleBit;
        }



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wavInfo">音频数据信息</param>
        /// <param name="durationSeconds">线性数据播放持续时长,单位/秒</param>
        /// <param name="data">线性数据</param>
        internal PcmDataInfo(BASS_CHANNELINFO_INTERNAL wavInfo, double durationSeconds, byte[] data)
            : this(wavInfo.chans, wavInfo.freq, CalSampleBit(wavInfo, durationSeconds, data.Length, false), durationSeconds, data)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wavInfo">音频数据信息</param>
        /// <param name="durationSeconds">线性数据播放持续时长,单位/秒</param>
        /// <param name="data">线性数据</param>
        internal PcmDataInfo(BASS_CHANNELINFO_INTERNAL wavInfo, double durationSeconds, short[] data)
             : this(wavInfo.chans, wavInfo.freq, CalSampleBit(wavInfo, durationSeconds, data.Length, true), durationSeconds, data)
        {

        }
    }
}
