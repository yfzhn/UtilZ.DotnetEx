using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    //FreezeX
    public partial class Chart
    {
        private void UpdateFreezeX(AxisFreezeInfo axisFreezeInfo, ChartCollection<AxisAbs> axisCollection, ChartCollection<ISeries> seriesCollection,
        IChartLegend legend, Canvas chartCanvas, Grid chartGrid, ScrollViewer scrollViewer, Grid chartContentGrid)
        {
            /************************************************************************************************************
             * 步骤:
             * 1.添加legend,并计算出发四周所占高度或宽度
             * 2.计算X轴总高度
             * 3.根据Y轴高度绘制Y轴,并计算Y轴宽度
             * 4.根据Y轴宽度计算X轴宽度并绘制X轴
             * 5.绘制坐标背景标记线
             * 6.绘各Series
             * 7.绘各Series和填充legend
             ************************************************************************************************************/

            this.Content = chartGrid;

            //第一步 添加legend,并计算出发四周所占高度或宽度
            LegendAddResult legendAddResult = this.AddLegend(legend, chartGrid);

            //第二步 计算X轴总高度
            AxisXHeightInfo axisXHeightInfo = this.CalculateAxisXHeight(axisCollection);


            double height = this.ActualHeight - axisXHeightInfo.TopAxisTotalHeight - axisXHeightInfo.BottomAxisTotalHeight - legendAddResult.Top - legendAddResult.Bottom;

            //第三步 根据Y轴高度绘制Y轴,并计算Y轴宽度
            double yAxisHeight = axisFreezeInfo.Height;
            if (yAxisHeight < ChartConstant.ZERO_D)
            {
                yAxisHeight = ChartConstant.ZERO_D;
            }
            else if (yAxisHeight - height < ChartConstant.ZERO_D)
            {
                //真实高度大于最小值,取更大值
                yAxisHeight = height;
            }
            AxisYWidthInfo axisYWidthInfo = this.DrawAxisYByAxisXHeightInfo(axisCollection, chartContentGrid.Children, seriesCollection, yAxisHeight, ChartConstant.ZERO_D);


            //第四步 根据Y轴宽度计算X轴宽度并绘制X轴
            double xAxisWidth = axisFreezeInfo.Width - axisYWidthInfo.LeftAxisTotalWidth - axisYWidthInfo.RightAxisTotalWidth - legendAddResult.Left - legendAddResult.Right - this._scrollBarWidth;
            if (xAxisWidth < ChartConstant.ZERO_D)
            {
                xAxisWidth = ChartConstant.ZERO_D;
            }
            Dictionary<AxisAbs, List<double>> axisXLabelDic = this.DrawAxisX(axisCollection, seriesCollection, chartGrid, xAxisWidth, axisYWidthInfo.LeftAxisTotalWidth);


            chartCanvas.Width = xAxisWidth;
            chartCanvas.Height = yAxisHeight;
            chartContentGrid.Children.Add(chartCanvas);

            //第五步 绘制坐标背景标记线
            this.DrawAxisBackgroundLabelLine(chartCanvas, axisYWidthInfo.AxisYLabelDic, axisXLabelDic);

            //第六步 布局UI
            this.FreezeXLayout(axisCollection, legend, chartCanvas, chartGrid, scrollViewer, chartContentGrid, legendAddResult, axisXHeightInfo, axisYWidthInfo);

            //第七步 绘各Series和填充legend
            this.DrawSeries(chartGrid, chartCanvas, seriesCollection, legendAddResult,legend);
        }

        private void FreezeXLayout(ChartCollection<AxisAbs> axisCollection, IChartLegend legend, Canvas chartCanvas, Grid chartGrid, ScrollViewer scrollViewer, Grid chartContentGrid, LegendAddResult legendAddResult, AxisXHeightInfo axisXHeightInfo, AxisYWidthInfo axisYWidthInfo)
        {
            //添加scrollViewer
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Content = chartContentGrid;
            scrollViewer.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            scrollViewer.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            chartGrid.Children.Add(scrollViewer);

            //上中下三行布局,legend另算
            var chartGridRowColumnDefinition = new ChartGridRowColumnDefinition(legendAddResult.HasLegend, legend, chartGrid, axisXHeightInfo);
            if (legendAddResult.HasLegend)
            {
                this.SetRowColumn(chartGrid, legend.LegendControl, chartGridRowColumnDefinition.Legend);
            }

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
                            rowColumnDefinition = chartGridRowColumnDefinition.TopAxis;
                            break;
                        case ChartDockOrientation.Bottom:
                            rowColumnDefinition = chartGridRowColumnDefinition.BottomAxis;
                            break;
                        case ChartDockOrientation.Left:
                        case ChartDockOrientation.Right:
                        default:
                            throw new NotImplementedException(axis.DockOrientation.ToString());
                    }

                    this.SetRowColumn(chartGrid, axis.AxisControl, rowColumnDefinition);
                }
            }

            this.SetRowColumn(chartGrid, scrollViewer, chartGridRowColumnDefinition.Chart);


            //左中右三列布局
            var chartGridRowColumnDefinition2 = new ChartGridRowColumnDefinition(chartContentGrid, axisYWidthInfo);
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
                            rowColumnDefinition = chartGridRowColumnDefinition2.LeftAxis;
                            break;
                        case ChartDockOrientation.Right:
                            rowColumnDefinition = chartGridRowColumnDefinition2.RightAxis;
                            break;
                        case ChartDockOrientation.Top:
                        case ChartDockOrientation.Bottom:
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
