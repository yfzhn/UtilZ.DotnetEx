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
    /// Wav辅助类
    /// </summary>
    public partial class WavHelper
    {
        private static readonly Dictionary<int, string> _pluginHandleFilePathDic = new Dictionary<int, string>();
        private static int _outputSampleRate;

        /// <summary>
        /// 获取输出采样率
        /// </summary>
        public static int OutputSampleRate
        {
            get { return _outputSampleRate; }
        }

        static WavHelper()
        {
            //默认加载插件
            LoadPlugins(NativeMethods.DllDir);


            //默认初始化
            int device = 0;
            BASS_DEVICEINFO_INTERNAL info = new BASS_DEVICEINFO_INTERNAL();
            for (int i = 0; ; i++)
            {
                if (!GetDeviceInfo(i, ref info))
                {
                    break;
                }

                if (info.driver != IntPtr.Zero &&
                    info.flags == (BASSDeviceInfo.BASS_DEVICE_DEFAULT | BASSDeviceInfo.BASS_DEVICE_ENABLED | BASSDeviceInfo.BASS_DEVICE_TYPE_SPEAKERS))
                {
                    device = -1;
                    break;
                }
            }

            Init(device, 48000, BASSInit.BASS_DEVICE_DEFAULT);
        }


        #region Initialization
        #region Plugin
        public static void LoadPlugins(string bassDllDir)
        {
            FreePlugin();

            if (!Directory.Exists(bassDllDir))
            {
                try
                {
                    //尝试创建目录
                    Directory.CreateDirectory(bassDllDir);
                }
                catch
                { }

                return;
            }

            string[] pluginFiles = Directory.GetFiles(bassDllDir);
            if (pluginFiles == null || pluginFiles.Length == 0)
            {
                return;
            }

            //遍历并加载插件
            foreach (string dllFilePath in pluginFiles)
            {
                try
                {
                    if (string.Equals(Path.GetFileName(dllFilePath), NativeMethods.BASS_DLL))
                    {
                        //忽略bass.dll
                        continue;
                    }

                    LoadPlugin(dllFilePath);
                }
                catch (Exception ex)
                {
                    WavLoger.OnRaiseLog(nameof(LoadPlugin), $"加载插件{dllFilePath}失败", ex);
                }
            }
        }

        public static void LoadPlugin(string dllFilePath)
        {
            try
            {
                if (string.Equals(Path.GetFileName(dllFilePath), NativeMethods.BASS_DLL))
                {
                    //忽略bass.dll
                    return;
                }

                FreePlugin(dllFilePath);//释放已加载的同名文件
                int handle = NativeMethods.BASS_PluginLoad(dllFilePath, BASSFlag.BASS_UNICODE);
                //If successful, the loaded plugin's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code. 
                if (handle == 0)
                {
                    int errCode = NativeMethods.BASS_ErrorGetCode();
                    if (errCode == BassErrorCode.BASS_ERROR_FILEFORM || errCode == BassErrorCode.BASS_ERROR_FILEOPEN)
                    {
                        //非bass插件
                        return;
                    }

                    throw new WavException(BassErrorCode.GetErrorInfo());
                }
                _pluginHandleFilePathDic.Add(handle, dllFilePath);
            }
            catch (Exception ex)
            {
                WavLoger.OnRaiseLog(nameof(LoadPlugin), $"加载插件{dllFilePath}失败", ex);
            }
        }



        public static void FreePlugin()
        {
            if (_pluginHandleFilePathDic == null || _pluginHandleFilePathDic.Count == 0)
            {
                return;
            }

            //这种方式一次性释放完,但是那个出错了就不知道了,所以还是用后面这种释放吧
            //bool result = NativeMethods.BASS_PluginFree(0);
            //if (result)
            //{
            //    _pluginHandleFilePathDic.Clear();
            //}
            //else
            //{
            //    throw new WavException(BassErrorCode.GetErrorInfo());
            //}

            int[] pluginHandleArr = _pluginHandleFilePathDic.Keys.ToArray();
            foreach (var pluginHandle in pluginHandleArr)
            {
                try
                {
                    FreePlugin(pluginHandle);
                }
                catch (Exception ex)
                {
                    WavLoger.OnRaiseLog(nameof(FreePlugin), $"释放插件{_pluginHandleFilePathDic[pluginHandle]}失败", ex);
                }
            }
        }

        /// <summary>
        /// 释放插件
        /// </summary>
        /// <param name="pluginHandle">插件句柄</param>
        public static void FreePlugin(int pluginHandle)
        {
            if (!_pluginHandleFilePathDic.ContainsKey(pluginHandle))
            {
                return;
            }

            NativeMethods.BASS_PluginFree(pluginHandle);
            _pluginHandleFilePathDic.Remove(pluginHandle);
        }

        /// <summary>
        /// 释放插件
        /// </summary>
        /// <param name="pluginFilePath">插件谇路径</param>
        public static void FreePlugin(string pluginFilePath)
        {
            if (string.IsNullOrWhiteSpace(pluginFilePath))
            {
                throw new ArgumentNullException(nameof(pluginFilePath));
            }

            foreach (var pluginHandleFilePathKV in _pluginHandleFilePathDic)
            {
                if (string.Equals(pluginHandleFilePathKV.Value, pluginFilePath))
                {
                    FreePlugin(pluginHandleFilePathKV.Key);
                    break;
                }
            }
        }
        #endregion


        /// <summary>
        /// 版本验证
        /// </summary>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned</returns>
        public static bool VersionValidate()
        {
            int version = version = NativeMethods.BASS_GetVersion();
            //bass版本:33819649
            //当前是基于bass的33820416版本开发
            return version == 33820416;
        }

        /// <summary>
        /// 获取设备数
        /// </summary>
        /// <returns>设备数</returns>
        public static int GetDeviceCount()
        {
            int deviceCount = 0;
            BASS_DEVICEINFO_INTERNAL info = new BASS_DEVICEINFO_INTERNAL();
            for (int i = 0; ; i++)
            {
                if (WavHelper.GetDeviceInfo(i, ref info))
                {
                    deviceCount++;
                }
                else
                {
                    break;
                }
            }

            return deviceCount;
        }

        /// <summary>
        /// Retrieves information on an output device
        /// </summary>
        /// <param name="device">The device to get the information of... 0 = first. </param>
        /// <param name="info">Pointer to a structure to receive the information</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static bool GetDeviceInfo(int device, ref BASS_DEVICEINFO_INTERNAL info)
        {
            return NativeMethods.BASS_GetDeviceInfo(device, ref info);
        }

        /// <summary>
        /// Sets the device to use for subsequent calls in the current thread
        /// </summary>
        /// <param name="device">The device to use... 0 = no sound, 1 = first real output device</param>
        public static void SetDevice(int device)
        {
            bool result = NativeMethods.BASS_SetDevice(device);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Retrieves the device setting of the current thread. 
        /// If successful,the device number is returned, else -1
        /// </summary>
        /// <returns></returns>
        public static int GetDevice()
        {
            return NativeMethods.BASS_GetDevice();
        }


        /// <summary>
        /// Initializes an output device
        /// </summary>
        /// <param name="device">The device to use... -1 = default device, 0 = no sound, 1 = first real output device. BASS_GetDeviceInfo can be used to enumerate the available devices. </param>
        /// <param name="freq">Output sample rate</param>
        /// <param name="flags">A combination of these flags</param>
        /// <param name="win">The application's main window... 0 = the desktop window (use this for console applications). This is only needed when using DirectSound output</param>
        /// <param name="clsid">Class identifier of the object to create, that will be used to initialize DirectSound... NULL = use default. </param>
        /// <returns>If the device was successfully initialized, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        public static void Init(int device, int freq, BASSInit flags, IntPtr win, IntPtr clsid)
        {
            FreeDevice();
            bool result = NativeMethods.BASS_Init(device, freq, flags, win, clsid);
            if (result)
            {
                _outputSampleRate = freq;
            }
            else
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Initializes an output device
        /// </summary>
        /// <param name="device">The device to use... -1 = default device, 0 = no sound, 1 = first real output device. BASS_GetDeviceInfo can be used to enumerate the available devices. </param>
        /// <param name="freq">Output sample rate</param>
        /// <param name="flags">A combination of these flags</param>
        /// <param name="win">The application's main window... 0 = the desktop window (use this for console applications). This is only needed when using DirectSound output</param>
        public static void Init(int device, int freq, BASSInit flags, IntPtr win)
        {
            Init(device, freq, flags, win, IntPtr.Zero);
        }

        /// <summary>
        /// Initializes an output device
        /// </summary>
        /// <param name="device">The device to use... -1 = default device, 0 = no sound, 1 = first real output device. BASS_GetDeviceInfo can be used to enumerate the available devices. </param>
        /// <param name="freq">Output sample rate</param>
        /// <param name="flags">A combination of these flags</param>
        public static void Init(int device, int freq, BASSInit flags = BASSInit.BASS_DEVICE_DEFAULT)
        {
            Init(device, freq, flags, IntPtr.Zero, IntPtr.Zero);
        }



        #region 输出以及系统音量
        /// <summary>
        /// Checks if the output has been started
        /// </summary>
        /// <returns>If the device has been started, then TRUE is returned, else FALSE is returned.</returns>
        public static bool OutputStarted()
        {
            return NativeMethods.BASS_IsStarted();
        }

        /// <summary>
        /// Starts (or resumes) the output
        /// </summary>
        public static void StartOutput()
        {
            bool result = NativeMethods.BASS_Start();
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Pause all musics/samples/streams output. 
        /// </summary>
        public static void PauseOutput()
        {
            bool result = NativeMethods.BASS_Pause();
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Stops all musics/samples/streams output.
        /// </summary>
        public static void StopOutput()
        {
            bool result = NativeMethods.BASS_Stop();
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Retrieves the current master volume level.(操作系统音量)
        /// </summary>
        /// <returns>volume The volume level... 0 (silent) to 1 (max).</returns>
        public static float GetVolume()
        {
            float result = NativeMethods.BASS_GetVolume();
            //If successful, the volume level is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. 
            if (result == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

        /// <summary>
        /// Sets the output master volume.(操作系统音量)
        /// </summary>
        /// <param name="volume">volume The volume level... 0 (silent) to 1 (max).</param>
        public static void SetVolume(float volume)
        {
            bool result = NativeMethods.BASS_SetVolume(volume);
            //If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. 
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }
        #endregion



        /// <summary>
        /// Updates the HSTREAM and HMUSIC channel playback buffers
        /// </summary>
        /// <param name="length">The amount of data to render, in milliseconds</param>
        public static void Update(int length)
        {
            bool result = NativeMethods.BASS_Update(length);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }


        /// <summary>
        /// Frees all resources used by the output device, including all its samples, streams and MOD musics
        /// </summary>
        public static void Free()
        {
            FreePlugin();
            FreeDevice();
        }

        private static void FreeDevice()
        {
            FreePlugin();

            if (!NativeMethods.BASS_Free())
            {
                int errCode = NativeMethods.BASS_ErrorGetCode();
                if (errCode != BassErrorCode.BASS_ERROR_INIT)
                {
                    throw new WavException(BassErrorCode.GetErrorInfo(errCode));
                }
            }
        }
        #endregion
    }
}
