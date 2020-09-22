using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Flags]
    public enum BASSInit
    {
        BASS_DEVICE_3D = 4,
        BASS_DEVICE_8BITS = 1,
        BASS_DEVICE_CPSPEAKERS = 0x400,
        BASS_DEVICE_DEFAULT = 0,
        BASS_DEVICE_DSOUND = 0x40000,
        BASS_DEVICE_FREQ = 0x4000,
        BASS_DEVICE_HOG = 0x10000,
        BASS_DEVICE_LATENCY = 0x100,
        BASS_DEVICE_MONO = 2,
        BASS_DEVICE_NOSPEAKER = 0x1000,
        BASS_DEVICE_SPEAKERS = 0x800,
        BASS_DEVICE_STEREO = 0x8000,
        BASS_DEVIDE_DMIX = 0x2000
    }
}
