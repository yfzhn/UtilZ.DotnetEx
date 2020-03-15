using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav.ExBass
{
    /// <summary>
    /// Bass库
    /// </summary>
    public class Bass
    {
        #region init
        /// <summary>
        /// bass.dll版本验证
        /// </summary>
        public static bool BASS_VersionValidate()
        {
            int version = -1;
            if (Environment.Is64BitProcess)
            {
                version = BassX64.BASS_GetVersion();
            }
            else
            {
                version = BassX86.BASS_GetVersion();
            }

            //bass版本:33819649
            //当前是基于bass的33820416版本开发
            return version == 33820416;
        }

        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <returns></returns>
        public static int BASS_ErrorGetCode()
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ErrorGetCode();
            }
            else
            {
                return BassX86.BASS_ErrorGetCode();
            }
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="device"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool BASS_GetDeviceInfo(int device, ref BASS_DEVICEINFO_INTERNAL info)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_GetDeviceInfo(device, ref info);
            }
            else
            {
                return BassX86.BASS_GetDeviceInfo(device, ref info);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq"></param>
        /// <param name="flags"></param>
        /// <param name="win"></param>
        /// <param name="clsid"></param>
        /// <returns></returns>
        public static bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, IntPtr clsid)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_Init(device, freq, flags, win, clsid);
            }
            else
            {
                return BassX86.BASS_Init(device, freq, flags, win, clsid);
            }
        }

        /// <summary>
        /// 设置选项值
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool BASS_SetConfig(BassConfigOption option, int value)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_SetConfig(option, value);
            }
            else
            {
                return BassX86.BASS_SetConfig(option, value);
            }
        }

        /// <summary>
        /// 获取选项值
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static int BASS_GetConfig(BassConfigOption option)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_GetConfig(option);
            }
            else
            {
                return BassX86.BASS_GetConfig(option);
            }
        }

        /// <summary>
        /// Retrieves the current master volume level[If successful, the volume level is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. ]
        /// </summary>
        /// <returns>If successful, the volume level is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>        
        public static float BASS_GetVolume()
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_GetVolume();
            }
            else
            {
                return BassX86.BASS_GetVolume();
            }
        }

        /// <summary>
        /// Sets the output master volume[If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.]
        /// </summary>
        /// <param name="volume">volume The volume level... 0 (silent) to 1 (max).  </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        public static bool BASS_SetVolume(float volume)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_SetVolume(volume);
            }
            else
            {
                return BassX86.BASS_SetVolume(volume);
            }
        }
        #endregion

        #region Plugins
        /// <summary>
        /// Plugs an "add-on" into the standard stream and sample creation functions
        /// </summary>
        /// <param name="file">Filename of the add-on/plugin</param>
        /// <returns>the loaded plugin's handle is returned</returns>
        public static int BASS_PluginLoad(string file)
        {
            int hplugin;
            if (Environment.Is64BitProcess)
            {
                hplugin = BassX64.BASS_PluginLoad(file, BASSFileFlag.BASS_UNICODE);
            }
            else
            {
                hplugin = BassX86.BASS_PluginLoad(file, BASSFileFlag.BASS_UNICODE);
            }

            if (hplugin == 0)
            {
                throw new ApplicationException("BASS_PluginLoad Fail," + BassErrorCode.GetErrorInfo());
            }

            return hplugin;
        }

        /// <summary>
        /// Plugs an "add-on" into the standard stream and sample creation functions
        /// </summary>
        /// <returns>the loaded plugin's handles is returned</returns>
        public static List<int> BASS_PluginLoad()
        {
            //拼接插件默认目录
            string assemblyDir = Path.GetDirectoryName(typeof(Bass).Assembly.Location);
            string pluginDir;
            if (Environment.Is64BitProcess)
            {
                pluginDir = Path.Combine(assemblyDir, @"x64\BassDecoders");
            }
            else
            {
                pluginDir = Path.Combine(assemblyDir, @"x86\BassDecoders");
            }

            List<int> pluginHandles = new List<int>();
            if (!Directory.Exists(pluginDir))
            {
                try
                {
                    //尝试创建目录
                    Directory.CreateDirectory(pluginDir);
                }
                catch
                { }

                return pluginHandles;
            }

            string[] pluginFiles = Directory.GetFiles(pluginDir);
            if (pluginFiles == null || pluginFiles.Length == 0)
            {
                return pluginHandles;
            }

            //遍历并加载插件
            foreach (string pluginFile in pluginFiles)
            {
                try
                {
                    pluginHandles.Add(Bass.BASS_PluginLoad(pluginFile));
                }
                catch
                { }
            }

            return pluginHandles;
        }

        /// <summary>
        /// Retrieves information on a plugin
        /// </summary>
        /// <param name="handle">The plugin handle</param>
        /// <returns>插件信息</returns>
        public static BASS_PLUGINFORM BASS_PluginGetInfo(int handle)
        {
            IntPtr ptr;
            if (Environment.Is64BitProcess)
            {
                ptr = BassX64.BASS_PluginGetInfo(handle);
            }
            else
            {
                ptr = BassX86.BASS_PluginGetInfo(handle);
            }

            if (ptr == IntPtr.Zero)
            {
                throw new ApplicationException("BASS_PluginGetInfo Fail," + BassErrorCode.GetErrorInfo());
            }

            return (BASS_PLUGINFORM)Marshal.PtrToStructure(ptr, typeof(BASS_PLUGININFO));
        }

        /// <summary>
        /// Unplugs an add-on.
        /// </summary>
        /// <param name="handle">The plugin handle... 0 = all plugins</param>
        public static void BASS_PluginFree(int handle)
        {
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_PluginFree(handle);
            }
            else
            {
                result = BassX86.BASS_PluginFree(handle);
            }

            if (!result)
            {
                throw new ApplicationException("BASS_PluginGetInfo Fail," + BassErrorCode.GetErrorInfo());
            }
        }
        #endregion

        #region channel
        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="restart"></param>
        /// <returns></returns>
        public static bool BASS_ChannelPlay(int handle, bool restart)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelPlay(handle, restart);
            }
            else
            {
                return BassX86.BASS_ChannelPlay(handle, restart);
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool BASS_ChannelPause(int handle)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelPause(handle);
            }
            else
            {
                return BassX86.BASS_ChannelPause(handle);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool BASS_ChannelStop(int handle)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelStop(handle);
            }
            else
            {
                return BassX86.BASS_ChannelStop(handle);
            }
        }

        /// <summary>
        /// Sets the playback position of a sample, MOD music, or stream
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="pos">The position, in units determined by the mode</param>
        /// <param name="mode">How to set the position. One of the following, with optional flags</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        public static bool BASS_ChannelSetPosition(int handle, long pos, BASSMode mode)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelSetPosition(handle, pos, mode);
            }
            else
            {
                return BassX86.BASS_ChannelSetPosition(handle, pos, mode);
            }
        }

        /// <summary>
        /// Translates a byte position into time (seconds), based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else a negative value is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static double BASS_ChannelBytes2Seconds(int handle, long pos)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelBytes2Seconds(handle, pos);
            }
            else
            {
                return BassX86.BASS_ChannelBytes2Seconds(handle, pos);
            }
        }

        /// <summary>
        /// Translates a time (seconds) position into bytes, based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static long BASS_ChannelSeconds2Bytes(int handle, double pos)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelSeconds2Bytes(handle, pos);
            }
            else
            {
                return BassX86.BASS_ChannelSeconds2Bytes(handle, pos);
            }
        }

        /// <summary>
        /// 获取当前播放状态
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static int BASS_ChannelIsActive(int handle)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelIsActive(handle);
            }
            else
            {
                return BassX86.BASS_ChannelIsActive(handle);
            }
        }

        /// <summary>
        /// 获取当前播放位置
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static long BASS_ChannelGetPosition(int handle, BASSMode mode)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetPosition(handle, mode);
            }
            else
            {
                return BassX86.BASS_ChannelGetPosition(handle, mode);
            }
        }

        /// <summary>
        /// 获取当前音乐持续时间,单位/秒
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM. HSAMPLE handles may also be used</param>
        /// <param name="mode">
        /// How to retrieve the length. One of the following.
        /// BASS_POS_BYTE Get the length in bytes.  
        /// BASS_POS_MUSIC_ORDER Get the length in orders. (HMUSIC only)  
        /// BASS_POS_OGG Get the number of bitstreams in an OGG file.  
        /// other modes may be supported by add-ons, see the documentation
        /// </param>
        /// <returns>If successful, then the channel's length is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        public static long BASS_ChannelGetLength(int handle, BASSMode mode)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetLength(handle, mode);
            }
            else
            {
                return BassX86.BASS_ChannelGetLength(handle, mode);
            }
        }

        /// <summary>
        /// Retrieves information on a channel
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static BASS_CHANNELINFO_INTERNAL BASS_ChannelGetInfo(int handle)
        {
            BASS_CHANNELINFO_INTERNAL info = new BASS_CHANNELINFO_INTERNAL();
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_ChannelGetInfo(handle, ref info);
            }
            else
            {
                result = BassX86.BASS_ChannelGetInfo(handle, ref info);
            }

            if (result)
            {
                return info;
            }
            else
            {
                throw new ApplicationException("BASS_ChannelGetInfo Fail," + BassErrorCode.GetErrorInfo());
            }
        }

        #region BASS_ChannelGetData
        /// <summary>
        /// 获取通道数据
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="buffer">缓存数组</param>
        /// <param name="length">长度</param>
        /// <returns>实际长度</returns>
        public static int BASS_ChannelGetData(int handle, IntPtr buffer, int length)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetData(handle, buffer, length);
            }
            else
            {
                return BassX86.BASS_ChannelGetData(handle, buffer, length);
            }
        }

        /// <summary>
        /// 获取通道数据
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="buffer">缓存数组</param>
        /// <param name="length">长度</param>
        /// <returns>实际长度</returns>
        public static int BASS_ChannelGetData(int handle, byte[] buffer, int length)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetData(handle, buffer, length);
            }
            else
            {
                return BassX86.BASS_ChannelGetData(handle, buffer, length);
            }
        }

        /// <summary>
        /// 获取通道数据
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="buffer">缓存数组</param>
        /// <param name="length">长度</param>
        /// <returns>实际长度</returns>
        public static int BASS_ChannelGetData(int handle, short[] buffer, uint length)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetData(handle, buffer, length);
            }
            else
            {
                return BassX86.BASS_ChannelGetData(handle, buffer, length);
            }
        }

        /// <summary>
        /// 获取通道数据
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="buffer">缓存数组</param>
        /// <param name="length">长度</param>
        /// <returns>实际长度</returns>
        public static int BASS_ChannelGetData(int handle, int[] buffer, int length)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetData(handle, buffer, length);
            }
            else
            {
                return BassX86.BASS_ChannelGetData(handle, buffer, length);
            }
        }

        /// <summary>
        /// 获取通道数据
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="buffer">缓存数组</param>
        /// <param name="length">长度</param>
        /// <returns>实际长度</returns>
        public static int BASS_ChannelGetData(int handle, float[] buffer, int length)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_ChannelGetData(handle, buffer, length);
            }
            else
            {
                return BassX86.BASS_ChannelGetData(handle, buffer, length);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        public static float BASS_ChannelGetAttribute(int handle, BASSAttribute attrib)
        {
            float value = 0;
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_ChannelGetAttribute(handle, attrib, ref value);
            }
            else
            {
                result = BassX86.BASS_ChannelGetAttribute(handle, attrib, ref value);
            }

            if (!result)
            {
                throw new ApplicationException("BASS_ChannelGetAttribute Fail," + BassErrorCode.GetErrorInfo());
            }

            return value;
        }

        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="size">The size of the attribute data... 0 = get the size of the attribute without getting the data. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int BASS_ChannelGetAttributeEx(int handle, BASSAttribute attrib, IntPtr value, int size)
        {
            int result;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_ChannelGetAttributeEx(handle, attrib, value, size);
            }
            else
            {
                result = BassX86.BASS_ChannelGetAttributeEx(handle, attrib, value, size);
            }

            return result;
        }

        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        public static void BASS_ChannelSetAttribute(int handle, BASSAttribute attrib, float value)
        {
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_ChannelSetAttribute(handle, attrib, value);
            }
            else
            {
                result = BassX86.BASS_ChannelSetAttribute(handle, attrib, value);
            }

            if (!result)
            {
                throw new ApplicationException("BASS_ChannelSetAttribute Fail," + BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following.
        /// BASS_ATTRIB_SCANINFO Scanned info. (HSTREAM only)  
        /// other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute data</param>
        /// <param name="size">The size of the attribute data</param>
        public static void BASS_ChannelSetAttributeEx(int handle, BASSAttribute attrib, IntPtr value, int size)
        {
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_ChannelSetAttributeEx(handle, attrib, value, size);
            }
            else
            {
                result = BassX86.BASS_ChannelSetAttributeEx(handle, attrib, value, size);
            }

            if (!result)
            {
                throw new ApplicationException("BASS_ChannelSetAttributeEx Fail," + BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Slides a channel's attribute from its current value to a new value
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM, HMUSIC, or HRECORD</param>
        /// <param name="attrib">The attribute to slide the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="duration">The length of time (in milliseconds) that it should take for the attribute to reach the value</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static void BASS_ChannelSlideAttribute(int handle, BASSAttribute attrib, float value, int duration)
        {
            bool result = false;
            if (Environment.Is64BitProcess)
            {
                result = BassX64.BASS_ChannelSlideAttribute(handle, attrib, value, duration);
            }
            else
            {
                result = BassX86.BASS_ChannelSlideAttribute(handle, attrib, value, duration);
            }

            if (!result)
            {
                throw new ApplicationException("BASS_ChannelSlideAttribute Fail," + BassErrorCode.GetErrorInfo());
            }
        }
        #endregion

        #region stream
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="mem">true:流;false:文件</param>
        /// <param name="file">文件路径</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="flags"></param>
        public static int BASS_StreamCreateFile(bool mem, string file, long offset, long length, BASSFileFlag flags)
        {
            int handle;
            if (Environment.Is64BitProcess)
            {
                handle = BassX64.BASS_StreamCreateFile(mem, file, offset, length, flags);
            }
            else
            {
                handle = BassX86.BASS_StreamCreateFile(mem, file, offset, length, flags);
            }

            int errCode = Bass.BASS_ErrorGetCode();
            if (errCode != BassErrorCode.BASS_OK)
            {
                throw new Exception(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file on the internet, optionally receiving the downloaded data in a callback function
        /// </summary>
        /// <param name="url">URL of the file to stream. Should begin with "http://" or "https://" or "ftp://", or another add-on supported protocol. The URL can be followed by custom HTTP request headers to be sent to the server; the URL and each header should be terminated with a carriage return and line feed ("\r\n"). </param>
        /// <param name="offset">File position to start streaming from. This is ignored by some servers, specifically when the length is unknown/undefined</param>
        /// <param name="flags">A combination of these flags</param>
        /// <param name="proc">Callback function to receive the file as it is downloaded... NULL = no callback</param>
        /// <param name="user">User instance data to pass to the callback function</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int BASS_StreamCreateURL(string url, int offset, BASSUrlFlag flags, DOWNLOADPROC proc, IntPtr user)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_StreamCreateURL(url, offset, flags, proc, user);
            }
            else
            {
                return BassX86.BASS_StreamCreateURL(url, offset, flags, proc, user);
            }
        }

        /// <summary>
        /// 释放文件资源
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool BASS_StreamFree(int handle)
        {
            if (Environment.Is64BitProcess)
            {
                return BassX64.BASS_StreamFree(handle);
            }
            else
            {
                return BassX86.BASS_StreamFree(handle);
            }
        }
        #endregion
    }
}
