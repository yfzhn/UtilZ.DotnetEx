using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
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
                //base.OnPaint(e);
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
                this._grafx.Graphics.Dispose();
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






        #region 鼠标操作事件
        /// <summary>
        /// 鼠标进入到本控件时的样式
        /// </summary>
        private Cursor _defaultCursor = null;

        /// <summary>
        /// 波形或语谱图可选中时鼠标样式
        /// </summary>
        private Cursor _wavAndVoiceSelecteMouseStyle = Cursors.IBeam;

        /// <summary>
        /// 全局视图中-缩放操作后显示区域移动鼠标样式
        /// </summary>
        private Cursor _zoomWavDisplayAreaMmoveMouseStyle = Cursors.SizeAll;

        /// <summary>
        /// 鼠标按下时的事件参数
        /// </summary>
        private MouseEventArgs _mouseDownArgs = null;

        /// <summary>
        /// 鼠标按下后,上次移动完的坐标
        /// </summary>
        private Point _lastMouseDownMoveLocation;

        /// <summary>
        /// 鼠标按下时所在区域
        /// </summary>
        private UIArea _mouseDownUIArea;

        /// <summary>
        /// 当前鼠标参数
        /// </summary>
        private MouseEventArgs _currentMouseArgs;



        /// <summary>
        /// 获取指定点所对应在UI中的区域
        /// </summary>
        /// <param name="point">指定点</param>
        /// <param name="plotPara"></param>
        /// <param name="selectedInfo"></param>
        /// <returns>指定点所对应在UI中的区域</returns>
        internal UIArea GetUIArea(Point point, WavePlotPara plotPara, SelectedInfo selectedInfo)
        {
            if (this.Inner(point, this._globalView))
            {
                if (plotPara != null)
                {
                    double unitWidth = this._content.Area.Width / plotPara.DurationMillisecond;
                    double sbx = unitWidth * plotPara.SBTOMillisecond;
                    double sex = unitWidth * plotPara.GetSETOMillisecond();
                    if (point.X - sbx > PlotConstant.ZEROR_D && point.X - sex < PlotConstant.ZEROR_D)
                    {
                        return UIArea.GlobalViewZoomDisplay;
                    }
                }

                return UIArea.GlobalView;
            }

            if (this.Inner(point, this._xAxis))
            {
                return UIArea.TimeArea;
            }

            if (this.Inner(point, this._content))
            {
                return UIArea.Wave;
            }

            return UIArea.Other;
        }
        private bool Inner(Point point, PlotElementInfoAbs plotElementInfo)
        {
            if (plotElementInfo == null)
            {
                return false;
            }

            if (point.Y <= plotElementInfo.Area.Bottom && point.Y >= plotElementInfo.Area.Top && point.X >= plotElementInfo.Area.Left && point.X <= plotElementInfo.Area.Right)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 重写OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            try
            {
                this._lastMouseDownMoveLocation = e.Location;
                this._mouseDownArgs = e;
                this._mouseDownUIArea = this.GetUIArea(e.Location, this._plotPara, this._selectedInfo);

                if (e.Button == MouseButtons.Right &&
                    (this._mouseDownUIArea == UIArea.Wave || this._mouseDownUIArea == UIArea.WaveSelected || this._mouseDownUIArea == UIArea.Voice || this._mouseDownUIArea == UIArea.VoiceSelected))
                {
                    SelectedInfo oldSelectedInfo = this._selectedInfo;
                    if (e.Clicks == 1)
                    {
                        //右键单击,取消选择
                        if (this._selectedInfo == null)
                        {
                            return;
                        }

                        this._selectedInfo = null;
                    }
                    else
                    {
                        //右键双击,全选
                        this._selectedInfo = new SelectedInfo(this._plotPara.BaseTime, 0d, this._plotPara.DurationMillisecond);
                    }

                    this.SelectedAreaChangeDraw(oldSelectedInfo);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            finally
            {
                base.OnMouseDown(e);
            }
        }

        /// <summary>
        /// 重写OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if (this._plotPara == null)
                {
                    return;
                }

                this._currentMouseArgs = e;
                UIArea area = this.GetUIArea(this._currentMouseArgs.Location, this._plotPara, this._selectedInfo);
                this.SetZoomWavMoveMouseStyle(area);

                switch (area)
                {
                    case UIArea.GlobalViewZoomDisplay:
                        if (e.Button != MouseButtons.Left || !this._zoomInfo.HasZoom())
                        {
                            return;
                        }

                        //有缩放时,拖动全局视图进行移动,改变放大后显示区域
                        double moveMillisecond = (e.X - this._lastMouseDownMoveLocation.X) * this._plotPara.DurationMillisecond / this._content.Area.Width;
                        double sbtoMillisecond = this._plotPara.SBTOMillisecond + moveMillisecond;
                        double setoMillisecond;
                        if (sbtoMillisecond < PlotConstant.ZEROR_D)
                        {
                            sbtoMillisecond = 0;
                            setoMillisecond = this._plotPara.GetSETOMillisecond() - this._plotPara.SBTOMillisecond;
                        }
                        else
                        {
                            setoMillisecond = this._plotPara.GetSETOMillisecond() + moveMillisecond;
                            if (this._plotPara.DurationMillisecond - setoMillisecond < PlotConstant.ZEROR_D)
                            {
                                setoMillisecond = this._plotPara.DurationMillisecond;
                                sbtoMillisecond = setoMillisecond - (this._plotPara.GetSETOMillisecond() - this._plotPara.SBTOMillisecond);
                            }
                        }

                        this._plotPara.SBTOMillisecond = sbtoMillisecond;
                        this._plotPara.UpdateSETOMillisecond(setoMillisecond);
                        this.Resample();
                        this.PartDraw_ZoomMove();
                        break;
                    case UIArea.Wave:
                    case UIArea.Voice:
                        if (e.Button != MouseButtons.Right)
                        {
                            return;
                        }

                        //波形图和语谱图上选中区域改变
                        int mouseDownX = this._mouseDownArgs.Location.X;
                        float beginX, endX;
                        if (e.X < mouseDownX)
                        {
                            beginX = e.X;
                            endX = mouseDownX;
                        }
                        else
                        {
                            beginX = mouseDownX;
                            endX = e.X;
                        }

                        double setOMillisecond = this._plotPara.GetSETOMillisecond();
                        double unitTime = (setOMillisecond - this._plotPara.SBTOMillisecond) / this._content.Area.Width;
                        double beginMillisecond = this._plotPara.SBTOMillisecond + beginX * unitTime;
                        double endMillisecond = this._plotPara.SBTOMillisecond + endX * unitTime;
                        SelectedInfo oldSelectedInfo = this._selectedInfo;
                        this._selectedInfo = new SelectedInfo(this._plotPara.BaseTime, beginMillisecond, endMillisecond);
                        this.SelectedAreaChangeDraw(oldSelectedInfo);
                        break;
                    case UIArea.WaveSelected:
                    case UIArea.VoiceSelected:
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            finally
            {
                this._lastMouseDownMoveLocation = e.Location;
                base.OnMouseMove(e);
            }
        }

        private void SetZoomWavMoveMouseStyle(UIArea area)
        {
            //Point location = this.PointToClient(Control.MousePosition);
            //Loger.Info(area.ToString());
            switch (area)
            {
                case UIArea.GlobalViewZoomDisplay:
                    if (this._zoomInfo.HasZoom())
                    {
                        this.Cursor = this._zoomWavDisplayAreaMmoveMouseStyle;
                    }
                    else
                    {
                        this.Cursor = this._defaultCursor;
                    }
                    break;
                case UIArea.Wave:
                case UIArea.Voice:
                    this.Cursor = this._wavAndVoiceSelecteMouseStyle;
                    break;
                case UIArea.WaveSelected:
                case UIArea.VoiceSelected:
                    this.Cursor = this._wavAndVoiceSelecteMouseStyle;
                    break;
                case UIArea.TimeArea:
                default:
                    this.Cursor = this._defaultCursor;
                    break;
            }
        }


        /// <summary>
        /// 播放线位置设置事件
        /// </summary>
        public event EventHandler<PlayLinePostionSettingArgs> PlayLinePostionSetting;

        /// <summary>
        /// 重写OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            //还原鼠标是否近下标识
            this._mouseDownArgs = null;
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1)
            {
                var plotPara = this._plotPara;
                var handler = this.PlayLinePostionSetting;
                if (handler != null && plotPara != null)
                {
                    float posScale = ((float)(e.X - this._content.Area.X)) / this._content.Area.Width;
                    double sbto = plotPara.SBTOMillisecond;
                    double seto = plotPara.GetSETOMillisecond();
                    double timeArea = seto - sbto;
                    double timeMilliseconds = sbto + posScale * (seto - sbto);
                    DateTime time = plotPara.BaseTime.AddMilliseconds(timeMilliseconds);
                    handler(this, new PlayLinePostionSettingArgs(timeMilliseconds, time));
                }
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// 重写OnMouseEnter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            this._defaultCursor = this.Cursor;
            this.Select();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// 重写OnMouseLeave
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            this.Cursor = this._defaultCursor;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 重写OnMouseWheel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    this.ZoomIn();
                }
                else
                {
                    this.ZoomOut();
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            finally
            {
                base.OnMouseWheel(e);
            }
        }
        #endregion
    }
}
