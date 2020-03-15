using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Wav.ExBass;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav
{
    // WAV播放控件-系统控件事件
    public partial class WavePlayer
    {
        /// <summary>
        /// 重写OnCreateControl
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            try
            {
                //加载默认目录存放的bass插件
                if (WavePlayer._instanceCount == 1)
                {
                    Bass.BASS_PluginLoad();
                }

                //创建默认双缓冲
                this.CreateBufferedGraphics();

                //初始化
                if (this.DesignMode)
                {
                    return;
                }

                //bass.dll版本验证
                if (this.IsVersionValidate && !Bass.BASS_VersionValidate())
                {
                    MessageBox.Show("bass.dll版本不匹配,可能会导致程序运行异常", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //查找是否有声音驱动
                int iDevice = 0;
                BASS_DEVICEINFO_INTERNAL info = new BASS_DEVICEINFO_INTERNAL();
                for (int i = 0; Bass.BASS_GetDeviceInfo(i, ref info); i++)
                {
                    if (info.driver != IntPtr.Zero && info.flags == (BASSDeviceInfo.BASS_DEVICE_DEFAULT | BASSDeviceInfo.BASS_DEVICE_ENABLED | BASSDeviceInfo.BASS_DEVICE_TYPE_SPEAKERS))
                    {
                        iDevice = -1;
                        break;
                    }
                }

                bool ret = Bass.BASS_Init(iDevice, this._freq, BASSInit.BASS_DEVICE_CPSPEAKERS, this.Handle, IntPtr.Zero);
                if (!ret)
                {
                    this.OnRaiseLog(new Exception("初始化失败" + BassErrorCode.GetErrorInfo()));
                }
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
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
                base.OnSizeChanged(e);

                //更新UI布局区域
                this.UpdateUIArea();

                //重新创建双缓冲
                this.CreateBufferedGraphics();

                //计算bass处于非播放时播放位置指示线X坐标
                this.CalculateWavPlayLocationLine();

                //清空局部刷新区域集合
                this.ClearPartRefreshArea();

                //刷新波形图
                this.RefreshWave(true, true, true, true, true, true);
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
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
                base.OnPaint(e);

                this._grafx.Render(e.Graphics);
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
        }

        /// <summary>
        /// 重写Dispose
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            //释放资源
            try
            {
                //停止播放
                this.Stop();

                //释放一个实例则实例数减1
                System.Threading.Interlocked.Decrement(ref WavePlayer._instanceCount);

                //释放加载的插件
                if (WavePlayer._instanceCount == 0)
                {
                    Bass.BASS_PluginFree(0);
                }

                //主波形区域背景画刷
                this._wavAreaBackgroundBrush.Dispose();

                //主波形选中区域背景画刷
                this._seleactionAreaBrush.Dispose();

                //左声道波形绘制画笔
                this._leftChannelPen.Dispose();

                //右声道波形绘制画笔
                this._rightChannelPen.Dispose();

                //波形中线绘制画笔
                this._wavMidLinePen.Dispose();

                //声道分隔线绘制画笔
                this._channelDivideLinePen.Dispose();

                //播放位置线绘制画笔
                this._playLineChannelPen.Dispose();

                //时间绘制画笔
                this._timePen.Dispose();

                //幅度绘制画笔
                this._dbPen.Dispose();

                //缩略波形图背景画刷
                this._zoomBackgroundBrush.Dispose();

                //缩略波形图选中区域背景画刷
                this._zoomSelectedBackgroundBrush.Dispose();

                //显示波形区域在缩略波形中的对应的区域背景画刷
                this._zoomDisplayAreaBrush.Dispose();

                //显示波形区域在缩略波形中与选中的区域重叠背景画刷
                this._zoomDisplaySelectedOverlapAreaBrush.Dispose();

                //显示波形区域在缩略波形中的对应的区域画笔
                this._zoomDisplayAreaPen.Dispose();

                //时间背景画刷
                this._timeBackgroundBrush.Dispose();

                //幅度背景画刷
                this._dbBackgroundBrush.Dispose();

                //缩略左声道波形绘制画笔
                this._zoomLeftChannelPen.Dispose();

                //缩略右声道波形绘制画笔
                this._zoomRightChannelPen.Dispose();

                //Logo背景画刷
                this._logoBackgroundBrush.Dispose();

                //绘制刻度文本的Brush
                this._fontBrush.Dispose();

                //用于绘制刻度值的字体
                this._stringFont.Dispose();
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
        }

        #region 鼠标事件
        /// <summary>
        /// 鼠标进入到本控件时的样式
        /// </summary>
        private Cursor _defaultCursor;

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
        /// 重写OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                this._currentMouseArgs = e;
                UIArea area = this.GetUIArea(this._currentMouseArgs.Location);
                switch (area)
                {
                    case UIArea.ZoomDisplayArea:
                    case UIArea.ZoomArea:
                        //设置鼠标样式
                        this.SetZoomWavMoveMouseStyle(area);

                        //计算新的显示区域
                        if (this._mouseDownArgs != null
                            && this._mouseDownArgs.Button == System.Windows.Forms.MouseButtons.Left
                            && this._mouseDownArgs.Clicks == 1
                            && this._mouseDownUIArea == UIArea.ZoomDisplayArea
                            && this._srcLeftChannelDataLength != 0)
                        {
                            int offset = (e.X - this._lastMouseDownMoveLocation.X) * this._srcLeftChannelDataLength / this._zoomArea.Width;

                            //缩略波形对应主波形中显示段改变
                            this.ZoomWavDisplaySegmentAreaChange(offset);

                            //更新主波形播放位置指示线上次指示线所在的位置是否在主波形显示段内
                            this.UpdateWavPlayLocationLingLastInArea();
                        }

                        break;
                    case UIArea.WavArea:
                        //在主波形区域内,在该区域内有缩放波形、选中波形区域、跳转播放位置、右键菜单等操作,所以设置鼠标光标样式为可选中的样式
                        if (this.Cursor != this._wavSelecteMouseStyle)
                        {
                            this.Cursor = this._wavSelecteMouseStyle;
                        }

                        //更新缩略波形区域
                        if (this._mouseDownArgs != null
                            && this._mouseDownArgs.Button == System.Windows.Forms.MouseButtons.Right
                            && this._mouseDownUIArea == UIArea.WavArea)
                        {
                            int mouseDownX = this._mouseDownArgs.Location.X;
                            float x, sectedAreaWidth;
                            if (e.X < mouseDownX)
                            {
                                x = e.X;
                                sectedAreaWidth = mouseDownX - e.X;
                            }
                            else
                            {
                                x = mouseDownX;
                                sectedAreaWidth = e.X - mouseDownX;
                            }

                            //计算主波形选中区域起始结束索引
                            this.CalculateWavSelectedAreaIndex(x, sectedAreaWidth);

                            //刷新波形选中区域改变
                            this.RefreshWavSelectedChange(x, sectedAreaWidth);
                        }

                        break;
                    //case UIArea.TimeArea:
                    //    break;
                    //case UIArea.DbArea:
                    //    break;
                    default:
                        if (this.Cursor != this._defaultCursor)
                        {
                            this.Cursor = this._defaultCursor;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
            finally
            {
                this._lastMouseDownMoveLocation = e.Location;
                base.OnMouseMove(e);
            }
        }

        /// <summary>
        /// 设置缩略波形图平移鼠标样式
        /// </summary>
        /// <param name="area">鼠标所在区域</param>
        private void SetZoomWavMoveMouseStyle(UIArea area)
        {
            if (this._wavArea.Width == this._zoomDisplayArea.Width)
            {
                //如果当前缩略波形全部为显示区域,则不作处理,不允许移动
                if (this.Cursor == this._zoomWavDisplayAreaMmoveMouseStyle)
                {
                    this.Cursor = this._defaultCursor;
                }

                return;
            }

            if (area == UIArea.ZoomDisplayArea && this.Cursor != this._zoomWavDisplayAreaMmoveMouseStyle)
            {
                //在缩略波形区域内,在该区域内可左右移动显示的波形区域,所以设置鼠标光标样式为可移动的样式
                this.Cursor = this._zoomWavDisplayAreaMmoveMouseStyle;
            }
            else if (area == UIArea.ZoomArea && this.Cursor != this._defaultCursor)
            {
                this.Cursor = this._defaultCursor;
            }
        }

        /// <summary>
        /// 刷新波形选中区域改变
        /// </summary>
        /// <param name="x">主波形象选中区域起始X</param>
        /// <param name="sectedAreaWidth">选中区域宽度</param>
        private void RefreshWavSelectedChange(float x, float sectedAreaWidth)
        {
            //主波形选中区域改变,则局部刷新缩略波形选中区域+主波形选中区域
            bool refreshFlag = this.AddPartRefreshArea(this._wavSelectedArea);//主波形旧的选中区域
            refreshFlag |= this.AddPartRefreshArea(this._zoomSelectedArea);//缩略波形旧的选中区域

            //更新缩略波形选中区域
            this.UpdateSelectedArea(x, sectedAreaWidth);

            //新的更新区域
            refreshFlag |= this.AddPartRefreshArea(this._wavSelectedArea);//主波形新的选中区域
            refreshFlag |= this.AddPartRefreshArea(this._zoomSelectedArea);//缩略波形新的选中区域

            //刷新波形
            if (refreshFlag)
            {
                this.RefreshWave(true, false, false, false, false, false);
            }
        }

        /// <summary>
        /// 更新主波形播放位置指示线上次指示线所在的位置是否在主波形显示段内
        /// </summary>
        private void UpdateWavPlayLocationLingLastInArea()
        {
            //更新主波形播放位置指示线上次指示线所在的位置是否在主波形显示段内
            long position = Bass.BASS_ChannelGetPosition(this._handle, BASSMode.BASS_POS_BYTE);
            float wavLocation = (float)position / (this._channelInfo.chans * WavePlayer.OFFSETPARA);
            this._isWavPlayLocationLingLastInArea = wavLocation >= this._ws && wavLocation <= this._we;
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
                this._mouseDownUIArea = this.GetUIArea(e.Location);

                //在主波形区域中键双击全选主波形区域
                if (this._mouseDownUIArea == UIArea.WavArea
                    && e.Button == System.Windows.Forms.MouseButtons.Middle && e.Clicks == 1
                    && this._bassDataTotalLength > 0)
                {
                    //全选
                    this._ss = this._ws;
                    this._se = this._we;

                    //刷新波形选中区域改变
                    this.RefreshWavSelectedChange(this._wavArea.X, this._wavArea.Width);
                }
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
            finally
            {
                base.OnMouseDown(e);
            }
        }

        /// <summary>
        /// 重写OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                UIArea area = this.GetUIArea(e.Location);
                switch (area)
                {
                    case UIArea.ZoomArea:

                        break;
                    case UIArea.WavArea:
                        if (this._mouseDownArgs != null && this._mouseDownArgs.Location == e.Location)
                        {
                            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                            {
                                //计算主波形选中区域起始结束索引
                                this.CalculateWavSelectedAreaIndex(0, 0);

                                //添加更新区域
                                bool refreshFlag = this.AddPartRefreshArea(this._wavSelectedArea);
                                refreshFlag |= this.AddPartRefreshArea(this._zoomSelectedArea);

                                //更新波形选中区域
                                this.UpdateSelectedArea(0, 0);

                                //添加更新区域
                                refreshFlag |= this.AddPartRefreshArea(this._wavSelectedArea);
                                refreshFlag |= this.AddPartRefreshArea(this._zoomSelectedArea);

                                //刷新
                                if (refreshFlag)
                                {
                                    this.RefreshWave(true, false, false, false, false, false);
                                }
                            }
                            else if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1 && this.GrtPlayState() == PlayStatus.PLAYING)
                            {
                                //播放位置跳转
                                //播放位置对应当前播放段的比例
                                float pscale = ((float)(e.X - this._wavArea.X)) / this._wavArea.Width;
                                long bytePosition = (long)((this._we - this._ws) * pscale) + this._ws;
                                long pos = bytePosition * this._channelInfo.chans * WavePlayer.OFFSETPARA;
                                float wavLocation = (float)pos / (this._channelInfo.chans * WavePlayer.OFFSETPARA);

                                //如果当前有开启环听,则当跳转的位置不在选中的区域范围内,则不允许跳转
                                if (this._isRingHear && (wavLocation >= this._se || wavLocation <= this._ss))
                                {
                                    return;
                                }

                                //设置播放位置
                                Bass.BASS_ChannelSetPosition(this._handle, pos, BASSMode.BASS_POS_BYTE);

                                //将当前的播放位置指示线区域添加为需要更新的区域
                                this.AddPartRefreshArea(new RectangleF(this._wavPlayLineX, this._wavArea.Y, this._playLineChannelPen.Width, this._wavArea.Height));
                                this.AddPartRefreshArea(new RectangleF(this._zoomPlayLineX, this._zoomArea.Y, this._playLineChannelPen.Width, this._zoomArea.Height));
                            }
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
            finally
            {
                //还原鼠标是否近下标识
                this._mouseDownArgs = null;
                base.OnMouseUp(e);
            }
        }

        /// <summary>
        /// 重写OnMouseLeave
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            try
            {
                this.Cursor = this._defaultCursor;
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
            finally
            {
                base.OnMouseLeave(e);
            }
        }

        /// <summary>
        /// 重写OnMouseEnter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            try
            {
                this._defaultCursor = this.Cursor;
                this.Select();
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
            finally
            {
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// 重写OnMouseWheel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                Zoom(e.Delta > 0);
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
            finally
            {
                base.OnMouseWheel(e);
            }
        }
        #endregion
    }
}
