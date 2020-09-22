using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 流声音播放器
    /// </summary>
    public class StreamSoundPlayer : SoundPlayerAbs
    {
        private readonly WaveInfo _waveInfo;
        /// <summary>
        /// 音频信息
        /// </summary>
        public override WaveInfo WaveInfo
        {
            get { return this._waveInfo; }
        }

        /// <summary>
        /// 获取或设置是否启用播放缓存[true:启用;false:禁用],默认为true
        /// </summary>
        public bool EnablePlayBuffer
        {
            get
            {
                return (int)WavHelper.ChannelGetAttribute(base._handle, BASSAttribute.BASS_ATTRIB_NOBUFFER) == 0;
            }
            set
            {
                float noBuffer;
                if (value)
                {
                    noBuffer = 0f;//启用
                }
                else
                {
                    noBuffer = 1f;//禁用
                }

                WavHelper.ChannelSetAttribute(base._handle, BASSAttribute.BASS_ATTRIB_NOBUFFER, noBuffer);
            }
        }


        private bool _appendCover = false;
        /// <summary>
        /// 在追加新数据时,如果空余缓冲区大小小于新数据大小时,是否使用新数据强制覆盖旧数据.
        /// 使用场景:播放器处于未播放状态时,一直在接收新数据;播放采样率、声道数等参数与追加数据的参数不匹配,导致追加数据速度大于播放数据的速度。
        /// [true:使用最新的数据覆盖旧数据,即使是未播放;false:阻塞,直到有足够的空余缓冲区]
        /// </summary>
        public bool AppendCover
        {
            get { return this._appendCover; }
            set
            {
                if (this._appendCover == value)
                {
                    return;
                }

                this._appendCover = value;
                this.Clear();
            }
        }

        /// <summary>
        /// 获取缓存容量
        /// </summary>
        public int BufferCapcity
        {
            get
            {
                return this._buffer.Length;
            }
        }

        private byte[] _buffer;
        private int _rpos = 0;
        private int _wpos = 0;
        private uint _rposCount = 0;
        private uint _wposCount = 0;

        /// <summary>
        /// bass间隔100毫秒要一次播放的数据,所以等待半个周期
        /// </summary>
        private const int _INTERVAL = 50;
        private STREAMPROC _streamProc;
        private readonly object _rwLock = new object();


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="waveInfo">音频信息</param>
        /// <param name="bufferCapcity">缓存容量,小于等于0自动计算</param>
        public StreamSoundPlayer(WaveInfo waveInfo, int bufferCapcity = -1)
            : base(SoundPlayerType.Stream)
        {
            this._waveInfo = waveInfo;
            int bitByteCount;


            BASSFlag flags;
            switch (waveInfo.SampleBit)
            {
                case SampleType.Sample8Bit:
                    flags = BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_8BITS;
                    bitByteCount = 1;
                    break;
                case SampleType.Sample16Bit:
                    flags = BASSFlag.BASS_DEFAULT;
                    bitByteCount = 2;
                    break;
                case SampleType.Sample32Bit:
                    flags = BASSFlag.BASS_DEFAULT | BASSFlag.BASS_SAMPLE_FLOAT;
                    bitByteCount = 4;
                    break;
                default:
                    throw new NotImplementedException($"未实现的采样类型:{waveInfo.SampleBit.ToString()}");
            }

            if (bufferCapcity == -1)
            {
                bufferCapcity = waveInfo.SampleRate * waveInfo.ChannelCount * bitByteCount * 10;//默认缓存10秒数据
            }

            if (bufferCapcity > 0)
            {
                this._buffer = new byte[bufferCapcity];
            }

            this._streamProc = new STREAMPROC(this.StreamProcCallback);
            base._handle = WavHelper.StreamCreate(waveInfo.SampleRate, waveInfo.ChannelCount, flags, this._streamProc, IntPtr.Zero);
            //base._device = WavHelper.ChannelGetDevice(base._handle);
        }




        private byte[] _callbackData = null;

        /// <summary>
        /// 自定义播放数据[参数1:请求数据长度;参数2:数据存放指针;参数3:返回请求到的数据长度]
        /// 用于播放外部缓存数据场景,当此回调有注册,则启用;为null则禁用.默认为null
        /// </summary>
        public Func<int, IntPtr, int> CustomerPlayDataFunc = null;

        private int StreamProcCallback(int handle, IntPtr bufferPtr, int length, IntPtr user)
        {
            try
            {
                var func = this.CustomerPlayDataFunc;
                if (func != null)
                {
                    return func(length, bufferPtr);
                }

                if (this._callbackData == null || this._callbackData.Length < length)
                {
                    this._callbackData = new byte[length];
                }

                if (this._wposCount > this._rposCount)
                {
                    int modLength = this._buffer.Length - this._rpos;
                    if (modLength >= length)
                    {
                        Array.Copy(this._buffer, this._rpos, this._callbackData, 0, length);
                        this._rpos += length;

                        //当在播放状态改变了AppendCover属性值,则可能出现死循环场景,此处多一个健壮性代码
                        //正常情况下这部分代码不需要
                        if (this._rpos >= this._buffer.Length)
                        {
                            this._rposCount++;
                            this._rpos = this._rpos % this._buffer.Length;
                        }
                    }
                    else
                    {
                        if (this._wpos + modLength < length)
                        {
                            //WavLoger.OnRaiseLog(this, $"{length}-0");
                            return 0;
                        }

                        Array.Copy(this._buffer, this._rpos, this._callbackData, 0, modLength);
                        int modLength2 = length - modLength;
                        Array.Copy(this._buffer, 0, this._callbackData, modLength, modLength2);
                        this._rpos = modLength2;
                        this._rposCount++;
                        //WavLoger.OnRaiseLog(this, $"read rount count +1 _ {this._rposCount}");
                    }
                }
                else
                {
                    int wpos = this._wpos;
                    if (wpos >= this._rpos)
                    {
                        if (wpos - this._rpos < length)
                        {
                            //注:如果没有数据,则返回长度为0;千万不能用Thread.Sleep,因为播放是单线程.会把播放线程停止下来,所有通道都不会再播放
                            //WavLoger.OnRaiseLog(this, $"{length}-0");
                            return 0;
                        }

                        int modLength = this._buffer.Length - this._rpos;
                        if (modLength >= length)
                        {
                            //剩余的数据长度足够
                            Array.Copy(this._buffer, this._rpos, this._callbackData, 0, length);
                            this._rpos += length;
                        }
                        else
                        {
                            //剩余的数据长度不够
                            int offset;
                            if (modLength > 0)
                            {
                                //先读取剩余部分
                                Array.Copy(this._buffer, this._rpos, this._callbackData, 0, modLength);
                                offset = modLength;
                                modLength = length - modLength;
                            }
                            else
                            {
                                offset = 0;
                                modLength = length;
                            }

                            //再读取起始部分
                            Array.Copy(this._buffer, 0, this._callbackData, offset, modLength);
                            this._rpos = modLength;
                            this._rposCount++;
                        }
                    }
                    else
                    {
                        WavLoger.OnRaiseLog(this, $"read  error _ {this._rposCount}");
                        return 0;
                    }
                }

                //WavLoger.OnRaiseLog(this, $"{length}-{length}");
                Marshal.Copy(this._callbackData, 0, bufferPtr, length);
                return length;
            }
            catch (Exception ex)
            {
                WavLoger.OnRaiseLog(this, "播放器获取播放数据发生异常", ex);
                return 0;
            }
        }


        /// <summary>
        /// 获取已播放和未播放的全部数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetAllData()
        {
            uint wposCount = this._wposCount;
            uint rposCount = this._rposCount;
            int wpos = this._wpos % this._buffer.Length;
            int rpos = this._rpos % this._buffer.Length;

            byte[] data;

            if (wposCount > rposCount)
            {
                data = new byte[this._buffer.Length];
                int modLen = data.Length - wpos;
                Array.Copy(this._buffer, wpos, data, 0, modLen);
                Array.Copy(this._buffer, 0, data, modLen, wpos);
            }
            else
            {
                if (wposCount == 0)
                {
                    if (wpos >= rpos)
                    {
                        data = new byte[wpos];
                        Array.Copy(this._buffer, 0, data, 0, data.Length);
                    }
                    else
                    {
                        data = new byte[0];
                    }
                }
                else
                {
                    data = new byte[this._buffer.Length];
                    int modLen = data.Length - wpos;
                    Array.Copy(this._buffer, wpos, data, 0, modLen);
                    Array.Copy(this._buffer, 0, data, modLen, wpos);
                }
            }

            return data;
        }

        /// <summary>
        /// 获取未播放的全部数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetNoPlayData()
        {
            lock (this._rwLock)
            {
                uint wposCount = this._wposCount;
                uint rposCount = this._rposCount;
                int wpos = this._wpos % this._buffer.Length;
                int rpos = this._rpos % this._buffer.Length;

                byte[] data;

                if (wposCount > rposCount)
                {
                    int modLen = this._buffer.Length - rpos;
                    data = new byte[modLen + wpos];
                    Array.Copy(this._buffer, rpos, data, 0, modLen);
                    Array.Copy(this._buffer, 0, data, modLen, wpos);
                }
                else
                {
                    if (wpos >= rpos)
                    {
                        data = new byte[wpos - rpos];
                        Array.Copy(this._buffer, rpos, data, 0, data.Length);
                    }
                    else
                    {
                        data = new byte[0];
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// 设置位置播放
        /// </summary>
        /// <param name="pos">指定播放位置</param>
        public void SetPos(int pos)
        {
            this._rpos = pos;
        }

        /// <summary>
        /// 追加数据到播放缓存
        /// </summary>
        /// <param name="data">数据</param>
        public void AppendData(byte[] data)
        {
            this.AppendData(data, 0, data.Length);
        }

        /// <summary>
        /// 追加数据到播放缓存
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">起始位置索引</param>
        /// <param name="length">数据长度</param>
        public void AppendData(byte[] data, int index, int length)
        {
            if (index < 0 || index + length > data.Length)
            {
                throw new ArgumentOutOfRangeException($"起始位置索引和数据长度参数超出范围");
            }

            lock (this._rwLock)
            {
                if (length > this._buffer.Length)
                {
                    //添加的数据长度超过缓存长度
                    if (this._appendCover)
                    {
                        index = length - this._buffer.Length;//重新计算起始位置索引
                        this.CoverAppendData(data, index, this._buffer.Length);
                    }
                    else
                    {
                        int appendDataLength = this._buffer.Length;
                        while (length > 0)
                        {
                            if (appendDataLength > length)
                            {
                                appendDataLength = length;
                            }

                            this.NoCoverAppendData(data, index, appendDataLength);
                            index += appendDataLength;
                            length -= appendDataLength;
                        }
                    }
                }
                else
                {
                    //添加的数据长度未超过缓存长度
                    if (this._appendCover)
                    {
                        this.CoverAppendData(data, index, length);
                    }
                    else
                    {
                        this.NoCoverAppendData(data, index, length);
                    }
                }
            }
        }

        /// <summary>
        /// 非覆盖式追加追加数据到播放缓存
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">起始位置索引</param>
        /// <param name="length">数据长度</param>
        private void NoCoverAppendData(byte[] data, int index, int length)
        {
            if (this._wposCount > this._rposCount)
            {
                //不在同一圈上,写的更快
                while (true)
                {
                    if (!this.AllowNoCoverAppendData())
                    {
                        return;
                    }

                    if (this._wposCount > this._rposCount)
                    {
                        if (this._buffer.Length - this._wpos >= length)
                        {
                            if (this._rpos - this._wpos >= length)
                            {
                                Array.Copy(data, index, this._buffer, this._wpos, length);
                                this._wpos += length;
                                break;
                            }
                            else
                            {
                                //WavLoger.OnRaiseLog(this, "this._wpos < modLength2 Thread.Sleep(50)");
                                Thread.Sleep(_INTERVAL);
                            }
                        }
                        else
                        {
                            //WavLoger.OnRaiseLog(this, "this._wpos < modLength2 Thread.Sleep(50)");
                            Thread.Sleep(_INTERVAL);
                        }
                    }
                    else
                    {
                        //剩下的空间不够
                        int tailLength = this._buffer.Length - this._wpos;
                        if (length <= tailLength)
                        {
                            Array.Copy(data, index, this._buffer, this._wpos, length);
                            this._wpos += length;
                        }
                        else
                        {
                            int modLength = length - tailLength;
                            while (this._rpos < modLength)
                            {
                                if (!this.AllowNoCoverAppendData())
                                {
                                    return;
                                }

                                //起始空间不够
                                //WavLoger.OnRaiseLog(this, "this._wpos < modLength2 Thread.Sleep(50)");
                                Thread.Sleep(_INTERVAL);
                            }

                            Array.Copy(data, index, this._buffer, this._wpos, tailLength);
                            index = index + tailLength;

                            Array.Copy(data, index, this._buffer, 0, modLength);
                            this._wpos = modLength;
                            this._wposCount++;
                        }
                        break;
                    }
                }
            }
            else
            {
                //同一圈上
                if (this._wpos + length >= this._buffer.Length)
                {
                    //剩下的空间不够
                    int tailLength = this._buffer.Length - this._wpos;
                    int modLength = length - tailLength;

                    while (this._rpos < modLength)
                    {
                        //起始空间不够
                        //WavLoger.OnRaiseLog(this, "this._wpos < modLength2 Thread.Sleep(50)");
                        Thread.Sleep(_INTERVAL);
                    }

                    Array.Copy(data, index, this._buffer, this._wpos, tailLength);
                    index = index + tailLength;

                    Array.Copy(data, index, this._buffer, 0, modLength);
                    this._wpos = modLength;
                    this._wposCount++;
                }
                else
                {
                    Array.Copy(data, index, this._buffer, this._wpos, length);
                    this._wpos += length;
                }
            }
        }

        private bool AllowNoCoverAppendData()
        {
            var status = base.Status;
            return status == SoundPlayerStatus.StartPlaying || status == SoundPlayerStatus.Playing;
        }

        /// <summary>
        /// 覆盖式追加追加数据到播放缓存
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="index">起始位置索引</param>
        /// <param name="length">数据长度</param>
        private void CoverAppendData(byte[] data, int index, int length)
        {
            //同一圈上
            if (this._wpos + length >= this._buffer.Length)
            {
                //剩下的空间不够
                int tailLength = this._buffer.Length - this._wpos;
                Array.Copy(data, index, this._buffer, this._wpos, tailLength);
                index = index + tailLength;

                int modLength = length - tailLength;
                Array.Copy(data, index, this._buffer, 0, modLength);
                this._wpos = modLength;
                this._wposCount++;
            }
            else
            {
                Array.Copy(data, index, this._buffer, this._wpos, length);
                this._wpos += length;
            }
        }

        /// <summary>
        /// 清空缓存数据
        /// </summary>
        public void Clear()
        {
            lock (this._rwLock)
            {
                this._wposCount = 0;
                this._rposCount = 0;
                this._rpos = 0;
                this._wpos = 0;
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._streamProc = null;
            this._buffer = null;
        }
    }
}
