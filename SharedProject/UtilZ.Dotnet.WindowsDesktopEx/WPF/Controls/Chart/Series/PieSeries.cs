using System;
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
    /// 饼图
    /// </summary>
    public class PieSeries : SeriesAbs
    {
        /// <summary>
        /// 获取或设置X坐标轴
        /// </summary>
        public override AxisAbs AxisX
        {
            get => throw new NotSupportedException("饼图不需要指定坐标轴");
            set => throw new NotSupportedException("饼图不需要指定坐标轴");
        }
        /// <summary>
        /// 获取或设置Y坐标轴
        /// </summary>
        public override AxisAbs AxisY
        {
            get => throw new NotSupportedException("饼图不需要指定坐标轴");
            set => throw new NotSupportedException("饼图不需要指定坐标轴");
        }
        /// <summary>
        /// 获取或设置创建坐标点对应的附加控件回调
        /// </summary>
        public override Func<PointInfo, FrameworkElement> CreatePointFunc
        {
            get => throw new NotSupportedException("饼图不支持创建自定义点标注");
            set => throw new NotSupportedException("饼图不支持创建自定义点标注");
        }
        /// <summary>
        /// 获取或设置Tooltip有效区域,鼠标点周围范围内有点则触发Tooltip,小于0使用默认值
        /// </summary>
        public override double TooltipArea
        {
            get => throw new NotSupportedException("饼图不支持此属性");
            set => throw new NotSupportedException("饼图不支持此属性");
        }
        /// <summary>
        /// 获取或设置Series样式
        /// </summary>
        public override Style Style
        {
            get => throw new NotSupportedException("饼图不支持此属性");
            set => throw new NotSupportedException("饼图不支持此属性");
        }

        private Style _labelStyle = null;
        /// <summary>
        /// 标签样式
        /// </summary>
        public Style LabelStyle
        {
            get { return _labelStyle; }
            set
            {
                _labelStyle = value;
                base.OnRaisePropertyChanged(nameof(LabelStyle));
            }
        }

        private double _radius = double.NaN;
        /// <summary>
        /// 饼图半径,小于等于0或为IsInfinity或NaN使用控件高度和宽度中的最小值
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

        //private double _pushOut = double.NaN;
        ///// <summary>
        ///// 选中的饼向外突出的距离,单位:像素.小于等于0或为IsInfinity或NaN此值无效
        ///// </summary>
        //public double PushOut
        //{
        //    get { return _pushOut; }
        //    set { _pushOut = value; }
        //}




        private readonly List<FrameworkElement> _elementList = new List<FrameworkElement>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public PieSeries()
            : base()
        {

        }



        /// <summary>
        /// 将Series添加到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        protected override void PrimitiveDraw(Canvas canvas)
        {
            this._elementList.Clear();
            base.RemoveLegendItem();

            Dictionary<IChartNoAxisValue, double> valueDic = this.GetValueDic();
            if (valueDic == null || valueDic.Count == 0)
            {
                return;
            }

            double r = this.GetRadius(canvas);
            double R = r * 2;
            double yOffset = (canvas.Height - R) / 2;
            double xOffset = (canvas.Width - R) / 2;
            Size arcSegmentSize = new Size(r, r);
            double total = valueDic.Values.Sum();
            double txtr = r * 2 / 3;


            /******************************************************************************************
             * 步骤:
             * 圆的标准方程(x - a)²+(y - b)²= r²中，有三个参数a、b、r，即圆心坐标为(a，b)，半径为r 
             * 此处计算时,以a=0,b=0,即加以在(0,0)的圆 => 圆的方程x²+y²= r²
             * => x=±(r/(Math.Sqrt(1+Math.Power(Math.Tan(angle),2)))) 
             * => y=Math.Tan(anglr)*x 
             ******************************************************************************************/


            IChartNoAxisValue pieValue;
            double value;
            Brush stroke = null;
            double beginAngle = ChartConstant.ZERO_D, endAngle = ChartConstant.ZERO_D, radians;
            double angle, x, y, txtX, txtY;
            Quadrant beginAngleQuadrant, endAngleQuadrant;

            Point center = new Point(r + xOffset, r + yOffset);
            Point lastPoint = new Point(R + xOffset, r + yOffset), point;
            double tbLabelLeft, tbLabelTop;

            for (int i = 0; i < valueDic.Count; i++)
            {
                pieValue = valueDic.ElementAt(i).Key;
                Path path = new Path();
                path.Visibility = base.Visibility;
                path.Style = pieValue.Style;
                if (path.Style == null)
                {
                    if (stroke == null)
                    {
                        stroke = ColorBrushHelper.GetColorByIndex(valueDic.Count);
                    }
                    path.Style = ChartStyleHelper.CreatePieSeriesStyle(stroke, ColorBrushHelper.GetColorByIndex(i));
                }

                var sliceControls = new List<FrameworkElement>() { path };
                base.AddLegendItem(new SeriesLegendItem(path.Fill.Clone(), pieValue.Title, this, sliceControls));

                value = valueDic.ElementAt(i).Value;
                if (value <= ChartConstant.ZERO_D)
                {
                    continue;
                }
                path.ToolTip = pieValue.TooltipText;

                angle = value * MathEx.ANGLE_360 / total;
                endAngle = endAngle + angle;
                radians = MathEx.AngleToRadians(endAngle);

                //+r是为了平移坐标
                x = Math.Cos(radians) * r + r + xOffset;
                y = Math.Sin(radians) * r + r + yOffset;
                point = new Point(x, y);

                List<PathSegment> pathSegments = new List<PathSegment>();
                pathSegments.Add(new LineSegment() { Point = lastPoint });
                pathSegments.Add(new ArcSegment() { Size = arcSegmentSize, Point = point, SweepDirection = SweepDirection.Clockwise });
                pathSegments.Add(new LineSegment() { Point = center });
                PathFigure pathFigure = new PathFigure(center, pathSegments, true);
                path.Data = new PathGeometry(new PathFigure[] { pathFigure });
                if (endAngle - beginAngle - 30d < ChartConstant.ZERO_D)
                {
                    beginAngleQuadrant = MathEx.GetQuadrantByAngle(MathEx.ANGLE_360 - beginAngle);
                    endAngleQuadrant = MathEx.GetQuadrantByAngle(MathEx.ANGLE_360 - endAngle);
                    Rect clipRect = this.GetClipRect(r, beginAngleQuadrant, endAngleQuadrant, x, y, center, lastPoint);
                    path.Clip = new RectangleGeometry(clipRect);
                }

                canvas.Children.Add(path);
                this._elementList.Add(path);


                //文本
                var tbLabel = new TextBlock { Text = pieValue.Label };
                tbLabel.HorizontalAlignment = HorizontalAlignment.Center;
                tbLabel.VerticalAlignment = VerticalAlignment.Center;
                tbLabel.Foreground = Brushes.White;
                canvas.Children.Add(tbLabel);

                var size = UITextHelper.MeasureTextSize(tbLabel);
                angle = endAngle - angle / 2;
                radians = MathEx.AngleToRadians(angle);
                txtX = Math.Cos(radians) * txtr + r;
                txtY = Math.Sin(radians) * txtr + r;
                tbLabelLeft = txtX - size.Width / 2 + xOffset;
                tbLabelTop = txtY - size.Height / 2 + yOffset;
                Canvas.SetLeft(tbLabel, tbLabelLeft);
                Canvas.SetTop(tbLabel, tbLabelTop);
                sliceControls.Add(tbLabel);

                beginAngle = endAngle;
                lastPoint = point;
            }
        }


        private Rect GetClipRect(double r, Quadrant beginAngleQuadrant, Quadrant endAngleQuadrant, double x, double y, Point center, Point lastPoint)
        {
            double clipX, clipY, clipWidth, clipHeight;
            switch (endAngleQuadrant)
            {
                case Quadrant.One:
                case Quadrant.PositiveXAxisAngle:
                    if (beginAngleQuadrant == Quadrant.Two)
                    {
                        clipX = lastPoint.X;
                        clipY = center.Y - r;
                        clipWidth = x - lastPoint.X;
                        clipHeight = r;
                    }
                    else
                    {
                        clipX = center.X;
                        clipY = lastPoint.Y;
                        clipWidth = x - center.X;
                        clipHeight = y - lastPoint.Y;
                    }
                    break;
                case Quadrant.Two:
                case Quadrant.PositiveYAxisAngle:
                    if (beginAngleQuadrant == Quadrant.Three)
                    {
                        clipX = center.X - r;
                        clipY = y;
                        clipWidth = r;
                        clipHeight = lastPoint.Y - y;
                    }
                    else
                    {
                        clipX = lastPoint.X;
                        clipY = y;
                        clipWidth = center.X - lastPoint.X;
                        clipHeight = center.Y - y;
                    }
                    break;
                case Quadrant.Three:
                case Quadrant.NegativeXAxisAngle:
                    if (beginAngleQuadrant == Quadrant.Four)
                    {
                        clipX = x;
                        clipY = center.Y;
                        clipWidth = lastPoint.X - x; ;
                        clipHeight = r;
                    }
                    else
                    {
                        clipX = x;
                        clipY = center.Y;
                        clipWidth = center.X - x;
                        clipHeight = lastPoint.Y - center.Y;
                    }
                    break;
                case Quadrant.Four:
                case Quadrant.NegativeYAxisAngle:
                    clipX = center.X;
                    clipY = center.Y;
                    clipWidth = lastPoint.X - center.X;
                    clipHeight = y - center.Y;
                    break;
                default:
                    throw new NotImplementedException(endAngleQuadrant.ToString());
            }

            return new Rect(clipX, clipY, clipWidth, clipHeight);
        }

        private Dictionary<IChartNoAxisValue, double> GetValueDic()
        {
            if (base._values == null || base._values.Count == 0)
            {
                return null;
            }

            IChartNoAxisValue pieValue;
            double itemValue;
            Dictionary<IChartNoAxisValue, double> valueDic = new Dictionary<IChartNoAxisValue, double>();
            foreach (var value in base._values)
            {
                pieValue = value as IChartNoAxisValue;
                if (pieValue == null)
                {
                    continue;
                }

                itemValue = ChartHelper.ConvertToDouble(pieValue.GetValue());
                if (!ChartHelper.DoubleHasValue(itemValue))
                {
                    itemValue = ChartConstant.ZERO_D;
                }

                valueDic[pieValue] = itemValue;
            }

            return valueDic;
        }

        private double GetRadius(Canvas canvas)
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
            }

            return radius;
        }



        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        protected override bool PrimitiveClear(Canvas canvas)
        {
            foreach (var element in this._elementList)
            {
                canvas.Children.Remove(element);
            }
            this._elementList.Clear();
            return false;
        }



        /// <summary>
        /// Series样式改变通知
        /// </summary>
        /// <param name="style"></param>
        protected override void StyleChanged(Style style)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Visibility改变通知
        /// </summary>
        /// <param name="oldVisibility">旧值</param>
        /// <param name="newVisibility">新值</param>
        protected override void VisibilityChanged(Visibility oldVisibility, Visibility newVisibility)
        {
            foreach (var element in this._elementList)
            {
                element.Visibility = newVisibility;
            }
        }
    }
}
