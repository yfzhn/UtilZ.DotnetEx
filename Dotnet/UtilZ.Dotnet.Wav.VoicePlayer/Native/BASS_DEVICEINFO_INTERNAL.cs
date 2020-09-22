using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct BASS_DEVICEINFO_INTERNAL
    {
        public IntPtr name;
        public IntPtr driver;
        public BASSDeviceInfo flags;
    }
}
