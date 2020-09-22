using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 声音播放器基类
    /// </summary>
    public abstract class SoundPlayerAbs : IDisposable
    {
        protected int _handle = WavConstant.NONE_HANDLE;
        /// <summary>
        /// 获取bass句柄
        /// </summary>
        public int Handle
        {
            get { return this._handle; }
        }

        /// <summary>
        /// 音频信息
        /// </summary>
        public abstract WaveInfo WaveInfo { get; }

        protected float? _volume = null;
        /// <summary>
        /// 获取或设置音量[0-1]
        /// </summary>
        public float Volume
        {
            get
            {
                if (this._volume.HasValue)
                {
                    return this._volume.Value;
                }
                else
                {
                    if (this.HandleValid())
                    {
                        return WavHelper.ChannelGetVolume(this._handle);
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }
            set
            {
                this._volume = value;
                if (this.HandleValid())
                {
                    this.UpdateVolume();
                }
            }
        }
        protected void UpdateVolume()
        {
            WavHelper.ChannelSetVolume(this._handle, this._volume.Value);
        }



        protected float? _balance = null;
        /// <summary>
        /// The panning/balance position of a channel[The pan position... -1.0f (full left) to +1.0f (full right), 0.0 = centre]
        /// </summary>
        public float Balance
        {
            get
            {
                if (this._balance.HasValue)
                {
                    return this._balance.Value;
                }
                else
                {
                    if (this.HandleValid())
                    {
                        return WavHelper.ChannelGetAttribute(this._handle, BASSAttribute.BASS_ATTRIB_PAN);
                    }
                    else
                    {
                        return 0f;
                    }
                }
            }
            set
            {
                this._balance = value;
                if (this.HandleValid())
                {
                    this.UpdateBalance();
                }
            }
        }

        protected void UpdateBalance()
        {
            WavHelper.ChannelSetAttribute(this._handle, BASSAttribute.BASS_ATTRIB_PAN, this._balance.Value);
        }



        private const int _PLAY_SPEED_ZOOM_MULIT = 200;
        protected int? _speed = null;
        /// <summary>
        /// 获取或设置播放速度,小于0减速,大于0加速
        /// </summary>
        public int Speed
        {
            get
            {
                if (this._speed.HasValue)
                {
                    return this._speed.Value;
                }
                else
                {
                    if (this.HandleValid())
                    {
                        int speed;
                        float playSampleRate = WavHelper.ChannelGetSpeed(this._handle);
                        var waveInfo = this.WaveInfo;
                        if (waveInfo == null)
                        {
                            throw new WavException("句柄已释放");
                        }

                        var sampleRate = waveInfo.SampleRate;
                        float offset = playSampleRate / sampleRate;
                        if (playSampleRate < sampleRate)
                        {
                            speed = (int)((1 - offset) * (0 - _PLAY_SPEED_ZOOM_MULIT));
                        }
                        else if (playSampleRate > sampleRate)
                        {
                            speed = (int)((offset - 1) * _PLAY_SPEED_ZOOM_MULIT);
                        }
                        else
                        {
                            speed = 0;
                        }

                        return speed;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            set
            {
                this._speed = value;
                if (this.HandleValid())
                {
                    this.UpdatePlaySpeed();
                }
            }
        }

        protected void UpdatePlaySpeed()
        {
            int? speed = this._speed;
            if (!speed.HasValue)
            {
                return;
            }

            float offset = (float)Math.Abs(speed.Value) / _PLAY_SPEED_ZOOM_MULIT;
            var waveInfo = this.WaveInfo;
            if (waveInfo == null)
            {
                throw new WavException("句柄已释放");
            }

            var sampleRate = waveInfo.SampleRate;
            float playSampleRate;
            if (speed.Value < 0)
            {
                playSampleRate = sampleRate * (1 - offset);
            }
            else if (speed.Value == 0)
            {
                playSampleRate = sampleRate;
            }
            else
            {
                playSampleRate = sampleRate * (1 + offset);
            }

            WavHelper.ChannelSetSpeed(this._handle, playSampleRate);
        }


        //指定设备报错
        //protected int _device = 1;
        ///// <summary>
        ///// 获取或设置输出设备(0 = no sound, 1 = first real output device, BASS_NODEVICE = no device)
        ///// </summary>
        //public int Device
        //{
        //    get { return this._device; }
        //    set
        //    {
        //        this._device = value;
        //        if (this.HandleValid())
        //        {
        //            WavHelper.ChannelSetDevice(this._handle, this._device);
        //        }
        //    }
        //}


        private SoundPlayerStatus _status = SoundPlayerStatus.Stoped;
        /// <summary>
        /// 获取播放器状态
        /// </summary>
        public SoundPlayerStatus Status
        {
            get
            {
                if (!this.HandleValid() && this._status != SoundPlayerStatus.Stoped)
                {
                    this._status = SoundPlayerStatus.Stoped;
                }

                return this._status;
            }
        }

        /// <summary>
        /// 声音播放器类型
        /// </summary>
        public SoundPlayerType PlayerType { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="soundPlayerType">声音播放器类型</param>
        public SoundPlayerAbs(SoundPlayerType soundPlayerType)
        {
            this.PlayerType = soundPlayerType;
        }



        protected bool HandleValid()
        {
            return this._handle != WavConstant.NONE_HANDLE;
        }

        protected virtual void FreeBASS()
        {
            try
            {
                if (this.HandleValid())
                {
                    WavHelper.StreamFree(this._handle);
                    this._handle = WavConstant.NONE_HANDLE;
                }
            }
            catch (Exception ex)
            {
                WavLoger.OnRaiseLog(this, ex);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.FreeBASS();
        }



        /// <summary>
        /// 初始化声音输出设备
        /// </summary>
        /// <param name="device"> -1 = default device, 0 = no sound, 1 = first real output device. BASS_GetDeviceInfo can be used to enumerate the available devices. </param>
        /// <param name="freq">输出率</param>
        /// <param name="flags">A combination of these flags</param>
        public static void InitDevice(int device, int freq, BASSInit flags)
        {
            WavHelper.Init(device, freq, flags);
        }

        /// <summary>
        /// Frees all resources used by the output device, including all its samples, streams and MOD musics
        /// </summary>
        public static void Free()
        {
            WavHelper.Free();
        }




        private bool ValidatePPSOperate(SoundPlayerStatus targetStatus)
        {
            if (!this.HandleValid())
            {
                string operate;
                switch (targetStatus)
                {
                    case SoundPlayerStatus.Playing:
                        operate = "播放";
                        break;
                    case SoundPlayerStatus.Paused:
                        operate = "暂停";
                        break;
                    case SoundPlayerStatus.Stoped:
                        operate = "停止";
                        break;
                    default:
                        throw new NotImplementedException($"未实现的操作状态{targetStatus.ToString()}验证");
                }

                throw new WavException($"执行{operate}操作失败,未加载音频数据");
            }

            if (this._status == targetStatus)
            {
                return false;
            }

            return true;
        }





        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (this.ValidatePPSOperate(SoundPlayerStatus.Playing))
            {
                this._status = SoundPlayerStatus.StartPlaying;
                WavHelper.ChannelPlay(this._handle, this.Status == SoundPlayerStatus.Stoped);
                this._status = SoundPlayerStatus.Playing;
                this.OnPlayStatusChanged();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (this.ValidatePPSOperate(SoundPlayerStatus.Paused))
            {
                this._status = SoundPlayerStatus.Pausing;
                WavHelper.ChannelPause(this._handle);
                this._status = SoundPlayerStatus.Paused;
                this.OnPlayStatusChanged();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (this.ValidatePPSOperate(SoundPlayerStatus.Stoped))
            {
                this._status = SoundPlayerStatus.Stoping;
                WavHelper.ChannelStop(this._handle);
                this._status = SoundPlayerStatus.Stoped;
                this.OnPlayStatusChanged();
            }
        }

        /// <summary>
        /// 播放状态改变通知
        /// </summary>
        protected virtual void OnPlayStatusChanged()
        {

        }




        /// <summary>
        /// 获取音频文件信息
        /// </summary>
        /// <returns>音频文件信息</returns>
        public BASS_CHANNELINFO_INTERNAL GetWavInfo()
        {
            if (this.HandleValid())
            {
                return WavHelper.ChannelGetInfo(this._handle);
            }
            else
            {
                throw new WavException("未加载音频数据");
            }
        }

        /// <summary>
        /// 声音淡入淡出
        /// </summary>
        /// <param name="volValue">音量目标值</param>
        /// <param name="duration">持续时间,单位/毫秒</param>
        public void VoiceSlide(float volValue, int duration)
        {
            if (!this.HandleValid())
            {
                return;
            }

            //float value = 0.2f; 
            //int duration=2*1000;
            WavHelper.ChannelSlideAttribute(this._handle, BASSAttribute.BASS_ATTRIB_VOL, volValue, duration);
        }



        /// <summary>
        /// 获取实时FFT数据,short类型
        /// </summary>
        /// <param name="fftData">FFT数据存放数据</param>
        /// <returns>获取到的FFT数据长度</returns>
        public int GetFFTDataShort(short[] fftData)
        {
            if (this.HandleValid())
            {
                return WavHelper.ChannelGetData(this._handle, fftData, fftData.Length);
            }
            else
            {
                throw new ArgumentException("句柄无效");
            }
        }

        /// <summary>
        /// 获取实时FFT数据,float类型
        /// </summary>
        /// <param name="fftData">FFT数据存放数据</param>
        /// <returns>获取到的FFT数据长度</returns>
        public int GetFFTDataFloat(float[] fftData)
        {
            if (this.HandleValid())
            {
                return WavHelper.ChannelGetData(this._handle, fftData, fftData.Length);
            }
            else
            {
                throw new ArgumentException("句柄无效");
            }
        }
    }
}
