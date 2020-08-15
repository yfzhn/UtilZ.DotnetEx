using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    ///全局视图图元信息
    /// </summary>
    public class GlobalViewPlotElementInfo : PlotElementInfoAbs
    {
        /// <summary>
        /// 全局视图高度像素
        /// </summary>
        public float Height { get; private set; }


        private Brush _globalViewZoomAreaBackBrush = new SolidBrush(Color.FromArgb(131, 32, 115));
        /// <summary>
        /// 获取或设置显示波形区域在缩略波形中的对应的区域背景画刷
        /// </summary>
        public Brush GlobalViewZoomAreaBackBrush
        {
            get { return this._globalViewZoomAreaBackBrush; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._globalViewZoomAreaBackBrush.Dispose();
                this._globalViewZoomAreaBackBrush = value;
            }
        }

        private Pen _globalViewZoomAreaPen = new Pen(new SolidBrush(Color.FromArgb(131, 32, 115)), PlotConstant.DEFAULLINE_WIDTH);
        /// <summary>
        /// 获取或设置显示波形区域在缩略波形中的对应的区域背景画刷
        /// </summary>
        public Pen GlobalViewZoomAreaPen
        {
            get { return this._globalViewZoomAreaPen; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._globalViewZoomAreaPen.Dispose();
                this._globalViewZoomAreaPen = value;
            }
        }

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
        /// <param name="height">全局视图高度像素</param>
        /// <param name="backgroudColor">背景色</param>
        /// <param name="foreColor">前景色</param>
        /// <param name="lineWidth">绘制线条宽度</param>
        /// <param name="order">绘制顺序,从上往下,依次递增</param>
        public GlobalViewPlotElementInfo(float height, Brush backgroudColor, SolidBrush foreColor, float lineWidth, int order)
            : base(backgroudColor, foreColor, lineWidth, order)
        {
            this.Height = height;
        }



        /// <summary>
        /// 重写Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this._globalViewZoomAreaBackBrush.Dispose();
            this._globalViewZoomAreaPen.Dispose();
            this._playLineChannelPen.Dispose();
        }
    }
}
