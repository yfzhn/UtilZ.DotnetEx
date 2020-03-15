using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// 播放状态枚举
    /// </summary>
    public enum PlayStatus
    {
        /// <summary>
        /// 停止播放[The channel is not active, or handle is not a valid channel.]
        /// </summary>
        STOPPED = 0,

        /// <summary>
        /// 播放[The channel is playing (or recording). ]
        /// </summary>
        PLAYING = 1,

        /// <summary>
        /// 已停止[Playback of the stream has been stalled due to a lack of sample data. The playback will automatically resume once there is sufficient data to do so]
        /// </summary>
        STALLED = 2,

        /// <summary>
        /// 暂停播放[The channel is paused]
        /// </summary>
        PAUSED = 3
    }
}
