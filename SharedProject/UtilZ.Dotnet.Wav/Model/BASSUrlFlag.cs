using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// Url标识
    /// </summary>
    [Flags]
    public enum BASSUrlFlag
    {
        /// <summary>
        /// Use 32-bit floating-point sample data. See Floating-point channels for info
        /// </summary>
        BASS_SAMPLE_FLOAT = 256,

        /// <summary>
        /// Decode/play the stream (MP3/MP2/MP1 only) in mono, reducing the CPU usage (if it was originally stereo). This flag is automatically applied if BASS_DEVICE_MONO was specified when calling BASS_Init
        /// </summary>
        BASS_SAMPLE_MONO = 2,

        /// <summary>
        /// Force the stream to not use hardware mixing.
        /// </summary>
        BASS_SAMPLE_SOFTWARE = 16,

        /// <summary>
        /// Enable 3D functionality. This requires that the BASS_DEVICE_3D flag was specified when calling BASS_Init, and the stream must be mono. The SPEAKER flags cannot be used together with this flag. 
        /// </summary>
        BASS_SAMPLE_3D = 8,

        /// <summary>
        /// Loop the file. This flag can be toggled at any time using BASS_ChannelFlags. This flag is ignored when streaming in blocks (BASS_STREAM_BLOCK). 
        /// </summary>
        BASS_SAMPLE_LOOP = 4,

        /// <summary>
        /// Enable the old implementation of DirectX 8 effects. See the DX8 effect implementations section for details. Use BASS_ChannelSetFX to add effects to the stream
        /// </summary>
        BASS_SAMPLE_FX = 128,

        /// <summary>
        /// restrict the download rate of internet file streams
        /// </summary>
        BASS_STREAM_RESTRATE = 0x80000,

        /// <summary>
        /// download/play internet file stream in small blocks
        /// </summary>
        BASS_STREAM_BLOCK = 0x100000,

        /// <summary>
        /// give server status info (HTTP/ICY tags) in DOWNLOADPROC
        /// </summary>
        BASS_STREAM_STATUS = 0x800000,

        /// <summary>
        /// automatically free the stream when it stop/ends
        /// </summary>
        BASS_STREAM_AUTOFREE = 0x40000,

        /// <summary>
        /// Decode the sample data, without playing it. Use BASS_ChannelGetData to retrieve decoded sample data. The BASS_SAMPLE_3D, BASS_STREAM_AUTOFREE and SPEAKER flags cannot be used together with this flag. The BASS_SAMPLE_SOFTWARE and BASS_SAMPLE_FX flags are also ignored
        /// </summary>
        BASS_STREAM_DECODE = 0x200000,

        /// <summary>
        /// url is in UTF-16 form. Otherwise it is ANSI on Windows or Windows CE, and UTF-8 on other platforms
        /// </summary>
        BASS_UNICODE = -2147483648,
    }
}
