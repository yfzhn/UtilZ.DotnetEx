using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    //FreezeAll
    public partial class Chart
    {
        private void UpdateFreezeAll(AxisFreezeInfo axisFreezeInfo, ChartCollection<AxisAbs> axisCollection, ChartCollection<ISeries> seriesCollection,
        IChartLegend legend, Canvas chartCanvas, Grid chartGrid, ScrollViewer scrollViewer, Grid chartContentGrid)
        {
            /************************************************************************************************************
            * 步骤:
            * 1.添加legend,并计算出发四周所占高度或宽度
            * 2.计算X轴总高度            
            * 3.根据Y轴高度绘制Y轴,并计算Y轴宽度
            * 4.根据X轴宽度绘制X轴
            * 5.绘制坐标背景标记线
            * 6.绘各Series
            * 7.填充legend
            * 8.计算Grid宽度和高度
            * 9.布局UI
            ************************************************************************************************************/

            this.Content = chartGrid;

            //第一步 添加legend,并计算出发四周所占高度或宽度
            LegendAddResult legendAddResult = this.AddLegend(legend, chartGrid);

            //第二步 计算X轴总高度
            AxisXHeightInfo axisXHeightInfo = this.CalculateAxisXHeight(axisCollection);
            double height = this.ActualHeight - axisXHeightInfo.TopAxisTotalHeight - axisXHeightInfo.BottomAxisTotalHeight - this._scrollBarWidth - legendAddResult.Top - legendAddResult.Bottom;

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

            //第四步 根据X轴宽度绘制X轴
            double width = this.ActualWidth - axisYWidthInfo.LeftAxisTotalWidth - axisYWidthInfo.RightAxisTotalWidth - this._scrollBarWidth - legendAddResult.Left - legendAddResult.Right;
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

            chartContentGrid.Width = xAxisWidth + axisYWidthInfo.LeftAxisTotalWidth + axisYWidthInfo.RightAxisTotalWidth;
            chartContentGrid.Height = yAxisHeight + axisXHeightInfo.TopAxisTotalHeight + axisXHeightInfo.BottomAxisTotalHeight;
            chartCanvas.Width = xAxisWidth;
            chartCanvas.Height = yAxisHeight;
            chartContentGrid.Children.Add(chartCanvas);

            //第五步 绘制坐标背景标记线
            this.DrawAxisBackgroundLabelLine(chartCanvas, axisYWidthInfo.AxisYLabelDic, axisXLabelDic);

            //第六步 绘各Series
            this.DrawSeries(chartCanvas, seriesCollection);

            //第七步 填充legend
            if (legendAddResult.HasLegend)
            {
                this.UpdateLegend(legend, seriesCollection);
            }


            //第八步 布局UI
            //添加scrollViewer
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            scrollViewer.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            scrollViewer.Content = chartContentGrid;
            chartGrid.Children.Add(scrollViewer);

            //布局legend
            var chartGridRowColumnDefinition = new ChartGridRowColumnDefinition(legendAddResult.HasLegend, legend, chartGrid);
            if (legendAddResult.HasLegend)
            {
                this.SetRowColumn(chartGrid, legend.LegendControl, chartGridRowColumnDefinition.Legend);
            }
            this.SetRowColumn(chartGrid, scrollViewer, chartGridRowColumnDefinition.Chart);


            //布局内部
            var chartGridRowColumnDefinition2 = new ChartGridRowColumnDefinition(false, null, chartContentGrid, axisYWidthInfo, axisXHeightInfo);
            if (axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I)
            {
                RowColumnDefinitionItem rowColumnDefinition;
                foreach (var axis in axisCollection)
                {
                    switch (axis.DockOrientation)
                    {
                        case ChartDockOrientation.Left:
                            rowColumnDefinition = chartGridRowColumnDefinition2.LeftAxis;
                            break;
                        case ChartDockOrientation.Top:
                            rowColumnDefinition = chartGridRowColumnDefinition2.TopAxis;
                            break;
                        case ChartDockOrientation.Right:
                            rowColumnDefinition = chartGridRowColumnDefinition2.RightAxis;
                            break;
                        case ChartDockOrientation.Bottom:
                            rowColumnDefinition = chartGridRowColumnDefinition2.BottomAxis;
                            break;
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
