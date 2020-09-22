using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    /// <summary>
    /// Stream File Position modes to be used with Un4seen.Bass.Bass.BASS_StreamGetFilePosition(System.Int32,Un4seen.Bass.BASSStreamFilePosition)
    /// </summary>
    public enum BASSStreamFilePosition
    {
        /// <summary>
        /// The amount of data in the asynchronous file reading buffer. 
        /// This requires that the BASS_ASYNCFILE flag was used at the stream's creation. 
        /// </summary>
        BASS_FILEPOS_ASYNCBUF = 7,

        /// <summary>
        /// The amount of data in the buffer of an internet file stream or "buffered" user file stream. 
        /// Unless streaming in blocks, this is the same as BASS_FILEPOS_DOWNLOAD.
        /// </summary>
        BASS_FILEPOS_BUFFER = 5,

        /// <summary>
        /// The percentage of buffering remaining before playback of an internet file stream or "buffered" user file stream can resume. 
        /// </summary>
        BASS_FILEPOS_BUFFERING = 9,

        /// <summary>
        /// Internet file stream or "buffered" user file stream is still connected? 0 = no, 1 = yes. 
        /// </summary>
        BASS_FILEPOS_CONNECTED = 4,

        /// <summary>
        /// Position that is to be decoded for playback next. This will be a bit ahead of the position actually being heard due to buffering.
        /// </summary>
        BASS_FILEPOS_CURRENT = 0,

        /// <summary>
        /// Download progress of an internet file stream or "buffered" user file stream. 
        /// </summary>
        BASS_FILEPOS_DOWNLOAD = 1,

        /// <summary>
        /// End of audio data. When streaming in blocks (the BASS_STREAM_BLOCK flag is in effect), the download buffer length is given. 
        /// </summary>
        BASS_FILEPOS_END = 2,

        /// <summary>
        /// HLS add-on: segment sequence number
        /// </summary>
        BASS_FILEPOS_HLS_SEGMENT = 0x10000,

        /// <summary>
        /// Total size of the file. 
        /// </summary>
        BASS_FILEPOS_SIZE = 8,

        /// <summary>
        /// Returns the socket hanlde used for streaming.
        /// </summary>
        BASS_FILEPOS_SOCKET = 6,

        /// <summary>
        /// Start of audio data. 
        /// </summary>
        BASS_FILEPOS_START = 3,

        /// <summary>
        /// WMA add-on: internet buffering progress (0-100%)
        /// </summary>
        BASS_FILEPOS_WMA_BUFFER = 0x3e8
    }
}
