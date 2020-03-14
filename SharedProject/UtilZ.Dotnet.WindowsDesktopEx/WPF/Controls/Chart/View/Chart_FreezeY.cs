using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    //FreezeY
    public partial class Chart
    {
        private void UpdateFreezeY(AxisFreezeInfo axisFreezeInfo, ChartCollection<AxisAbs> axisCollection, ChartCollection<ISeries> seriesCollection,
            IChartLegend legend, Canvas chartCanvas, Grid chartGrid, ScrollViewer scrollViewer, Grid chartContentGrid)
        {
            /************************************************************************************************************
            * 步骤:
            * 1.添加legend,并计算出发四周所占高度或宽度
            * 2.计算X轴总高度
            * 3.根据X轴总高度计算图表区域高度高度(等于Y轴高度)
            * 4.根据Y轴高度绘制Y轴,并计算Y轴宽度
            * 5.根据宽度绘制X轴
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
            double yAxisHeight = axisFreezeInfo.Height - axisXHeightInfo.TopAxisTotalHeight - axisXHeightInfo.BottomAxisTotalHeight - legendAddResult.Top - legendAddResult.Bottom - this._scrollBarWidth;
            if (yAxisHeight < ChartConstant.ZERO_D)
            {
                yAxisHeight = ChartConstant.ZERO_D;
            }

            //第四步 根据Y轴高度绘制Y轴,并计算Y轴宽度
            AxisYWidthInfo axisYWidthInfo = this.DrawAxisYByAxisXHeightInfo(axisCollection, chartGrid.Children, seriesCollection, yAxisHeight, axisXHeightInfo.TopAxisTotalHeight);


            //第五步 根据宽度绘制X轴
            double width = this.ActualWidth - axisYWidthInfo.LeftAxisTotalWidth - axisYWidthInfo.RightAxisTotalWidth - legendAddResult.Left - legendAddResult.Right;
            double xAxisWidth = axisFreezeInfo.Width;
            if (xAxisWidth < ChartConstant.ZERO_D)
            {
                xAxisWidth = ChartConstant.ZERO_D;
            }
            else if (xAxisWidth - width < ChartConstant.ZERO_D)
            {
                //真实宽度大于最小值,取更大值
                xAxisWidth = width;
            }
            Dictionary<AxisAbs, List<double>> axisXLabelDic = this.DrawAxisX(axisCollection, seriesCollection, chartContentGrid, xAxisWidth, ChartConstant.ZERO_D);

            chartCanvas.Width = xAxisWidth;
            chartCanvas.Height = yAxisHeight;
            chartContentGrid.Children.Add(chartCanvas);

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



            //添加scrollViewer
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.Content = chartContentGrid;
            scrollViewer.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            scrollViewer.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            chartGrid.Children.Add(scrollViewer);


            //左中右三列布局,legend另算
            var chartGridRowColumnDefinition = new ChartGridRowColumnDefinition(legendAddResult.HasLegend, legend, chartGrid, axisYWidthInfo);
            if (legendAddResult.HasLegend)
            {
                this.SetRowColumn(chartGrid, legend.LegendControl, chartGridRowColumnDefinition.Legend);
            }

            if (axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I)
            {
                RowColumnDefinitionItem rowColumnDefinition;
                foreach (var axis in axisCollection)
                {
                    if (axis.AxisType != AxisType.Y)
                    {
                        continue;
                    }

                    switch (axis.DockOrientation)
                    {
                        case ChartDockOrientation.Left:
                            rowColumnDefinition = chartGridRowColumnDefinition.LeftAxis;
                            break;
                        case ChartDockOrientation.Right:
                            rowColumnDefinition = chartGridRowColumnDefinition.RightAxis;
                            break;
                        case ChartDockOrientation.Top:
                        case ChartDockOrientation.Bottom:
                        default:
                            throw new NotImplementedException(axis.DockOrientation.ToString());
                    }

                    this.SetRowColumn(chartGrid, axis.AxisControl, rowColumnDefinition);
                }
            }

            this.SetRowColumn(chartGrid, scrollViewer, chartGridRowColumnDefinition.Chart);






            //上中下三行布局
            var chartGridRowColumnDefinition2 = new ChartGridRowColumnDefinition(chartContentGrid, axisXHeightInfo);
            if (axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I)
            {
                RowColumnDefinitionItem rowColumnDefinition;
                foreach (var axis in axisCollection)
                {
                    if (axis.AxisType != AxisType.X)
                    {
                        continue;
                    }

                    switch (axis.DockOrientation)
                    {
                        case ChartDockOrientation.Top:
                            rowColumnDefinition = chartGridRowColumnDefinition2.TopAxis;
                            break;
                        case ChartDockOrientation.Bottom:
                            rowColumnDefinition = chartGridRowColumnDefinition2.BottomAxis;
                            break;
                        case ChartDockOrientation.Left:
                        case ChartDockOrientation.Right:
                        default:
                            throw new NotImplementedException(axis.DockOrientation.ToString());
                    }

                    this.SetRowColumn(chartContentGrid, axis.AxisControl, rowColumnDefinition);
                }
            }
            this.SetRowColumn(chartContentGrid, chartCanvas, chartGridRowColumnDefinition2.Chart);
        }
    }
}
