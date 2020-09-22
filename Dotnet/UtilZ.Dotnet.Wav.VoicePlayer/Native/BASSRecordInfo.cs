using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Flags]
    public enum BASSRecordInfo
    {
        DSCAPS_CERTIFIED = 0x40,
        DSCAPS_EMULDRIVER = 0x20,
        DSCAPS_NONE = 0
    }
}
