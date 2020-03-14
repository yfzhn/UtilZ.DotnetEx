using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    //NoneFreeze
    public partial class Chart
    {
        private void UpdateNoneFreeze(AxisFreezeInfo axisFreezeInfo, ChartCollection<AxisAbs> axisCollection, ChartCollection<ISeries> seriesCollection,
            IChartLegend legend, Canvas chartCanvas, Grid chartGrid, ScrollViewer scrollViewer)
        {
            /************************************************************************************************************
             * 步骤:
             * 1.添加legend,并计算出发四周所占高度或宽度
             * 2.计算X轴总高度
             * 3.根据X轴总高度计算图表区域高度高度(等于Y轴高度)
             * 4.根据Y轴高度绘制Y轴,并计算Y轴宽度
             * 5.根据Y轴宽度计算X轴宽度并绘制X轴
             * 6.绘制坐标背景标记线
             * 7.绘各Series
             * 8.填充legend
             * 9.布局UI
             ************************************************************************************************************/

            //chartGrid.ShowGridLines = true;
            //chartCanvas.Background = ColorBrushHelper.GetNextColor();
            this.Content = chartGrid;



            //第一步 添加legend,并计算出发四周所占高度或宽度
            LegendAddResult legendAddResult = this.AddLegend(legend, chartGrid);


            //第二步 计算X轴总高度
            AxisXHeightInfo axisXHeightInfo = this.CalculateAxisXHeight(axisCollection);


            //第三步 根据X轴总高度计算图表区域高度高度(等于Y轴高度)
            double yAxisHeight = axisFreezeInfo.Height - axisXHeightInfo.TopAxisTotalHeight - axisXHeightInfo.BottomAxisTotalHeight - legendAddResult.Top - legendAddResult.Bottom;
            if (yAxisHeight < ChartConstant.ZERO_D)
            {
                yAxisHeight = ChartConstant.ZERO_D;
            }

            //第四步 根据Y轴高度绘制Y轴,并计算Y轴宽度
            AxisYWidthInfo axisYWidthInfo = this.DrawAxisYByAxisXHeightInfo(axisCollection, chartGrid.Children, seriesCollection, yAxisHeight, ChartConstant.ZERO_D);


            //第五步 根据Y轴宽度计算X轴宽度并绘制X轴
            double xAxisWidth = axisFreezeInfo.Width - axisYWidthInfo.LeftAxisTotalWidth - axisYWidthInfo.RightAxisTotalWidth - legendAddResult.Left - legendAddResult.Right;
            if (xAxisWidth < ChartConstant.ZERO_D)
            {
                xAxisWidth = ChartConstant.ZERO_D;
            }
            Dictionary<AxisAbs, List<double>> axisXLabelDic = this.DrawAxisX(axisCollection, seriesCollection, chartGrid, xAxisWidth, ChartConstant.ZERO_D);

            chartCanvas.Width = xAxisWidth;
            chartCanvas.Height = yAxisHeight;
            scrollViewer.BorderThickness = new Thickness(ChartConstant.ZERO_D);
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewer.Background = Brushes.Transparent;
            scrollViewer.Content = chartCanvas;
            chartGrid.Children.Add(scrollViewer);

            //第六步 绘制坐标背景标记线
            this.DrawAxisBackgroundLabelLine(chartCanvas, axisYWidthInfo.AxisYLabelDic, axisXLabelDic);

            //第七步 绘各Series
            this.DrawSeries(chartCanvas, seriesCollection);


            //第八步 填充legend
            if (legendAddResult.HasLegend)
            {
                this.UpdateLegend(legend, seriesCollection);
            }

            //第九步 布局UI
            var chartGridRowColumnDefinition = new ChartGridRowColumnDefinition(legendAddResult.HasLegend, legend, chartGrid, axisYWidthInfo, axisXHeightInfo);
            if (legendAddResult.HasLegend)
            {
                this.SetRowColumn(chartGrid, legend.LegendControl, chartGridRowColumnDefinition.Legend);
            }

            if (axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I)
            {
                RowColumnDefinitionItem rowColumnDefinition;
                foreach (var axis in axisCollection)
                {
                    switch (axis.DockOrientation)
                    {
                        case ChartDockOrientation.Left:
                            rowColumnDefinition = chartGridRowColumnDefinition.LeftAxis;
                            break;
                        case ChartDockOrientation.Top:
                            rowColumnDefinition = chartGridRowColumnDefinition.TopAxis;
                            break;
                        case ChartDockOrientation.Right:
                            rowColumnDefinition = chartGridRowColumnDefinition.RightAxis;
                            break;
                        case ChartDockOrientation.Bottom:
                            rowColumnDefinition = chartGridRowColumnDefinition.BottomAxis;
                            break;
                        default:
                            throw new NotImplementedException(axis.DockOrientation.ToString());
                    }

                    this.SetRowColumn(chartGrid, axis.AxisControl, rowColumnDefinition);
                }
            }

            this.SetRowColumn(chartGrid, scrollViewer, chartGridRowColumnDefinition.Chart);


            chartCanvas.MouseWheel += ChartCanvas_MouseWheel;
            chartCanvas.MouseLeftButtonDown += ChartCanvas_MouseLeftButtonDown;
            chartCanvas.MouseMove += ChartCanvas_MouseMove;
        }


        //平移
        private Point _beginPoint;
        private void ChartCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            ChartCollection<AxisAbs> axes = this.Axes;
            if (axes == null || axes.Count == 0)
            {
                return;
            }

            var endPoint = e.MouseDevice.GetPosition(this._chartCanvas);
            double left = endPoint.X - this._beginPoint.X;
            double top = endPoint.Y - this._beginPoint.Y;
            if (left == ChartConstant.ZERO_D && top == ChartConstant.ZERO_D)
            {
                return;
            }

            this._beginPoint = endPoint;

            //平移坐标
            Canvas chartCanvas = this._chartCanvas;
            List<AxisAbs> zoomedAxisList = null;
            bool translateResult;
            foreach (var axis in axes)
            {
                if (!axis.AllowZoomTranslate)
                {
                    continue;
                }

                switch (axis.AxisType)
                {
                    case AxisType.X:
                        translateResult = axis.TranslateX(left);
                        break;
                    case AxisType.Y:
                        translateResult = axis.TranslateY(top);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (translateResult)
                {
                    if (zoomedAxisList == null)
                    {
                        zoomedAxisList = new List<AxisAbs>();
                    }
                    zoomedAxisList.Add(axis);
                }
            }

            //更新series
            ChartCollection<ISeries> seriesCollection = this.Series;
            if (seriesCollection != null && zoomedAxisList != null)
            {
                foreach (var series in seriesCollection)
                {
                    if (zoomedAxisList.Contains(series.AxisX) || zoomedAxisList.Contains(series.AxisY))
                    {
                        series.Update();
                    }
                }
            }
        }

        private void ChartCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this._beginPoint = e.MouseDevice.GetPosition(this._chartCanvas);
        }







        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ChartCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta == 0)
            {
                return;
            }

            ChartCollection<AxisAbs> axes = this.Axes;
            if (axes == null)
            {
                return;
            }

            //缩放坐标
            List<AxisAbs> zoomedAxisList = null;
            Canvas chartCanvas = this._chartCanvas;
            double zoomChangeSize, translate;
            if (e.Delta > 0)
            {
                //放大
                foreach (var axis in axes)
                {
                    if (!axis.AllowZoomTranslate)
                    {
                        continue;
                    }

                    if (axis.ZoomOut(chartCanvas, out zoomChangeSize))
                    {
                        if (zoomedAxisList == null)
                        {
                            zoomedAxisList = new List<AxisAbs>();
                        }

                        translate = (ChartConstant.ZERO_D - zoomChangeSize) / 2;
                        zoomedAxisList.Add(axis);
                        switch (axis.AxisType)
                        {
                            case AxisType.Y:
                                axis.TranslateY(translate);
                                break;
                            case AxisType.X:
                                axis.TranslateX(translate);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
            else
            {
                //缩小
                foreach (var axis in axes)
                {
                    if (!axis.AllowZoomTranslate)
                    {
                        continue;
                    }

                    if (axis.ZoomIn(chartCanvas, out zoomChangeSize))
                    {
                        if (zoomedAxisList == null)
                        {
                            zoomedAxisList = new List<AxisAbs>();
                        }
                        translate = (ChartConstant.ZERO_D - zoomChangeSize) / 2;
                        zoomedAxisList.Add(axis);
                        switch (axis.AxisType)
                        {
                            case AxisType.Y:
                                axis.TranslateY(translate);
                                break;
                            case AxisType.X:
                                axis.TranslateX(translate);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }

            //更新series
            ChartCollection<ISeries> seriesCollection = this.Series;
            if (seriesCollection != null && zoomedAxisList != null)
            {
                foreach (var series in seriesCollection)
                {
                    if (zoomedAxisList.Contains(series.AxisX) || zoomedAxisList.Contains(series.AxisY))
                    {
                        series.Update();
                    }
                }
            }
        }
    }
}