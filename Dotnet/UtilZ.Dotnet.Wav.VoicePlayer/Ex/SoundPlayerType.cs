using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Ex
{
    /// <summary>
    /// 声音播放器类型
    /// </summary>
    public enum SoundPlayerType
    {
        /// <summary>
        /// 基于本地文件
        /// </summary>
        File,

        /// <summary>
        /// 基于数据流
        /// </summary>
        Stream,

        ///// <summary>
        ///// 基于网络url
        ///// </summary>
        //Url
    }
}
