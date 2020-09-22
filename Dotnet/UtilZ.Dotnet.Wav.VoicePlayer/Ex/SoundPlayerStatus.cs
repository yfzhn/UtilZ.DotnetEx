using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 声道播放器状态
    /// </summary>
    public enum SoundPlayerStatus
    {
        /// <summary>
        /// 启动播放中
        /// </summary>
        StartPlaying,

        /// <summary>
        /// 正在播放
        /// </summary>
        Playing,

        /// <summary>
        /// 执行暂停中
        /// </summary>
        Pausing,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused,

        /// <summary>
        /// 执行停止中
        /// </summary>
        Stoping,

        /// <summary>
        /// 已停止
        /// </summary>
        Stoped,
    }
}
