using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// BASSAttribute
    /// </summary>
    public enum BASSAttribute
    {
        /// <summary>
        /// EAX wet/dry mix. (HCHANNEL/HMUSIC/HSTREAM only)
        /// </summary>
        BASS_ATTRIB_EAXMIX = 4,

        /// <summary>
        /// Sample rate
        /// </summary>
        BASS_ATTRIB_FREQ = 1,

        /// <summary>
        /// Amplification level. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_AMPLIFY = 0x100,

        /// <summary>
        /// BPM. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_BPM = 0x103,

        /// <summary>
        /// Pan separation level. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_PANSEP = 0x101,

        /// <summary>
        /// Position scaler. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_PSCALER = 0x102,

        /// <summary>
        /// Speed. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_SPEED = 0x104,

        /// <summary>
        /// A channel volume level. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_VOL_CHAN = 0x200,

        /// <summary>
        /// Global volume level. (HMUSIC)
        /// </summary>
        BASS_ATTRIB_MUSIC_VOL_GLOBAL = 0x105,

        /// <summary>
        /// An instrument/sample volume level. (HMUSIC) 
        /// </summary>
        BASS_ATTRIB_MUSIC_VOL_INST = 0x300,

        /// <summary>
        /// Buffer level to resume stalled playback. (HSTREAM)  
        /// </summary>
        BASS_ATTRIB_NET_RESUME = 9,

        /// <summary>
        /// Playback buffering switch. (HMUSIC/HSTREAM)
        /// </summary>
        BASS_ATTRIB_NOBUFFER = 5,

        /// <summary>
        /// Playback ramping switch
        /// </summary>
        BASS_ATTRIB_NORAMP = 11,

        /// <summary>
        /// Panning/balance position.
        /// </summary>

        BASS_ATTRIB_PAN = 3,

        /// <summary>
        /// Sample rate conversion quality
        /// </summary>
        BASS_ATTRIB_SRC = 8,

        /// <summary>
        /// Volume level
        /// </summary>
        BASS_ATTRIB_VOL = 2
    }
}
