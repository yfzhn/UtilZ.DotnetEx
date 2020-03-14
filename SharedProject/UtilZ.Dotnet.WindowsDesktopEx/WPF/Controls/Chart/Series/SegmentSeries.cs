using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 线段图
    /// </summary>
    public class SegmentSeries : SeriesAbs
    {
        private SeriesOrientation _orientation = SeriesOrientation.Horizontal;
        /// <summary>
        /// 获取或设置ColumnSeries方向
        /// </summary>
        public SeriesOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation == value)
                {
                    return;
                }

                _orientation = value;
                base.OnRaisePropertyChanged(nameof(Orientation));
            }
        }





        private readonly List<Line> _segmentList = new List<Line>();
        private Thickness _margin = new Thickness(ChartConstant.ZERO_D);
        /// <summary>
        /// 构造函数
        /// </summary>
        public SegmentSeries()
            : base()
        {

        }




        /// <summary>
        /// Series样式改变通知
        /// </summary>
        /// <param name="style">新样式</param>
        protected override void StyleChanged(Style style)
        {
            base.RemoveLegendItem();
            this.AddLegendItem();
            foreach (var segment in this._segmentList)
            {
                segment.Style = style;
            }
        }

        private void AddLegendItem()
        {
            Line line = new Line();
            line.Style = this.GetStyle();
            base.AddLegendItem(new SeriesLegendItem(line.Stroke.Clone(), base.Title, this));
        }


        /// <summary>
        /// 将Series绘制到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        protected override void PrimitiveDraw(Canvas canvas)
        {
            this._segmentList.Clear();
            this.AddLegendItem();

            if (base._values == null || base._values.Count == 0)
            {
                return;
            }

            IChartAxisDoubleValue doubleValue;
            double x1, x2, y1, y2;
            foreach (var value in base._values)
            {
                doubleValue = value as IChartAxisDoubleValue;
                if (doubleValue == null)
                {
                    continue;
                }

                switch (this._orientation)
                {
                    case SeriesOrientation.Horizontal:
                        object obj = doubleValue.GetYValue1().GetValue();
                        y1 = this.AxisY.GetY(doubleValue.GetYValue1());
                        if (ChartHelper.DoubleHasValue(y1))
                        {
                            //第一个有值
                            y2 = y1;
                        }
                        else
                        {
                            y2 = this.AxisY.GetY(doubleValue.GetYValue2());
                            if (ChartHelper.DoubleHasValue(y2))
                            {
                                //第一个没有值,第二个有值
                                y1 = y2;
                            }
                            else
                            {
                                //两个都没有值
                                continue;
                            }
                        }

                        x1 = this.AxisX.GetX(doubleValue.GetXValue1());
                        if (!ChartHelper.DoubleHasValue(x1))
                        {
                            continue;
                        }

                        x2 = this.AxisX.GetX(doubleValue.GetXValue2());
                        if (!ChartHelper.DoubleHasValue(x2))
                        {
                            continue;
                        }
                        break;
                    case SeriesOrientation.Vertical:
                        x1 = this.AxisX.GetX(doubleValue.GetXValue1());
                        if (ChartHelper.DoubleHasValue(x1))
                        {
                            //第一个有值
                            x2 = x1;
                        }
                        else
                        {
                            x2 = this.AxisX.GetX(doubleValue.GetXValue2());
                            if (ChartHelper.DoubleHasValue(x2))
                            {
                                //第一个没有值,第二个有值
                                x1 = x2;
                            }
                            else
                            {
                                //两个都没有值
                                continue;
                            }
                        }

                        y1 = this.AxisY.GetY(doubleValue.GetYValue1());
                        if (!ChartHelper.DoubleHasValue(y1))
                        {
                            continue;
                        }

                        y2 = this.AxisY.GetY(doubleValue.GetYValue2());
                        if (!ChartHelper.DoubleHasValue(y2))
                        {
                            continue;
                        }
                        break;
                    default:
                        throw new NotImplementedException(this._orientation.ToString());
                }

                Line line = new Line();
                line.Style = this.GetStyle();
                line.X1 = x1;
                line.X2 = x2;
                line.Y1 = y1;
                line.Y2 = y2;
                line.Visibility = base.Visibility;
                if (this.EnableTooltip)
                {
                    line.ToolTip = doubleValue.TooltipText;
                }
                line.Tag = doubleValue;
                canvas.Children.Add(line);
                this._segmentList.Add(line);
            }
        }

        private Style GetStyle()
        {
            var style = this.Style;
            if (style == null)
            {
                style = ChartStyleHelper.GetSegmentSeriesDefaultStyle();
            }

            return style;
        }




        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        protected override bool PrimitiveClear(Canvas canvas)
        {
            foreach (var segment in this._segmentList)
            {
                canvas.Children.Remove(segment);
            }

            this._segmentList.Clear();
            return false;
        }



        /// <summary>
        /// EnableTooltip改变通知
        /// </summary>
        /// <param name="enableTooltip">新EnableTooltip值</param>
        protected override void EnableTooltipChanged(bool enableTooltip)
        {
            foreach (var segment in this._segmentList)
            {
                segment.ToolTip = ((IChartAxisDoubleValue)segment.Tag).TooltipText;
            }
        }



        /// <summary>
        /// Visibility改变通知
        /// </summary>
        /// <param name="oldVisibility">旧值</param>
        /// <param name="newVisibility">新值</param>
        protected override void VisibilityChanged(Visibility oldVisibility, Visibility newVisibility)
        {
            foreach (var segment in this._segmentList)
            {
                segment.Visibility = newVisibility;
            }
        }
    }
}
