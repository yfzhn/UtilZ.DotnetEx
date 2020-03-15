using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// Number of bytes wanted (up to 268435455 or 0xFFFFFFF), and/or the following flags.
    /// </summary>
    public enum BASS_ChannelGetDataLengthMode : uint
    {
        /// <summary>
        /// Return floating-point sample data.  
        /// </summary>
        BASS_DATA_FLOAT = 0x40000000,

        /// <summary>
        /// Return 8.24 fixed-point data.  
        /// </summary>
        BASS_DATA_FIXED = 0x20000000,

        /// <summary>
        /// 256 sample FFT (returns 128 values).  
        /// </summary>
        BASS_DATA_FFT256 = 0x80000000,

        /// <summary>
        /// 512 sample FFT (returns 256 values).  
        /// </summary>
        BASS_DATA_FFT512 = 0x80000001,

        /// <summary>
        /// 1024 sample FFT (returns 512 values).  
        /// </summary>
        BASS_DATA_FFT1024 = 0x80000002,

        /// <summary>
        /// 2048 sample FFT (returns 1024 values).  
        /// </summary>
        BASS_DATA_FFT2048 = 0x80000003,

        /// <summary>
        /// 4096 sample FFT (returns 2048 values).  
        /// </summary>
        BASS_DATA_FFT4096 = 0x80000004,

        /// <summary>
        /// 8192 sample FFT (returns 4096 values).  
        /// </summary>
        BASS_DATA_FFT8192 = 0x80000005,

        /// <summary>
        /// 16384 sample FFT (returns 8192 values).  
        /// </summary>
        BASS_DATA_FFT16384 = 0x80000006,

        /// <summary>
        /// 32768 sample FFT (returns 16384 values).  
        /// </summary>
        BASS_DATA_FFT32768 = 0x80000007,

        /// <summary>
        /// Return the complex FFT result rather than the magnitudes. This increases the amount of data returned (as listed above) fourfold, as it returns real and imaginary parts and the full FFT result (not only the first half). The real and imaginary parts are interleaved in the returned data.  
        /// </summary>
        BASS_DATA_FFT_COMPLEX = 0x80,

        /// <summary>
        /// Perform a separate FFT for each channel, rather than a single combined FFT. The size of the data returned (as listed above) is multiplied by the number of channels.  
        /// </summary>
        BASS_DATA_FFT_INDIVIDUAL = 0x10,

        /// <summary>
        /// Prevent a Hann window being applied to the sample data when performing an FFT.  
        /// </summary>
        BASS_DATA_FFT_NOWINDOW = 0x20,

        /// <summary>
        /// Remove any DC bias from the sample data when performing an FFT.  
        /// </summary>
        BASS_DATA_FFT_REMOVEDC = 0x40,

        /// <summary>
        /// Query the amount of data the channel has buffered for playback, or from recording. This flag cannot be used with decoding channels as they do not have playback buffers. buffer is ignored when using this flag.  
        /// </summary>
        BASS_DATA_AVAILABLE = 0
    }
}
