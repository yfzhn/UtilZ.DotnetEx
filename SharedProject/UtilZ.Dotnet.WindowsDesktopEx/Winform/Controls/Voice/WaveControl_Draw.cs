using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    //WavePlayer-Draw
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
            //全部绘制
            Graphics graphics = this._grafx.Graphics;
            ////重置平移
            //graphics.ResetTransform();

            //清空所有已绘制的图形
            graphics.Clear(this.BackColor);

            WavePlotPara plotPara = this._plotPara;
            IEnumerable<ChannelPlotData> plotDatas = this._plotDatas;

            if (plotPara == null ||
                plotDatas == null ||
                (plotDatas != null && plotDatas.Count() == 0))
            {
                return;
            }

            this.DrawGlobalView(graphics, plotPara, plotDatas);//绘制全局视图
            this.DrawTimeAxis(graphics, plotPara);//绘制X轴
            this.DrawWaveSpecturum(graphics, plotPara, plotDatas);//绘制波形图
            this.DrawWaveSpecturumAxis(graphics, plotDatas);//绘制Y轴
            this.RefreshInvalidateArea();//刷新
        }





        private void DrawWaveSpecturumAxis(Graphics graphics, IEnumerable<ChannelPlotData> plotDatas)
        {
            YAxisPlotElementInfo yAxis = this._yAxis;
            if (yAxis == null)
            {
                return;
            }

            RectangleF axisRectangle = yAxis.Area;
            graphics.FillRectangle(yAxis.BackgroudColor, axisRectangle);

            if (plotDatas == null)
            {
                return;
            }

            int channelCount = plotDatas.Count();
            if (channelCount == 0)
            {
                return;
            }

            float axisHeight = axisRectangle.Height / channelCount;
            float axisHeightHalf = axisHeight / 2;
            //const float DB_MAX = 90.308733622833976f;// 20*Math.Log10(short.MaxValue);
            const int DB_MAX = 90;//直接使用整数,坐标好计算和绘制一些

            Font font = yAxis.Font;

            SizeF labelTextSize = graphics.MeasureString(DB_MAX.ToString(), font);
            const float LABEL_INTERVAL_MIN = 20;
            int axisLabelCount = (int)(axisHeightHalf / (labelTextSize.Height + LABEL_INTERVAL_MIN));
            while (axisLabelCount > 1 && DB_MAX % axisLabelCount != 0)
            {
                axisLabelCount -= 1;
            }

            if (axisLabelCount == 0)
            {
                axisLabelCount = 1;
            }

            float offsetY = axisHeightHalf / axisLabelCount;
            float dbInterval = ((float)DB_MAX) / axisLabelCount;
            float lableX1 = axisRectangle.X;
            float lableX2 = lableX1 + 5f;
            float labelTextX = lableX2 + 3f;
            float lableY, labelTextY;
            float zeoroY = axisRectangle.Y + axisHeightHalf;
            string labelText;
            float labelTextHeightHalf = labelTextSize.Height / 2;
            Pen pen = yAxis.Pen;

            for (int i = 0; i < channelCount; i++)
            {
                labelTextY = zeoroY;
                labelText = "0 db";
                graphics.DrawLine(pen, lableX1, zeoroY, lableX2, zeoroY);
                graphics.DrawString(labelText, font, yAxis.ForeColor, labelTextX, labelTextY);

                for (int j = 1; j <= axisLabelCount; j++)
                {
                    if (j == axisLabelCount)
                    {
                        //0值以下
                        lableY = zeoroY + axisHeightHalf;
                        labelTextY = lableY - labelTextSize.Height;
                        labelText = (dbInterval * j).ToString();
                        graphics.DrawLine(pen, lableX1, lableY, lableX2, lableY);
                        graphics.DrawString(labelText, font, yAxis.ForeColor, labelTextX, labelTextY);

                        //0值以上
                        lableY = zeoroY - axisHeightHalf;
                        labelTextY = lableY;
                        graphics.DrawLine(pen, lableX1, lableY, lableX2, lableY);
                        graphics.DrawString(labelText, font, yAxis.ForeColor, labelTextX, labelTextY);
                    }
                    else
                    {
                        //0值以下
                        lableY = zeoroY + offsetY * j;
                        labelTextY = lableY - labelTextHeightHalf;
                        labelText = (dbInterval * j).ToString();
                        graphics.DrawLine(pen, lableX1, lableY, lableX2, lableY);
                        graphics.DrawString(labelText, font, yAxis.ForeColor, labelTextX, labelTextY);

                        //0值以上
                        lableY = zeoroY - offsetY * j;
                        labelTextY = lableY - labelTextHeightHalf;
                        graphics.DrawLine(pen, lableX1, lableY, lableX2, lableY);
                        graphics.DrawString(labelText, font, yAxis.ForeColor, labelTextX, labelTextY);
                    }
                }

                zeoroY += axisHeight;
            }
        }


        private RectangleF? DrawWaveSpecturum(Graphics graphics, WavePlotPara plotPara, IEnumerable<ChannelPlotData> plotDatas)
        {
            PlotElementInfoAbs wave = this._content;
            if (wave == null)
            {
                return null;
            }

            //填充主波形背景
            if (wave.BackgroudColor != null)
            {
                graphics.FillRectangle(wave.BackgroudColor, wave.Area);
            }

            if (plotPara == null || plotDatas == null)
            {
                return null;
            }

            double sbto = plotPara.SBTOMillisecond;//显示区域起始时间
            double seto = plotPara.GetSETOMillisecond();//显示区域结束时间
            RectangleF? wavSelectedArea = this.GetSelectedAreaBackground(wave.Area, plotPara, sbto, seto);
            if (wavSelectedArea.HasValue)
            {
                //填充主波形选中背景
                graphics.FillRectangle(this._seleactionAreaBrush, wavSelectedArea.Value);
            }

            //绘制波形图
            this.DrawWaveDb(graphics, wave, plotPara, plotDatas, false);

            return wavSelectedArea;
        }



        private RectangleF? GetSelectedAreaBackground(RectangleF waveRectangle, WavePlotPara plotPara, double beginMillisecond, double endMillisecond)
        {
            if (this._selectedInfo == null)
            {
                return null;
            }

            double sb = this._selectedInfo.BeginMillisecond;//选中区域起始时间
            double se = this._selectedInfo.EndMillisecond;//选中区域结束时间
            if (se - sb <= PlotConstant.ZEROR_D)
            {
                //至少选中,如果没有选中区域,则不需要绘制该区域背景
                return null;
            }

            /*******************************************************************************************
             * 选中与显示有以下五种场景,分别为ABCDE:
             *           A           B               C                 D               E
             *       |+++++++|  |+++++++++|     |+++++++++|       |+++++++++|     |+++++++++|
             * |-----|-------|--|--|------|-----|---------|-------|---|-----|-----|---------|
             * |-------------------|----------------------------------|-----------------------|
             * 0                   sbto                              seto                     end
             *******************************************************************************************/
            if (se - beginMillisecond < PlotConstant.ZEROR_D ||
                sb - endMillisecond > PlotConstant.ZEROR_D)
            {
                //场景A和E
                return null;
            }

            float x1;
            if (sb - beginMillisecond < PlotConstant.ZEROR_D)
            {
                //场景B
                x1 = 0f;
            }
            else
            {
                //场景C或D
                x1 = waveRectangle.X + (float)((waveRectangle.Width / (endMillisecond - beginMillisecond)) * (sb - beginMillisecond));
            }

            float x2;
            if (sb - endMillisecond > PlotConstant.ZEROR_D)
            {
                //场景D
                x2 = waveRectangle.X + waveRectangle.Width;
            }
            else
            {
                //场景A或B
                x2 = waveRectangle.X + (float)((waveRectangle.Width / (endMillisecond - beginMillisecond)) * (se - beginMillisecond));
            }

            return new RectangleF(x1, waveRectangle.Y, x2 - x1, waveRectangle.Height);
        }





        private void DrawTimeAxis(Graphics graphics, WavePlotPara plotPara)
        {
            XAxisPlotElementInfo axisX = this._xAxis;
            if (axisX == null)
            {
                return;
            }

            //时间图背景
            RectangleF rectangleTimeArea = axisX.Area;
            graphics.FillRectangle(axisX.BackgroudColor, rectangleTimeArea);

            if (plotPara == null)
            {
                return;
            }

            //边框
            //graphics.DrawLine(this._borderPen, rectangleTimeArea.X, rectangleTimeArea.Y, rectangleTimeArea.X, rectangleTimeArea.Y + rectangleTimeArea.Height);
            //float borderX = rectangleTimeArea.X + rectangleTimeArea.Width - this._borderPen.Width;
            //graphics.DrawLine(this._borderPen, borderX, rectangleTimeArea.Y, borderX, rectangleTimeArea.Y + rectangleTimeArea.Height);
            //graphics.DrawLine(this._borderPen, rectangleTimeArea.X, rectangleTimeArea.Y, rectangleTimeArea.X + rectangleTimeArea.Width, rectangleTimeArea.Y);

            //绘制起始时间-毫秒
            double beginTimeMillisecond = plotPara.SBTOMillisecond;

            //绘制结束时间-毫秒
            double endTimeMillisecond = plotPara.GetSETOMillisecond();

            //绘制总时长
            double durationTimeMillisecond = endTimeMillisecond - beginTimeMillisecond;
            if (durationTimeMillisecond <= 0d)
            {
                return;
            }

            //绘制时间间隔
            double separatorSecond = this.CalSegDurationSecond(axisX, graphics, durationTimeMillisecond);

            //绘制起始时间-秒
            double beginTimeSecond = beginTimeMillisecond / 1000;

            //绘制时间-秒
            double timeSecond = PlotConstant.ZEROR_D;
            if (beginTimeMillisecond > PlotConstant.ZEROR_D)
            {
                int mult = (int)(beginTimeSecond / separatorSecond);
                double mod = beginTimeSecond % separatorSecond;
                if (mod - separatorSecond < PlotConstant.ZEROR_D)
                {
                    mult = mult + 1;
                }
                timeSecond = separatorSecond * mult;
            }

            //绘制时间
            DateTime time = plotPara.BaseTime.AddSeconds(timeSecond);
            DateTime endTime = plotPara.BaseTime.AddMilliseconds(endTimeMillisecond);


            float labelHeight = rectangleTimeArea.Height / 3;
            float y1 = rectangleTimeArea.Y + rectangleTimeArea.Height;
            float y2 = y1 - labelHeight;
            float y3 = y1 - labelHeight / 2;
            float labelX, labelSmallX;
            double separatorSecondHalf = separatorSecond / 2;

            float secondLength = (float)(axisX.Area.Width / (durationTimeMillisecond / 1000));
            string labelText;
            SizeF labelTextSize;
            float labelTextX, lastLabelTextRightX = 0f;
            float labelTextY = rectangleTimeArea.Y + labelHeight / 2;

            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            var contentWidth = axisX.Area.Width;
            Pen pen = axisX.Pen;

            while (true)
            {
                //刻度-x
                labelX = (float)(secondLength * (timeSecond - beginTimeSecond));
                labelSmallX = (float)(secondLength * (timeSecond - beginTimeSecond - separatorSecondHalf));

                //绘制刻度文本
                labelText = this.GetLabelText(time, separatorSecond);
                labelTextSize = graphics.MeasureString(labelText, axisX.Font);
                labelTextX = labelX - labelTextSize.Width / 2;

                switch (labelTextLocation)
                {
                    case AxisLabelLocation.First:
                        if (labelTextX < PlotConstant.ZEROR_D && beginTimeMillisecond <= PlotConstant.ZEROR_D)
                        {
                            labelTextX = (float)PlotConstant.ZEROR_D;
                        }
                        labelTextLocation = AxisLabelLocation.Middle;
                        break;
                    case AxisLabelLocation.Middle:
                        if (labelTextX + labelTextSize.Width - contentWidth > PlotConstant.ZEROR_D)
                        {
                            labelTextX = contentWidth - labelTextSize.Width;
                        }
                        break;
                    case AxisLabelLocation.Last:
                        if (labelTextX + labelTextSize.Width - contentWidth > PlotConstant.ZEROR_D)
                        {
                            labelTextX = contentWidth - labelTextSize.Width;
                            if (labelTextX - lastLabelTextRightX < PlotConstant.ZEROR_D)
                            {
                                if (labelSmallX - contentWidth < PlotConstant.ZEROR_D)
                                {
                                    graphics.DrawLine(pen, labelSmallX, y1, labelSmallX, y3);
                                }

                                return;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                graphics.DrawString(labelText, axisX.Font, axisX.ForeColor, labelTextX, labelTextY);
                lastLabelTextRightX = labelTextX + labelTextSize.Width;

                //绘制刻度
                if (labelSmallX > PlotConstant.ZEROR_D)
                {
                    graphics.DrawLine(pen, labelSmallX, y1, labelSmallX, y3);
                }

                if (labelX - contentWidth < PlotConstant.ZEROR_D)
                {
                    graphics.DrawLine(pen, labelX, y1, labelX, y2);
                }

                if (labelTextLocation == AxisLabelLocation.Last)
                {
                    break;
                }

                timeSecond += separatorSecond;
                time = plotPara.BaseTime.AddSeconds(timeSecond);
                if (time >= endTime)
                {
                    time = endTime;
                    labelTextLocation = AxisLabelLocation.Last;
                }
            }
        }

        private double CalSegDurationSecond(XAxisPlotElementInfo axisX, Graphics graphics, double durationTimeMillisecond)
        {
            string labelText = this.GetLabelText(DateTime.Now, double.NaN);
            SizeF labelTextSize = graphics.MeasureString(labelText, axisX.Font);
            float labelTextMinWidth = labelTextSize.Width * 2;
            int maxLabelCount = (int)(axisX.Area.Width / labelTextMinWidth);
            double separatorSecond;
            double segDurationMillisecond = durationTimeMillisecond / maxLabelCount;
            if (segDurationMillisecond < 500)
            {
                separatorSecond = 0.5d;
            }
            else if (segDurationMillisecond < 1000)
            {
                separatorSecond = 1d;
            }
            else
            {
                separatorSecond = (int)Math.Ceiling(segDurationMillisecond / 1000);
            }

            return separatorSecond;
        }

        private string GetLabelText(DateTime time, double separatorSecond)
        {
            string labelText;
            var handler = this.CustomTimeAxisLabelFun;
            if (handler != null)
            {
                labelText = handler(time, separatorSecond);
            }
            else
            {
                if (separatorSecond < 0.01d)
                {
                    labelText = time.ToString("HH:mm:ss.fff");
                }
                else if (separatorSecond < 0.1d)
                {
                    labelText = time.ToString("HH:mm:ss.ff");
                }
                else if (separatorSecond < 1d)
                {
                    labelText = time.ToString("HH:mm:ss.f");
                }
                else
                {
                    labelText = time.ToString("HH:mm:ss");
                }
            }

            return labelText;
        }


        private RectangleF? DrawGlobalView(Graphics graphics, WavePlotPara plotPara, IEnumerable<ChannelPlotData> plotDatas)
        {
            GlobalViewPlotElementInfo globalView = this._globalView;
            if (globalView == null)
            {
                return null;
            }

            //填充整体视图波形背景
            if (globalView.BackgroudColor != null)
            {
                graphics.FillRectangle(globalView.BackgroudColor, globalView.Area);
            }

            if (plotDatas == null)
            {
                return null;
            }

            //整体视图中缩放后显示区域
            RectangleF? globalViewZoomArea = null;
            if (this._zoomInfo.HasZoom())
            {
                float x1 = (float)(globalView.Area.Width * plotPara.SBTOMillisecond / plotPara.DurationMillisecond);
                float x2 = (float)(globalView.Area.Width * plotPara.GetSETOMillisecond() / plotPara.DurationMillisecond);
                globalViewZoomArea = new RectangleF(x1, globalView.Area.Y, x2 - x1, globalView.Area.Height);
                graphics.FillRectangle(globalView.GlobalViewZoomAreaBackBrush, globalViewZoomArea.Value);
            }

            RectangleF? wavSelectedArea = this.GetSelectedAreaBackground(globalView.Area, plotPara, PlotConstant.ZEROR_D, plotPara.DurationMillisecond);
            if (wavSelectedArea.HasValue)
            {
                //填充选中背景
                graphics.FillRectangle(this._seleactionGlobalViewAreaBrush, wavSelectedArea.Value);
            }

            //绘制整体视图波形
            this.DrawWaveDb(graphics, globalView, plotPara, plotDatas, true);

            if (globalViewZoomArea.HasValue)
            {
                this.DrawDisplayAreaStyle(globalView, graphics, globalViewZoomArea.Value);
            }

            return wavSelectedArea;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="wave"></param>
        /// <param name="plotPara"></param>
        /// <param name="pcmDatas"></param>
        /// <param name="dataType">true:GlobalViewData;alse:DrawData</param>
        private void DrawWaveDb(Graphics graphics, PlotElementInfoAbs wave, WavePlotPara plotPara, IEnumerable<ChannelPlotData> pcmDatas, bool dataType)
        {
            PointF[] points;
            if (dataType)
            {
                points = new PointF[plotPara.GlobalViewPcmDataLength];
            }
            else
            {
                points = new PointF[plotPara.DrawPcmDataLength];
            }

            if (points.Length == 0)
            {
                return;
            }

            int channeCount = pcmDatas.Count();
            float channelWaveAreaHeight = wave.Area.Height / channeCount;
            float channelWaveHalfHeight = channelWaveAreaHeight / 2;
            float separatorY = wave.Area.Y + channelWaveHalfHeight;
            float lx, y;
            bool drawSeparator = false;
            float value;
            float db;
            short[] pcmData;
            bool negativePoint;
            float xStep = this.CalXStep(points.Length);
            //const float DB_MAX = 10.397177190355384f;  // = Math.Log(short.MaxValue);
            const float DB_MAX = 4.5154366811416988f;  // = Math.Log10(short.MaxValue);

            foreach (ChannelPlotData channelPlotData in pcmDatas)
            {
                if (drawSeparator)
                {
                    graphics.DrawLine(this._channelSeparatorPen, 0f, separatorY, wave.Area.Width, separatorY);
                }

                if (dataType)
                {
                    pcmData = channelPlotData.GlobalViewData;
                }
                else
                {
                    pcmData = channelPlotData.DrawData;
                }

                lx = 0f;
                //float minY = 0f, maxY = 0f;
                //float dbMin = 0f, dbMax = 0f;
                for (int i = 0; i < points.Length && i < pcmData.Length; i++)
                {
                    y = separatorY - ((float)pcmData[i] / short.MaxValue) * channelWaveHalfHeight;

                    //转分呗后画出的波形似乎不太对啊
                    //if (pcmData[i] == 0)
                    //{
                    //    y = separatorY;
                    //}
                    //else
                    //{
                    //    value = (float)pcmData[i];
                    //    if (value < 0)
                    //    {
                    //        negativePoint = true;
                    //        value = Math.Abs(value);
                    //    }
                    //    else
                    //    {
                    //        negativePoint = false;
                    //    }

                    //    //dB = 20 * log(A1 / A2)  => db = log(A)
                    //    //value = 20 * (float)(Math.Log10(value / short.MaxValue));                        
                    //    //db = (float)(Math.Log(value));
                    //    db = (float)(Math.Log10(value));

                    //    if (db < dbMin)
                    //    {
                    //        dbMin = db;
                    //    }

                    //    if (db > dbMax)
                    //    {
                    //        dbMax = db;
                    //    }


                    //    if (negativePoint)
                    //    {
                    //        y = separatorY + channelWaveHalfHeight * db / DB_MAX;
                    //    }
                    //    else
                    //    {
                    //        y = separatorY - channelWaveHalfHeight * db / DB_MAX;
                    //    }

                    //    if (y < minY)
                    //    {
                    //        minY = y;
                    //    }

                    //    if (y > maxY)
                    //    {
                    //        maxY = y;
                    //    }
                    //}

                    points[i] = new PointF(lx, y);
                    lx = lx + xStep;
                }

                graphics.DrawLines(wave.Pen, points);

                separatorY += channelWaveAreaHeight;
                drawSeparator = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="wave"></param>
        /// <param name="plotPara"></param>
        /// <param name="pcmDatas"></param>
        /// <param name="dataType">true:GlobalViewData;alse:DrawData</param>
        private void DrawWave_A(Graphics graphics, PlotElementInfoAbs wave, WavePlotPara plotPara, IEnumerable<ChannelPlotData> pcmDatas, bool dataType)
        {
            PointF[] points;
            if (dataType)
            {
                points = new PointF[plotPara.GlobalViewPcmDataLength];
            }
            else
            {
                points = new PointF[plotPara.DrawPcmDataLength];
            }

            if (points.Length == 0)
            {
                return;
            }

            int channeCount = pcmDatas.Count();
            float channelWaveAreaHeight = wave.Area.Height / channeCount;
            float channelWaveHalfHeight = channelWaveAreaHeight / 2;
            float separatorY = wave.Area.Y + channelWaveHalfHeight;
            float lx, y;
            bool drawSeparator = false;
            short[] pcmData;
            float xStep = this.CalXStep(points.Length);

            foreach (ChannelPlotData channelPlotData in pcmDatas)
            {
                if (drawSeparator)
                {
                    graphics.DrawLine(this._channelSeparatorPen, 0f, separatorY, wave.Area.Width, separatorY);
                }

                if (dataType)
                {
                    pcmData = channelPlotData.GlobalViewData;
                }
                else
                {
                    pcmData = channelPlotData.DrawData;
                }

                lx = 0f;
                for (int i = 0; i < points.Length && i < pcmData.Length; i++)
                {
                    y = separatorY - ((float)pcmData[i] / short.MaxValue) * channelWaveHalfHeight;
                    points[i] = new PointF(lx, y);
                    lx = lx + xStep;
                }

                graphics.DrawLines(wave.Pen, points);
                separatorY += channelWaveAreaHeight;
                drawSeparator = true;
            }
        }

        /// <summary>
        /// 绘制显示区域样式
        /// </summary>
        /// <param name="globalView"></param>
        /// <param name="graphics"></param>
        /// <param name="globalViewZoomArea"></param>
        private void DrawDisplayAreaStyle(GlobalViewPlotElementInfo globalView, Graphics graphics, RectangleF globalViewZoomArea)
        {
            Pen globalViewZoomAreaPen = globalView.GlobalViewZoomAreaPen;
            //绘制边框
            graphics.DrawRectangle(globalViewZoomAreaPen, globalViewZoomArea.X, globalViewZoomArea.Y, globalViewZoomArea.Width, globalViewZoomArea.Height);

            //绘制内部细线
            float zoomDAEndX = globalViewZoomArea.X + globalViewZoomArea.Width;
            float zoomDABeginX = globalViewZoomArea.X;
            float zoomDAY1 = globalViewZoomArea.Y;
            float zoomDAY2 = zoomDAY1 + globalViewZoomArea.Height;
            int interval = 2;
            if (globalViewZoomArea.Width > 20)
            {
                //左边竖线
                this.DrawVerLine(graphics, globalViewZoomAreaPen, zoomDABeginX + interval, zoomDABeginX + 10, zoomDAY1, zoomDAY2, interval);
                //右边竖线
                this.DrawVerLine(graphics, globalViewZoomAreaPen, zoomDAEndX - 8, zoomDAEndX - 1, zoomDAY1, zoomDAY2, interval);
            }
            else
            {
                //宽度小于20就左右边一起了
                this.DrawVerLine(graphics, globalViewZoomAreaPen, zoomDABeginX + interval, zoomDAEndX, zoomDAY1, zoomDAY2, interval);
            }
        }

        /// <summary>
        /// 绘制垂直线
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="globalViewZoomAreaPen">Pen</param>
        /// <param name="beginX">起始X</param>
        /// <param name="endX">结束X</param>
        /// <param name="beginY">起始Y</param>
        /// <param name="endY">结束Y</param>
        /// <param name="interval">像素间隔</param>
        private void DrawVerLine(Graphics graphics, Pen globalViewZoomAreaPen, float beginX, float endX, float beginY, float endY, int interval)
        {
            PointF zoomDAP1, zoomDAP2;
            if (endX - beginX < 1)//如果一个点都没有则至少绘制一条线
            {
                zoomDAP1 = new PointF(beginX, beginY);
                zoomDAP2 = new PointF(beginX, endY);
                graphics.DrawLine(globalViewZoomAreaPen, zoomDAP1, zoomDAP2);
            }
            else
            {
                for (float x = beginX; x < endX; x += interval)
                {
                    zoomDAP1 = new PointF(x, beginY);
                    zoomDAP2 = new PointF(x, endY);
                    graphics.DrawLine(globalViewZoomAreaPen, zoomDAP1, zoomDAP2);
                }
            }
        }




        private float CalXStep(int pointCount)
        {
            if (pointCount < 1)
            {
                return this._content.Area.Width;
            }
            return this._content.Area.Width / pointCount;//两个点之间的间隔,减1是因为线段数比点数少1,两个点组成一条线
            //return this._specturumDrawInfo.ContentArea.Width / this._plotPara.DrawPcmDataLength;//两个点之间的间隔,减1是因为线段数比点数少1,两个点组成一条线
        }





        /// <summary>
        /// 完整绘制:整体视图-时间线-波形图
        /// 用于缩放-平移场景
        /// </summary>
        private void PartDraw_ZoomMove()
        {
            //完整绘制:整体视图-时间线-波形图
            //全部绘制
            Graphics graphics = this._grafx.Graphics;

            //清空所有已绘制的图形
            graphics.Clear(this.BackColor);

            WavePlotPara plotPara = this._plotPara;
            IEnumerable<ChannelPlotData> plotDatas = this._plotDatas;
            if (plotPara == null)
            {
                return;
            }

            RectangleF? selectedArea;
            SelectedInfo selectedInfo = this._selectedInfo;

            selectedArea = this.DrawGlobalView(graphics, plotPara, plotDatas);
            this.AddPartRefreshArea(this._globalView);
            if (selectedInfo != null)
            {
                selectedInfo.LastGlobalViewSelectedArea = selectedArea;
            }


            this.DrawTimeAxis(graphics, plotPara);
            this.AddPartRefreshArea(this._xAxis);

            selectedArea = this.DrawWaveSpecturum(graphics, plotPara, plotDatas);
            if (selectedInfo != null)
            {
                selectedInfo.LastWaveSelectedArea = selectedArea;
            }

            this.AddPartRefreshArea(this._content);
            this.RefreshInvalidateArea();
        }



        private RectangleF? _globalViewLastPlayPositionLineArea = null;
        private RectangleF? _waveLastPlayPositionLineArea = null;
        /// <summary>
        /// 更新播放位置指示线
        /// </summary>
        /// <param name="plotPara"></param>
        /// <param name="offsetTimeMilliseconds">播放时间</param>
        private void PrimitiveUpdatePostionLine(WavePlotPara plotPara, double offsetTimeMilliseconds)
        {
            if (!this._playPositionLine)
            {
                return;
            }

            this.AddPartRefreshArea(this._globalViewLastPlayPositionLineArea);
            this.AddPartRefreshArea(this._waveLastPlayPositionLineArea);


            //全部绘制
            Graphics graphics = this._grafx.Graphics;
            IEnumerable<ChannelPlotData> plotDatas = this._plotDatas;
            //全局视图
            this.DrawGlobalView(graphics, plotPara, plotDatas);

            //波形图
            this.DrawWaveSpecturum(graphics, plotPara, plotDatas);

            ContentPlotElementInfo content = this._content;
            float maxX = content.Area.X + content.Area.Width;
            const int OFFSET1 = 1;
            const int OFFSET2 = 2;

            //缩略波形图播放位置指示线
            var globalView = this._globalView;
            if (globalView != null)
            {
                float globalViewX = (float)(content.Area.Width * offsetTimeMilliseconds / plotPara.DurationMillisecond);
                if (globalViewX + globalView.PlayLineChannelPen.Width < maxX)
                {
                    PointF zoomP1 = new PointF(globalViewX, globalView.Area.Y);
                    PointF zoomP2 = new PointF(globalViewX, globalView.Area.Y + globalView.Area.Height);
                    graphics.DrawLine(globalView.PlayLineChannelPen, zoomP1, zoomP2);
                    this._globalViewLastPlayPositionLineArea = new RectangleF(globalViewX - OFFSET1, globalView.Area.Y - OFFSET1, globalView.PlayLineChannelPen.Width + OFFSET2, globalView.Area.Height + OFFSET2);
                    this.AddPartRefreshArea(this._globalViewLastPlayPositionLineArea);
                }
            }
            else
            {
                this._globalViewLastPlayPositionLineArea = null;
            }



            //主波形图播放位置指示线
            double sbto = plotPara.SBTOMillisecond;
            double seto = plotPara.GetSETOMillisecond();
            double timeArea = seto - sbto;
            float contentX = (float)(content.Area.Width * (offsetTimeMilliseconds - sbto) / timeArea);
            bool drawWavePlayLine = plotPara.ContainsShowTime(offsetTimeMilliseconds);

            if (drawWavePlayLine && contentX > PlotConstant.ZEROR_D && this.Width - contentX > PlotConstant.ZEROR_D)
            {
                if (contentX + content.PlayLineChannelPen.Width - maxX < PlotConstant.ZEROR_D)
                {
                    var contentArea = content.Area;
                    PointF wavP1 = new PointF(contentX, contentArea.Y);
                    PointF wavP2 = new PointF(contentX, contentArea.Y + contentArea.Height);
                    graphics.DrawLine(content.PlayLineChannelPen, wavP1, wavP2);
                    this._waveLastPlayPositionLineArea = new RectangleF(contentX - OFFSET1, contentArea.Y - OFFSET1, content.PlayLineChannelPen.Width + OFFSET2, contentArea.Height + OFFSET2);
                    this.AddPartRefreshArea(this._waveLastPlayPositionLineArea);
                }
            }
            else
            {
                this._waveLastPlayPositionLineArea = null;
            }

            this.RefreshInvalidateArea();
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

            WavePlotPara plotPara = this._plotPara;
            IEnumerable<ChannelPlotData> plotDatas = this._plotDatas;
            if (plotPara == null)
            {
                return;
            }

            //全局视图
            this.DrawGlobalView(graphics, plotPara, plotDatas);
            this.AddPartRefreshArea(this._globalView);

            //波形图
            this.DrawWaveSpecturum(graphics, plotPara, plotDatas);
            this.AddPartRefreshArea(this._content.Area);

            this.RefreshInvalidateArea();
        }



        private void SelectedAreaChangeDraw(SelectedInfo oldSelectedInfo)
        {
            //全部绘制
            Graphics graphics = this._grafx.Graphics;
            //重置平移
            graphics.ResetTransform();

            //清空所有已绘制的图形
            //graphics.Clear(this.BackColor);

            WavePlotPara plotPara = this._plotPara;
            IEnumerable<ChannelPlotData> plotDatas = this._plotDatas;
            if (plotPara == null)
            {
                return;
            }

            if (oldSelectedInfo != null)
            {
                this.AddPartRefreshArea(oldSelectedInfo.LastGlobalViewSelectedArea);
                this.AddPartRefreshArea(oldSelectedInfo.LastWaveSelectedArea);
            }


            SelectedInfo selectedInfo = this._selectedInfo;
            RectangleF? selectedArea;


            //理论上选择区域改变,是不需要重新绘制时间的,但是在某种情况下,时间时间区域会消失,只好重绘,以后如果找到原因了,再删除这个case内的代码
            this.DrawTimeAxis(graphics, plotPara);
            this.AddPartRefreshArea(this._xAxis);


            selectedArea = this.DrawGlobalView(graphics, plotPara, plotDatas);
            this.AddPartRefreshArea(selectedArea);
            if (selectedInfo != null)
            {
                selectedInfo.LastGlobalViewSelectedArea = selectedArea;
            }


            selectedArea = this.DrawWaveSpecturum(graphics, plotPara, plotDatas);
            this.AddPartRefreshArea(selectedArea);
            if (selectedInfo != null)
            {
                selectedInfo.LastWaveSelectedArea = selectedArea;
            }

            this.RefreshInvalidateArea();
        }
    }
}
