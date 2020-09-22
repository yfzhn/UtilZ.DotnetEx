using System;
using System.Runtime.InteropServices;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;
using BASSFlag = UtilZ.Dotnet.Wav.VoicePlayer.Native.BASSFlag;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    // WavHelper_Sample
    public partial class WavHelper
    {
        public int SampleCreate(int dataLen, int freq, int chan, int dataMax, BASSFlag flags)
        {
            int handle = Native.NativeMethods.BASS_SampleCreate(dataLen, freq, chan, dataMax, flags);
            //If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        public bool SampleGetData(int hand, byte[] data)
        {
            return Native.NativeMethods.BASS_SampleGetData(hand, data);
        }

        public bool SampleSetData(int hand, byte[] data)
        {
            return Native.NativeMethods.BASS_SampleSetData(hand, data);
        }

        public bool SampleFree(int hand)
        {
            return Native.NativeMethods.BASS_SampleFree(hand);
        }

        public bool SampleGetInfo(int hand, BASS_SAMPLE bSample)
        {
            return Native.NativeMethods.BASS_SampleGetInfo(hand, bSample);
        }

        public bool SampleSetInfo(int hand, BASS_SAMPLE bSample)
        {
            return Native.NativeMethods.BASS_SampleSetInfo(hand, bSample);
        }

        /// <summary>
        /// 音频源文件输入 使用后必须释放
        ///  Marshal.FreeHGlobal(tt);
        /// </summary>
        /// <param name="data">源数据 需要采用带格式数据 如WAV 前44个字节</param>
        /// <param name="bassFlag">BASSFlag.BASS_STREAM_DECODE</param>
        /// <param name="dataPtr">分配的内存</param>
        /// <returns></returns>
        public int StreamCreateFile(byte[] data, out IntPtr dataPtr, BASSFlag bassFlag = BASSFlag.BASS_STREAM_DECODE)
        {
            dataPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, dataPtr, data.Length);
            int result = Native.NativeMethods.BASS_StreamCreateFileMemory(true, dataPtr, 0, data.Length, bassFlag);
            return result ;
        }


        /// <summary>
        ///  音频源转换目标
        /// </summary>
        /// <param name="sample">目标采样率</param>
        /// <param name="chan">目标通道</param>
        /// <param name="bassFlag">BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END</param>
        /// <returns></returns>
        public int MixerStreamCreate(int sample, int chan, BASSFlag bassFlag= BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MIXER_END)
        {
            int result = Native.NativeMethods.BASS_Mixer_StreamCreate(sample, chan, bassFlag);
            return result;
        }

        /// <summary>
        /// 音频源转采样率
        /// </summary>
        /// <param name="mixS">目标句柄</param>
        /// <param name="chan">源句柄</param>
        /// <param name="bassFlag"></param>
        /// <returns></returns>
        public bool MixerStreamAddChannel(int mixS, int chan, BASSFlag bassFlag= BASSFlag.BASS_DEFAULT)
        {
            bool result = Native.NativeMethods.BASS_Mixer_StreamAddChannel(mixS, chan, bassFlag);
            return result;
        }

        public bool ReSampleByOneChannel(byte[] sourceData, int sourceSample, int targetSample, out byte[] newData)
        {
            newData =null;
            return false;
        }
    }
}