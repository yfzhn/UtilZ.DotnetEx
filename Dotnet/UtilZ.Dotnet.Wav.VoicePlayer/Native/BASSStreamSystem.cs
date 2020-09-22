using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// User file system flag to be used with Un4seen.Bass.Bass.
    /// BASS_StreamCreateFileUser(Un4seen.Bass.BASSStreamSystem,Un4seen.Bass.BASSFlag,
    /// Un4seen.Bass.BASS_FILEPROCS,System.IntPtr)
    /// </summary>
    public enum BASSStreamSystem
    {
        /// <summary>
        /// Unbuffered
        /// </summary>
        STREAMFILE_NOBUFFER,

        /// <summary>
        /// Buffered
        /// </summary>
        STREAMFILE_BUFFER,

        /// <summary>
        /// Buffered, with the data pushed to BASS via BASS_StreamPutFileData. 
        /// </summary>
        STREAMFILE_BUFFERPUSH
    }
}
