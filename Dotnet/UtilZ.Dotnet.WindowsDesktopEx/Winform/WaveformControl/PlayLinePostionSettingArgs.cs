using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 播放线位置设置事件参数
    /// </summary>
    public class PlayLinePostionSettingArgs : EventArgs
    {
        /// <summary>
        /// 相对起始时间的偏移时间,毫秒
        /// </summary>
        public double OffsetMilliseconds { get; private set; }

        /// <summary>
        /// 相对起始时间的偏移时间
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="offsetMilliseconds">相对起始时间的偏移时间,毫秒</param>
        /// <param name="time">相对起始时间的偏移时间</param>
        public PlayLinePostionSettingArgs(double offsetMilliseconds, DateTime time)
        {
            this.OffsetMilliseconds = offsetMilliseconds;
            this.Time = time;
        }
    }
}
