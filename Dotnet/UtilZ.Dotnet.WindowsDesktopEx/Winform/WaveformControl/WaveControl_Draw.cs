using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 波形图控件-绘制
    /// </summary>
    public partial class WaveControl
    {
        /// <summary>
        /// 当前局部刷新区域集合
        /// </summary>
        private readonly List<Rectangle> _partRefreshAreas = new List<Rectangle>();

        /// <summary>
        /// 添加局部刷新区域[添加成功返回true,失败返回false]
        /// </summary>
        /// <param name="plotElementInfo">需要局部刷新的矩形区域</param>
        private void AddPartRefreshArea(PlotElementInfoAbs plotElementInfo)
        {
            if (plotElementInfo == null)
            {
                return;
            }

            this.AddPartRefreshArea(plotElementInfo.Area);
        }

        /// <summary>
        /// 添加局部刷新区域[添加成功返回true,失败返回false]
        /// </summary>
        /// <param name="reg">需要局部刷新的矩形区域</param>
        /// <returns>添加成功返回true,失败返回false</returns>
        private bool AddPartRefreshArea(RectangleF? reg)
        {
            if (reg.HasValue)
            {
                return this.AddPartRefreshArea(reg.Value);
            }

            return false;
        }

        /// <summary>
        /// 添加局部刷新区域[添加成功返回true,失败返回false]
        /// </summary>
        /// <param name="reg">需要局部刷新的矩形区域</param>
        /// <returns>添加成功返回true,失败返回false</returns>
        private bool AddPartRefreshArea(RectangleF reg)
        {
            if (reg.Width < PlotConstant.PRECISION || reg.Height < PlotConstant.PRECISION ||
                reg.X + reg.Width < PlotConstant.PRECISION || reg.X > this.Width ||
                reg.Y + reg.Height < PlotConstant.PRECISION || reg.Y > this.Height)
            {
                //不在当前控件范围内,无视
                return false;
            }

            //新建个矩形用于完全覆盖要更新的区域
            this._partRefreshAreas.Add(new Rectangle((int)reg.X - 1, (int)reg.Y - 1, (int)reg.Width + 3, (int)reg.Height + 3));
            return true;
        }

        private void RefreshInvalidateArea()
        {
            if (this._partRefreshAreas.Count == 0)
            {
                //刷新控件
                this.Refresh();
            }
            else
            {
                //局部刷新
                foreach (Rectangle reg in this._partRefreshAreas)
                {
                    this.Invalidate(reg);
                }

                //清空局部刷新区域集合
                this._partRefreshAreas.Clear();

                //更新区域
                this.Update();
            }
        }










        /// <summary>
        /// 全部绘制
        /// </summary>
        private void AllDraw()
        {
            if (this._grafx == null)
            {
                return;
            }

            //全部绘制
            Graphics graphics = this._grafx.Graphics;

            //清空所有已绘制的图形
            graphics.Clear(this.BackColor);

            VoicePlotData plotPara = this._plotPara;
            if (plotPara == null)
            {
                this.RefreshInvalidateArea();//刷新
                return;
            }

            this.DrawGlobalView(graphics, plotPara);//绘制全局视图
            //this.DrawTimeAxis(graphics, plotPara);//绘制X轴
            //this.DrawWaveSpecturum(graphics, plotPara);//绘制波形图
            //this.DrawWaveSpecturumAxis(graphics, plotPara);//绘制Y轴
            this.RefreshInvalidateArea();//刷新
        }


















        private RectangleF? DrawGlobalView(Graphics graphics, VoicePlotData plotData)
        {
            GlobalViewPlotElementInfo globalView = this._globalView;
            if (globalView == null || !plotData.HasChannelData)
            {
                return null;
            }

            //填充整体视图波形背景
            if (globalView.BackgroudColor != null)
            {
                graphics.FillRectangle(globalView.BackgroudColor, globalView.Area);
            }

            //整体视图中缩放后显示区域
            RectangleF? globalViewZoomArea = null;
            if (this._zoomInfo.HasZoom())
            {
                float x1 = (float)(globalView.Area.Width * plotData.SBTOMillisecond / plotData.DurationMillisecond);
                float x2 = (float)(globalView.Area.Width * plotData.GetSETOMillisecond() / plotData.DurationMillisecond);
                globalViewZoomArea = new RectangleF(x1, globalView.Area.Y, x2 - x1, globalView.Area.Height);
                graphics.FillRectangle(globalView.GlobalViewZoomAreaBackBrush, globalViewZoomArea.Value);
            }

            RectangleF? wavSelectedArea = this.GetSelectedAreaBackground(globalView.Area, plotData, PlotConstant.ZEROR_D, plotData.DurationMillisecond);
            if (wavSelectedArea.HasValue)
            {
                //填充选中背景
                graphics.FillRectangle(this._seleactionGlobalViewAreaBrush, wavSelectedArea.Value);
            }

            //绘制整体视图波形
            this.DrawWaveDb(graphics, globalView, plotData, true);

            if (globalViewZoomArea.HasValue)
            {
                this.DrawDisplayAreaStyle(globalView, graphics, globalViewZoomArea.Value);
            }

            return wavSelectedArea;
        }




























        /// <summary>
        /// 部分绘制-波形线密度改变
        /// 
        /// </summary>
        private void PartDraw_DrawDensityChanged()
        {
            //全部绘制
            Graphics graphics = this._grafx.Graphics;

            //清空所有已绘制的图形
            graphics.Clear(this.BackColor);

            //WavePlotPara plotPara = this._plotPara;
            //IEnumerable<ChannelPlotData> plotDatas = this._plotDatas;
            //if (plotPara == null)
            //{
            //    return;
            //}

            ////全局视图
            //this.DrawGlobalView(graphics, plotPara, plotDatas);
            //this.AddPartRefreshArea(this._globalView);

            ////波形图
            //this.DrawWaveSpecturum(graphics, plotPara, plotDatas);
            //this.AddPartRefreshArea(this._content.Area);

            //this.RefreshInvalidateArea();
        }
    }
}
