using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.Base;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 雷达Series
    /// </summary>
    public class RadarSeries : SeriesAbs
    {
        /// <summary>
        /// 获取或设置X坐标轴
        /// </summary>
        public override AxisAbs AxisX
        {
            get => throw new NotSupportedException("雷达图不需要指定坐标轴");
            set => throw new NotSupportedException("雷达图不需要指定坐标轴");
        }
        /// <summary>
        /// 获取或设置Y坐标轴
        /// </summary>
        public override AxisAbs AxisY
        {
            get => throw new NotSupportedException("雷达图不需要指定坐标轴");
            set => throw new NotSupportedException("雷达图不需要指定坐标轴");
        }
        /// <summary>
        /// 获取或设置创建坐标点对应的附加控件回调
        /// </summary>
        public override Func<PointInfo, FrameworkElement> CreatePointFunc
        {
            get => throw new NotSupportedException("雷达图不支持创建自定义点标注");
            set => throw new NotSupportedException("雷达图不支持创建自定义点标注");
        }
        /// <summary>
        /// 获取或设置Tooltip有效区域,鼠标点周围范围内有点则触发Tooltip,小于0使用默认值
        /// </summary>
        public override double TooltipArea
        {
            get => throw new NotSupportedException("雷达图不支持此属性");
            set => throw new NotSupportedException("雷达图不支持此属性");
        }


        private double _radius = double.NaN;
        /// <summary>
        /// 雷达图半径,小于等于0或为IsInfinity或NaN使用控件高度和宽度中的最小值
        /// </summary>
        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                base.OnRaisePropertyChanged(nameof(Radius));
            }
        }




        /// <summary>
        /// 构造函数
        /// </summary>
        public RadarSeries()
        {

        }



        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        protected override bool PrimitiveClear(Canvas canvas)
        {
            foreach (var frameworkElement in this._frameworkElementList)
            {
                canvas.Children.Remove(frameworkElement);
            }
            this._frameworkElementList.Clear();
            return false;
        }


        private readonly List<FrameworkElement> _frameworkElementList = new List<FrameworkElement>();
        /// <summary>
        /// 将Series绘制到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        protected override void PrimitiveDraw(Canvas canvas)
        {
            this._frameworkElementList.Clear();
            Dictionary<IChartLabelValue, RadarLabel> radarLabelDic = this.GetLabelList();
            if (radarLabelDic == null || radarLabelDic.Count < 2)
            {
                //少于2个点无法画线
                return;
            }

            double r = this.GetRadius(canvas, radarLabelDic.Values.ElementAt(0).LabelValue.LabelStyle);
            double R = r * 2;
            double yOffset = (canvas.Height - R) / 2;
            double xOffset = (canvas.Width - R) / 2;

            this.DrawLabelAxis(canvas, radarLabelDic, r, yOffset, xOffset);
            this.DrawSeries(canvas, radarLabelDic, r, yOffset, xOffset);
        }

        private void DrawSeries(Canvas canvas, Dictionary<IChartLabelValue, RadarLabel> radarLabelDic, double r, double yOffset, double xOffset)
        {
            IChartNoAxisValue chartNoAxisValue;
            IChartRadarValue chartRadarValue;
            IEnumerable enumerable;
            IChartLabelValue chartLabelValue;
            RadarLabel radarLabel;
            double x, y, r2;

            for (int i = 0; i < base._values.Count; i++)
            {
                chartNoAxisValue = base._values[i] as IChartNoAxisValue;
                if (chartNoAxisValue == null)
                {
                    continue;
                }

                Path path = new Path();
                path.Style = chartNoAxisValue.Style;
                if (path.Style == null)
                {
                    path.Style = ChartStyleHelper.CreateRadarSeriesItemStytle(ColorBrushHelper.GetColorByIndex(i));
                }
                base.AddLegendItem(new SeriesLegendItem(path.Stroke.Clone(), chartNoAxisValue.Title, this));

                enumerable = chartNoAxisValue.GetValue() as IEnumerable;
                if (enumerable == null)
                {
                    continue;
                }

                var polyLineSegment = new PolyLineSegment();
                foreach (var item in enumerable)
                {
                    chartRadarValue = item as IChartRadarValue;
                    if (chartRadarValue == null)
                    {
                        continue;
                    }

                    chartLabelValue = chartRadarValue.GetLabel();
                    if (chartLabelValue == null)
                    {
                        continue;
                    }

                    if (!radarLabelDic.TryGetValue(chartLabelValue, out radarLabel))
                    {
                        continue;
                    }

                    double rvalue = chartRadarValue.GetValue();
                    if (!ChartHelper.DoubleHasValue(rvalue))
                    {
                        continue;
                    }

                    r2 = r * (rvalue - radarLabel.MinValue) / radarLabel.Area;
                    x = Math.Cos(radarLabel.Radians) * r2 + r + xOffset;
                    y = Math.Sin(radarLabel.Radians) * r2 + r + yOffset;
                    polyLineSegment.Points.Add(new Point(x, y));
                }

                if (polyLineSegment.Points.Count < 2)
                {
                    //少于2个点无法画线
                    continue;
                }

                var pathFigure = new PathFigure();
                pathFigure.StartPoint = polyLineSegment.Points[0];
                pathFigure.IsClosed = true;
                pathFigure.Segments.Add(polyLineSegment);

                PathFigureCollection pfc = new PathFigureCollection();
                pfc.Add(pathFigure);

                if (this.EnableTooltip)
                {
                    path.ToolTip = chartNoAxisValue.TooltipText;
                }
                path.Tag = chartNoAxisValue.TooltipText;

                path.Data = new PathGeometry(pfc);
                canvas.Children.Add(path);
                this._frameworkElementList.Add(path);
            }
        }

        private void DrawLabelAxis(Canvas canvas, Dictionary<IChartLabelValue, RadarLabel> radarLabelDic, double r, double yOffset, double xOffset)
        {
            double angle = MathEx.ANGLE_360 / radarLabelDic.Count;
            double appendAngle = ChartConstant.ZERO_D, radians;
            Quadrant quadrant;
            double tbLabelLeft, tbLabelTop;
            var polyLineSegment = new PolyLineSegment();

            for (int i = 0; i < radarLabelDic.Values.Count; i++)
            {
                var label = radarLabelDic.Values.ElementAt(i);
                label.CalArea();

                Line axisLine = new Line();
                axisLine.Visibility = base.Visibility;
                axisLine.Style = label.LabelValue.LabelLineStyle;
                if (axisLine.Style == null)
                {
                    axisLine.Style = ChartStyleHelper.CreateRadarSeriesAxisStytle(ColorBrushHelper.GetColorByIndex(i));
                }

                axisLine.X1 = r + xOffset;
                axisLine.Y1 = r + yOffset;

                radians = MathEx.AngleToRadians(appendAngle);
                //+r是为了平移坐标
                axisLine.X2 = Math.Cos(radians) * r + r + xOffset;
                axisLine.Y2 = Math.Sin(radians) * r + r + yOffset;
                label.Radians = radians;

                canvas.Children.Add(axisLine);
                this._frameworkElementList.Add(axisLine);


                TextBlock tbLabel = new TextBlock();
                tbLabel.Text = label.LabelValue.Label;
                tbLabel.Style = label.LabelValue.LabelStyle;
                canvas.Children.Add(tbLabel);
                this._frameworkElementList.Add(tbLabel);


                Size labelTextSize = UITextHelper.MeasureTextSize(tbLabel);
                quadrant = MathEx.GetQuadrantByAngle(MathEx.ANGLE_360 - appendAngle);
                tbLabelLeft = axisLine.X2;
                tbLabelTop = axisLine.Y2;
                switch (quadrant)
                {
                    case Quadrant.PositiveXAxisAngle:
                        //tbLabelLeft = ChartConstant.ZERO_D;
                        tbLabelTop -= labelTextSize.Height / 2;
                        break;
                    case Quadrant.One:
                        //tbLabelLeft = ChartConstant.ZERO_D;
                        tbLabelTop -= labelTextSize.Height;
                        break;
                    case Quadrant.PositiveYAxisAngle:
                        tbLabelLeft -= labelTextSize.Width / 2;
                        tbLabelTop -= labelTextSize.Height;
                        break;
                    case Quadrant.Two:
                        tbLabelLeft -= labelTextSize.Width;
                        tbLabelTop -= labelTextSize.Height;
                        break;
                    case Quadrant.NegativeXAxisAngle:
                        tbLabelLeft -= labelTextSize.Width;
                        tbLabelTop -= labelTextSize.Height / 2;
                        break;
                    case Quadrant.Three:
                        tbLabelLeft -= labelTextSize.Width;
                        //tbLabelTop = ChartConstant.ZERO_D;
                        break;
                    case Quadrant.NegativeYAxisAngle:
                        tbLabelLeft -= labelTextSize.Width / 2;
                        //tbLabelTop = ChartConstant.ZERO_D;
                        break;
                    case Quadrant.Four:
                        //tbLabelLeft = ChartConstant.ZERO_D;
                        //tbLabelTop = ChartConstant.ZERO_D;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Canvas.SetLeft(tbLabel, tbLabelLeft);
                Canvas.SetTop(tbLabel, tbLabelTop);
                appendAngle = appendAngle + angle;

                polyLineSegment.Points.Add(new Point(axisLine.X2, axisLine.Y2));
            }


            var pathFigure = new PathFigure();
            pathFigure.StartPoint = polyLineSegment.Points[0];
            pathFigure.IsClosed = true;
            pathFigure.Segments.Add(polyLineSegment);

            PathFigureCollection pfc = new PathFigureCollection();
            pfc.Add(pathFigure);

            Path path = new Path();
            path.Style = this.Style;
            if (path.Style == null)
            {
                path.Style = ChartStyleHelper.CreateLineSeriesStyle(ColorBrushHelper.GetColorByIndex(0), 1d, ChartConstant.ZERO_D);
            }
            path.Data = new PathGeometry(pfc);
            canvas.Children.Add(path);
            this._frameworkElementList.Add(path);
        }

        private double GetRadius(Canvas canvas, Style labelStyle)
        {
            double radius = this._radius;
            if (!ChartHelper.DoubleHasValue(radius))
            {
                radius = canvas.Width;
                if (canvas.Height - radius < ChartConstant.ZERO_D)
                {
                    radius = canvas.Height;
                }
                radius = radius / 2;

                TextBlock tbLabel = new TextBlock();
                tbLabel.Text = "测量TextBlock";
                tbLabel.Style = labelStyle;
                Size labelTextSize = UITextHelper.MeasureTextSize(tbLabel);
                radius -= labelTextSize.Height;
            }

            return radius;
        }

        private Dictionary<IChartLabelValue, RadarLabel> GetLabelList()
        {
            if (base._values == null || base._values.Count == 0)
            {
                return null;
            }

            IChartNoAxisValue chartNoAxisValue;
            IChartRadarValue chartRadarValue;
            IEnumerable enumerable;
            Dictionary<IChartLabelValue, RadarLabel> radarLabelDic = new Dictionary<IChartLabelValue, RadarLabel>();
            IChartLabelValue chartLabelValue;
            RadarLabel radarLabel;

            foreach (var value in base._values)
            {
                chartNoAxisValue = value as IChartNoAxisValue;
                if (chartNoAxisValue == null)
                {
                    continue;
                }

                enumerable = chartNoAxisValue.GetValue() as IEnumerable;
                if (enumerable == null)
                {
                    continue;
                }

                foreach (var item in enumerable)
                {
                    chartRadarValue = item as IChartRadarValue;
                    if (chartRadarValue == null)
                    {
                        continue;
                    }

                    chartLabelValue = chartRadarValue.GetLabel();
                    if (chartLabelValue == null)
                    {
                        continue;
                    }

                    if (radarLabelDic.TryGetValue(chartLabelValue, out radarLabel))
                    {
                        double rvalue = chartRadarValue.GetValue();
                        if (!ChartHelper.DoubleHasValue(rvalue))
                        {
                            continue;
                        }

                        if (!ChartHelper.DoubleHasValue(radarLabel.MaxValue) ||
                            radarLabel.MaxValue - rvalue < ChartConstant.ZERO_D)
                        {
                            radarLabel.MaxValue = rvalue;
                        }

                        if (!ChartHelper.DoubleHasValue(radarLabel.MinValue) ||
                            rvalue - radarLabel.MinValue < ChartConstant.ZERO_D)
                        {
                            radarLabel.MinValue = rvalue;
                        }
                    }
                    else
                    {
                        radarLabel = new RadarLabel(chartLabelValue);
                        radarLabelDic.Add(chartLabelValue, radarLabel);
                    }
                }
            }

            //foreach (var item in radarLabelDic.Values)
            //{
            //    if (!ChartHelper.DoubleHasValue(item.MaxValue))
            //    {
            //        item.MaxValue = ChartConstant.ZERO_D;
            //    }

            //    if (!ChartHelper.DoubleHasValue(item.MinValue))
            //    {
            //        item.MinValue = ChartConstant.ZERO_D;
            //    }
            //}
            return radarLabelDic;
        }




        /// <summary>
        /// EnableTooltip改变通知
        /// </summary>
        /// <param name="enableTooltip">新EnableTooltip值</param>
        protected override void EnableTooltipChanged(bool enableTooltip)
        {
            foreach (var frameworkElement in this._frameworkElementList)
            {
                if (frameworkElement is Path)
                {
                    if (enableTooltip)
                    {
                        frameworkElement.ToolTip = frameworkElement.Tag;
                    }
                    else
                    {
                        frameworkElement.ToolTip = null;
                    }
                }
            }
        }



        /// <summary>
        /// Series样式改变通知
        /// </summary>
        /// <param name="style">新样式</param>
        protected override void StyleChanged(Style style)
        {
            throw new NotSupportedException("不支持的操作");
        }




        /// <summary>
        /// Visibility改变通知
        /// </summary>
        /// <param name="oldVisibility">旧值</param>
        /// <param name="newVisibility">新值</param>
        protected override void VisibilityChanged(Visibility oldVisibility, Visibility newVisibility)
        {
            foreach (var frameworkElement in this._frameworkElementList)
            {
                frameworkElement.Visibility = newVisibility;
            }
        }
    }

    internal class RadarLabel
    {
        public IChartLabelValue LabelValue { get; private set; }

        /// <summary>
        /// 最小值,double.NaN自动计算
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// 最大值,double.NaN自动计算
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// 坐标线弧度
        /// </summary>
        public double Radians { get; set; }

        public double Area { get; private set; }

        public RadarLabel(IChartLabelValue labelValue)
        {
            this.LabelValue = labelValue;
            this.MinValue = labelValue.MinValue;
            this.MaxValue = labelValue.MaxValue;
        }


        internal void CalArea()
        {
            this.Area = this.MaxValue - this.MinValue;
        }
    }
}
