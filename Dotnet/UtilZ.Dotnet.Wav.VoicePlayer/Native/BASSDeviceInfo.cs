﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum BASSDeviceInfo
    {
        BASS_DEVICE_DEFAULT = 2,
        BASS_DEVICE_DISABLED = 0x40,
        BASS_DEVICE_ENABLED = 1,
        BASS_DEVICE_INIT = 4,
        BASS_DEVICE_INPUT = 0x10,
        BASS_DEVICE_LOOPBACK = 8,
        BASS_DEVICE_NONE = 0,
        BASS_DEVICE_TYPE_DIGITAL = 0x8000000,
        BASS_DEVICE_TYPE_DISPLAYPORT = 0x40000000,
        BASS_DEVICE_TYPE_HANDSET = 0x7000000,
        BASS_DEVICE_TYPE_HDMI = 0xa000000,
        BASS_DEVICE_TYPE_HEADPHONES = 0x4000000,
        BASS_DEVICE_TYPE_HEADSET = 0x6000000,
        BASS_DEVICE_TYPE_LINE = 0x3000000,
        BASS_DEVICE_TYPE_MASK = -16777216,
        BASS_DEVICE_TYPE_MICROPHONE = 0x5000000,
        BASS_DEVICE_TYPE_NETWORK = 0x1000000,
        BASS_DEVICE_TYPE_SPDIF = 0x9000000,
        BASS_DEVICE_TYPE_SPEAKERS = 0x2000000,
        BASS_DEVICE_UNPLUGGED = 0x20
    }


}
