using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// BASSInit
    /// </summary>
    [Flags]
    public enum BASSInit
    {

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_3D = 4,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_8BITS = 1,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_CPSPEAKERS = 0x400,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_DEFAULT = 0,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_FREQ = 0x4000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_LATENCY = 0x100,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_MONO = 2,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_NOSPEAKER = 0x1000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_SPEAKERS = 0x800,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_STEREO = 0x8000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVIDE_DMIX = 0x2000
    }
}
