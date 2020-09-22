using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 文件声音播放器
    /// </summary>
    public class FileSoundPlayer : SoundPlayerAbs
    {
        /// <summary>
        /// 播放的音频文件
        /// </summary>
        private string _fileName = null;

        /// <summary>
        /// 获取或设置播放的音频文件
        /// </summary>
        public string FileName
        {
            get { return this._fileName; }
        }

        /// <summary>
        /// 获取或设置当前获取当前播放位置,单位/秒
        /// </summary>
        public double Position
        {
            get
            {
                if (base.HandleValid())
                {
                    long bytePos = WavHelper.ChannelGetPosition(this._handle, BASSMode.BASS_POS_BYTE);
                    return WavHelper.ChannelBytes2Seconds(this._handle, bytePos);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (base.HandleValid())
                {
                    long bytePos = WavHelper.ChannelSeconds2Bytes(this._handle, value);
                    WavHelper.ChannelSetPosition(this._handle, bytePos, BASSMode.BASS_POS_BYTE);
                }
                else
                {
                    throw new WavException("未加载音频数据");
                }
            }
        }

        /// <summary>
        /// 播放时间,单位/秒
        /// </summary>
        public double Duration
        {
            get
            {
                if (base.HandleValid())
                {
                    long bytePos = WavHelper.ChannelGetLength(this._handle, BASSMode.BASS_POS_BYTE);
                    return WavHelper.ChannelBytes2Seconds(this._handle, bytePos);
                }
                else
                {
                    return 0;
                }
            }
        }


        private WaveInfo _waveInfo = null;
        /// <summary>
        /// 音频信息
        /// </summary>
        public override WaveInfo WaveInfo
        {
            get
            {
                if (this._waveInfo == null && this.HandleValid())
                {
                    BASS_CHANNELINFO_INTERNAL info = WavHelper.ChannelGetInfo(this._handle);
                    this._waveInfo = new WaveInfo(info.freq, info.chans, (SampleType)info.origres);
                }

                return this._waveInfo;
            }
        }

        /// <summary>
        /// 播放完成事件
        /// </summary>
        public event EventHandler<PlayEndArgs> PlayEnd;

        private int _playEndSyncHandle;
        private readonly SYNCPROC _playEndNotifyCallback;
        //private readonly SYNCPROC _playPositionChangedNotifyCallback;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileSoundPlayer()
            : base(SoundPlayerType.File)
        {
            this._playEndNotifyCallback = new SYNCPROC(this.PlayEndNotify);
            //this._playPositionChangedNotifyCallback = new SYNCPROC(this.PlayPositionChangedNotify);
        }

        //private void PlayPositionChangedNotify(int handle, int channel, int data, IntPtr user)
        //{
        //    WavLoger.OnRaiseLog(this, "PlayPositionChangedNotify");
        //}

        private void PlayEndNotify(int handle, int channel, int data, IntPtr user)
        {
            this._waveInfo = null;

            var handler = this.PlayEnd;
            if (handler != null)
            {
                handler(this, new PlayEndArgs(this._fileName));
            }
        }

        /// <summary>
        /// 播放状态改变通知
        /// </summary>
        protected override void OnPlayStatusChanged()
        {
            base.OnPlayStatusChanged();

            var state = WavHelper.ChannelIsActive(base._handle);
            if (state != BASSActive.BASS_ACTIVE_PAUSED &&
                state != BASSActive.BASS_ACTIVE_PAUSED_DEVICE &&
                state != BASSActive.BASS_ACTIVE_PLAYING)
            {
                this._waveInfo = null;
            }
        }

        /// <summary>
        /// 加载音频文件
        /// </summary>
        /// <param name="filePath">音频路径</param>
        public void LoadWav(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "文件路径不能为空或null");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("音频文件不存在", filePath);
            }

            this._fileName = filePath;
            this.FreeBASS();
            base._handle = WavHelper.StreamCreateFile(filePath);
            this._playEndSyncHandle = WavHelper.ChannelSetSync(this._handle, BASSSync.BASS_SYNC_END, -1, this._playEndNotifyCallback, IntPtr.Zero);
            //WavHelper.ChannelSetSync(this._handle, BASSSync.BASS_SYNC_MIXTIME, -1, this._playPositionChangedNotifyCallback, IntPtr.Zero);
            this.SetPlayerAttribute();
        }

        //private bool _firtSetPlayerAttribute = true;
        private void SetPlayerAttribute()
        {
            if (base._volume.HasValue)
            {
                base.UpdateVolume();
            }

            if (base._speed.HasValue)
            {
                //WavHelper.ChannelSetSpeed(base._handle, base._speed.Value);
                base.UpdatePlaySpeed();
            }

            if (base._balance.HasValue)
            {
                base.UpdateBalance();
            }

            //if (this._firtSetPlayerAttribute)
            //{
            //    base._device = WavHelper.ChannelGetDevice(base._handle);
            //    this._firtSetPlayerAttribute = false;
            //}
            //else
            //{
            //    int device = WavHelper.ChannelGetDevice(base._handle);
            //    if (device != base._device)
            //    {
            //        WavHelper.ChannelSetDevice(base._handle, base._device);
            //    }
            //}
        }

        protected override void FreeBASS()
        {
            this._waveInfo = null;
            base.FreeBASS();
            if (base.HandleValid())
            {
                WavHelper.BASS_ChannelRemoveSync(base._handle, this._playEndSyncHandle);
            }
        }



        private const double _PRE = 0d;

        /// <summary>
        /// 将文件由压缩格式转换为无压缩的线性数据格式
        /// </summary>
        /// <param name="srcFilePath">要转换的源文件路径</param>
        /// <param name="dstFilePath">转换结果文件路径</param>
        public static void ConvertToPcm(string srcFilePath, string dstFilePath)
        {
            PrimitiveSub(srcFilePath, dstFilePath, double.NaN, double.NaN);
        }

        /// <summary>
        /// 从文件中指定位置起始截取指定时长的数据,并保存到新文件
        /// /// </summary>
        /// <param name="srcFilePath">要转换的源文件路径</param>
        /// <param name="dstFilePath">转换结果文件路径</param>
        /// <param name="beginTimeMilliseconds">截取起始时刻,单位/毫秒[小于等于0或NaN或NegativeInfinity表示从起始位置开始截取]</param>
        /// <param name="durationMilliseconds">截取时长,单位/毫秒[NaN或PositiveInfinity表示截取到结束]</param>
        public static void Sub(string srcFilePath, string dstFilePath, double beginTimeMilliseconds, double durationMilliseconds)
        {
            if (double.IsPositiveInfinity(beginTimeMilliseconds))
            {
                throw new ArgumentOutOfRangeException($"截取起始时刻值{beginTimeMilliseconds}无效");
            }

            if (!double.IsNaN(durationMilliseconds) &&
                !double.IsPositiveInfinity(durationMilliseconds) &&
                durationMilliseconds < _PRE || double.IsNegativeInfinity(durationMilliseconds))
            {
                throw new ArgumentOutOfRangeException($"截取时长值{beginTimeMilliseconds}无效");
            }

            PrimitiveSub(srcFilePath, dstFilePath, beginTimeMilliseconds, durationMilliseconds);
        }

        private static void PrimitiveSub(string srcFilePath, string dstFilePath, double beginTimeMilliseconds, double durationMilliseconds)
        {
            if (string.IsNullOrWhiteSpace(srcFilePath))
            {
                throw new ArgumentNullException(nameof(srcFilePath));
            }

            if (string.IsNullOrWhiteSpace(srcFilePath))
            {
                throw new ArgumentNullException(nameof(dstFilePath));
            }

            if (!File.Exists(srcFilePath))
            {
                throw new FileNotFoundException("源文件不存在", srcFilePath);
            }

            if (File.Exists(dstFilePath))
            {
                throw new FileNotFoundException("目标文件已存在", dstFilePath);
            }

            PcmDataInfo pcmDataInfo = GetPcmDataByte(srcFilePath);
            if (beginTimeMilliseconds - pcmDataInfo.DurationSeconds * 1000 > _PRE)
            {
                throw new ArgumentOutOfRangeException($"起始时刻{beginTimeMilliseconds}超出范围");
            }

            int sampleBitByteCount = pcmDataInfo.SampleBit / 8;//采样位数所占字节数
            double secondByteCount = pcmDataInfo.Freq * pcmDataInfo.ChanelCount * sampleBitByteCount;//1秒数据长度,字节数
            double msByteCount = secondByteCount / 1000;//1毫秒数据长度,字节数

            int beginIndex, dataSize;
            if (beginTimeMilliseconds < _PRE ||
                double.IsNaN(beginTimeMilliseconds) ||
                double.IsNegativeInfinity(beginTimeMilliseconds))
            {
                //小于等于0或NaN或NegativeInfinity表示从起始位置开始截取
                beginIndex = 0;
            }
            else
            {
                beginIndex = (int)(beginTimeMilliseconds * msByteCount);
            }

            if (double.IsNaN(durationMilliseconds) || double.IsPositiveInfinity(durationMilliseconds))
            {
                //NaN或PositiveInfinity表示截取到结束
                dataSize = pcmDataInfo.Data.Length - beginIndex;
            }
            else
            {
                dataSize = (int)(durationMilliseconds * msByteCount);
                if (beginIndex + dataSize > pcmDataInfo.Data.Length)
                {
                    //截取长度超出范围
                    throw new ArgumentOutOfRangeException($"截取长度{durationMilliseconds}超出范围");
                    //dataSize = pcmDataInfo.Data.Length - beginIndex;
                }
            }

            byte[] buffer;
            using (FileStream fs = File.OpenWrite(dstFilePath))
            {
                //RIFF，资源交换文件标志,4byte
                buffer = Encoding.UTF8.GetBytes("RIFF");
                fs.Write(buffer, 0, 4);

                //从RIFF下一个地址开始到文件尾的总字节数,4byte
                int length = dataSize + 44 - 8;//44字节头
                buffer = BitConverter.GetBytes(length);
                fs.Write(buffer, 0, 4);

                //WAVE，代表wav文件格式,4byte
                buffer = Encoding.UTF8.GetBytes("WAVE");
                fs.Write(buffer, 0, 4);

                //fmt ，波形格式标志（fmt ）,4byte
                buffer = Encoding.UTF8.GetBytes("fmt ");
                fs.Write(buffer, 0, 4);

                //采样位数
                buffer = BitConverter.GetBytes(pcmDataInfo.SampleBit);
                fs.Write(buffer, 0, 4);

                //为1时表示线性PCM编码，大于1时表示有压缩的编码。这里是0001H,2byte
                buffer = BitConverter.GetBytes((Int16)1);
                fs.Write(buffer, 0, 2);

                //[声道数] 1为单声道，2为双声道,2byte
                buffer = BitConverter.GetBytes((Int16)pcmDataInfo.ChanelCount);
                fs.Write(buffer, 0, 2);

                //[采样频率],4byte
                buffer = BitConverter.GetBytes(pcmDataInfo.Freq);
                fs.Write(buffer, 0, 4);

                //[播放速度(传输速度)] 数据传输率,含义:播波形数据传输速率（每秒平均字节数,每秒所需播放字节数）,Bit率;[数据类型:long] 注: Bite率=采样频率*声道数*采样样本位数/8
                buffer = BitConverter.GetBytes(pcmDataInfo.ChanelCount * pcmDataInfo.SampleBit * pcmDataInfo.Freq / 8);
                fs.Write(buffer, 0, 4);

                //块对齐=通道数*每次采样得到的样本位数/8，0002H，也就是2=1*16/8,2byte
                short blockAlign = (short)(pcmDataInfo.ChanelCount * pcmDataInfo.SampleBit / 8);
                buffer = BitConverter.GetBytes(blockAlign);
                fs.Write(buffer, 0, 2);

                //[采样位数] 样本数据位数，0010H即16，一个量化样本占2byte,2byte ????
                buffer = BitConverter.GetBytes((Int16)pcmDataInfo.SampleBit);
                fs.Write(buffer, 0, 2);

                //data标志,4byte
                buffer = Encoding.UTF8.GetBytes("data");
                fs.Write(buffer, 0, 4);

                //Wav文件实际音频数据所占的大小，这里是001437C8H即1325000，再加上2CH就正好是1325044，整个文件的大小,4byte               
                buffer = BitConverter.GetBytes(dataSize);
                fs.Write(buffer, 0, 4);

                fs.Flush();

                fs.Write(pcmDataInfo.Data, beginIndex, dataSize);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取解码后的线性byte数据
        /// </summary>
        /// <returns>解码后的线性byte数据</returns>
        public static PcmDataInfo GetPcmDataByte(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("源文件不存在", filePath);
            }

            int handle = WavHelper.StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_UNICODE | BASSFlag.BASS_STREAM_DECODE);
            try
            {
                long byteLength = WavHelper.ChannelGetLength(handle, BASSMode.BASS_POS_BYTE);
                double durationSeconds = WavHelper.ChannelBytes2Seconds(handle, byteLength);

                byte[] pcmData = new byte[byteLength];
                int count = WavHelper.ChannelGetData(handle, pcmData, (int)byteLength);
                if (count != byteLength)
                {
                    WavLoger.OnRaiseLog(filePath, $"获取到文件长度:{byteLength},读取到数据长度:{count},二者不一致,根据实际读取进行拷贝");
                    byte[] pcmDataSrc = pcmData;
                    pcmData = new byte[count];
                    Array.Copy(pcmDataSrc, pcmData, pcmData.Length);
                }

                BASS_CHANNELINFO_INTERNAL wavInfo = WavHelper.ChannelGetInfo(handle);
                return new PcmDataInfo(wavInfo, durationSeconds, pcmData);
            }
            finally
            {
                WavHelper.StreamFree(handle);
            }
        }

        /// <summary>
        /// 获取解码后的线性short数据
        /// </summary>
        /// <returns>解码后的线性short数据</returns>
        public static PcmDataInfo GetPcmDataShort(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("源文件不存在", filePath);
            }

            int handle = WavHelper.StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_UNICODE | BASSFlag.BASS_STREAM_DECODE);
            try
            {
                long byteLength = WavHelper.ChannelGetLength(handle, BASSMode.BASS_POS_BYTE);
                double durationSeconds = WavHelper.ChannelBytes2Seconds(handle, byteLength);

                int shortCount = (int)(byteLength);
                short[] pcmData = new short[shortCount];
                int count = WavHelper.ChannelGetData(handle, pcmData, shortCount);

                //注:结果数据长度之所以除以2,是因为short=2byte,bass获取数据时必须使用字节长度,否则数据会少一半;
                //但是使用字节长度获取数据后,后一半数据又全是0,需要截取掉

                count = count / 2;
                short[] pcmDataSrc = pcmData;
                pcmData = new short[count];
                Array.Copy(pcmDataSrc, pcmData, count);

                BASS_CHANNELINFO_INTERNAL wavInfo = WavHelper.ChannelGetInfo(handle);
                return new PcmDataInfo(wavInfo, durationSeconds, pcmData);
            }
            finally
            {
                WavHelper.StreamFree(handle);
            }
        }
    }

    /// <summary>
    /// 播放完成事件
    /// </summary>
    public class PlayEndArgs : EventArgs
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 文件名
        /// </summary>
        /// <param name="fileName"></param>
        public PlayEndArgs(string fileName)
        {
            this.FileName = fileName;
        }
    }
}
