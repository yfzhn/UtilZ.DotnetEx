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
    /// 录音辅助类
    /// </summary>
    public class RecordHelper
    {
        private static bool _configUTF8 = false;

        #region Recording
        /// <summary>
        /// Frees all resources used by the recording device. 
        /// </summary>
        public static void RecordFree()
        {
            bool result = NativeMethods.BASS_RecordFree();
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Retrieves the recording device setting of the current thread. 
        /// </summary>
        /// <returns>If successful, the device number is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        public static int RecordGetDevice()
        {
            int device = NativeMethods.BASS_RecordGetDevice();
            if (device != -1)
            {
                return device;
            }

            throw new WavException(BassErrorCode.GetErrorInfo());
        }

        /// <summary>
        /// Retrieves information on a recording device
        /// </summary>
        /// <param name="device">The device to get the information of... 0 = first.</param>
        /// <param name="info">Pointer to a structure to receive the information. </param>
        /// <returns>returned BASS_DEVICEINFO.</returns>
        public static BASS_DEVICEINFO RecordGetDeviceInfo(int device)
        {
            BASS_DEVICEINFO info = new BASS_DEVICEINFO(device);
            if (NativeMethods.BASS_RecordGetDeviceInfo(device, ref info._internal))
            {
                return info;
            }

            throw new WavException(BassErrorCode.GetErrorInfo());
        }

        /// <summary>
        /// Retrieves information on a recording device
        /// </summary>
        /// <param name="device">The device to get the information of... 0 = first.</param>
        /// <param name="info">Pointer to a structure to receive the information. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        public static bool RecordGetDeviceInfo(int device, BASS_DEVICEINFO info)
        {
            bool flag = NativeMethods.BASS_RecordGetDeviceInfo(device, ref info._internal);
            if (flag)
            {
                //int bassVer = NativeMethods.BASS_GetVersion();
                //if (_configUTF8)
                //{
                //    int num;
                //    info._name = Utils.IntPtrAsStringUtf8(info._internal.name, out num);
                //    info.driver = Utils.IntPtrAsStringUtf8(info._internal.driver, out num);
                //    if ((num > 0) && (bassVer > 0x2040800))
                //    {
                //        try
                //        {
                //            //info.id = Utils.IntPtrAsStringUtf8(new IntPtr((info._internal.driver.ToPointer() + num) + 1), out num);
                //        }
                //        catch
                //        {
                //        }
                //    }
                //}
                //else
                //{
                //    info._name = Utils.IntPtrAsStringAnsi(info._internal.name);
                //    info.driver = Utils.IntPtrAsStringAnsi(info._internal.driver);
                //    if (!string.IsNullOrEmpty(info.driver) && (bassVer > 0x2040800))
                //    {
                //        try
                //        {
                //            //info.id = Utils.IntPtrAsStringAnsi(new IntPtr((info._internal.driver.ToPointer() + info.driver.Length) + 1));
                //        }
                //        catch
                //        {
                //        }
                //    }
                //}
                //info.flags = info._internal.flags;
            }
            return flag;
        }

        /// <summary>
        /// Retrieves information on the recording device being used. 
        /// </summary>
        /// <param name="info">Pointer to a structure to receive the information. </param>
        /// <returns>BASS_RECORDINFO</returns>
        public static BASS_RECORDINFO RecordGetInfo()
        {
            BASS_RECORDINFO info = new BASS_RECORDINFO();
            bool result = NativeMethods.BASS_RecordGetInfo(info);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return info;
        }

        /// <summary>
        /// Retrieves the current settings of a recording input source.
        /// </summary>
        /// <param name="input">The input to get the settings of... 0 = first, -1 = master. </param>
        /// <param name="volume">Pointer to a variable to receive the volume... NULL = don't retrieve the volume. </param>
        /// <returns>If an error occurs, -1 is returned, use BASS_ErrorGetCode to get the error code. 
        /// If successful, then the settings are returned. The BASS_INPUT_OFF flag will be set if the input is disabled, 
        /// otherwise the input is enabled. The type of input is also indicated in the high 8 bits (use BASS_INPUT_TYPE_MASK to test) of the return value,
        /// and can be one of the following. If the volume is requested but not available, volume will receive -1. </returns>
        public static int RecordGetInput(int input, ref float volume)
        {
            return NativeMethods.BASS_RecordGetInput(input, ref volume);
        }

        /// <summary>
        /// Retrieves the current settings of a recording input source.
        /// </summary>
        /// <param name="input">The input to get the settings of... 0 = first, -1 = master. </param>
        /// <param name="volume">Pointer to a variable to receive the volume... NULL = don't retrieve the volume. </param>
        /// <returns>If an error occurs, -1 is returned, use BASS_ErrorGetCode to get the error code. 
        /// If successful, then the settings are returned. The BASS_INPUT_OFF flag will be set if the input is disabled, 
        /// otherwise the input is enabled. The type of input is also indicated in the high 8 bits (use BASS_INPUT_TYPE_MASK to test) of the return value,
        /// and can be one of the following. If the volume is requested but not available, volume will receive -1. </returns>
        public static BASSInput RecordGetInput(int input)
        {
            int num = NativeMethods.BASS_RecordGetInput(input, IntPtr.Zero);
            if (num != -1)
            {
                return (((BASSInput)num) & ((BASSInput)0xff0000));
            }

            return BASSInput.BASS_INPUT_NONE;
        }

        /// <summary>
        /// Retrieves the text description of a recording input source. 
        /// </summary>
        /// <param name="input">The input to get the description of... 0 = first, -1 = master. </param>
        /// <returns>InputName</returns>
        public static string RecordGetInputName(int input)
        {
            IntPtr ptr = NativeMethods.BASS_RecordGetInputName(input);
            if ((ptr == IntPtr.Zero))
            {
                return null;
            }

            if (_configUTF8 && (NativeMethods.BASS_GetVersion() >= 0x2040a0e))
            {
                return Utils.IntPtrAsStringUtf8(ptr);
            }

            return Utils.IntPtrAsStringAnsi(ptr);
        }

        /// <summary>
        /// Initializes a recording device. 
        /// </summary>
        /// <param name="device">The device to use... -1 = default device, 0 = first. BASS_RecordGetDeviceInfo can be used to enumerate the available devices. </param>
        public static void RecordInit(int device)
        {
            bool result = NativeMethods.BASS_RecordInit(device);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Sets the recording device to use for subsequent calls in the current thread. 
        /// </summary>
        /// <param name="device">The device to use... 0 = first. </param>
        public static void RecordSetDevice(int device)
        {
            bool result = NativeMethods.BASS_RecordSetDevice(device);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Adjusts the settings of a recording input source. 
        /// </summary>
        /// <param name="input">The input to adjust the settings of... 0 = first, -1 = master. </param>
        /// <param name="setting">The new setting... a combination of these flags.
        /// BASS_INPUT_OFF Disable the input.This flag cannot be used when the device supports only one input at a time.  
        /// BASS_INPUT_ON Enable the input. If the device only allows one input at a time, then any previously enabled input will be disabled by this
        /// </param>
        /// <param name="volume">The volume level... 0 (silent) to 1 (max), less than 0 = leave current. </param>
        public static void RecordSetInput(int input, BASSInput setting, float volume)
        {
            bool result = NativeMethods.BASS_RecordSetInput(input, setting, volume);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Starts recording. 
        /// </summary>
        /// <param name="freq">The sample rate to record at... 0 = device's current sample rate. </param>
        /// <param name="chans">The number of channels... 1 = mono, 2 = stereo, etc. 0 = device's current channel count</param>
        /// <param name="flags">A combination of these flags.
        /// BASS_SAMPLE_8BITS Use 8-bit resolution. If neither this or the BASS_SAMPLE_FLOAT flag are specified, then the recorded data is 16-bit.  
        /// BASS_SAMPLE_FLOAT Use 32-bit floating-point sample data.See Floating-point channels for info.
        /// BASS_RECORD_PAUSE Start the recording paused.
        /// The HIWORD - use MAKELONG(flags, period) - can be used to set the period(in milliseconds) between calls to the callback function.
        ///              The minimum period is 5ms, the maximum is half the BASS_CONFIG_REC_BUFFER setting. If the period specified is outside this range, it is automatically capped. The default is 100ms.
        /// </param>
        /// <param name="proc">The user defined function to receive the recorded sample data... can be NULL if you do not wish to use a callback. </param>
        /// <param name="user">User instance data to pass to the callback function. </param>
        /// <returns>If successful, the new recording's handle is returned, else throw WavException. </returns>
        public static int RecordStart(int freq, int chans, BASSFlag flags, RECORDPROC proc, IntPtr user)
        {
            int flagRet = (int)flags + 20 << 8;
            int handle = NativeMethods.BASS_RecordStart(freq, chans, flags, proc, user);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        /// <summary>
        /// Starts recording. 
        /// </summary>
        /// <param name="freq">The sample rate to record at... 0 = device's current sample rate. </param>
        /// <param name="chans">The number of channels... 1 = mono, 2 = stereo, etc. 0 = device's current channel count</param>
        /// <param name="flags">A combination of these flags.
        /// BASS_SAMPLE_8BITS Use 8-bit resolution. If neither this or the BASS_SAMPLE_FLOAT flag are specified, then the recorded data is 16-bit.  
        /// BASS_SAMPLE_FLOAT Use 32-bit floating-point sample data.See Floating-point channels for info.
        /// BASS_RECORD_PAUSE Start the recording paused.
        /// The HIWORD - use MAKELONG(flags, period) - can be used to set the period(in milliseconds) between calls to the callback function.
        ///              The minimum period is 5ms, the maximum is half the BASS_CONFIG_REC_BUFFER setting. If the period specified is outside this range, it is automatically capped. The default is 100ms.
        /// </param>
        /// <param name="period">Set the period(in milliseconds) between calls to the callback function(Un4seen.Bass.RECORDPROC). 
        /// The minimum period is 5ms, the maximum the maximum is half the BASS_CONFIG_REC_BUFFER setting. 
        /// If the period specified is outside this range, it is automatically capped. The default is 100ms</param>
        /// <param name="proc">The user defined function to receive the recorded sample data... can be NULL if you do not wish to use a callback. </param>
        /// <param name="user">User instance data to pass to the callback function. </param>
        /// <returns>If successful, the new recording's handle is returned, else throw WavException. </returns>
        public static int RecordStart(int freq, int chans, BASSFlag flags, int period, RECORDPROC proc, IntPtr user)
        {
            int handle = NativeMethods.BASS_RecordStart(freq, chans, (BASSFlag)Utils.MakeLong((int)flags, period), proc, user);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }
        #endregion
    }
}
