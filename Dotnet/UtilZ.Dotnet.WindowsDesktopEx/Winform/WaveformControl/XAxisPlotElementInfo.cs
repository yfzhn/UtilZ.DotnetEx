using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// X坐标图元信息
    /// </summary>
    public class XAxisPlotElementInfo : PlotElementInfoAbs
    {
        /// <summary>
        /// 元素块高度像素
        /// </summary>
        public float Height { get; private set; }

        /// <summary>
        /// 文本字体
        /// </summary>
        public Font Font { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="height">元素块高像素</param>
        /// <param name="backgroudColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="lineWidth">绘制线条宽度</param>
        /// <param name="font">文本字体</param>
        /// <param name="order">绘制顺序,从上往下,依次递增</param>
        public XAxisPlotElementInfo(float height, Brush backgroudColor, SolidBrush foreColor,
            float lineWidth, Font font, int order)
            : base(backgroudColor, foreColor, lineWidth, order)
        {
            this.Height = height;
            this.Font = font;
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
