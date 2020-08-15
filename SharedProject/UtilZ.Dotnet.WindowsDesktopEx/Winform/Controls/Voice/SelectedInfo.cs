using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 鼠标选中信息
    /// </summary>
    internal class SelectedInfo
    {
        /// <summary>
        /// 总数据起始时间
        /// </summary>
        public DateTime PlotBeginTime { get; private set; }

        /// <summary>
        /// 起始总时长,单位/毫秒
        /// </summary>
        public double BeginMillisecond { get; set; }

        /// <summary>
        /// 结束时间,单位/毫秒
        /// </summary>
        public double EndMillisecond { get; set; }

        public SelectedInfo(DateTime plotBeginTime, double beginMillisecond, double endMillisecond)
        {
            this.PlotBeginTime = plotBeginTime;
            this.BeginMillisecond = beginMillisecond;
            this.EndMillisecond = endMillisecond;
        }


        public RectangleF? LastGlobalViewSelectedArea { get; set; } = null;
        public RectangleF? LastWaveSelectedArea { get; set; } = null;
    }
}
