using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 输出设备信息类型
    /// </summary>
    [Flags]
    public enum BASSDeviceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_DEFAULT = 2,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_ENABLED = 1,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_INIT = 4,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_NONE = 0,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_DIGITAL = 0x8000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_DISPLAYPORT = 0x40000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_HANDSET = 0x7000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_HDMI = 0xa000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_HEADPHONES = 0x4000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_HEADSET = 0x6000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_LINE = 0x3000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_MASK = -16777216,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_MICROPHONE = 0x5000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_NETWORK = 0x1000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_SPDIF = 0x9000000,

        /// <summary>
        /// 
        /// </summary>
        BASS_DEVICE_TYPE_SPEAKERS = 0x2000000
    }
}
