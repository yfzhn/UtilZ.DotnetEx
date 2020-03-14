using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 坐标辅助类
    /// </summary>
    public class ChartHelper
    {
        /// <summary>
        /// 向上取整
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="muilt">放大倍数</param>
        /// <returns>处理结果</returns>
        public static double DoubleToCeilingInteger(double value, long? muilt = null)
        {
            long m;
            if (muilt == null)
            {
                m = CalDoubleToIntegerMuilt(value);
            }
            else
            {
                m = muilt.Value;
            }

            return Math.Ceiling(value / m) * m;
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        /// <param name="value">目标值</param>
        /// <param name="muilt">缩小倍数</param>
        /// <returns>处理结果</returns>
        public static double DoubleToFloorInteger(double value, long? muilt = null)
        {
            long m;
            if (muilt == null)
            {
                m = CalDoubleToIntegerMuilt(value);
            }
            else
            {
                m = muilt.Value;
            }

            return Math.Floor(value / m) * m;
        }


        /// <summary>
        /// 计算double值向上或向下取整的倍数
        /// </summary>
        /// <param name="value">目标值</param>
        /// <returns>double值向上或向下取整的倍数</returns>
        public static long CalDoubleToIntegerMuilt(double value)
        {
            int length = ((long)(Math.Abs(value) / 10)).ToString().Length;
            return (long)Math.Pow(10, length);
        }



        /// <summary>
        /// 判断double什是否有效[有效返回true;无效返回false]
        /// </summary>
        /// <param name="value">目标值</param>
        /// <returns>有效返回true;无效返回false</returns>
        public static bool DoubleHasValue(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// 绘制X轴坐标刻度线
        /// </summary>
        /// <param name="axis">X轴坐标</param>
        /// <param name="canvas">画布</param>
        /// <param name="x1">第一个X</param>
        /// <param name="x2">第二个X</param>
        public static void DrawXAxisLabelLine(AxisAbs axis, Canvas canvas, double x1, double x2)
        {
            if (!axis.DrawAxisLine || !DoubleHasValue(x1) || !DoubleHasValue(x2))
            {
                return;
            }

            var labelLinePath = new Path();
            labelLinePath.Style = axis.AxisLineStyle;
            if (labelLinePath.Style == null)
            {
                labelLinePath.Style = ChartStyleHelper.GetDefaultAxisLabelLineStyle();
            }

            Point point1, point2;
            if (axis.IsAxisXBottom())
            {
                point1 = new Point(ChartConstant.ZERO_D, ChartConstant.ZERO_D);
                point2 = new Point(canvas.Width, ChartConstant.ZERO_D);
            }
            else
            {
                point1 = new Point(ChartConstant.ZERO_D, canvas.Height);
                point2 = new Point(canvas.Width, canvas.Height);
            }

            PathFigure labelPathFigure = new PathFigure();
            labelPathFigure.StartPoint = point1;
            labelPathFigure.Segments.Add(new LineSegment(point2, true));
            labelLinePath.Data = new PathGeometry()
            {
                Figures = new PathFigureCollection(new PathFigure[] { labelPathFigure })
            };
            canvas.Children.Add(labelLinePath);
        }

        /// <summary>
        /// 绘制X轴坐标刻度线
        /// </summary>
        /// <param name="axis">X轴坐标</param>
        /// <param name="canvas">画布</param>
        /// <param name="xList">X轴刻度集合</param>
        public static void DrawXAxisLabelLine(AxisAbs axis, Canvas canvas, List<double> xList)
        {
            if (!axis.DrawAxisLine || xList == null || xList.Count == 0)
            {
                return;
            }

            var labelLinePath = new Path();
            labelLinePath.Style = axis.AxisLineStyle;
            if (labelLinePath.Style == null)
            {
                labelLinePath.Style = ChartStyleHelper.GetDefaultAxisLabelLineStyle();
            }

            GeometryGroup geometryGroup = new GeometryGroup();
            Point point1, point2;
            double x;
            int lastIndex = xList.Count - 1;

            for (int i = 0; i < xList.Count; i++)
            {
                x = xList[i];
                if (axis.IsAxisXBottom())
                {
                    point1 = new Point(x, ChartConstant.ZERO_D);
                    point2 = new Point(x, axis.LabelSize);
                }
                else
                {
                    point1 = new Point(x, canvas.Height - axis.LabelSize);
                    point2 = new Point(x, canvas.Height);
                }

                PathFigure labelPathFigure = new PathFigure();
                labelPathFigure.StartPoint = point1;
                labelPathFigure.Segments.Add(new LineSegment(point2, true));
                geometryGroup.Children.Add(new PathGeometry()
                {
                    Figures = new PathFigureCollection(new PathFigure[] { labelPathFigure })
                });
            }


            //坐标轴
            if (axis.IsAxisXBottom())
            {
                point1 = new Point(xList.First(), ChartConstant.ZERO_D);
                point2 = new Point(xList.Last(), ChartConstant.ZERO_D);
            }
            else
            {
                point1 = new Point(xList.First(), canvas.Height);
                point2 = new Point(xList.Last(), canvas.Height);
            }
            PathFigure axisPathFigure = new PathFigure();
            axisPathFigure.StartPoint = point1;
            axisPathFigure.Segments.Add(new LineSegment(point2, true));
            geometryGroup.Children.Add(new PathGeometry()
            {
                Figures = new PathFigureCollection(new PathFigure[] { axisPathFigure })
            });


            labelLinePath.Data = geometryGroup;
            canvas.Children.Add(labelLinePath);
        }

        /// <summary>
        /// 绘制Y轴坐标刻度线
        /// </summary>
        /// <param name="axis">Y轴坐标</param>
        /// <param name="canvas">画布</param>
        /// <param name="y1">第一个Y</param>
        /// <param name="y2">第二个Y</param>
        public static void DrawYAxisLabelLine(AxisAbs axis, Canvas canvas, double y1, double y2)
        {
            if (!axis.DrawAxisLine || !DoubleHasValue(y1) || !DoubleHasValue(y2))
            {
                return;
            }

            var labelLinePath = new Path();
            labelLinePath.Style = axis.AxisLineStyle;
            if (labelLinePath.Style == null)
            {
                labelLinePath.Style = ChartStyleHelper.GetDefaultAxisLabelLineStyle();
            }

            Point point1, point2;
            if (axis.IsAxisYLeft())
            {
                point1 = new Point(canvas.Width, y1);
                point2 = new Point(canvas.Width, y2);
            }
            else
            {
                point1 = new Point(ChartConstant.ZERO_D, y1);
                point2 = new Point(ChartConstant.ZERO_D, y2);
            }

            PathFigure labelPathFigure = new PathFigure();
            labelPathFigure.StartPoint = point1;
            labelPathFigure.Segments.Add(new LineSegment(point2, true));
            labelLinePath.Data = new PathGeometry()
            {
                Figures = new PathFigureCollection(new PathFigure[] { labelPathFigure })
            };
            canvas.Children.Add(labelLinePath);
        }

        /// <summary>
        /// 绘制Y轴坐标刻度线
        /// </summary>
        /// <param name="axis">Y轴坐标</param>
        /// <param name="canvas">画布</param>
        /// <param name="yList">Y轴刻度集合</param>
        public static void DrawYAxisLabelLine(AxisAbs axis, Canvas canvas, List<double> yList)
        {
            if (!axis.DrawAxisLine || yList == null || yList.Count == 0)
            {
                return;
            }

            var labelLinePath = new Path();
            labelLinePath.Style = axis.AxisLineStyle;
            if (labelLinePath.Style == null)
            {
                labelLinePath.Style = ChartStyleHelper.GetDefaultAxisLabelLineStyle();
            }

            GeometryGroup geometryGroup = new GeometryGroup();
            Point point1, point2;
            double y;
            int lastIndex = yList.Count - 1;

            for (int i = 0; i < yList.Count; i++)
            {
                y = yList[i];
                if (axis.IsAxisYLeft())
                {
                    point1 = new Point(canvas.Width - axis.LabelSize, y);
                    point2 = new Point(canvas.Width, y);
                }
                else
                {
                    point1 = new Point(ChartConstant.ZERO_D, y);
                    point2 = new Point(axis.LabelSize, y);
                }

                PathFigure labelPathFigure = new PathFigure();
                labelPathFigure.StartPoint = point1;
                labelPathFigure.Segments.Add(new LineSegment(point2, true));
                geometryGroup.Children.Add(new PathGeometry()
                {
                    Figures = new PathFigureCollection(new PathFigure[] { labelPathFigure })
                });
            }

            //坐标轴
            if (axis.IsAxisYLeft())
            {
                point1 = new Point(canvas.Width, yList.First());
                point2 = new Point(canvas.Width, yList.Last());
            }
            else
            {
                point1 = new Point(ChartConstant.ZERO_D, yList.First());
                point2 = new Point(ChartConstant.ZERO_D, yList.Last());
            }
            PathFigure axisPathFigure = new PathFigure();
            axisPathFigure.StartPoint = point1;
            axisPathFigure.Segments.Add(new LineSegment(point2, true));
            geometryGroup.Children.Add(new PathGeometry()
            {
                Figures = new PathFigureCollection(new PathFigure[] { axisPathFigure })
            });


            labelLinePath.Data = geometryGroup;
            canvas.Children.Add(labelLinePath);
        }




        /// <summary>
        /// 计算LabelStepX坐标轴宽度或Y坐标轴高度
        /// </summary>
        /// <param name="area">坐标值范围</param>
        /// <param name="axisSize">X坐标轴宽度或Y坐标轴高度</param>
        /// <param name="labelStepValue">labelStep值</param>
        /// <returns>LabelStepX坐标轴宽度或Y坐标轴高度</returns>
        public static double CalculateLabelStepSize(double area, double axisSize, double labelStepValue)
        {
            return labelStepValue * axisSize / area;
        }



        /// <summary>
        /// 创建坐标轴Label控件
        /// </summary>
        /// <param name="axis">坐标轴</param>
        /// <param name="labelText">标签文本</param>
        /// <returns>Label控件</returns>
        public static TextBlock CreateLabelControl(AxisAbs axis, string labelText)
        {
            var textBlock = new TextBlock();
            textBlock.Text = labelText;
            if (axis.LabelStyle == null)
            {
                textBlock.Style = ChartStyleHelper.GetAxisLabelStyle(axis.DockOrientation);
            }
            else
            {
                textBlock.Style = axis.LabelStyle;
            }

            return textBlock;
        }





        private static TextBlock _measureTextLabel = null;
        /// <summary>
        /// 测量标签文本大小
        /// </summary>
        /// <param name="axis">坐标轴</param>
        /// <param name="labelText">标签文本</param>
        /// <returns>标签文本大小</returns>
        public static Size MeasureLabelTextSize(AxisAbs axis, string labelText)
        {
            if (_measureTextLabel == null)
            {
                _measureTextLabel = new TextBlock();
            }

            TextBlock measureTextLabel = _measureTextLabel;
            measureTextLabel.Text = labelText;
            if (axis.LabelStyle == null)
            {
                measureTextLabel.Style = ChartStyleHelper.GetAxisLabelStyle(axis.DockOrientation);
            }
            else
            {
                measureTextLabel.Style = axis.LabelStyle;
            }

            var size = UITextHelper.MeasureTextSize(measureTextLabel);
            measureTextLabel.Style = null;
            return size;
        }



        /// <summary>
        /// 获取IChartItem中坐标轴的值
        /// </summary>
        /// <param name="item">目标项</param>
        /// <param name="x">X轴为true;Y轴为false</param>
        /// <returns>IChartItem中坐标轴的值</returns>
        public static object GetChartItemAxisValue(IChartItem item, bool x)
        {
            if (item == null)
            {
                return null;
            }
             ;
            object value;
            if (item is IChartAxisValue)
            {
                var chartAxisValue = (IChartAxisValue)item;
                if (x)
                {
                    value = chartAxisValue.GetXValue();
                }
                else
                {
                    value = chartAxisValue.GetYValue();
                }
            }
            else if (item is IChartChildValue)
            {
                value = ((IChartChildValue)item).GetValue();
            }
            else
            {
                throw new NotSupportedException($"类型{item.GetType().FullName}未实现{nameof(IChartValue)}或{nameof(IChartChildValue)}接口");
            }

            return value;
        }







        /// <summary>
        /// 创建ColumnSeries显示控件
        /// </summary>
        /// <param name="series">ColumnSeries</param>
        /// <returns>ColumnSeries显示控件</returns>
        internal static Rectangle CreateColumn(IColumnSeries series)
        {
            var column = new Rectangle();
            column.Style = series.GetStyle();
            return column;
        }

        /// <summary>
        /// 设置ColumnSeriesTooltip
        /// </summary>
        /// <param name="series">ColumnSeries</param>
        /// <param name="tooltipText">Tooltip</param>
        /// <param name="column">Column控件</param>
        internal static void SetColumnTooltipText(IColumnSeries series, string tooltipText, FrameworkElement column)
        {
            if (series.EnableTooltip &&
                !string.IsNullOrWhiteSpace(tooltipText))
            {
                column.ToolTip = tooltipText;
            }
        }


        /// <summary>
        /// 将object转换为double,转换失败返回double.NaN
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <returns>转换结果</returns>
        public static double ConvertToDouble(object obj)
        {
            if (obj == null)
            {
                return double.NaN;
            }

            double value;
            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                try
                {
                    value = Convert.ToDouble(obj);
                }
                catch
                {
                    value = double.NaN;
                }
            }

            return value;
        }



        /// <summary>
        /// 将object转换为DateTime,转换失败返回null
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <returns>转换结果</returns>
        public static DateTime? ConvertToDateTime(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            DateTime? value;
            if (obj is DateTime)
            {
                value = (DateTime)obj;
            }
            else
            {
                try
                {
                    value = Convert.ToDateTime(obj);
                }
                catch
                {
                    value = null;
                }
            }

            return value;
        }
    }
}
