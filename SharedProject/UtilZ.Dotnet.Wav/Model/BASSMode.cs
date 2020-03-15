using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// BASSMode
    /// </summary>
    [Flags]
    public enum BASSMode
    {
        /// <summary>
        /// byte position
        /// </summary>
        BASS_POS_BYTE = 0,

        /// <summary>
        /// order.row position, MAKELONG(order,row)
        /// </summary>
        BASS_POS_MUSIC_ORDER = 1,

        /// <summary>
        /// OGG bitstream number
        /// </summary>
        BASS_POS_OGG = 3,

        /// <summary>
        /// allow seeking to inexact position
        /// </summary>
        BASS_POS_INEXACT = 0x8000000,

        /// <summary>
        ///get the decoding (not playing) position
        /// </summary>
        BASS_POS_DECODE = 0x10000000,

        /// <summary>
        /// decode to the position instead of seeking
        /// </summary>
        BASS_POS_DECODETO = 0x20000000,

        /// <summary>
        /// scan to the position
        /// </summary>
        BASS_POS_SCAN = 0x40000000

        //BASS_MIDI_DECAYSEEK = 0x4000,
        //BASS_MIXER_NORAMPIN = 0x800000,
        //BASS_MUSIC_POSRESET = 0x8000,
        //BASS_MUSIC_POSRESETEX = 0x400000,
        //BASS_POS_BYTES = 0,
        //BASS_POS_CD_TRACK = 4,
        //BASS_POS_DECODE = 0x10000000,
        //BASS_POS_DECODETO = 0x20000000,
        //BASS_POS_INEXACT = 0x8000000,
        //BASS_POS_MIDI_TICK = 2,
        //BASS_POS_MUSIC_ORDERS = 1,
        //BASS_POS_OGG = 3,
        //BASS_POS_SCAN = 0x40000000
    }
}
