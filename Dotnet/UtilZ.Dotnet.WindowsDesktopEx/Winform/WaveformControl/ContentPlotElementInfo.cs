using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 内容图元信息
    /// </summary>
    public class ContentPlotElementInfo : PlotElementInfoAbs
    {
        private Pen _playLineChannelPen = new Pen(Color.Yellow, PlotConstant.DEFAULLINE_WIDTH);
        /// <summary>
        /// 获取或设置播放位置线绘制画笔
        /// </summary>
        public Pen PlayLineChannelPen
        {
            get { return _playLineChannelPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _playLineChannelPen.Dispose();
                _playLineChannelPen = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="backgroudColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="lineWidth">绘制线条宽度</param>
        /// <param name="order">绘制顺序,从上往下,依次递增</param>
        public ContentPlotElementInfo(Brush backgroudColor, SolidBrush foreColor, float lineWidth, int order)
            : base(backgroudColor, foreColor, lineWidth, order)
        {

        }

        /// <summary>
        /// 重写Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._playLineChannelPen.Dispose();
        }
    }
}
