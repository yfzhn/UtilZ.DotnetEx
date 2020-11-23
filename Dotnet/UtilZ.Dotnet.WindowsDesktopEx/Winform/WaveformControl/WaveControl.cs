using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 波形图控件
    /// </summary>
    public partial class WaveControl : Control
    {
        /// <summary>
        /// 图形双缓冲缓冲区对象
        /// </summary>
        private BufferedGraphics _grafx = null;

       

        /// <summary>
        /// 构造函数
        /// </summary>
        public WaveControl()
        {
            //设置绘制样式
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            this.Init();

            //大小
            this.Size = new System.Drawing.Size(200, 130);
            //最小大小
            this.MinimumSize = this.Size;
        }

        private void Init()
        {
            int order = 1;

            base.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);

            //全局视图图元信息
            this._globalView = new GlobalViewPlotElementInfo(25f, new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)),
            new SolidBrush(System.Drawing.Color.FromArgb(84, 217, 150)), PlotConstant.DEFAULLINE_WIDTH, order++)
            { AreaType = AreaType.GlobalView };

            //x轴(时间)坐标图元信息
            this._xAxis = new XAxisPlotElementInfo(25f,
                 new SolidBrush(System.Drawing.Color.FromArgb(23, 23, 23)),
                 new SolidBrush(System.Drawing.Color.FromArgb(127, 113, 90)),
                 PlotConstant.DEFAULLINE_WIDTH, new Font("MS UI Gothic", 8), order++)
            { AreaType = AreaType.AxisX };

            //y坐标图元信息
            this._yAxis = new YAxisPlotElementInfo(30f,
                 new SolidBrush(System.Drawing.Color.FromArgb(23, 23, 23)),
                 new SolidBrush(System.Drawing.Color.FromArgb(127, 113, 90)),
                 PlotConstant.DEFAULLINE_WIDTH, new Font("MS UI Gothic", 8), DockStyle.Right)
            { AreaType = AreaType.AxisY };


            //内容图元信息
            this._content = new ContentPlotElementInfo(new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)),
                new SolidBrush(System.Drawing.Color.FromArgb(84, 217, 150)), PlotConstant.DEFAULLINE_WIDTH, order++)
            { AreaType = AreaType.Content };
        }

        private void UpdateDrawAreaActualSize()
        {
            var globalView = this._globalView;
            var xAxis = this._xAxis;
            var yAxis = this._yAxis;
            var content = this._content;

            float contentHeight = this.Height;
            var plotElementInfoList = new List<PlotElementInfoAbs>();
            plotElementInfoList.Add(content);
            if (globalView != null)
            {
                plotElementInfoList.Add(globalView);
                contentHeight -= globalView.Height;
            }

            if (xAxis != null)
            {
                plotElementInfoList.Add(xAxis);
                contentHeight -= xAxis.Height;
            }

            plotElementInfoList = plotElementInfoList.OrderBy(t => { return t.Order; }).ToList();

            float contentX = 0f;
            float contentWidth = this.Width;


            if (yAxis != null)
            {
                contentWidth = this.Width - yAxis.Width;
                if (yAxis.Dock == DockStyle.Left)
                {
                    contentX = yAxis.Width;
                }
            }


            float y = 0f;
            foreach (var plotElementInfo in plotElementInfoList)
            {
                switch (plotElementInfo.AreaType)
                {
                    case AreaType.GlobalView:
                        globalView.Area = new RectangleF(contentX, y, contentWidth, globalView.Height);
                        y += globalView.Height;
                        break;
                    case AreaType.AxisX:
                        xAxis.Area = new RectangleF(contentX, y, contentWidth, xAxis.Height);
                        y += xAxis.Height;
                        break;
                    case AreaType.Content:
                        content.Area = new RectangleF(contentX, y, contentWidth, contentHeight);
                        y += contentHeight;
                        break;
                    default:
                        throw new NotSupportedException(plotElementInfo.AreaType.ToString());
                }
            }

            if (yAxis != null)
            {
                if (yAxis.Dock == DockStyle.Left)
                {
                    yAxis.Area = new RectangleF(0f, content.Area.Y, yAxis.Width, contentHeight);
                }
                else
                {
                    yAxis.Area = new RectangleF(contentWidth, content.Area.Y, yAxis.Width, contentHeight);
                }
            }
        }

        /// <summary>
        /// 重写OnSizeChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            try
            {
                //重新创建双缓冲
                this.CreateBufferedGraphics();
                this.UpdateDrawAreaActualSize();
                this.SizeChangedResample();
                this.AllDraw();
                base.OnSizeChanged(e);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 创建双缓冲绘图
        /// </summary>
        private void CreateBufferedGraphics()
        {
            // Sets the maximum size for the primary graphics buffer
            // of the buffered graphics context for the application
            // domain.  Any allocation requests for a buffer larger 
            // than this will create a temporary buffered graphics 
            // context to host the graphics buffer.
            BufferedGraphicsManager.Current.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            BufferedGraphics grafx = null;
            //当宽度或高度为0时,缓冲区创建会报系统异常
            if (this.Width != 0 && this.Height != 0)
            {
                // Allocates a graphics buffer the size of this form
                // using the pixel format of the Graphics created by 
                // the Form.CreateGraphics() method, which returns a 
                // Graphics object that matches the pixel format of the form.
                //BufferedGraphics grafx = BufferedGraphicsManager.Current.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
                grafx = BufferedGraphicsManager.Current.Allocate(Graphics.FromHwnd(this.Handle), new Rectangle(0, 0, this.Width, this.Height));
                grafx.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                grafx.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            }

            if (this._grafx != null)
            {
                this._grafx.Graphics.Dispose();
                this._grafx.Dispose();
            }

            this._grafx = grafx;
        }

        /// <summary>
        /// 重写OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                this._grafx.Render(e.Graphics);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 重写Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this._grafx != null)
            {
                this._grafx.Dispose();
                this._grafx = null;
            }

            this.PrimitiveDisposable(this._xAxis);
            this.PrimitiveDisposable(this._yAxis);
            this.PrimitiveDisposable(this._content);
            this.PrimitiveDisposable(this._channelSeparatorPen);
            this.PrimitiveDisposable(this._seleactionAreaBrush);
            this.PrimitiveDisposable(this._seleactionGlobalViewAreaBrush);
        }

        private void PrimitiveDisposable(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// 重写OnPaintBackground
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //配合构造函数中this.DoubleBuffered = true;解决闪屏问题
            return;
        }
    }
}
