using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav.ExBass
{
    /// <summary>
    /// x86位外部库
    /// </summary>
    internal class BassX86
    {
        #region init
        /// <summary>
        /// Retrieves the version of BASS that is loaded
        /// </summary>
        /// <returns>The BASS version. For example, 0x02040103 (hex), would be version 2.4.1.3</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_GetVersion();

        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <returns></returns>
        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ErrorGetCode();

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll", EntryPoint = "BASS_GetDeviceInfo")]
        public static extern bool BASS_GetDeviceInfo([In] int device, [In, Out] ref BASS_DEVICEINFO_INTERNAL info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="freq"></param>
        /// <param name="flags"></param>
        /// <param name="win"></param>
        /// <param name="clsid"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, IntPtr clsid);

        /// <summary>
        /// 设置选项值
        /// </summary>
        /// <param name="option"></param>
        /// <param name="newvalue"></param>
        /// <returns></returns>
        [DllImport(@"x86\bass.dll", CharSet = CharSet.Unicode)]
        public static extern bool BASS_SetConfig(BassConfigOption option, int newvalue);

        /// <summary>
        /// 获取选项值
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        [DllImportAttribute(@"x86\bass.dll", CharSet = CharSet.Unicode)]
        public static extern int BASS_GetConfig(BassConfigOption option);

        /// <summary>
        /// Retrieves the current master volume level[If successful, the volume level is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. ]
        /// </summary>
        /// <returns>If successful, the volume level is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern float BASS_GetVolume();

        /// <summary>
        /// Sets the output master volume[If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.]
        /// </summary>
        /// <param name="volume">volume The volume level... 0 (silent) to 1 (max).  </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_SetVolume(float volume);
        #endregion

        #region Plugins
        /// <summary>
        /// Plugs an "add-on" into the standard stream and sample creation functions
        /// </summary>
        /// <param name="file">Filename of the add-on/plugin</param>
        /// <param name="flags">A combination of these flags.BASS_UNICODE file is in UTF-16 form. Otherwise it is ANSI on Windows or Windows CE, and UTF-8 on other platforms </param>
        /// <returns>If successful, the loaded plugin's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code  </returns>
        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_PluginLoad([In, MarshalAs(UnmanagedType.LPWStr)] string file, BASSFileFlag flags);

        /// <summary>
        /// Retrieves information on a plugin
        /// </summary>
        /// <param name="handle">The plugin handle</param>
        /// <returns>If successful, a pointer to the plugin info is returned, else NULL is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern IntPtr BASS_PluginGetInfo(int handle);

        /// <summary>
        /// Unplugs an add-on.
        /// </summary>
        /// <param name="handle">The plugin handle... 0 = all plugins</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_PluginFree(int handle);
        #endregion

        #region channel
        /// <summary>
        /// 音乐播放
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="restart"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelPlay(int handle, [MarshalAs(UnmanagedType.Bool)] bool restart);

        /// <summary>
        /// 音乐暂停
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelPause(int handle);

        /// <summary>
        /// 音乐停止Stops a sample, stream, MOD music, or recording. 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelStop(int handle);

        /// <summary>
        /// Sets the playback position of a sample, MOD music, or stream
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="pos">The position, in units determined by the mode</param>
        /// <param name="mode">How to set the position. One of the following, with optional flags</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelSetPosition(int handle, long pos, BASSMode mode);

        /// <summary>
        /// Translates a byte position into time (seconds), based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else a negative value is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern double BASS_ChannelBytes2Seconds(int handle, long pos);

        /// <summary>
        /// Translates a time (seconds) position into bytes, based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);

        /// <summary>
        /// 获取当前播放状态
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>

        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelIsActive(int handle);

        /// <summary>
        /// 获取当前播放位置
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [DllImport(@"x86\bass.dll")]
        public static extern long BASS_ChannelGetPosition(int handle, BASSMode mode);

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
        [DllImport(@"x86\bass.dll")]
        public static extern long BASS_ChannelGetLength(int handle, BASSMode mode);

        /// <summary>
        /// Retrieves information on a channel
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="info">Pointer to structure to receive the channel information</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll", EntryPoint = "BASS_ChannelGetInfo")]
        public static extern bool BASS_ChannelGetInfo(int handle, [In, Out] ref BASS_CHANNELINFO_INTERNAL info);

        #region BASS_ChannelGetData
        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelGetData(int handle, IntPtr buffer, int length);

        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelGetData(int handle, [In, Out] byte[] buffer, int length);

        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelGetData(int handle, [In, Out] short[] buffer, uint length);

        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelGetData(int handle, [In, Out] int[] buffer, int length);

        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelGetData(int handle, [In, Out] float[] buffer, int length);
        #endregion

        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelGetAttribute(int handle, BASSAttribute attrib, ref float value);

        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="size">The size of the attribute data... 0 = get the size of the attribute without getting the data. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(@"x86\bass.dll")]
        public static extern int BASS_ChannelGetAttributeEx(int handle, BASSAttribute attrib, IntPtr value, int size);

        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelSetAttribute(int handle, BASSAttribute attrib, float value);

        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following.
        /// BASS_ATTRIB_SCANINFO Scanned info. (HSTREAM only)  
        /// other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute data</param>
        /// <param name="size">The size of the attribute data</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelSetAttributeEx(int handle, BASSAttribute attrib, IntPtr value, int size);

        /// <summary>
        /// Slides a channel's attribute from its current value to a new value
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM, HMUSIC, or HRECORD</param>
        /// <param name="attrib">The attribute to slide the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="time">The length of time (in milliseconds) that it should take for the attribute to reach the value</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_ChannelSlideAttribute(int handle, BASSAttribute attrib, float value, int time);
        #endregion

        #region stream
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mem">true:流;false:文件</param>
        /// <param name="file"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(@"x86\bass.dll", EntryPoint = "BASS_StreamCreateFile")]
        public static extern int BASS_StreamCreateFile([MarshalAs(UnmanagedType.Bool)] bool mem, [In, MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFileFlag flags);

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file on the internet, optionally receiving the downloaded data in a callback function
        /// </summary>
        /// <param name="url"></param>
        /// <param name="offset"></param>
        /// <param name="flags"></param>
        /// <param name="proc"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [DllImport(@"x86\bass.dll", EntryPoint = "BASS_StreamCreateURL")]
        public static extern int BASS_StreamCreateURL([In, MarshalAs(UnmanagedType.LPWStr)] string url, int offset, BASSUrlFlag flags, DOWNLOADPROC proc, IntPtr user);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(@"x86\bass.dll")]
        public static extern bool BASS_StreamFree(int handle);
        #endregion
    }
}
