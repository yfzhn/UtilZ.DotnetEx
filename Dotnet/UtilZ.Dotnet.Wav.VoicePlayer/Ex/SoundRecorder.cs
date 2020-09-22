using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 录音机
    /// </summary>
    public class SoundRecorder
    {
        private readonly int _device;
        /// <summary>
        /// 当前使用的录音设备索引
        /// </summary>
        public int Device
        {
            get { return this._device; }
        }

        /// <summary>
        /// 录音数据输出委托[第一个参数:设备索引;第二个参数:数据;第三个参数:数据长度]
        /// </summary>
        private readonly Action<int, byte[], int> _dataOutput;
        private readonly RECORDPROC _recordPro = null;
        private bool _recordFlag = false;
        private int _handle = 0;
        /// <summary>
        /// 获取录音句柄
        /// </summary>
        public int Handle
        {
            get { return this._handle; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dataOutput">录音数据输出委托[第一个参数:设备索引;第二个参数:数据;第三个参数:数据长度]</param>
        /// <param name="device">录音设备索引,-1 = default device, 0 = first. BASS_RecordGetDeviceInfo can be used to enumerate the available devices</param>
        public SoundRecorder(Action<int, byte[], int> dataOutput, int device = 0)
        {
            if (dataOutput == null)
            {
                throw new ArgumentNullException(nameof(dataOutput));
            }

            RecordHelper.RecordInit(device);
            this._device = device;
            this._dataOutput = dataOutput;
            this._recordPro = new RECORDPROC(RecordProCallback);
        }

        /// <summary>
        ///  瞬时录音数据,长度非固定,时长时短
        /// </summary>
        private byte[] _recBuffer = null;
        private bool RecordProCallback(int handle, IntPtr buffer, int length, IntPtr user)
        {
            if (length > 0 && buffer != IntPtr.Zero)
            {
                if (this._recBuffer == null || this._recBuffer.Length < length)
                {
                    this._recBuffer = new byte[length];
                }
                Marshal.Copy(buffer, this._recBuffer, 0, length);
                this._dataOutput(this._device, this._recBuffer, length);
            }

            return this._recordFlag;
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="freq">录音采样率,默认48000</param>
        /// <param name="chans">声道数[1-6],默认1</param>
        /// <param name="sampleType">采样类型,建议默认值16位</param>
        public void Start(int freq = 48000, int chans = 1, SampleType sampleType = SampleType.Sample16Bit)
        {
            BASSFlag flags = this.SampleTypeToBASSFlag(sampleType);
            this._recordFlag = true;
            this._handle = RecordHelper.RecordStart(freq, chans, flags, this._recordPro, IntPtr.Zero);
            WavHelper.ChannelPlay(this._handle, false);
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="freq">录音采样率,默认48000</param>
        /// <param name="chans">声道数[1-6],默认1</param>
        /// <param name="period">录音数据输出间隔,单位/毫秒,默认100.最小值5ms,the maximum the maximum is half the BASS_CONFIG_REC_BUFFER setting. 
        /// If the period specified is outside this range, it is automatically capped. The default is 100ms</param>
        /// <param name="sampleType">采样类型,建议默认值16位</param>
        public void Start(int freq = 48000, int chans = 1, int period = 100, SampleType sampleType = SampleType.Sample16Bit)
        {
            BASSFlag flags = this.SampleTypeToBASSFlag(sampleType);
            this._recordFlag = true;
            this._handle = RecordHelper.RecordStart(freq, chans, flags, period, this._recordPro, IntPtr.Zero);
            WavHelper.ChannelPlay(this._handle, false);
        }

        private BASSFlag SampleTypeToBASSFlag(SampleType sampleType)
        {
            BASSFlag flags;
            switch (sampleType)
            {
                case SampleType.Sample8Bit:
                    flags = BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_SAMPLE_8BITS;
                    break;
                case SampleType.Sample16Bit:
                    flags = BASSFlag.BASS_RECORD_PAUSE;
                    break;
                case SampleType.Sample32Bit:
                    flags = BASSFlag.BASS_RECORD_PAUSE | BASSFlag.BASS_SAMPLE_FLOAT;
                    break;
                default:
                    throw new NotImplementedException($"未实现的采样类型:{sampleType.ToString()}");
            }

            return flags;
        }



        /// <summary>
        /// 停止录音
        /// </summary>
        public void Stop()
        {
            this._recordFlag = false;
        }

        /// <summary>
        /// 获取当前使用的录音设备的信息
        /// </summary>
        /// <returns>当前使用的录音设备的信息</returns>
        public BASS_DEVICEINFO GetDeviceInfo()
        {
            return RecordHelper.RecordGetDeviceInfo(this._device);
        }





        /// <summary>
        /// 获取可用的录音设备信息列表
        /// </summary>
        /// <param name="maxDeviceIndex">可能的最大设备索引,默认为5</param>
        /// <returns>可用的录音设备信息列表</returns>
        public static List<BASS_DEVICEINFO> GetAvailableDeviceInfoList(uint maxDeviceIndex = 5)
        {
            //The device to get the information of... 0 = first. 
            List<BASS_DEVICEINFO> availableDeviceInfoList = new List<BASS_DEVICEINFO>();
            for (int device = 0; device < maxDeviceIndex; device++)
            {
                BASS_DEVICEINFO info = new BASS_DEVICEINFO(device);
                if (NativeMethods.BASS_RecordGetDeviceInfo(device, ref info._internal))
                {
                    availableDeviceInfoList.Add(info);
                }
            }

            return availableDeviceInfoList;
        }

        /// <summary>
        /// Frees all resources used by the recording device. 
        /// </summary>
        public static void Free()
        {
            RecordHelper.RecordFree();
        }
    }
}
