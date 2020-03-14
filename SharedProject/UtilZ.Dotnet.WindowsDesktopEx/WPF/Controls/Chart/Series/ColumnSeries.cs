using System;
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
    /// 条形图
    /// </summary>
    public class ColumnSeries : SeriesAbs, IColumnSeries
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
        /// 获取ColumnSeries样式
        /// </summary>
        /// <returns>ColumnSeries样式</returns>
        public Style GetStyle()
        {
            return base.Style;
        }


        private readonly List<Rectangle> _columnElementList = new List<Rectangle>();



        /// <summary>
        /// 构造函数
        /// </summary>
        public ColumnSeries()
            : base()
        {

        }

        /// <summary>
        /// 重写StyleChanged
        /// </summary>
        /// <param name="style">新样式</param>
        protected override void StyleChanged(Style style)
        {
            base.RemoveLegendItem();
            Brush legendBrush = ChartHelper.CreateColumn(this).Fill.Clone();
            base.AddLegendItem(new SeriesLegendItem(legendBrush, base.Title, this));

            foreach (var columnElement in this._columnElementList)
            {
                columnElement.Style = style;
            }
        }



        /// <summary>
        /// 重写PrimitiveAdd
        /// </summary>
        /// <param name="canvas"></param>
        protected override void PrimitiveDraw(Canvas canvas)
        {
            this._columnElementList.Clear();
            Brush legendBrush = ChartHelper.CreateColumn(this).Fill.Clone();
            base.AddLegendItem(new SeriesLegendItem(legendBrush, base.Title, this));

            if (base._values == null || base._values.Count == 0)
            {
                return;
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
            double x, y, size;
            Rectangle columnElement;

            foreach (var value in base._values)
            {
                x = this.AxisX.GetX(value);
                if (!ChartHelper.DoubleHasValue(x))
                {
                    continue;
                }

                y = this.AxisY.GetY(value);
                if (!ChartHelper.DoubleHasValue(y))
                {
                    continue;
                }

                columnElement = ChartHelper.CreateColumn(this);
                columnElement.Visibility = base.Visibility;
                if (value != null)
                {
                    ChartHelper.SetColumnTooltipText(this, value.TooltipText, columnElement);
                }

                if (ChartHelper.DoubleHasValue(this._size))
                {
                    columnElement.Height = this._size;
                }

                canvas.Children.Add(columnElement);
                this._columnElementList.Add(columnElement);

                if (this.AxisY.IsAxisYLeft())
                {
                    size = x;
                    Canvas.SetLeft(columnElement, ChartConstant.ZERO_D);
                }
                else
                {
                    size = canvas.Width - x;
                    Canvas.SetRight(columnElement, ChartConstant.ZERO_D);
                }

                if (size < ChartConstant.ZERO_D)
                {
                    size = ChartConstant.ZERO_D;
                }
                columnElement.Width = size;

                Canvas.SetTop(columnElement, y);
            }
        }





        private void PrimitiveDrawVertical(Canvas canvas)
        {
            double x, y, size;
            Rectangle columnElement;

            foreach (var value in base._values)
            {
                x = this.AxisX.GetX(value);
                if (!ChartHelper.DoubleHasValue(x))
                {
                    continue;
                }

                y = this.AxisY.GetY(value);
                if (!ChartHelper.DoubleHasValue(y))
                {
                    continue;
                }

                columnElement = ChartHelper.CreateColumn(this);
                columnElement.Visibility = base.Visibility;
                if (value != null)
                {
                    ChartHelper.SetColumnTooltipText(this, value.TooltipText, columnElement);
                }

                if (ChartHelper.DoubleHasValue(this._size))
                {
                    columnElement.Width = this._size;
                }

                canvas.Children.Add(columnElement);
                this._columnElementList.Add(columnElement);

                if (this.AxisX.IsAxisXBottom())
                {
                    size = canvas.Height - y;
                    Canvas.SetBottom(columnElement, ChartConstant.ZERO_D);
                }
                else
                {
                    size = y;
                    Canvas.SetTop(columnElement, ChartConstant.ZERO_D);
                }

                if (size < ChartConstant.ZERO_D)
                {
                    size = ChartConstant.ZERO_D;
                }
                columnElement.Height = size;
                Canvas.SetLeft(columnElement, x);
            }
        }




        /// <summary>
        /// 重写PrimitiveRemove
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
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
        /// 重写VisibilityChanged
        /// </summary>
        /// <param name="oldVisibility"></param>
        /// <param name="newVisibility"></param>
        protected override void VisibilityChanged(Visibility oldVisibility, Visibility newVisibility)
        {
            foreach (var columnElement in this._columnElementList)
            {
                columnElement.Visibility = newVisibility;
            }
        }
    }
}
