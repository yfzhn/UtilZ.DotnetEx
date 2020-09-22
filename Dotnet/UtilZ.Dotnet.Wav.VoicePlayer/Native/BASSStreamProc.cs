using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// STREAMPROC flag used with Un4seen.Bass.Bass.BASS_StreamCreate(System.Int32,System.Int32,Un4seen.Bass.BASSFlag,Un4seen.Bass.STREAMPROC,System.IntPtr)
    /// resp. used with a user sample stream to be used with Un4seen.Bass.STREAMPROC.
    /// </summary>
    public enum BASSStreamProc
    {
        /// <summary>
        /// Flag to signify that the end of the stream is reached.
        /// </summary>
        BASS_STREAMPROC_END = int.MinValue,

        /// <summary>
        /// Create a "push" stream.
        /// Instead of BASS pulling data from a STREAMPROC function, data is pushed to BASS 
        /// via Un4seen.Bass.Bass.BASS_StreamPutData(System.Int32,System.IntPtr,System.Int32).
        /// </summary>
        STREAMPROC_DUMMY = 0,

        /// <summary>
        ///  Create a "dummy" stream.
        ///  A dummy stream doesn't have any sample data of its own, but a decoding dummy
        ///  stream (with BASS_STREAM_DECODE flag) can be used to apply DSP/FX processing
        ///  to any sample data, by setting DSP/FX on the stream and feeding the data through
        ///  Un4seen.Bass.Bass.BASS_ChannelGetData(System.Int32,System.IntPtr,System.Int32).
        ///  The dummy stream should have the same sample format as the data being fed through
        ///  it.
        /// </summary>
        STREAMPROC_PUSH = -1
    }



}
