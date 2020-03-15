using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 文件信息
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct BASS_CHANNELINFO_INTERNAL
    {
        /// <summary>
        /// Default playback rate
        /// </summary>
        public int freq;

        /// <summary>
        /// Number of channels... 1=mono, 2=stereo, etc
        /// </summary>
        public int chans;

        /// <summary>
        /// A combination of these flags
        /// </summary>
        public BASSFileFlag flags;

        /// <summary>
        /// The type of channel it is, which can be one of the following.
        /// </summary>
        public int ctype;

        /// <summary>
        /// The original resolution (bits per sample)... 0 = undefined
        /// </summary>
        public int origres;

        /// <summary>
        /// The plugin that is handling the channel... 0 = not using a plugin. Note this is only available with streams created using the plugin system via the standard BASS stream creation functions, not those created by add-on functions. Information on the plugin can be retrieved via BASS_PluginGetInfo.
        /// </summary>
        public int plugin;

        /// <summary>
        /// The sample that is playing on the channel. (HCHANNEL only) 
        /// </summary>
        public int sample;

        /// <summary>
        /// The filename associated with the channel. (HSTREAM only) 
        /// </summary>
        public IntPtr filename;
    }
}
