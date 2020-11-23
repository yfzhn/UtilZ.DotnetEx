using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 波形图控件-事件
    /// </summary>
    public partial class WaveControl
    {
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



        ///// <summary>
        ///// 获取指定点所对应在UI中的区域
        ///// </summary>
        ///// <param name="point">指定点</param>
        ///// <param name="plotPara"></param>
        ///// <param name="selectedInfo"></param>
        ///// <returns>指定点所对应在UI中的区域</returns>
        //internal UIArea GetUIArea(Point point, WavePlotPara plotPara, SelectedInfo selectedInfo)
        //{
        //    if (this.Inner(point, this._globalView))
        //    {
        //        if (plotPara != null)
        //        {
        //            double unitWidth = this._content.Area.Width / plotPara.DurationMillisecond;
        //            double sbx = unitWidth * plotPara.SBTOMillisecond;
        //            double sex = unitWidth * plotPara.GetSETOMillisecond();
        //            if (point.X - sbx > PlotConstant.ZEROR_D && point.X - sex < PlotConstant.ZEROR_D)
        //            {
        //                return UIArea.GlobalViewZoomDisplay;
        //            }
        //        }

        //        return UIArea.GlobalView;
        //    }

        //    if (this.Inner(point, this._xAxis))
        //    {
        //        return UIArea.TimeArea;
        //    }

        //    if (this.Inner(point, this._content))
        //    {
        //        return UIArea.Wave;
        //    }

        //    return UIArea.Other;
        //}
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
                //this._lastMouseDownMoveLocation = e.Location;
                //this._mouseDownArgs = e;
                //this._mouseDownUIArea = this.GetUIArea(e.Location, this._plotPara, this._selectedInfo);

                //if (e.Button == MouseButtons.Right &&
                //    (this._mouseDownUIArea == UIArea.Wave || this._mouseDownUIArea == UIArea.WaveSelected || this._mouseDownUIArea == UIArea.Voice || this._mouseDownUIArea == UIArea.VoiceSelected))
                //{
                //    SelectedInfo oldSelectedInfo = this._selectedInfo;
                //    if (e.Clicks == 1)
                //    {
                //        //右键单击,取消选择
                //        if (this._selectedInfo == null)
                //        {
                //            return;
                //        }

                //        this._selectedInfo = null;
                //    }
                //    else
                //    {
                //        //右键双击,全选
                //        this._selectedInfo = new SelectedInfo(this._plotPara.BaseTime, 0d, this._plotPara.DurationMillisecond);
                //    }

                //    this.SelectedAreaChangeDraw(oldSelectedInfo);
                //}
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
                //if (this._plotPara == null)
                //{
                //    return;
                //}

                //this._currentMouseArgs = e;
                //UIArea area = this.GetUIArea(this._currentMouseArgs.Location, this._plotPara, this._selectedInfo);
                //this.SetZoomWavMoveMouseStyle(area);

                //switch (area)
                //{
                //    case UIArea.GlobalViewZoomDisplay:
                //        if (e.Button != MouseButtons.Left || !this._zoomInfo.HasZoom())
                //        {
                //            return;
                //        }

                //        //有缩放时,拖动全局视图进行移动,改变放大后显示区域
                //        double moveMillisecond = (e.X - this._lastMouseDownMoveLocation.X) * this._plotPara.DurationMillisecond / this._content.Area.Width;
                //        double sbtoMillisecond = this._plotPara.SBTOMillisecond + moveMillisecond;
                //        double setoMillisecond;
                //        if (sbtoMillisecond < PlotConstant.ZEROR_D)
                //        {
                //            sbtoMillisecond = 0;
                //            setoMillisecond = this._plotPara.GetSETOMillisecond() - this._plotPara.SBTOMillisecond;
                //        }
                //        else
                //        {
                //            setoMillisecond = this._plotPara.GetSETOMillisecond() + moveMillisecond;
                //            if (this._plotPara.DurationMillisecond - setoMillisecond < PlotConstant.ZEROR_D)
                //            {
                //                setoMillisecond = this._plotPara.DurationMillisecond;
                //                sbtoMillisecond = setoMillisecond - (this._plotPara.GetSETOMillisecond() - this._plotPara.SBTOMillisecond);
                //            }
                //        }

                //        this._plotPara.SBTOMillisecond = sbtoMillisecond;
                //        this._plotPara.UpdateSETOMillisecond(setoMillisecond);
                //        this.Resample();
                //        this.PartDraw_ZoomMove();
                //        break;
                //    case UIArea.Wave:
                //    case UIArea.Voice:
                //        if (e.Button != MouseButtons.Right)
                //        {
                //            return;
                //        }

                //        //波形图和语谱图上选中区域改变
                //        int mouseDownX = this._mouseDownArgs.Location.X;
                //        float beginX, endX;
                //        if (e.X < mouseDownX)
                //        {
                //            beginX = e.X;
                //            endX = mouseDownX;
                //        }
                //        else
                //        {
                //            beginX = mouseDownX;
                //            endX = e.X;
                //        }

                //        double setOMillisecond = this._plotPara.GetSETOMillisecond();
                //        double unitTime = (setOMillisecond - this._plotPara.SBTOMillisecond) / this._content.Area.Width;
                //        double beginMillisecond = this._plotPara.SBTOMillisecond + beginX * unitTime;
                //        double endMillisecond = this._plotPara.SBTOMillisecond + endX * unitTime;
                //        SelectedInfo oldSelectedInfo = this._selectedInfo;
                //        this._selectedInfo = new SelectedInfo(this._plotPara.BaseTime, beginMillisecond, endMillisecond);
                //        this.SelectedAreaChangeDraw(oldSelectedInfo);
                //        break;
                //    case UIArea.WaveSelected:
                //    case UIArea.VoiceSelected:
                //    default:
                //        break;
                //}
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
            //switch (area)
            //{
            //    case UIArea.GlobalViewZoomDisplay:
            //        if (this._zoomInfo.HasZoom())
            //        {
            //            this.Cursor = this._zoomWavDisplayAreaMmoveMouseStyle;
            //        }
            //        else
            //        {
            //            this.Cursor = this._defaultCursor;
            //        }
            //        break;
            //    case UIArea.Wave:
            //    case UIArea.Voice:
            //        this.Cursor = this._wavAndVoiceSelecteMouseStyle;
            //        break;
            //    case UIArea.WaveSelected:
            //    case UIArea.VoiceSelected:
            //        this.Cursor = this._wavAndVoiceSelecteMouseStyle;
            //        break;
            //    case UIArea.TimeArea:
            //    default:
            //        this.Cursor = this._defaultCursor;
            //        break;
            //}
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
            ////还原鼠标是否近下标识
            //this._mouseDownArgs = null;
            //if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1)
            //{
            //    var plotPara = this._plotPara;
            //    var handler = this.PlayLinePostionSetting;
            //    if (handler != null && plotPara != null)
            //    {
            //        float posScale = ((float)(e.X - this._content.Area.X)) / this._content.Area.Width;
            //        double sbto = plotPara.SBTOMillisecond;
            //        double seto = plotPara.GetSETOMillisecond();
            //        double timeArea = seto - sbto;
            //        double timeMilliseconds = sbto + posScale * (seto - sbto);
            //        DateTime time = plotPara.BaseTime.AddMilliseconds(timeMilliseconds);
            //        handler(this, new PlayLinePostionSettingArgs(timeMilliseconds, time));
            //    }
            //}

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
                //if (e.Delta > 0)
                //{
                //    this.ZoomIn();
                //}
                //else
                //{
                //    this.ZoomOut();
                //}
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
