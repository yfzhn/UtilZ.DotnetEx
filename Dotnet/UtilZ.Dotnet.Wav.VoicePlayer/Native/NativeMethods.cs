using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    internal class NativeMethods
    {
        internal const string BASS_DLL = @"bass.dll";
        internal const string BASS_MIX_DLL = @"bassmix.dll";
        private static string _dllDir;
        public static string DllDir
        {
            get { return _dllDir; }
        }

        static NativeMethods()
        {
            string assemblyDir = Path.GetDirectoryName(typeof(NativeMethods).Assembly.Location);
            if (Environment.Is64BitProcess)
            {
                _dllDir = Path.Combine(assemblyDir, "x64");
            }
            else
            {
                _dllDir = Path.Combine(assemblyDir, "x86");
            }

            //加载bass.dll
            string dllPath = Path.Combine(_dllDir, BASS_DLL);
            LoadLibraryEx(dllPath);

            dllPath = Path.Combine(_dllDir, BASS_MIX_DLL);
            LoadLibraryEx(dllPath);
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/ms684179(v=vs.85).aspx
        /// <summary>
        /// 加载C++ dll
        /// </summary>
        /// <param name="dllPath">dll路径</param>
        /// <param name="hFile">This parameter is reserved for future use. It must be NULL.</param>
        /// <param name="dwFlags">The action to be taken when loading the module. If no flags are specified, the behavior of this function is identical to that of the LoadLibrary function. This parameter can be one of the following values.</param>
        /// <returns>库句柄</returns>
        [DllImport("Kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryEx(string dllPath, int hFile = 0, int dwFlags = 0x00000008);


        #region basic
        /// <summary>
        /// Retrieves the version of BASS that is loaded
        /// </summary>
        /// <returns>The BASS version. For example, 0x02040103 (hex), would be version 2.4.1.3</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_GetVersion")]
        internal static extern int BASS_GetVersion();


        /// <summary>
        /// Retrieves the error code for the most recent BASS function call in the current thread. 
        /// </summary>
        /// <returns>If no error occurred during the last BASS function call then BASS_OK is returned,
        /// else one of the BASS_ERROR values is returned. See the function description for an explanation of what the error code means</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_ErrorGetCode")]
        internal static extern int BASS_ErrorGetCode();

        /// <summary>
        /// Retrieves the current CPU usage of BASS. 
        /// </summary>
        /// <returns></returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_ErrorGetCode")]
        internal static extern float BASS_GetCPU();
        #endregion



        #region Plugins
        /// <summary>
        /// Plugs an "add-on" into the standard stream and sample creation functions. 
        /// </summary>
        /// <param name="file">Filename of the add-on/plugin</param>
        /// <param name="flags">A combination of these flags.BASS_UNICODE file is in UTF-16 form. Otherwise it is ANSI on Windows or Windows CE, and UTF-8 on other platforms </param>
        /// <returns>If successful, the loaded plugin's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_PluginLoad")]
        internal static extern int BASS_PluginLoad([In, MarshalAs(UnmanagedType.LPWStr)] string file, BASSFlag flags);


        /// <summary>
        /// Retrieves information on a plugin
        /// </summary>
        /// <param name="handle">The plugin handle</param>
        /// <returns>If successful, a pointer to the plugin info is returned, else NULL is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_PluginGetInfo")]
        internal static extern IntPtr BASS_PluginGetInfo(int handle);


        /// <summary>
        /// Unplugs an add-on.
        /// </summary>
        /// <param name="handle">The plugin handle... 0 = all plugins</param>
        [DllImport(BASS_DLL, EntryPoint = "BASS_PluginFree")]
        internal static extern bool BASS_PluginFree(int handle);
        #endregion


        #region Initialization
        /// <summary>
        /// Retrieves the device setting of the current thread. 
        /// </summary>
        /// <returns></returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_GetDevice")]
        internal static extern int BASS_GetDevice();

        /// <summary>
        /// Retrieves information on an output device
        /// </summary>
        /// <param name="device">The device to get the information of... 0 = first. </param>
        /// <param name="info">Pointer to a structure to receive the information</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_GetDeviceInfo")]
        internal static extern bool BASS_GetDeviceInfo([In] int device, [In, Out] ref BASS_DEVICEINFO_INTERNAL info);

        /// <summary>
        /// Initializes an output device
        /// </summary>
        /// <param name="device">The device to use... -1 = default device, 0 = no sound, 1 = first real output device. BASS_GetDeviceInfo can be used to enumerate the available devices. </param>
        /// <param name="freq">Output sample rate</param>
        /// <param name="flags">A combination of these flags</param>
        /// <param name="win">The application's main window... 0 = the desktop window (use this for console applications). This is only needed when using DirectSound output</param>
        /// <param name="clsid">Class identifier of the object to create, that will be used to initialize DirectSound... NULL = use default. </param>
        /// <returns>If the device was successfully initialized, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_Init")]
        internal static extern bool BASS_Init(int device, int freq, BASSInit flags, IntPtr win, IntPtr clsid);

        /// <summary>
        /// Sets the device to use for subsequent calls in the current thread
        /// </summary>
        /// <param name="device">The device to use... 0 = no sound, 1 = first real output device</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_SetDevice(int device);

        /// <summary>
        /// Frees all resources used by the output device, including all its samples, streams and MOD musics
        /// </summary>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_Free")]
        internal static extern bool BASS_Free();

        /// <summary>
        /// Checks if the output has been started
        /// </summary>
        /// <returns>If the device has been started, then TRUE is returned, else FALSE is returned.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_IsStarted();

        /// <summary>
        /// Starts (or resumes) the output
        /// </summary>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_Start();

        /// <summary>
        /// Stops the output, pausing all musics/samples/streams on it. 
        /// </summary>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_Pause();

        /// <summary>
        /// Stops the output, stopping all musics/samples/streams on it.
        /// </summary>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_Stop();


        /// <summary>
        /// Retrieves the current master volume level.(操作系统音量)
        /// </summary>
        /// <returns>If successful, the volume level is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_GetVolume")]
        internal static extern float BASS_GetVolume();

        /// <summary>
        /// Sets the output master volume.(操作系统音量)
        /// </summary>
        /// <param name="volume">volume The volume level... 0 (silent) to 1 (max).</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_SetVolume(float volume);



        /// <summary>
        /// Updates the HSTREAM and HMUSIC channel playback buffers
        /// </summary>
        /// <param name="length">The amount of data to render, in milliseconds</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_Update(int length);
        #endregion


        #region Stream
        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file. 
        /// </summary>
        /// <param name="mem">TRUE = stream the file from memory(true:流;false:文件)</param>
        /// <param name="file">Filename (mem = FALSE) or a memory location (mem = TRUE).</param>
        /// <param name="offset">File offset to begin streaming from (only used if mem = FALSE). </param>
        /// <param name="length">Data length... 0 = use all data up to the end of the file (if mem = FALSE)</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_StreamCreateFile")]
        internal static extern int BASS_StreamCreateFileUnicode([MarshalAs(UnmanagedType.Bool)] bool mem, [In, MarshalAs(UnmanagedType.LPWStr)] string file, long offset, long length, BASSFlag flags);

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file. 
        /// </summary>
        /// <param name="mem">TRUE = stream the file from memory(true:流;false:文件)</param>
        /// <param name="file">Filename (mem = FALSE) or a memory location (mem = TRUE).</param>
        /// <param name="offset">File offset to begin streaming from (only used if mem = FALSE). </param>
        /// <param name="length">Data length... 0 = use all data up to the end of the file (if mem = FALSE)</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_StreamCreateFile")]
        internal static extern int BASS_StreamCreateFileMemory([MarshalAs(UnmanagedType.Bool)] bool mem, IntPtr memory, long offset, long length, BASSFlag flags);

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file via user callback functions. 
        /// </summary>
        /// <param name="system">File system to use, one of the following. </param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="procs">The user defined file functions. </param>
        /// <param name="user">User instance data to pass to the callback functions. </param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user);


        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file on the internet, optionally receiving the downloaded data in a callback function. 
        /// </summary>
        /// <param name="url">URL of the file to stream. Should begin with "http://" or "https://" or "ftp://", or another add-on supported protocol. The URL can be followed by custom HTTP request headers to be sent to the server; 
        /// the URL and each header should be terminated with a carriage return and line feed ("\r\n"). </param>
        /// <param name="offset">File position to start streaming from. This is ignored by some servers, specifically when the length is unknown/undefined.</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="proc">Callback function to receive the file as it is downloaded... NULL = no callback. </param>
        /// <param name="user">User instance data to pass to the callback function. </param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_StreamCreateURL", CharSet = CharSet.Ansi)]
        internal static extern int BASS_StreamCreateURLAscii([In, MarshalAs(UnmanagedType.LPStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file on the internet, optionally receiving the downloaded data in a callback function. 
        /// </summary>
        /// <param name="url">URL of the file to stream. Should begin with "http://" or "https://" or "ftp://", or another add-on supported protocol. The URL can be followed by custom HTTP request headers to be sent to the server; 
        /// the URL and each header should be terminated with a carriage return and line feed ("\r\n"). </param>
        /// <param name="offset">File position to start streaming from. This is ignored by some servers, specifically when the length is unknown/undefined.</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="proc">Callback function to receive the file as it is downloaded... NULL = no callback. </param>
        /// <param name="user">User instance data to pass to the callback function. </param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_StreamCreateURL")]
        internal static extern int BASS_StreamCreateURLUnicode([In, MarshalAs(UnmanagedType.LPWStr)] string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user);



        [DllImport(BASS_DLL)]
        internal static extern int BASS_SampleCreate(int length, int freq, int chans, int max, BASSFlag flags);

        /// <summary>
        /// Creates a user sample stream. 
        /// </summary>
        /// <param name="freq">The default sample rate. The sample rate can be changed using BASS_ChannelSetAttribute. </param>
        /// <param name="chans">The number of channels... 1 = mono, 2 = stereo, 4 = quadraphonic, 6 = 5.1, 8 = 7.1. </param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="proc">The user defined stream writing function, or one of the following.
        /// STREAMPROC_DEVICE Create a "dummy" stream for the device's final output mix. This allows DSP/FX to be applied to all channels that are playing on the device,
        /// rather than individual channels. DSP/FX parameter change latency is also reduced because channel playback buffering is avoided. The stream is created with the device's current output sample format;
        /// the freq, chans, and flags parameters are ignored.It will always be floating-point except on platforms/architectures that do not support floating-point (see BASS_CONFIG_FLOAT), where it will be 16-bit instead. 
        /// STREAMPROC_DUMMY Create a "dummy" stream. A dummy stream does not have any sample data of its own, but a decoding dummy stream (with BASS_STREAM_DECODE flag) can be used to apply DSP/FX processing to any sample data, by setting DSP/FX on the stream and feeding the data through BASS_ChannelGetData. The dummy stream should have the same sample format as the data being fed through it.  
        /// STREAMPROC_PUSH Create a "push" stream.Instead of BASS pulling data from a STREAMPROC function, data is pushed to BASS via BASS_StreamPutData.</param>
        /// <param name="user">User instance data to pass to the callback function. Unused when creating a dummy or push stream. </param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamCreate(int freq, int chans, BASSFlag flags, STREAMPROC proc, IntPtr user);

        /// <summary>
        /// Creates a user sample stream. 
        /// </summary>
        /// <param name="freq">The default sample rate. The sample rate can be changed using BASS_ChannelSetAttribute. </param>
        /// <param name="chans">The number of channels... 1 = mono, 2 = stereo, 4 = quadraphonic, 6 = 5.1, 8 = 7.1. </param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="proc">The user defined stream writing function, or one of the following.
        /// STREAMPROC_DEVICE Create a "dummy" stream for the device's final output mix. This allows DSP/FX to be applied to all channels that are playing on the device,
        /// rather than individual channels. DSP/FX parameter change latency is also reduced because channel playback buffering is avoided. The stream is created with the device's current output sample format;
        /// the freq, chans, and flags parameters are ignored.It will always be floating-point except on platforms/architectures that do not support floating-point (see BASS_CONFIG_FLOAT), where it will be 16-bit instead. 
        /// STREAMPROC_DUMMY Create a "dummy" stream. A dummy stream does not have any sample data of its own, but a decoding dummy stream (with BASS_STREAM_DECODE flag) can be used to apply DSP/FX processing to any sample data, by setting DSP/FX on the stream and feeding the data through BASS_ChannelGetData. The dummy stream should have the same sample format as the data being fed through it.  
        /// STREAMPROC_PUSH Create a "push" stream.Instead of BASS pulling data from a STREAMPROC function, data is pushed to BASS via BASS_StreamPutData.</param>
        /// <param name="user">User instance data to pass to the callback function. Unused when creating a dummy or push stream. </param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_StreamCreate")]
        internal static extern int BASS_StreamCreatePtr(int freq, int chans, BASSFlag flags, IntPtr procPtr, IntPtr user);



        /// <summary>
        /// Retrieves the file position/status of a stream. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="mode">The file position/status to retrieve. One of the following</param>
        /// <returns>If successful, then the requested file position/status is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern long BASS_StreamGetFilePosition(int handle, BASSStreamFilePosition mode);

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutData(int handle, IntPtr buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutData(int handle, byte[] buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutData(int handle, short[] buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutData(int handle, int[] buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutData(int handle, float[] buffer, int length);

        /// <summary>
        /// Adds data to a "push buffered" user file stream's buffer. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="buffer">Pointer to the file data.</param>
        /// <param name="length">The amount of data in bytes, or BASS_FILEDATA_END to end the file. </param>
        /// <returns>If successful, the number of bytes read from buffer is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutFileData(int handle, IntPtr buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutFileData(int handle, byte[] buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutFileData(int handle, short[] buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutFileData(int handle, int[] buffer, int length);
        [DllImport(BASS_DLL)]
        internal static extern int BASS_StreamPutFileData(int handle, float[] buffer, int length);

        /// <summary>
        /// Frees a sample stream's resources, including any sync/DSP/FX it has. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_StreamFree")]
        internal static extern bool BASS_StreamFree(int handle);
        #endregion


        #region channel
        /// <summary>
        /// Starts (or resumes) playback of a sample, stream, MOD music, or recording
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="restart">Restart playback from the beginning? If handle is a user stream (created with BASS_StreamCreate),
        /// its current buffer contents are cleared. If it is a MOD music, its BPM/etc are reset to their initial values</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelPlay")]
        internal static extern bool BASS_ChannelPlay(int handle, [MarshalAs(UnmanagedType.Bool)] bool restart);

        /// <summary>
        /// Pauses a sample, stream, MOD music, or recording. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelPause")]
        internal static extern bool BASS_ChannelPause(int handle);

        /// <summary>
        /// Stops a sample, stream, MOD music, or recording. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelStop")]
        internal static extern bool BASS_ChannelStop(int handle);





        /// <summary>
        /// Retrieves information on a channel
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="info">Pointer to structure to receive the channel information</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelGetInfo")]
        internal static extern bool BASS_ChannelGetInfo(int handle, [In, Out] ref BASS_CHANNELINFO_INTERNAL info);

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
        [DllImport(BASS_DLL)]
        internal static extern long BASS_ChannelGetLength(int handle, BASSMode mode);

        /// <summary>
        /// Retrieves the immediate sample data (or an FFT representation of it) of a sample channel, stream, MOD music, or recording channel. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. </param>
        /// <param name="buffer">Pointer to a buffer to receive the data... can be NULL when handle is a recording channel (HRECORD),
        /// to discard the requested amount of data from the recording buffer. </param>
        /// <param name="length">Number of bytes wanted (up to 268435455 or 0xFFFFFFF), and/or the following flags.
        /// BASS_DATA_FLOAT Return floating-point sample data.  
        /// BASS_DATA_FIXED Return 8.24 fixed-point data.
        /// BASS_DATA_FFT256 256 sample FFT(returns 128 values).
        /// BASS_DATA_FFT512 512 sample FFT(returns 256 values).
        /// BASS_DATA_FFT1024 1024 sample FFT(returns 512 values).
        /// BASS_DATA_FFT2048 2048 sample FFT(returns 1024 values).
        /// BASS_DATA_FFT4096 4096 sample FFT(returns 2048 values).
        /// BASS_DATA_FFT8192 8192 sample FFT(returns 4096 values).
        /// BASS_DATA_FFT16384 16384 sample FFT(returns 8192 values).
        /// BASS_DATA_FFT32768 32768 sample FFT(returns 16384 values).
        /// BASS_DATA_FFT_COMPLEX Return the complex FFT result rather than the magnitudes.This increases the amount of data returned(as listed above) fourfold, 
        ///                       as it returns real and imaginary parts and the full FFT result(not only the first half).The real and imaginary parts are interleaved in the returned data.
        /// BASS_DATA_FFT_INDIVIDUAL Perform a separate FFT for each channel, rather than a single combined FFT.The size of the data returned(as listed above) is multiplied by the number of channels.
        /// BASS_DATA_FFT_NOWINDOW Prevent a Hann window being applied to the sample data when performing an FFT.
        /// BASS_DATA_FFT_NYQUIST Return an extra value for the Nyquist frequency magnitude.The Nyquist frequency is always included in a complex FFT result.
        /// BASS_DATA_FFT_REMOVEDC Remove any DC bias from the sample data when performing an FFT.
        /// BASS_DATA_AVAILABLE Query the amount of data the channel has buffered for playback, or from recording.This flag cannot be used with decoding channels as they do not have playback buffers.
        ///                     buffer is ignored when using this flag.</param>
        /// <returns>If an error occurs, -1 is returned, use BASS_ErrorGetCode to get the error code. When requesting FFT data, the number of bytes read from the channel (to perform the FFT) is returned.
        /// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT or BASS_DATA_FIXED flag). 
        /// When using the BASS_DATA_AVAILABLE flag, the number of bytes in the channel's buffer is returned. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelGetData(int handle, IntPtr buffer, int length);

        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelGetData(int handle, [In, Out] byte[] buffer, int length);

        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelGetData(int handle, [In, Out] short[] buffer, int length);

        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelGetData(int handle, [In, Out] int[] buffer, int length);

        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelGetData(int handle, [In, Out] float[] buffer, int length);







        /// <summary>
        /// Retrieves the playback position of a sample, stream, or MOD music. Can also be used with a recording channel
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="mode">How to retrieve the position. One of the following, with optional flags.
        /// BASS_POS_BYTE Get the position in bytes.
        /// BASS_POS_MUSIC_ORDER Get the position in orders and rows... LOWORD = order, HIWORD = row* scaler(BASS_ATTRIB_MUSIC_PSCALER). (HMUSIC only)
        /// BASS_POS_DECODE Flag: Get the decoding/rendering position, which may be ahead of the playback position due to buffering.This flag is unnecessary with decoding channels because the decoding position will always be given for them anyway, as they do not have playback buffers.  
        /// other modes & flags may be supported by add-ons, see the documentation</param>
        /// <returns></returns>
        [DllImport(BASS_DLL)]
        internal static extern long BASS_ChannelGetPosition(int handle, BASSMode mode);

        /// <summary>
        /// Sets the playback position of a sample, MOD music, or stream
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="pos">The position, in units determined by the mode</param>
        /// <param name="mode">How to set the position. One of the following, with optional flags</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_ChannelSetPosition(int handle, long pos, BASSMode mode);

        /// <summary>
        /// Translates a byte position into time (seconds), based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else a negative value is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL)]
        internal static extern double BASS_ChannelBytes2Seconds(int handle, long pos);

        /// <summary>
        /// Translates a time (seconds) position into bytes, based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL)]
        internal static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);







        /// <summary>
        /// Locks a stream, MOD music or recording channel to the current thread.
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC, HSTREAM or HRECORD</param>
        /// <param name="state">If FALSE, unlock the channel, else lock it.</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_ChannelLock(int handle, [MarshalAs(UnmanagedType.Bool)] bool state);

        /// <summary>
        /// Retrieves the device that a channel is using.
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <returns>If successful, the device number is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelGetDevice(int handle);

        /// <summary>
        /// Changes the device that a stream, MOD music or sample is using.
        /// </summary>
        /// <param name="handle">The channel or sample handle... a HMUSIC, HSTREAM or HSAMPLE. </param>
        /// <param name="device">The device to use... 0 = no sound, 1 = first real output device, BASS_NODEVICE = no device</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned and the channel remains on its current device. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_ChannelSetDevice(int handle, int device);

        /// <summary>
        /// Checks if a sample, stream, or MOD music is active (playing) or stalled. Can also check if a recording is in progress
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. </param>
        /// <returns>The return value is one of the following</returns>
        [DllImport(BASS_DLL)]
        internal static extern BASSActive BASS_ChannelIsActive(int handle);

        /// <summary>
        /// Updates the playback buffer of a stream or MOD music
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC or HSTREAM. </param>
        /// <param name="length">The amount of data to render, in milliseconds... 0 = default (2 x update period). This is capped at the space available in the buffer.</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_ChannelUpdate(int handle, int length);




        /// <summary>
        /// Retrieves tags/headers from a channel. 
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC or HSTREAM.</param>
        /// <param name="tags">The tags/headers wanted... one of the following
        /// BASS_TAG_AM_MIME Android media codec MIME type. A single string is returned.  
        /// BASS_TAG_AM_NAME Android media codec name.A single string is returned.This in only available on Android 4.3 and above.
        /// BASS_TAG_APE APE (v1 or v2) tags.A pointer to a series of null-terminated UTF-8 strings is returned, the final string ending with a double null. Each string is in the form of "key=value", or "key=value1/value2/..." if there are multiple values.
        /// BASS_TAG_APE_BINARY APE binary tag.A pointer to a TAG_APE_BINARY structure is returned.
        /// + tag number (0=first) 
        /// BASS_TAG_CA_CODEC CoreAudio codec information.A pointer to a TAG_CA_CODEC structure is returned.
        /// BASS_TAG_HTTP HTTP headers, only available when streaming from a HTTP server. A pointer to a series of null-terminated strings is returned, the final string ending with a double null.  
        /// BASS_TAG_ICY ICY (Shoutcast) tags. A pointer to a series of null-terminated strings is returned, the final string ending with a double null.  
        /// BASS_TAG_ID3 ID3v1 tags.A pointer to a TAG_ID3 structure is returned.
        /// BASS_TAG_ID3V2 ID3v2 tags.A pointer to a variable length block is returned.ID3v2 tags are supported at both the start and end of the file, and in designated RIFF/AIFF chunks. See www.id3.org for details of the block's structure.  
        /// BASS_TAG_LYRICS3 Lyrics3v2 tag.A single string is returned, containing the Lyrics3v2 information. See www.id3.org/Lyrics3v2 for details of its format.  
        /// BASS_TAG_META Shoutcast metadata.A single string is returned, containing the current stream title and url (usually omitted). The format of the string is: StreamTitle= 'xxx'; StreamUrl='xxx';  
        /// BASS_TAG_MF Media Foundation metadata.A pointer to a series of null-terminated UTF-8 strings is returned, the final string ending with a double null.  
        /// BASS_TAG_MP4 MP4/iTunes metadata. A pointer to a series of null-terminated UTF-8 strings is returned, the final string ending with a double null.  
        /// BASS_TAG_MUSIC_AUTH MOD music author. Only available in files created with the OpenMPT tracker.  
        /// BASS_TAG_MUSIC_INST MOD instrument name.Only available with formats that have instruments, eg.IT and XM(and MO3).  
        /// + instrument number (0=first) 
        /// BASS_TAG_MUSIC_MESSAGE MOD message text.
        /// BASS_TAG_MUSIC_NAME MOD music title.
        /// BASS_TAG_MUSIC_ORDERS MOD music order list.A pointer to a byte array is returned, with each byte being the pattern number played at that order position. Pattern number 254 is "+++" (skip order) and 255 is "---" (end song).  
        /// BASS_TAG_MUSIC_SAMPLE MOD sample name.
        /// + sample number (0=first) 
        /// BASS_TAG_OGG OGG comments.A pointer to a series of null-terminated UTF-8 strings is returned, the final string ending with a double null.  
        /// BASS_TAG_RIFF_BEXT RIFF/BWF "bext" chunk tags.A pointer to a TAG_BEXT structure is returned.
        /// BASS_TAG_RIFF_CART RIFF/BWF "cart" chunk tags. A pointer to a TAG_CART structure is returned.
        /// BASS_TAG_RIFF_CUE RIFF "cue " chunk.A pointer to a TAG_CUE structure is returned.
        /// BASS_TAG_RIFF_DISP RIFF "DISP" chunk text (CF_TEXT) tag. A single string is returned.
        /// BASS_TAG_RIFF_INFO RIFF "INFO" chunk tags. A pointer to a series of null-terminated strings is returned, the final string ending with a double null. The tags are in the form of "XXXX=text", where "XXXX" is the chunk ID.
        /// BASS_TAG_RIFF_SMPL RIFF "smpl" chunk.A pointer to a TAG_SMPL structure is returned.
        /// BASS_TAG_VENDOR OGG encoder.A single UTF-8 string is returned.
        /// BASS_TAG_WAVEFORMAT WAVE "fmt " chunk contents. A pointer to a WAVEFORMATEX structure is returned.As well as WAVE files, this is also provided by Media Foundation codecs.  
        /// BASS_TAG_WMA WMA tags.A pointer to a series of null-terminated UTF-8 strings is returned, the final string ending with a double null.  
        /// other tags may be supported by add-ons, see the documentation
        /// </param>
        /// <returns>If successful, the requested tags are returned, else NULL is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL)]
        internal static extern IntPtr BASS_ChannelGetTags(int handle, BASSTag tags);

        /// <summary>
        /// Modifies and retrieves a channel's flags
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM. </param>
        /// <param name="flags">A combination of these flags.
        /// BASS_SAMPLE_LOOP Loop the channel.
        /// BASS_STREAM_AUTOFREE Automatically free the channel when playback ends.Note that the BASS_MUSIC_AUTOFREE flag is identical to this flag. (HSTREAM/HMUSIC only)
        /// BASS_STREAM_RESTRATE Restrict the download rate. (HSTREAM)
        /// BASS_MUSIC_NONINTER Use non-interpolated sample mixing. (HMUSIC)
        /// BASS_MUSIC_SINCINTER Use sinc interpolated sample mixing. (HMUSIC)
        /// BASS_MUSIC_RAMP Use "normal" ramping. (HMUSIC)
        /// BASS_MUSIC_RAMPS Use "sensitive" ramping. (HMUSIC)
        /// BASS_MUSIC_SURROUND Use surround sound. (HMUSIC)
        /// BASS_MUSIC_SURROUND2 Use surround sound mode 2. (HMUSIC)
        /// BASS_MUSIC_FT2MOD Use FastTracker 2 .MOD playback. (HMUSIC)
        /// BASS_MUSIC_PT1MOD Use ProTracker 1 .MOD playback. (HMUSIC)
        /// BASS_MUSIC_POSRESET Stop all notes when seeking. (HMUSIC)
        /// BASS_MUSIC_POSRESETEX Stop all notes and reset BPM/etc when seeking. (HMUSIC)
        /// BASS_MUSIC_STOPBACK Stop when a backward jump effect is played. (HMUSIC)
        /// BASS_SPEAKER_xxx Speaker assignment flags. (HSTREAM/HMUSIC)  
        /// other flags may be supported by add-ons, see the documentation. </param>
        /// <param name="mask">The flags (as above) to modify. Flags that are not included in this are left as they are, so it can be set to 0 in order to just retrieve the current flags. To modify the speaker flags, any of the BASS_SPEAKER_xxx flags can be used in the mask (no need to include all of them). </param>
        /// <returns>If successful, the channel's updated flags are returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelFlags(int handle, BASSFlag flags, BASSFlag mask);

        /// <summary>
        /// Sets up a synchronizer on a MOD music, stream or recording channel. 
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC, HSTREAM or HRECORD. </param>
        /// <param name="type">The type of sync (see the table below). The following flags may also be used.
        /// BASS_SYNC_MIXTIME Call the sync function immediately when the sync is triggered, instead of delaying the call until the sync event is actually heard. This is automatic with some sync types (see table below), and always with decoding and recording channels, as they cannot be played/heard.
        /// BASS_SYNC_ONETIME Call the sync only once and then remove it from the channel.
        /// BASS_SYNC_THREAD Call the sync asynchronously in the dedicated sync thread.This only affects mixtime syncs (except BASS_SYNC_FREE syncs) and allows the callback function to safely call BASS_StreamFree or BASS_MusicFree on the same channel handle.</param>
        /// <param name="param">The sync parameter. Depends on the sync type... see the table below. </param>
        /// <param name="proc">The callback function. </param>
        /// <param name="user">User instance data to pass to the callback function. </param>
        /// <returns>If successful, then the new synchronizer's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user);

        /// <summary>
        /// Removes a synchronizer from a MOD music, stream or recording channel. 
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC, HSTREAM or HRECORD</param>
        /// <param name="sync">Handle of the synchronizer to remove</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_ChannelRemoveSync(int handle, int sync);
        #endregion


        #region Attribute
        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attrib">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelGetAttribute")]
        internal static extern bool BASS_ChannelGetAttribute(int handle, BASSAttribute attrib, ref float value);

        /// <summary>
        /// Retrieves the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="size">The size of the attribute data... 0 = get the size of the attribute without getting the data. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>

        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelGetAttributeEx")]
        internal static extern int BASS_ChannelGetAttributeEx(int handle, BASSAttribute attrib, IntPtr value, int size);

        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to set the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelSetAttribute")]
        internal static extern bool BASS_ChannelSetAttribute(int handle, BASSAttribute attrib, float value);

        /// <summary>
        /// Sets the value of a channel's attribute
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="attribute">The attribute to set the value of... one of the following.
        /// BASS_ATTRIB_SCANINFO Scanned info. (HSTREAM only)  
        /// other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute data</param>
        /// <param name="size">The size of the attribute data</param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelSetAttributeEx")]
        internal static extern bool BASS_ChannelSetAttributeEx(int handle, BASSAttribute attrib, IntPtr value, int size);


        /// <summary>
        /// Slides a channel's attribute from its current value to a new value
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM, HMUSIC, or HRECORD</param>
        /// <param name="attribute">The attribute to slide the value of... one of the following,other attributes may be supported by add-ons, see the documentation</param>
        /// <param name="value">The new attribute value. See the attribute's documentation for details on the possible values. </param>
        /// <param name="time">The length of time (in milliseconds) that it should take for the attribute to reach the value</param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_ChannelSlideAttribute")]
        internal static extern bool BASS_ChannelSlideAttribute(int handle, BASSAttribute attrib, float value, int time);

        /// <summary>
        /// Checks if an attribute (or any attribute) of a sample, stream, or MOD music is sliding
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="attrib">The attribute to check for sliding... one of the following, or 0 for any attribute.
        /// BASS_ATTRIB_EAXMIX EAX wet/dry mix.  
        /// BASS_ATTRIB_FREQ Sample rate.
        /// BASS_ATTRIB_PAN Panning/balance position.
        /// BASS_ATTRIB_VOL Volume level.  
        /// BASS_ATTRIB_MUSIC_AMPLIFY Amplification level. (HMUSIC only)
        /// BASS_ATTRIB_MUSIC_BPM BPM. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_PANSEP Pan separation level. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_PSCALER Position scaler. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_SPEED Speed. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_VOL_CHAN A channel volume level. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_VOL_GLOBAL Global volume level. (HMUSIC)
        /// BASS_ATTRIB_MUSIC_VOL_INST An instrument/sample volume level. (HMUSIC)
        /// other attributes may be supported by add-ons, see the documentation.</param>
        /// <returns>If the attribute is sliding, then TRUE is returned, else FALSE is returned.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_ChannelIsSliding(int handle, BASSAttribute attrib);
        #endregion



        #region Config
        /// <summary>
        /// Retrieves the value of a config option. 
        /// </summary>
        /// <param name="option">The option to get the value of... one of the following.</param>
        /// <returns>If successful, the value of the requested config option is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_GetConfig(BASSConfig option);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_GetConfig")]
        internal static extern bool BASS_GetConfigBool(BASSConfig option);

        /// <summary>
        /// Retrieves the value of a pointer config option
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following</param>
        /// <returns>If successful, the value of the requested config option is returned, else NULL is returned. NULL may also be a valid setting with some config options, in which case the error code should be used to confirm whether it's an error. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern IntPtr BASS_GetConfigPtr(BASSConfig option);







        /// <summary>
        /// Sets the value of a config option. 
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following.</param>
        /// <param name="newvalue">The new option setting. See the option's documentation for details on the possible values. </param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_SetConfig(BASSConfig option, [In, MarshalAs(UnmanagedType.Bool)] bool newvalue);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_SetConfig(BASSConfig option, int newvalue);

        /// <summary>
        /// Sets the value of a pointer config option
        /// </summary>
        /// <param name="option">The option to set the value of... one of the following</param>
        /// <param name="newvalue">The new option setting. See the option's documentation for details on the possible values. </param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_SetConfigPtr(BASSConfig option, IntPtr newvalue);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_SetConfigPtr")]
        internal static extern bool BASS_SetConfigStringUnicode(BASSConfig option, [In, MarshalAs(UnmanagedType.LPWStr)] string newvalue);
        #endregion


        #region Sample
        [DllImport(BASS_DLL, EntryPoint = "BASS_SampleCreate")]
        internal static extern int BASS_SampleCreate(int length, int freq, int chans, int max, int flags);

        [DllImport(BASS_DLL, EntryPoint = "BASS_SampleGetData")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BASS_SampleGetData(int handle, byte[] buffer);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_SampleSetData")]
        internal static extern bool BASS_SampleSetData(int handle, byte[] buffer);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL, EntryPoint = "BASS_SampleFree")]
        internal static extern bool BASS_SampleFree(int handle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_MIX_DLL)]
        internal static extern bool BASS_Mixer_StreamAddChannel(int handle, int channel, BASSFlag flags);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_MIX_DLL)]
        internal static extern bool BASS_Mixer_StreamAddChannelEx(int handle, int channel, BASSFlag flags, long start, long length);
        [DllImport(BASS_MIX_DLL)]
        internal static extern int BASS_Mixer_StreamCreate(int freq, int chans, BASSFlag flags);

     
        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("bass")]
        //public static extern bool BASS_SampleGetData(int handle, IntPtr buffer);
        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("bass")]
        //public static extern bool BASS_SampleGetData(int handle, byte[] buffer);
        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("bass")]
        //public static extern bool BASS_SampleGetData(int handle, short[] buffer);
        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("bass")]
        //public static extern bool BASS_SampleGetData(int handle, int[] buffer);
        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("bass")]
        //public static extern bool BASS_SampleGetData(int handle, float[] buffer);



        [DllImport(BASS_DLL, EntryPoint = "BASS_SampleGetInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BASS_SampleGetInfo(int handle, [In, Out] BASS_SAMPLE info);

        [DllImport(BASS_DLL, EntryPoint = "BASS_SampleSetInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BASS_SampleSetInfo(int handle, [In, Out] BASS_SAMPLE info);
        #endregion




        #region 3D

        #endregion





        #region Recording
        /// <summary>
        /// Frees all resources used by the recording device. 
        /// </summary>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_RecordFree();

        /// <summary>
        /// Retrieves the recording device setting of the current thread. 
        /// </summary>
        /// <returns>If successful, the device number is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_RecordGetDevice();

        /// <summary>
        /// Retrieves information on a recording device
        /// </summary>
        /// <param name="device">The device to get the information of... 0 = first.</param>
        /// <param name="info">Pointer to a structure to receive the information. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_RecordGetDeviceInfo")]
        internal static extern bool BASS_RecordGetDeviceInfo([In] int device, [In, Out] ref BASS_DEVICEINFO_INTERNAL info);

        /// <summary>
        /// Retrieves information on the recording device being used. 
        /// </summary>
        /// <param name="info">Pointer to a structure to receive the information. </param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_RecordGetInfo([In, Out] BASS_RECORDINFO info);

        /// <summary>
        /// Retrieves the current settings of a recording input source.
        /// </summary>
        /// <param name="input">The input to get the settings of... 0 = first, -1 = master. </param>
        /// <param name="volume">Pointer to a variable to receive the volume... NULL = don't retrieve the volume. </param>
        /// <returns>If an error occurs, -1 is returned, use BASS_ErrorGetCode to get the error code. 
        /// If successful, then the settings are returned. The BASS_INPUT_OFF flag will be set if the input is disabled, 
        /// otherwise the input is enabled. The type of input is also indicated in the high 8 bits (use BASS_INPUT_TYPE_MASK to test) of the return value,
        /// and can be one of the following. If the volume is requested but not available, volume will receive -1. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_RecordGetInput(int input, ref float volume);

        /// <summary>
        /// Retrieves the current settings of a recording input source.
        /// </summary>
        /// <param name="input">The input to get the settings of... 0 = first, -1 = master. </param>
        /// <param name="volume">Pointer to a variable to receive the volume... NULL = don't retrieve the volume. </param>
        /// <returns>If an error occurs, -1 is returned, use BASS_ErrorGetCode to get the error code. 
        /// If successful, then the settings are returned. The BASS_INPUT_OFF flag will be set if the input is disabled, 
        /// otherwise the input is enabled. The type of input is also indicated in the high 8 bits (use BASS_INPUT_TYPE_MASK to test) of the return value,
        /// and can be one of the following. If the volume is requested but not available, volume will receive -1. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_RecordGetInput(int input, IntPtr volume);

        /// <summary>
        /// Retrieves the text description of a recording input source. 
        /// </summary>
        /// <param name="input">The input to get the description of... 0 = first, -1 = master. </param>
        /// <returns>If successful, then a pointer to the description is returned, else NULL is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL, EntryPoint = "BASS_RecordGetInputName")]
        internal static extern IntPtr BASS_RecordGetInputName(int input);

        /// <summary>
        /// Initializes a recording device. 
        /// </summary>
        /// <param name="device">The device to use... -1 = default device, 0 = first. BASS_RecordGetDeviceInfo can be used to enumerate the available devices. </param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_RecordInit(int device);

        /// <summary>
        /// Sets the recording device to use for subsequent calls in the current thread. 
        /// </summary>
        /// <param name="device">The device to use... 0 = first. </param>
        /// <returns>If successful, then TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_RecordSetDevice(int device);

        /// <summary>
        /// Adjusts the settings of a recording input source. 
        /// </summary>
        /// <param name="input">The input to adjust the settings of... 0 = first, -1 = master. </param>
        /// <param name="setting">The new setting... a combination of these flags.
        /// BASS_INPUT_OFF Disable the input.This flag cannot be used when the device supports only one input at a time.  
        /// BASS_INPUT_ON Enable the input. If the device only allows one input at a time, then any previously enabled input will be disabled by this
        /// </param>
        /// <param name="volume">The volume level... 0 (silent) to 1 (max), less than 0 = leave current. </param>
        /// <returns>If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(BASS_DLL)]
        internal static extern bool BASS_RecordSetInput(int input, BASSInput setting, float volume);

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
        /// <returns>If successful, the new recording's handle is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. </returns>
        [DllImport(BASS_DLL)]
        internal static extern int BASS_RecordStart(int freq, int chans, BASSFlag flags, RECORDPROC proc, IntPtr user);
        #endregion
    }
}
