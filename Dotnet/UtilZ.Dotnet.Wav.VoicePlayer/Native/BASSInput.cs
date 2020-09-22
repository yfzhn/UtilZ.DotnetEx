using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Flags]
    public enum BASSInput
    {
        BASS_INPUT_NONE = 0,
        BASS_INPUT_OFF = 0x10000,
        BASS_INPUT_ON = 0x20000
    }
}
