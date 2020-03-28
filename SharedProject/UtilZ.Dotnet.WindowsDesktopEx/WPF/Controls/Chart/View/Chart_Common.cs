using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    //Common
    public partial class Chart
    {
        private LegendAddResult AddLegend(IChartLegend legend, Grid chartGrid)
        {
            double left = 0d, top = 0d, right = 0d, bottom = 0d;
            double legendSize = double.NaN;
            bool hasLegend = false;
            if (legend != null)
            {
                FrameworkElement legendControl = legend.LegendControl;
                if (legendControl != null)
                {
                    legendSize = legend.Size;
                    if (legendSize > ChartConstant.ZERO_D)
                    {
                        chartGrid.Children.Add(legendControl);//使用Grid包装,主是为了居中对齐好布局,Canvas中水平垂直方向太麻烦
                        hasLegend = true;

                        switch (legend.DockOrientation)
                        {
                            case ChartDockOrientation.Left:
                                left = legendSize;
                                break;
                            case ChartDockOrientation.Top:
                                top = legendSize;
                                break;
                            case ChartDockOrientation.Right:
                                right = legendSize;
                                break;
                            case ChartDockOrientation.Bottom:
                                bottom = legendSize;
                                break;
                            default:
                                throw new NotImplementedException(legend.DockOrientation.ToString());
                        }
                    }
                }
            }

            return new LegendAddResult(hasLegend, left, top, right, bottom);
        }

        private AxisXHeightInfo CalculateAxisXHeight(ChartCollection<AxisAbs> axisCollection)
        {
            double topAxisTotalHeight = ChartConstant.ZERO_D, bottomAxisTotalHeight = ChartConstant.ZERO_D;
            if (axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I)
            {
                double axisHeight;
                foreach (var axis in axisCollection)
                {
                    axis.Validate();//验证坐标轴有效性

                    if (axis.AxisType != AxisType.X)
                    {
                        continue;
                    }

                    axisHeight = axis.GetXAxisHeight();
                    if (axis.IsAxisXBottom())
                    {
                        bottomAxisTotalHeight += axisHeight;
                    }
                    else
                    {
                        topAxisTotalHeight += axisHeight;
                    }
                }
            }

            return new AxisXHeightInfo(topAxisTotalHeight, bottomAxisTotalHeight);
        }

        private AxisYWidthInfo DrawAxisYByAxisXHeightInfo(ChartCollection<AxisAbs> axisCollection, UIElementCollection children,
            ChartCollection<ISeries> seriesCollection, double yAxisHeight, double top)
        {
            if (!ChartHelper.DoubleHasValue(yAxisHeight) || yAxisHeight <= ChartConstant.ZERO_D)
            {
                return new AxisYWidthInfo(ChartConstant.ZERO_D, ChartConstant.ZERO_D, null);
            }

            double leftAxisTotalWidth = ChartConstant.ZERO_D, rightAxisTotalWidth = ChartConstant.ZERO_D;
            Dictionary<AxisAbs, List<double>> axisYLabelDic = null;
            if (axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I)
            {
                FrameworkElement axisYControl;
                double axisLeft = ChartConstant.ZERO_D, axisRight = ChartConstant.ZERO_D;
                List<double> yList;
                foreach (var axis in axisCollection)
                {
                    if (axis.AxisType != AxisType.Y)
                    {
                        continue;
                    }

                    yList = axis.DrawY(seriesCollection, yAxisHeight);
                    if (axis.EnableBackgroundLabelLine && yList != null)
                    {
                        if (axisYLabelDic == null)
                        {
                            axisYLabelDic = new Dictionary<AxisAbs, List<double>>();
                        }
                        axisYLabelDic.Add(axis, yList);
                    }

                    axisYControl = axis.AxisControl;
                    axisYControl.VerticalAlignment = VerticalAlignment.Top;
                    children.Add(axisYControl);

                    if (axis.IsAxisYLeft())
                    {
                        axisYControl.HorizontalAlignment = HorizontalAlignment.Left;
                        axisYControl.Margin = new Thickness(axisLeft, top, ChartConstant.ZERO_D, ChartConstant.ZERO_D);
                        leftAxisTotalWidth += axis.Width;
                        axisLeft += axis.Width;
                    }
                    else
                    {
                        axisYControl.HorizontalAlignment = HorizontalAlignment.Right;
                        axisYControl.Margin = new Thickness(ChartConstant.ZERO_D, top, axisRight, ChartConstant.ZERO_D);
                        rightAxisTotalWidth += axis.Width;
                        axisRight += axis.Width;
                    }
                }
            }

            return new AxisYWidthInfo(leftAxisTotalWidth, rightAxisTotalWidth, axisYLabelDic);
        }

        private Dictionary<AxisAbs, List<double>> DrawAxisX(ChartCollection<AxisAbs> axisCollection, ChartCollection<ISeries> seriesCollection,
            Grid chartGrid, double xAxisWidth, double left)
        {
            if (!ChartHelper.DoubleHasValue(xAxisWidth) || xAxisWidth <= ChartConstant.ZERO_D)
            {
                return null;
            }

            Dictionary<AxisAbs, List<double>> axisXLabelDic = null;
            bool hasAxis = axisCollection != null && axisCollection.Count > ChartConstant.ZERO_I;
            if (hasAxis)
            {
                FrameworkElement axisXControl;
                double axisTop = ChartConstant.ZERO_D, axisBottom = ChartConstant.ZERO_D;
                List<double> xList;

                foreach (var axis in axisCollection)
                {
                    if (axis.AxisType != AxisType.X)
                    {
                        continue;
                    }

                    xList = axis.DrawX(seriesCollection, xAxisWidth);
                    if (axis.EnableBackgroundLabelLine && xList != null)
                    {
                        if (axisXLabelDic == null)
                        {
                            axisXLabelDic = new Dictionary<AxisAbs, List<double>>();
                        }
                        axisXLabelDic.Add(axis, xList);
                    }

                    axisXControl = axis.AxisControl;
                    axisXControl.HorizontalAlignment = HorizontalAlignment.Left;
                    chartGrid.Children.Add(axisXControl);
                    if (axis.IsAxisXBottom())
                    {
                        axisXControl.VerticalAlignment = VerticalAlignment.Top;
                        axisXControl.Margin = new Thickness(left, axisTop, ChartConstant.ZERO_D, ChartConstant.ZERO_D);
                        axisTop += axis.Height;

                    }
                    else
                    {
                        axisXControl.VerticalAlignment = VerticalAlignment.Bottom;
                        axisXControl.Margin = new Thickness(left, ChartConstant.ZERO_D, ChartConstant.ZERO_D, axisBottom);
                        axisBottom += axis.Height;
                    }
                }
            }

            return axisXLabelDic;
        }

        private void DrawAxisBackgroundLabelLine(Canvas chartCanvas, Dictionary<AxisAbs, List<double>> axisYLabelDic, Dictionary<AxisAbs, List<double>> axisXLabelDic)
        {
            if (axisYLabelDic != null)
            {
                foreach (var kv in axisYLabelDic)
                {
                    kv.Key.AddBackgroundLabelLine(chartCanvas, kv.Value);
                }
            }

            if (axisXLabelDic != null)
            {
                foreach (var kv in axisXLabelDic)
                {
                    kv.Key.AddBackgroundLabelLine(chartCanvas, kv.Value);
                }
            }
        }














        private class DrawSeriesPara
        {
            public Grid ChartGrid { get; private set; }
            public Canvas ChartCanvas { get; private set; }
            public IEnumerable<ISeries> SeriesCollection { get; private set; }
            public LegendAddResult LegendAddResult { get; private set; }
            public IChartLegend Legend { get; private set; }

            public DrawSeriesPara(Grid chartGrid, Canvas chartCanvas, IEnumerable<ISeries> seriesCollection, LegendAddResult legendAddResult, IChartLegend legend)
            {
                this.ChartGrid = chartGrid;
                this.ChartCanvas = chartCanvas;
                this.SeriesCollection = seriesCollection;
                this.LegendAddResult = legendAddResult;
                this.Legend = legend;
            }
        }


        private void DrawSeries(Grid chartGrid, Canvas chartCanvas, IEnumerable<ISeries> seriesCollection,
            LegendAddResult legendAddResult, IChartLegend legend)
        {
            var para = new PartAsynWaitPara<DrawSeriesPara, object>();
            para.Para = new DrawSeriesPara(chartGrid, chartCanvas, seriesCollection, legendAddResult, legend);
            para.Caption = "正在绘制,请稍等...";
            para.Function = (inPara) =>
            {
                //第七步 绘各Series
                this.DrawSeries(chartCanvas, seriesCollection);

                //填充legend
                if (legendAddResult != null &&
                    legendAddResult.HasLegend &&
                    legend != null)
                {
                    inPara.Para.ChartGrid.Dispatcher.Invoke(new Action<IChartLegend, IEnumerable<ISeries>>((le, se) =>
                   {
                       this.UpdateLegend(le, se);
                   }), new object[] { inPara.Para.Legend, inPara.Para.SeriesCollection });
                }

                return null;
            };
            para.IsShowCancel = false;
            para.CancelAbort = false;
            para.AsynWaitBackground = Brushes.Transparent;
            WPFPartAsynWaitHelper.Wait(para, chartGrid);




            ////第七步 绘各Series
            //this.DrawSeries(chartCanvas, seriesCollection);

            ////填充legend
            //if (legendAddResult != null &&
            //    legendAddResult.HasLegend &&
            //    legend != null)
            //{
            //    this.UpdateLegend(legend, seriesCollection);
            //}
        }

        private void DrawSeries(Canvas chartCanvas, IEnumerable<ISeries> seriesCollection)
        {
            bool denyDraw = chartCanvas.Dispatcher.Invoke(new Func<bool>(() =>
            {
                return chartCanvas.Width <= ChartConstant.ZERO_D || chartCanvas.Height <= ChartConstant.ZERO_D;
            }));

            //if (chartCanvas.Width <= ChartConstant.ZERO_D || chartCanvas.Height <= ChartConstant.ZERO_D)
            if (denyDraw)
            {
                return;
            }

            if (seriesCollection != null)
            {
                foreach (ISeries series in seriesCollection)
                {
                    series.Draw(chartCanvas);
                }
            }
        }


        private void UpdateLegend(IChartLegend legend, IEnumerable<ISeries> seriesCollection)
        {
            var legendBrushList = new List<SeriesLegendItem>();
            if (seriesCollection != null)
            {
                foreach (ISeries series in seriesCollection)
                {
                    series.AppendLegendItemToList(legendBrushList);
                }
            }

            legend.UpdateLegend(legendBrushList);
        }


















        private void SetRowColumn(Grid chartGrid, FrameworkElement element, RowColumnDefinitionItem rowColumnDefinition)
        {
            if (element == null || rowColumnDefinition == null)
            {
                return;
            }

            if (chartGrid.RowDefinitions.Count > 1 && rowColumnDefinition.RowIndex != ChartConstant.ROW_COLUMN_DEFAULT_INDEX)
            {
                Grid.SetRow(element, rowColumnDefinition.RowIndex);
                if (rowColumnDefinition.RowSpan != ChartConstant.ROW_COLUMN_DEFAULT_INDEX)
                {
                    Grid.SetRowSpan(element, rowColumnDefinition.RowSpan);
                }
            }

            if (chartGrid.ColumnDefinitions.Count > 1 && rowColumnDefinition.ColumnIndex != ChartConstant.ROW_COLUMN_DEFAULT_INDEX)
            {
                Grid.SetColumn(element, rowColumnDefinition.ColumnIndex);
                if (rowColumnDefinition.ColumnSpan != ChartConstant.ROW_COLUMN_DEFAULT_INDEX)
                {
                    Grid.SetColumnSpan(element, rowColumnDefinition.ColumnSpan);
                }
            }
        }

    }
}
