using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class BASS_FILEPROCS
    {
        public FILECLOSEPROC close;
        public FILELENPROC length;
        public FILEREADPROC read;
        public FILESEEKPROC seek;
        public BASS_FILEPROCS(FILECLOSEPROC closeCallback, FILELENPROC lengthCallback, FILEREADPROC readCallback, FILESEEKPROC seekCallback)
        {
            this.close = closeCallback;
            this.length = lengthCallback;
            this.read = readCallback;
            this.seek = seekCallback;
        }
    }



}
