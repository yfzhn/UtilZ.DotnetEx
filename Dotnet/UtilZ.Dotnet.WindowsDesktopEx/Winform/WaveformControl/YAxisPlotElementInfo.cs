using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// Y坐标图元信息
    /// </summary>
    public class YAxisPlotElementInfo : PlotElementInfoAbs
    {
        /// <summary>
        /// 元素块宽度像素
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// 文本字体
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// 坐标轴依靠方向
        /// </summary>
        public DockStyle Dock { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="width">元素块宽度像素</param>
        /// <param name="backgroudColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="lineWidth">绘制线条宽度</param>
        /// <param name="font">文本字体</param>
        /// <param name="dock">坐标轴依靠方向</param>
        public YAxisPlotElementInfo(float width, Brush backgroudColor, SolidBrush foreColor,
            float lineWidth, Font font, DockStyle dock)
            : base(backgroudColor, foreColor, lineWidth, 0)
        {
            this.Width = width;
            this.Font = font;
            this.Dock = dock;
        }


        /// <summary>
        /// 重写Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            base.PrimitiveDisposable(this.Font);
        }
    }
}
