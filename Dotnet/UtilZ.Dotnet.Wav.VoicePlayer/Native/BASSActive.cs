using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Native
{
    public enum BASSActive
    {
        /// <summary>
        /// The channel is not active, or handle is not a valid channel.
        /// </summary>
        BASS_ACTIVE_STOPPED,

        /// <summary>
        /// The channel is playing (or recording). 
        /// </summary>
        BASS_ACTIVE_PLAYING,

        /// <summary>
        /// The channel is paused. 
        /// </summary>
        BASS_ACTIVE_STALLED,

        /// <summary>
        /// The channel's device is paused.
        /// </summary>
        BASS_ACTIVE_PAUSED,

        /// <summary>
        /// Playback of the stream has been stalled due to a lack of sample data. Playback will automatically resume once there is sufficient data to do so. 
        /// </summary>
        BASS_ACTIVE_PAUSED_DEVICE
    }
}
