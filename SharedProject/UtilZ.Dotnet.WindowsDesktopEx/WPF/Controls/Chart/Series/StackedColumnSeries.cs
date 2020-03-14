using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 堆叠条形图
    /// </summary>
    public class StackedColumnSeries : SeriesAbs, IColumnSeries
    {
        private SeriesOrientation _orientation = SeriesOrientation.Vertical;
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

        private double _size = double.NaN;
        /// <summary>
        /// 获取或设置ColumnSeries水平方向高度,垂直方向宽度,为double.NaN则自动计算,默认为double.NaN
        /// </summary>
        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }


        /// <summary>
        /// 获取或设置Series样式
        /// </summary>
        public override Style Style
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException($"请通过{nameof(TitleStyleDic)}属性设置堆叠标题名称以及样式");
        }
        /// <summary>
        /// 获取或设置Series标题
        /// </summary>
        public override string Title
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException($"请通过{nameof(TitleStyleDic)}属性设置堆叠标题名称以及样式");
        }

        private Dictionary<string, Style> _titleStyleDic = null;
        /// <summary>
        /// 堆叠标题及样式[key:标题;value:样式(为null自动创建)]
        /// </summary>
        public Dictionary<string, Style> TitleStyleDic
        {
            get { return _titleStyleDic; }
            set
            {
                _titleStyleDic = value;
                this.StyleChanged2();
            }
        }


        /// <summary>
        /// 获取样式
        /// </summary>
        /// <returns>样式</returns>
        public Style GetStyle()
        {
            return this.GetStyle(0);
        }

        private Style GetStyle(int index)
        {
            Style style;
            if (this._titleStyleDic == null ||
                index < 0 ||
                index >= this._titleStyleDic.Count)
            {
                style = ChartStyleHelper.CreateColumnSeriesStyle(ColorBrushHelper.GetColorByIndex(index));
            }
            else
            {
                style = this._titleStyleDic.Values.ElementAt(index);
            }
            return style;
        }


        private readonly List<Rectangle> _columnElementList = new List<Rectangle>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public StackedColumnSeries()
           : base()
        {

        }


        /// <summary>
        /// Series样式改变通知
        /// </summary>
        /// <param name="style">新样式</param>
        protected override void StyleChanged(Style style)
        {
            //不需要实现
        }

        private void StyleChanged2()
        {
            if (this._columnElementList.Count > 0)
            {
                throw new NotSupportedException();
            }
            else
            {
                base.RemoveLegendItem();
            }

            //var legendTemplateColumn = new Rectangle();
            //Brush legendBrush;
            //for (int i = 0; i < this._columnElementList.Count; i++)
            //{
            //    this._columnElementList[i].Style = this.GetStyle(i);
            //    legendBrush = this._columnElementList[i].Fill;
            //    base.AddLegendItem(new SeriesLegendItem(legendBrush, this._titleStyleDic.Keys.ElementAt(i), this));
            //}

            //titleStyleDic.Count -
            //if ()
        }



        /// <summary>
        /// 将Series添加到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        protected override void PrimitiveDraw(Canvas canvas)
        {
            this._columnElementList.Clear();
            if (this._titleStyleDic == null || this._titleStyleDic.Count == 0)
            {
                return;
            }

            //添加到SeriesLegendItem集合中
            var legendTemplateColumn = new Rectangle();
            Brush legendBrush;
            for (int i = 0; i < this._titleStyleDic.Count; i++)
            {
                legendTemplateColumn.Style = this.GetStyle(i);
                legendBrush = legendTemplateColumn.Fill.Clone();
                base.AddLegendItem(new SeriesLegendItem(legendBrush, this._titleStyleDic.Keys.ElementAt(i), this));
            }

            switch (this._orientation)
            {
                case SeriesOrientation.Horizontal:
                    this.PrimitiveDrawHorizontal(canvas);
                    break;
                case SeriesOrientation.Vertical:
                    this.PrimitiveDrawVertical(canvas);
                    break;
                default:
                    throw new NotImplementedException(this._orientation.ToString());
            }
        }

        private void PrimitiveDrawHorizontal(Canvas canvas)
        {
            double x, y, leftOrRight, size;
            object obj;
            IChartAxisValue chartAxisValue;
            IEnumerable enumerable;
            IChartChildValue chartChildValue;
            Rectangle columnElement;
            int stytleIndex;

            foreach (var value in base._values)
            {
                chartAxisValue = value as IChartAxisValue;
                if (chartAxisValue == null)
                {
                    continue;
                }

                y = this.AxisY.GetY(value);
                if (!ChartHelper.DoubleHasValue(y))
                {
                    continue;
                }

                obj = chartAxisValue.GetXValue();
                if (obj == null || !(obj is IEnumerable))
                {
                    continue;
                }

                enumerable = (IEnumerable)obj;
                stytleIndex = -1;
                leftOrRight = ChartConstant.ZERO_D;

                foreach (var item in enumerable)
                {
                    stytleIndex++;
                    if (item == null || !(item is IChartChildValue))
                    {
                        continue;
                    }

                    chartChildValue = (IChartChildValue)item;
                    x = this.AxisX.GetX(chartChildValue);
                    if (!ChartHelper.DoubleHasValue(x))
                    {
                        continue;
                    }

                    columnElement = ChartHelper.CreateColumn(this);
                    columnElement.Style = this.GetStyle(stytleIndex);
                    columnElement.Visibility = base.Visibility;
                    ChartHelper.SetColumnTooltipText(this, chartChildValue.TooltipText, columnElement);


                    if (ChartHelper.DoubleHasValue(this._size))
                    {
                        columnElement.Height = this._size;
                    }

                    canvas.Children.Add(columnElement);
                    this._columnElementList.Add(columnElement);

                    if (this.AxisY.IsAxisYLeft())
                    {
                        size = x;
                        Canvas.SetLeft(columnElement, leftOrRight);
                    }
                    else
                    {
                        size = canvas.Width - x;
                        Canvas.SetRight(columnElement, leftOrRight);
                    }

                    if (size < ChartConstant.ZERO_D)
                    {
                        size = ChartConstant.ZERO_D;
                    }
                    columnElement.Width = size;
                    Canvas.SetTop(columnElement, y);
                    leftOrRight += columnElement.Width;
                }
            }
        }

        private void PrimitiveDrawVertical(Canvas canvas)
        {
            double x, y, topOrBottom, size;
            IChartAxisValue chartAxisValue;
            object obj;
            IEnumerable enumerable;
            IChartChildValue chartChildValue;
            Rectangle columnElement;
            int stytleIndex;

            foreach (var value in base._values)
            {
                chartAxisValue = value as IChartAxisValue;
                if (chartAxisValue == null)
                {
                    continue;
                }

                x = this.AxisX.GetX(chartAxisValue);
                if (!ChartHelper.DoubleHasValue(x))
                {
                    continue;
                }


                obj = chartAxisValue.GetYValue();
                if (obj == null || !(obj is IEnumerable))
                {
                    continue;
                }

                enumerable = (IEnumerable)obj;
                stytleIndex = -1;
                topOrBottom = ChartConstant.ZERO_D;

                foreach (var item in enumerable)
                {
                    stytleIndex++;
                    if (item == null || !(item is IChartChildValue))
                    {
                        continue;
                    }

                    chartChildValue = (IChartChildValue)item;
                    y = this.AxisY.GetY(chartChildValue);
                    if (!ChartHelper.DoubleHasValue(y))
                    {
                        continue;
                    }

                    columnElement = ChartHelper.CreateColumn(this);
                    columnElement.Style = this.GetStyle(stytleIndex);
                    columnElement.Visibility = base.Visibility;
                    ChartHelper.SetColumnTooltipText(this, chartChildValue.TooltipText, columnElement);

                    if (ChartHelper.DoubleHasValue(this._size))
                    {
                        columnElement.Width = this._size;
                    }

                    canvas.Children.Add(columnElement);
                    this._columnElementList.Add(columnElement);

                    if (this.AxisX.IsAxisXBottom())
                    {
                        size = canvas.Height - y;
                        Canvas.SetBottom(columnElement, topOrBottom);
                    }
                    else
                    {
                        size = y;
                        Canvas.SetTop(columnElement, topOrBottom);
                    }

                    if (size < ChartConstant.ZERO_D)
                    {
                        size = ChartConstant.ZERO_D;
                    }
                    columnElement.Height = size;
                    Canvas.SetLeft(columnElement, x);
                    topOrBottom += columnElement.Height;
                }
            }
        }





        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        protected override bool PrimitiveClear(Canvas canvas)
        {
            //条形力移除后需要完整的重新绘制
            foreach (var columnElement in this._columnElementList)
            {
                base._canvas.Children.Remove(columnElement);
            }

            this._columnElementList.Clear();
            return true;
        }



        /// <summary>
        /// Visibility改变通知
        /// </summary>
        /// <param name="oldVisibility">旧值</param>
        /// <param name="newVisibility">新值</param>
        protected override void VisibilityChanged(Visibility oldVisibility, Visibility newVisibility)
        {
            foreach (var columnElement in this._columnElementList)
            {
                columnElement.Visibility = newVisibility;
            }
        }
    }
}
