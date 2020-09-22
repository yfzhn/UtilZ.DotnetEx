using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    public delegate void SYNCPROC(int handle, int channel, int data, IntPtr user);

    public delegate void FILECLOSEPROC(IntPtr user);
    public delegate long FILELENPROC(IntPtr user);
    public delegate int FILEREADPROC(IntPtr buffer, int length, IntPtr user);
    public delegate bool FILESEEKPROC(long offset, IntPtr user);

    public delegate void DOWNLOADPROC(IntPtr buffer, int length, IntPtr user);

    public delegate int STREAMPROC(int handle, IntPtr buffer, int length, IntPtr user);

    public delegate bool RECORDPROC(int handle, IntPtr buffer, int length, IntPtr user);


}
