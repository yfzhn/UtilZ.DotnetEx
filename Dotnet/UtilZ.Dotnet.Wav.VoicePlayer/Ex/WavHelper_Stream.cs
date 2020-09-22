using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    //WavHelper-Stream
    public partial class WavHelper
    {
        #region Stream
        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file. 
        /// </summary>
        /// <param name="file">Filename</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int StreamCreateFile(string file)
        {
            BASSFlag flags = BASSFlag.BASS_DEFAULT | BASSFlag.BASS_UNICODE;
            return StreamCreateFile(file, 0L, 0L, flags);
        }

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file. 
        /// </summary>
        /// <param name="memory">An unmanaged pointer to the memory location as an IntPtr</param>
        /// <param name="offset">Offset to begin streaming from (unused for memory streams, set to 0). </param>
        /// <param name="length">Data length (needs to be set to the length of the memory stream in bytes which should be played)</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int StreamCreateFile(IntPtr memory, long offset, long length, BASSFlag flags)
        {
            int handle = NativeMethods.BASS_StreamCreateFileMemory(true, memory, offset, length, flags);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file. 
        /// </summary>
        /// <param name="mem">TRUE = stream the file from memory(true:流;false:文件)</param>
        /// <param name="file">Filename (mem = FALSE) or a memory location (mem = TRUE).</param>
        /// <param name="offset">File offset to begin streaming from (only used if mem = FALSE). </param>
        /// <param name="length">Data length... 0 = use all data up to the end of the file (if mem = FALSE)</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <returns>If successful, the new stream's handle is returned, else 0 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int StreamCreateFile(string file, long offset, long length, BASSFlag flags)
        {
            flags |= BASSFlag.BASS_DEFAULT | BASSFlag.BASS_UNICODE;
            int handle = NativeMethods.BASS_StreamCreateFileUnicode(false, file, offset, length, flags);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file via user callback functions. 
        /// </summary>
        /// <param name="system">File system to use, one of the following. </param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="procs">The user defined file functions. </param>
        /// <param name="user">User instance data to pass to the callback functions. </param>
        /// <returns>If successful, the new stream's handle is returned, else throw WavException.</returns>
        public static int StreamCreateFileUser(BASSStreamSystem system, BASSFlag flags, BASS_FILEPROCS procs, IntPtr user)
        {
            int handle = NativeMethods.BASS_StreamCreateFileUser(system, flags, procs, user);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        /// <summary>
        /// Creates a sample stream from an MP3, MP2, MP1, OGG, WAV, AIFF or plugin supported file on the internet, optionally receiving the downloaded data in a callback function. 
        /// </summary>
        /// <param name="url">URL of the file to stream. Should begin with "http://" or "https://" or "ftp://", or another add-on supported protocol. The URL can be followed by custom HTTP request headers to be sent to the server; 
        /// the URL and each header should be terminated with a carriage return and line feed ("\r\n"). </param>
        /// <param name="offset">File position to start streaming from. This is ignored by some servers, specifically when the length is unknown/undefined.</param>
        /// <param name="flags">A combination of these flags.</param>
        /// <param name="proc">Callback function to receive the file as it is downloaded... NULL = no callback. </param>
        /// <param name="user">User instance data to pass to the callback function. </param>
        /// <returns>If successful, the new stream's handle is returned, else throw WavException</returns>
        public static int StreamCreateURL(string url, int offset, BASSFlag flags, DOWNLOADPROC proc, IntPtr user)
        {
            flags |= BASSFlag.BASS_DEFAULT | BASSFlag.BASS_UNICODE;
            int handle = NativeMethods.BASS_StreamCreateURLUnicode(url, offset, flags, proc, user);
            if (handle == 0)
            {
                flags &= BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_SPEAKER_REAR2RIGHT | BASSFlag.BASS_SPEAKER_CENTER | BASSFlag.BASS_SPEAKER_PAIR8 | BASSFlag.BASS_MIDI_SINCINTER | BASSFlag.BASS_AAC_STEREO | BASSFlag.BASS_MUSIC_DECODE | BASSFlag.BASS_MUSIC_NOSAMPLE | BASSFlag.BASS_MIDI_FONT_NOFX | BASSFlag.BASS_MIDI_FONT_XGDRUMS | BASSFlag.BASS_SAMPLE_OVER_DIST | BASSFlag.BASS_MIDI_NOCROP | BASSFlag.BASS_MIDI_DECAYSEEK | BASSFlag.BASS_MIDI_NOFX | BASSFlag.BASS_AAC_FRAME960 | BASSFlag.BASS_AC3_DYNAMIC_RANGE | BASSFlag.BASS_AC3_DOWNMIX_DOLBY | BASSFlag.BASS_MUSIC_FLOAT | BASSFlag.BASS_MUSIC_FX | BASSFlag.BASS_SAMPLE_VAM | BASSFlag.BASS_SAMPLE_MUTEMAX | BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_MUSIC_3D | BASSFlag.BASS_MUSIC_LOOP | BASSFlag.BASS_FX_BPM_MULT2 | BASSFlag.BASS_FX_BPM_BKGRND;
                handle = NativeMethods.BASS_StreamCreateURLAscii(url, offset, flags, proc, user);
            }

            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

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
        /// <returns>If successful, the new stream's handle is returned, else throw WavException</returns>
        public static int StreamCreate(int freq, int chans, BASSFlag flags, STREAMPROC proc, IntPtr user)
        {
            int handle = NativeMethods.BASS_StreamCreate(freq, chans, flags, proc, IntPtr.Zero);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

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
        /// <returns>If successful, the new stream's handle is returned, else throw WavException</returns>

        public static int StreamCreate(int freq, int chans, BASSFlag flags, BASSStreamProc proc)
        {
            int handle = NativeMethods.BASS_StreamCreatePtr(freq, chans, flags, new IntPtr((int)proc), IntPtr.Zero);
            if (handle == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return handle;
        }

        /// <summary>
        /// Retrieves the file position/status of a stream. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="mode">The file position/status to retrieve. One of the following</param>
        /// <returns> requested file position/status is returned</returns>
        public static long StreamGetFilePosition(int handle, BASSStreamFilePosition mode)
        {
            long pos = NativeMethods.BASS_StreamGetFilePosition(handle, mode);
            if (pos == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return pos;
        }

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else throw WavException.</returns>
        public static int StreamPutData(int handle, IntPtr buffer, int length)
        {
            int amount = NativeMethods.BASS_StreamPutData(handle, buffer, length);
            if (amount == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return amount;
        }

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else throw WavException.</returns>
        public static int StreamPutData(int handle, byte[] buffer, int length)
        {
            int amount = NativeMethods.BASS_StreamPutData(handle, buffer, length);
            if (amount == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return amount;
        }

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else throw WavException.</returns>
        public static int StreamPutData(int handle, short[] buffer, int length)
        {
            int amount = NativeMethods.BASS_StreamPutData(handle, buffer, length);
            if (amount == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return amount;
        }

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else throw WavException.</returns>
        public static int StreamPutData(int handle, int[] buffer, int length)
        {
            int amount = NativeMethods.BASS_StreamPutData(handle, buffer, length);
            if (amount == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return amount;
        }

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else throw WavException.</returns>
        public static int StreamPutData(int handle, float[] buffer, int length)
        {
            int amount = NativeMethods.BASS_StreamPutData(handle, buffer, length);
            if (amount == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return amount;
        }

        /// <summary>
        /// Adds sample data to a "push" stream. 
        /// </summary>
        /// <param name="handle">The stream handle</param>
        /// <param name="buffer">Pointer to the sample data... NULL = allocate space in the queue buffer so that there is at least length bytes of free space. </param>
        /// <param name="length">The amount of data in bytes, optionally using the BASS_STREAMPROC_END flag to signify the end of the stream. 0 can be used to just check how much data is queued. </param>
        /// <returns>If successful, the amount of queued data is returned, else throw WavException.</returns>
        public static unsafe int StreamPutData(int handle, byte[] buffer, int startIdx, int length)
        {
            fixed (byte* numRef = &(buffer[startIdx]))
            {
                byte* numPtr = numRef;
                return StreamPutData(handle, new IntPtr((void*)numPtr), length);
            }
        }

        /// <summary>
        /// Adds data to a "push buffered" user file stream's buffer. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="buffer">Pointer to the file data.</param>
        /// <param name="length">The amount of data in bytes, or BASS_FILEDATA_END to end the file. </param>
        /// <returns>If successful, the number of bytes read from buffer is returned, else throw WavException</returns>
        public static int StreamPutFileData(int handle, IntPtr buffer, int length)
        {
            int count = NativeMethods.BASS_StreamPutFileData(handle, buffer, length);
            if (count == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return count;
        }

        /// <summary>
        /// Adds data to a "push buffered" user file stream's buffer. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="buffer">Pointer to the file data.</param>
        /// <param name="length">The amount of data in bytes, or BASS_FILEDATA_END to end the file. </param>
        /// <returns>If successful, the number of bytes read from buffer is returned, else throw WavException</returns>
        public static int StreamPutFileData(int handle, byte[] buffer, int length)
        {
            int count = NativeMethods.BASS_StreamPutFileData(handle, buffer, length);
            if (count == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return count;
        }

        /// <summary>
        /// Adds data to a "push buffered" user file stream's buffer. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="buffer">Pointer to the file data.</param>
        /// <param name="length">The amount of data in bytes, or BASS_FILEDATA_END to end the file. </param>
        /// <returns>If successful, the number of bytes read from buffer is returned, else throw WavException</returns>
        public static int StreamPutFileData(int handle, short[] buffer, int length)
        {
            int count = NativeMethods.BASS_StreamPutFileData(handle, buffer, length);
            if (count == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return count;
        }

        /// <summary>
        /// Adds data to a "push buffered" user file stream's buffer. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="buffer">Pointer to the file data.</param>
        /// <param name="length">The amount of data in bytes, or BASS_FILEDATA_END to end the file. </param>
        /// <returns>If successful, the number of bytes read from buffer is returned, else throw WavException</returns>
        public static int StreamPutFileData(int handle, int[] buffer, int length)
        {
            int count = NativeMethods.BASS_StreamPutFileData(handle, buffer, length);
            if (count == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return count;
        }

        /// <summary>
        /// Adds data to a "push buffered" user file stream's buffer. 
        /// </summary>
        /// <param name="handle">The stream handle. </param>
        /// <param name="buffer">Pointer to the file data.</param>
        /// <param name="length">The amount of data in bytes, or BASS_FILEDATA_END to end the file. </param>
        /// <returns>If successful, the number of bytes read from buffer is returned, else throw WavException</returns>
        public static int StreamPutFileData(int handle, float[] buffer, int length)
        {
            int count = NativeMethods.BASS_StreamPutFileData(handle, buffer, length);
            if (count == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return count;
        }

        /// <summary>
        /// Frees a sample stream's resources, including any sync/DSP/FX it has. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        public static void StreamFree(int handle)
        {
            bool result = NativeMethods.BASS_StreamFree(handle);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }
        #endregion
    }
}
