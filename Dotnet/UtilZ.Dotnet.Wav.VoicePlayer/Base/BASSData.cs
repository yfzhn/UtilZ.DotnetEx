using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Base
{
    /// <summary>
    /// Flags to be used with Un4seen.Bass.Bass.BASS_ChannelGetData(System.Int32,System.IntPtr,System.Int32).
    /// </summary>
    [Flags]
    public enum BASSData
    {
        /// <summary>
        /// 256 sample FFT (returns 128 floating-point values)
        /// </summary>
        BASS_DATA_FFT256 = int.MinValue,

        /// <summary>
        /// 512 sample FFT (returns 256 floating-point values)
        /// </summary>
        BASS_DATA_FFT512 = -2147483647,

        /// <summary>
        /// 1024 sample FFT (returns 512 floating-point values)
        /// </summary>
        BASS_DATA_FFT1024 = -2147483646,

        /// <summary>
        /// 2048 sample FFT (returns 1024 floating-point values)
        /// </summary>
        BASS_DATA_FFT2048 = -2147483645,

        /// <summary>
        /// 4096 sample FFT (returns 2048 floating-point values)
        /// </summary>
        BASS_DATA_FFT4096 = -2147483644,

        /// <summary>
        /// 8192 sample FFT (returns 4096 floating-point values)
        /// </summary>
        BASS_DATA_FFT8192 = -2147483643,

        /// <summary>
        /// 16384 sample FFT (returns 8192 floating-point values)
        /// </summary>
        BASS_DATA_FFT16384 = -2147483642,

        /// <summary>
        /// 32768 sample FFT (returns 16384 floating-point values)
        /// </summary>
        BASS_DATA_FFT32768 = -2147483641,

        /// <summary>
        /// Query how much data is buffered
        /// </summary>
        BASS_DATA_AVAILABLE = 0,

        /// <summary>
        /// FFT flag: FFT for each channel, else all combined
        /// </summary>
        BASS_DATA_FFT_INDIVIDUAL = 16,

        /// <summary>
        /// FFT flag: no Hanning window
        /// </summary>
        BASS_DATA_FFT_NOWINDOW = 32,

        /// <summary>
        /// FFT flag: pre-remove DC bias
        /// </summary>
        BASS_DATA_FFT_REMOVEDC = 64,

        /// <summary>
        /// FFT flag: return complex data
        /// </summary>
        BASS_DATA_FFT_COMPLEX = 128,

        /// <summary>
        /// FFT flag: return extra Nyquist value
        /// </summary>
        BASS_DATA_FFT_NYQUIST = 256,

        /// <summary>
        /// flag: return 8.24 fixed-point data
        /// </summary>
        BASS_DATA_FIXED = 536870912,

        /// <summary>
        /// flag: return floating-point sample data
        /// </summary>
        BASS_DATA_FLOAT = 1073741824
    }
}
