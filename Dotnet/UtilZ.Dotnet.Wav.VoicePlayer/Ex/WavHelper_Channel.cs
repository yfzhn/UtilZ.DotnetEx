using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Wav.VoicePlayer.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Native;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    //WavHelper-Channel
    public partial class WavHelper
    {
        #region Channel
        /// <summary>
        /// Starts (or resumes) playback of a sample, strea
        /// MOD music, or recording
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <param name="restart">Restart playback from the beginning? If handle is a user stream (created with BASS_StreamCreate),
        /// its current buffer contents are cleared. If it is a MOD music, its BPM/etc are reset to their initial values</param>
        public static void ChannelPlay(int handle, bool restart)
        {
            bool result = NativeMethods.BASS_ChannelPlay(handle, restart);
            //If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Pauses a sample, stream, MOD music, or recording. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        public static void ChannelPause(int handle)
        {
            bool result = NativeMethods.BASS_ChannelPause(handle);
            //If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code. 
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }


        /// <summary>
        /// Stops a sample, stream, MOD music, or recording. 
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>             
        public static void ChannelStop(int handle)
        {
            bool result = NativeMethods.BASS_ChannelStop(handle);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }





        /// <summary>
        /// Retrieves information on a channel
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD</param>
        /// <returns>Pointer to structure to receive the channel information</returns>
        public static BASS_CHANNELINFO_INTERNAL ChannelGetInfo(int handle)
        {
            BASS_CHANNELINFO_INTERNAL info = new BASS_CHANNELINFO_INTERNAL();
            bool result = NativeMethods.BASS_ChannelGetInfo(handle, ref info);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return info;
        }

        /// <summary>
        /// Retrieves the playback length of a channel.
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
        public static long ChannelGetLength(int handle, BASSMode mode)
        {
            long result = NativeMethods.BASS_ChannelGetLength(handle, mode);
            if (result == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

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
        /// <returns>When requesting FFT data, the number of bytes read from the channel (to perform the FFT) is returned.
        /// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT or BASS_DATA_FIXED flag). 
        /// When using the BASS_DATA_AVAILABLE flag, the number of bytes in the channel's buffer is returned. </returns>
        public static int ChannelGetData(int handle, IntPtr buffer, int length)
        {
            int result = NativeMethods.BASS_ChannelGetData(handle, buffer, length);
            if (result == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

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
        /// <returns>When requesting FFT data, the number of bytes read from the channel (to perform the FFT) is returned.
        /// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT or BASS_DATA_FIXED flag). 
        /// When using the BASS_DATA_AVAILABLE flag, the number of bytes in the channel's buffer is returned. </returns>
        public static int ChannelGetData(int handle, byte[] buffer, int length)
        {
            int result = NativeMethods.BASS_ChannelGetData(handle, buffer, length);
            //if (result == -1)
            //{
            //    throw new WavException(BassErrorCode.GetErrorInfo());
            //}

            return result;
        }

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
        /// <returns>When requesting FFT data, the number of bytes read from the channel (to perform the FFT) is returned.
        /// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT or BASS_DATA_FIXED flag). 
        /// When using the BASS_DATA_AVAILABLE flag, the number of bytes in the channel's buffer is returned. </returns>
        public static int ChannelGetData(int handle, short[] buffer, int length)
        {
            int result = NativeMethods.BASS_ChannelGetData(handle, buffer, length);
            //if (result == -1)
            //{
            //    throw new WavException(BassErrorCode.GetErrorInfo());
            //}

            return result;
        }

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
        /// <returns>When requesting FFT data, the number of bytes read from the channel (to perform the FFT) is returned.
        /// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT or BASS_DATA_FIXED flag). 
        /// When using the BASS_DATA_AVAILABLE flag, the number of bytes in the channel's buffer is returned. </returns>
        public static int ChannelGetData(int handle, int[] buffer, int length)
        {
            int result = NativeMethods.BASS_ChannelGetData(handle, buffer, length);
            //if (result == -1)
            //{
            //    throw new WavException(BassErrorCode.GetErrorInfo());
            //}

            return result;
        }

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
        /// <returns>When requesting FFT data, the number of bytes read from the channel (to perform the FFT) is returned.
        /// When requesting sample data, the number of bytes written to buffer will be returned (not necessarily the same as the number of bytes read when using the BASS_DATA_FLOAT or BASS_DATA_FIXED flag). 
        /// When using the BASS_DATA_AVAILABLE flag, the number of bytes in the channel's buffer is returned. </returns>
        public static int ChannelGetData(int handle, float[] buffer, int length)
        {
            int result = NativeMethods.BASS_ChannelGetData(handle, buffer, length);
            //if (result == -1)
            //{
            //    throw new WavException(BassErrorCode.GetErrorInfo());
            //}

            return result;
        }






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
        public static long ChannelGetPosition(int handle, BASSMode mode)
        {
            return NativeMethods.BASS_ChannelGetPosition(handle, mode);
        }

        /// <summary>
        /// Sets the playback position of a sample, MOD music, or stream
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HSTREAM or HMUSIC</param>
        /// <param name="pos">The position, in units determined by the mode</param>
        /// <param name="mode">How to set the position. One of the following, with optional flags</param>
        public static void ChannelSetPosition(int handle, long pos, BASSMode mode)
        {
            bool result = NativeMethods.BASS_ChannelSetPosition(handle, pos, mode);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Translates a byte position into time (seconds), based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else a negative value is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static double ChannelBytes2Seconds(int handle, long pos)
        {
            double result = NativeMethods.BASS_ChannelBytes2Seconds(handle, pos);
            if (result < 0d)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

        /// <summary>
        /// Translates a time (seconds) position into bytes, based on a channel's format
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <param name="pos">The position to translate</param>
        /// <returns>If successful, then the translated length is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static long ChannelSeconds2Bytes(int handle, double pos)
        {
            long result = NativeMethods.BASS_ChannelSeconds2Bytes(handle, pos);
            if (result < 0d)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }






        /// <summary>
        /// Locks a stream, MOD music or recording channel to the current thread.
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC, HSTREAM or HRECORD</param>
        /// <param name="state">true:lock the channel;false:unlock the channel</param>
        public static void ChannelLock(int handle, bool state)
        {
            bool result = NativeMethods.BASS_ChannelLock(handle, state);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// Retrieves the device that a channel is using.
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. HSAMPLE handles may also be used</param>
        /// <returns>If successful, the device number is returned, else -1 is returned. Use BASS_ErrorGetCode to get the error code</returns>
        public static int ChannelGetDevice(int handle)
        {
            int result = NativeMethods.BASS_ChannelGetDevice(handle);
            if (result == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

        /// <summary>
        /// Changes the device that a stream, MOD music or sample is using.
        /// </summary>
        /// <param name="handle">The channel or sample handle... a HMUSIC, HSTREAM or HSAMPLE. </param>
        /// <param name="device">The device to use... 0 = no sound, 1 = first real output device, BASS_NODEVICE = no device</param>
        public static void ChannelSetDevice(int handle, int device)
        {
            bool result = NativeMethods.BASS_ChannelSetDevice(handle, device);
            if (!result)
            {
                int errCode = NativeMethods.BASS_ErrorGetCode();
                if (errCode != BassErrorCode.BASS_ERROR_ALREADY)
                {
                    //非已设定或初始化才抛出错误异常
                    throw new WavException(BassErrorCode.GetErrorInfo());
                }
            }
        }

        /// <summary>
        /// Checks if a sample, stream, or MOD music is active (playing) or stalled. Can also check if a recording is in progress
        /// </summary>
        /// <param name="handle">The channel handle... a HCHANNEL, HMUSIC, HSTREAM, or HRECORD. </param>
        /// <returns>The return value is one of the following</returns>
        public static BASSActive ChannelIsActive(int handle)
        {
            return NativeMethods.BASS_ChannelIsActive(handle);
        }

        /// <summary>
        /// Updates the playback buffer of a stream or MOD music
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC or HSTREAM. </param>
        /// <param name="length">The amount of data to render, in milliseconds... 0 = default (2 x update period). This is capped at the space available in the buffer.</param>
        public static void ChannelUpdate(int handle, int length)
        {
            bool result = NativeMethods.BASS_ChannelUpdate(handle, length);
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }



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
        public static IntPtr BASS_ChannelGetTags(int handle, BASSTag tags)
        {
            IntPtr result = NativeMethods.BASS_ChannelGetTags(handle, tags);
            if (result == IntPtr.Zero)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }

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
        public static BASSFlag ChannelFlags(int handle, BASSFlag flags, BASSFlag mask)
        {
            int result = NativeMethods.BASS_ChannelFlags(handle, flags, mask);
            if (result == -1)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return (BASSFlag)result;
        }

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
        public static int ChannelSetSync(int handle, BASSSync type, long param, SYNCPROC proc, IntPtr user)
        {
            int result = NativeMethods.BASS_ChannelSetSync(handle, type, param, proc, user);
            if (result == 0)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }

            return result;
        }


        /// <summary>
        /// Removes a synchronizer from a MOD music, stream or recording channel. 
        /// </summary>
        /// <param name="handle">The channel handle... a HMUSIC, HSTREAM or HRECORD</param>
        /// <param name="sync">Handle of the synchronizer to remove</param>        
        public static void BASS_ChannelRemoveSync(int handle, int sync)
        {
            bool result = NativeMethods.BASS_ChannelRemoveSync(handle, sync);

            //If successful, TRUE is returned, else FALSE is returned. Use BASS_ErrorGetCode to get the error code
            if (!result)
            {
                throw new WavException(BassErrorCode.GetErrorInfo());
            }
        }
        #endregion
    }
}
