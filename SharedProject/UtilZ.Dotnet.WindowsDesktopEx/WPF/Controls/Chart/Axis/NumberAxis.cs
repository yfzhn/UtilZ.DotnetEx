using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 数值坐标轴
    /// </summary>
    public class NumberAxis : AxisAbs
    {
        private double _labelStep = double.NaN;
        /// <summary>
        /// 坐标轴刻度值间隔,为double.NaN时自动计算
        /// </summary>
        public double LabelStep
        {
            get { return _labelStep; }
            set
            {
                _labelStep = value;
                base.OnRaisePropertyChanged(nameof(LabelStep));
            }
        }

        private double _minValue = double.NaN;
        /// <summary>
        /// 坐标轴刻度最小值,为double.NaN时自动计算
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
            }
        }

        private double _maxValue = double.NaN;
        /// <summary>
        /// 坐标轴刻度最大值,为double.NaN时自动计算
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
            }
        }


        private bool _showLastLabel = true;
        /// <summary>
        /// 是否显示刻度标记[true:显示;false:不显示]
        /// </summary>
        public bool ShowLastLabel
        {
            get { return _showLastLabel; }
            set
            {
                _showLastLabel = value;
                base.OnRaisePropertyChanged(nameof(ShowLastLabel));
            }
        }



        /// <summary>
        /// 获取或设置自定义LabelText
        /// </summary>
        public Func<double, string> CustomAxisTextFormatCunc;
        private NumberAxisData _axisData = null;



        /// <summary>
        /// 构造函数
        /// </summary>
        public NumberAxis()
            : base()
        {
            base.DockOrientation = ChartDockOrientation.Left;
        }





        /// <summary>
        /// 获取X坐标轴高度
        /// </summary>
        /// <returns>X坐标轴高度</returns>
        protected override double PrimitiveGetXAxisHeight()
        {
            string labelText = this.CreateAxisText(123d);
            double axisHeight = ChartHelper.MeasureLabelTextSize(this, labelText).Height;
            return base.CalculateAxisSize(axisHeight);
        }




        private string CreateAxisText(double value)
        {
            string axisText;
            var customAxisTextFormatCunc = this.CustomAxisTextFormatCunc;
            if (customAxisTextFormatCunc != null)
            {
                axisText = customAxisTextFormatCunc(value);
            }
            else
            {
                axisText = value.ToString();
            }

            return axisText;
        }

        private NumberAxisData CreateAxisData(ChartCollection<ISeries> seriesCollection)
        {
            if (!ChartHelper.DoubleHasValue(this._minValue) || !ChartHelper.DoubleHasValue(this._maxValue))
            {
                NumberAxisValueArea result = this.GetMinAndMaxValue(seriesCollection);
                if (ChartHelper.DoubleHasValue(this._minValue))
                {
                    result.Min = this._minValue;
                }

                if (ChartHelper.DoubleHasValue(this._maxValue))
                {
                    result.Max = this._maxValue;
                }

                this.ZoomAxisValueArea(result);
                this.CompleteAxisAreaValue(result);
                return this.CreateNumberAxisData(result.Min, result.Max);
            }
            else
            {
                return this.CreateNumberAxisData(this._minValue, this._maxValue);
            }
        }


        private NumberAxisData CreateNumberAxisData(double min, double max)
        {
            if (!ChartHelper.DoubleHasValue(min) ||
                !ChartHelper.DoubleHasValue(max) ||
                max - min <= base._PRE)
            {
                return null;
            }

            return new NumberAxisData(min, max);
        }

        private void ZoomAxisValueArea(NumberAxisValueArea result)
        {
            long minMuilt, maxMuilt;
            if (ChartHelper.DoubleHasValue(this._minValue))
            {
                if (ChartHelper.DoubleHasValue(this._maxValue))
                {
                    //不调整
                }
                else
                {
                    maxMuilt = ChartHelper.CalDoubleToIntegerMuilt(result.Max);
                    result.Max = ChartHelper.DoubleToCeilingInteger(result.Max, maxMuilt);
                }
            }
            else
            {
                minMuilt = ChartHelper.CalDoubleToIntegerMuilt(result.Min);
                if (ChartHelper.DoubleHasValue(this._maxValue))
                {
                    result.Min = ChartHelper.DoubleToFloorInteger(result.Min, minMuilt);
                }
                else
                {
                    maxMuilt = ChartHelper.CalDoubleToIntegerMuilt(result.Max);
                    long muilt = minMuilt > maxMuilt ? minMuilt : maxMuilt;
                    result.Min = ChartHelper.DoubleToFloorInteger(result.Min, muilt);
                    result.Max = ChartHelper.DoubleToCeilingInteger(result.Max, muilt);
                }
            }
        }

        private NumberAxisValueArea CompleteAxisAreaValue(NumberAxisValueArea result)
        {
            if (ChartHelper.DoubleHasValue(result.Max))
            {
                if (!ChartHelper.DoubleHasValue(result.Min) && ChartHelper.DoubleHasValue(this._labelStep))
                {
                    result.Min = result.Max - this._labelStep;
                }
            }
            else
            {
                if (ChartHelper.DoubleHasValue(result.Min) && ChartHelper.DoubleHasValue(this._labelStep))
                {
                    result.Max = result.Min + this._labelStep;
                }
            }

            return result;
        }






        #region GetMinAndMaxValue
        private NumberAxisValueArea GetMinAndMaxValue(ChartCollection<ISeries> seriesCollection)
        {
            double min = double.NaN, max = double.NaN;
            if (seriesCollection == null || seriesCollection.Count == ChartConstant.ZERO_I)
            {
                return new NumberAxisValueArea(min, max);
            }

            double tmpMin, tmpMax;
            foreach (var series in seriesCollection)
            {
                if (series.AxisX != this && series.AxisY != this)
                {
                    continue;
                }

                this.PrimitiveGetMinAndMaxValue(this, series.Values, out tmpMin, out tmpMax);
                if (double.IsNaN(min) || tmpMin - min < base._PRE)
                {
                    min = tmpMin;
                }

                if (double.IsNaN(max) || tmpMax - max > base._PRE)
                {
                    max = tmpMax;
                }
            }

            return new NumberAxisValueArea(min, max);
        }

        private void PrimitiveGetMinAndMaxValue(AxisAbs axis, ValueCollection values, out double min, out double max)
        {
            min = double.NaN;
            max = double.NaN;

            if (values == null || values.Count == 0)
            {
                return;
            }

            double pre = double.IsNaN(axis.PRE) ? ChartConstant.ZERO_D : axis.PRE;
            IChartAxisValue chartAxisValue;
            IChartAxisDoubleValue doubleValue;

            foreach (var value in values)
            {
                chartAxisValue = value as IChartAxisValue;
                if (chartAxisValue == null)
                {
                    doubleValue = value as IChartAxisDoubleValue;
                    if (doubleValue != null)
                    {
                        this.GetIChartAxisDoubleValueMinAndMax(axis, doubleValue, ref pre, ref min, ref max);
                    }
                }
                else
                {
                    this.GetIChartAxisValueMinAndMax(axis, chartAxisValue, ref pre, ref min, ref max);
                }
            }
        }

        private void GetIChartAxisDoubleValueMinAndMax(AxisAbs axis, IChartAxisDoubleValue doubleValue, ref double pre, ref double min, ref double max)
        {
            switch (axis.AxisType)
            {
                case AxisType.X:
                    this.PrimitiveGetIChartAxisDoubleValueMinAndMax(doubleValue.GetXValue1(), ref pre, ref min, ref max);
                    this.PrimitiveGetIChartAxisDoubleValueMinAndMax(doubleValue.GetXValue2(), ref pre, ref min, ref max);
                    break;
                case AxisType.Y:
                    this.PrimitiveGetIChartAxisDoubleValueMinAndMax(doubleValue.GetYValue1(), ref pre, ref min, ref max);
                    this.PrimitiveGetIChartAxisDoubleValueMinAndMax(doubleValue.GetYValue2(), ref pre, ref min, ref max);
                    break;
                default:
                    throw new NotImplementedException(axis.AxisType.ToString());
            }
        }

        private void PrimitiveGetIChartAxisDoubleValueMinAndMax(IChartChildValue chartChildValue, ref double pre, ref double min, ref double max)
        {
            if (chartChildValue == null)
            {
                return;
            }

            object obj = chartChildValue.GetValue();
            this.GetMinAndMax(obj, ref pre, ref min, ref max);
        }

        private void GetIChartAxisValueMinAndMax(AxisAbs axis, IChartAxisValue chartAxisValue, ref double pre, ref double min, ref double max)
        {
            object obj;
            switch (axis.AxisType)
            {
                case AxisType.X:
                    obj = chartAxisValue.GetXValue();
                    break;
                case AxisType.Y:
                    obj = chartAxisValue.GetYValue();
                    break;
                default:
                    throw new NotImplementedException(axis.AxisType.ToString());
            }

            if (obj == null)
            {
                return;
            }

            if (obj is IEnumerable)
            {
                this.GetCollectionMinAndMax(obj, ref pre, ref min, ref max);
            }
            else
            {
                this.GetMinAndMax(obj, ref pre, ref min, ref max);
            }
        }

        private void GetCollectionMinAndMax(object obj, ref double pre, ref double min, ref double max)
        {
            double tmp, tmpTotalChild;
            IEnumerable enumerable = (IEnumerable)obj;
            tmpTotalChild = double.NaN;
            foreach (var item in enumerable)
            {
                if (item == null || !(item is IChartChildValue))
                {
                    continue;
                }

                tmp = ChartHelper.ConvertToDouble(((IChartChildValue)item).GetValue());
                if (!ChartHelper.DoubleHasValue(tmp))
                {
                    continue;
                }

                if (!ChartHelper.DoubleHasValue(min) || tmp - min < pre)
                {
                    min = tmp;
                }

                if (ChartHelper.DoubleHasValue(tmpTotalChild))
                {
                    tmpTotalChild += tmp;
                }
                else
                {
                    tmpTotalChild = tmp;
                }
            }

            if (!ChartHelper.DoubleHasValue(tmpTotalChild))
            {
                return;
            }

            if (!ChartHelper.DoubleHasValue(max) || tmpTotalChild - max > pre)
            {
                max = tmpTotalChild;
            }
        }

        private void GetMinAndMax(object obj, ref double pre, ref double min, ref double max)
        {
            var tmp = ChartHelper.ConvertToDouble(obj);
            if (!ChartHelper.DoubleHasValue(tmp))
            {
                return;
            }

            if (!ChartHelper.DoubleHasValue(min) || tmp - min < pre)
            {
                min = tmp;
            }

            if (!ChartHelper.DoubleHasValue(max) || tmp - max > pre)
            {
                max = tmp;
            }
        }
        #endregion




        private double CalculateLabelStep(double valueArea, double axisSize)
        {
            double labelStep = this._labelStep;

            if (double.IsNaN(labelStep))
            {
                int labelCount = (int)(axisSize / ChartConstant.DEFAULT_STEP_SIZE);
                if (axisSize % ChartConstant.DEFAULT_STEP_SIZE > ChartConstant.ZERO_D)
                {
                    labelCount += 1;
                }

                labelStep = valueArea / labelCount;
                if (valueArea % labelCount > base._PRE)
                {
                    //不能整除,则调整
                    var muilt = ChartHelper.CalDoubleToIntegerMuilt(labelStep);
                    var step2 = ChartHelper.DoubleToCeilingInteger(labelStep, muilt);
                    while (step2 >= valueArea && muilt >= 1)
                    {
                        muilt = muilt / 10;
                        step2 = ChartHelper.DoubleToCeilingInteger(labelStep, muilt);
                    }

                    if (!double.IsNaN(step2))
                    {
                        labelStep = step2;
                    }

                    labelStep = (double)((long)(labelStep * 100)) / 100;
                }
            }

            return labelStep;
        }





        /// <summary>
        /// 子类重写此函数时,必须设置Y轴宽度
        /// </summary>
        /// <param name="axisCanvas">画布</param>
        /// <param name="seriesCollection">Series集合</param>
        /// <returns>Label的Y列表</returns>
        protected override List<double> PrimitiveDrawY(Canvas axisCanvas, ChartCollection<ISeries> seriesCollection)
        {
            this._axisData = this.CreateAxisData(seriesCollection);
            return this.DrawY(axisCanvas);
        }

        private List<double> DrawY(Canvas axisCanvas)
        {
            if (this._axisData == null)
            {
                axisCanvas.Width = ChartConstant.AXIS_DEFAULT_SIZE;
                return null;
            }

            List<double> yList;
            switch (base.Orientation)
            {
                case AxisLabelOrientation.BottomToTop:
                    yList = this.DrawYAxisBottomToTop(axisCanvas, this._axisData);
                    break;
                case AxisLabelOrientation.TopToBottom:
                    yList = this.DrawYAxisTopToBottom(axisCanvas, this._axisData);
                    break;
                default:
                    throw new ArgumentException($"未知的{base.Orientation.ToString()}");
            }
            ChartHelper.DrawYAxisLabelLine(this, axisCanvas, yList);
            return yList;
        }

        private List<double> DrawYAxisTopToBottom(Canvas axisCanvas, NumberAxisData axisData)
        {
            List<double> yList = new List<double>();
            double axisHeight = axisCanvas.Height;
            double labelStep = this.CalculateLabelStep(axisData.Area, axisHeight);
            double labelStepSize = ChartHelper.CalculateLabelStepSize(axisData.Area, axisHeight, labelStep);
            double labelTextLineInterval = base.GetAxisYLabelTextLineInterval();
            double top = ChartConstant.ZERO_D, top2;
            double value = axisData.MinValue;
            double y = ChartConstant.ZERO_D;
            TextBlock label;
            Size labelSize;
            double labelWidth = ChartConstant.ZERO_D;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            bool addLabelControl;
            double lastLabelY = axisHeight;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(value));
                labelSize = UITextHelper.MeasureTextSize(label);
                if (labelSize.Width - labelWidth > base._PRE)
                {
                    labelWidth = labelSize.Width;
                }

                if (axisHeight > labelSize.Height)
                {
                    addLabelControl = false;
                    switch (labelTextLocation)
                    {
                        case AxisLabelLocation.First:
                            axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetTop(label, top);
                            lastLabelY = top + labelSize.Height;
                            labelTextLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            top2 = top - labelSize.Height / 2;
                            if (top2 <= axisHeight)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;
                                Canvas.SetTop(label, top2);
                                lastLabelY = top2 + labelSize.Height;
                            }
                            break;
                        case AxisLabelLocation.Last:
                            if (lastLabelY + labelSize.Height < axisHeight)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;
                                Canvas.SetBottom(label, ChartConstant.ZERO_D);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (addLabelControl)
                    {
                        if (base.IsAxisYLeft())
                        {
                            Canvas.SetRight(label, labelTextLineInterval);
                        }
                        else
                        {
                            Canvas.SetLeft(label, labelTextLineInterval);
                        }
                    }
                }

                if (labelTextLocation == AxisLabelLocation.Last)
                {
                    if (this._showLastLabel)
                    {
                        yList.Add(y);
                    }
                    break;
                }
                yList.Add(y);

                value += labelStep;
                if (value >= axisData.MaxValue)
                //if (tmp - maxValue > _PRE)
                {
                    labelStepSize = (labelStep - (value - axisData.MaxValue)) * labelStepSize / labelStep;
                    double labelHeight = ChartHelper.MeasureLabelTextSize(this, axisData.MaxValue.ToString()).Height + 10d;
                    if (labelStepSize < labelHeight)
                    {
                        break;
                    }

                    value = axisData.MaxValue;
                    y = axisHeight;
                    labelTextLocation = AxisLabelLocation.Last;
                }
                else
                {
                    y += labelStepSize;
                }

                top += labelStepSize;
            }

            axisCanvas.Width = base.CalculateAxisSize(labelWidth);
            return yList;
        }

        private List<double> DrawYAxisBottomToTop(Canvas axisCanvas, NumberAxisData axisData)
        {
            List<double> yList = new List<double>();
            double axisHeight = axisCanvas.Height;
            double labelStep = this.CalculateLabelStep(axisData.Area, axisHeight);
            double labelStepSize = ChartHelper.CalculateLabelStepSize(axisData.Area, axisHeight, labelStep);
            double labelTextLineInterval = base.GetAxisYLabelTextLineInterval();
            double bottom = ChartConstant.ZERO_D, bottom2;
            double value = axisData.MinValue;
            double y = axisHeight;
            TextBlock label;
            Size labelSize;
            double labelWidth = ChartConstant.ZERO_D;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            bool addLabelControl;
            double lastLabelY = axisHeight;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(value));
                labelSize = UITextHelper.MeasureTextSize(label);
                if (labelSize.Width - labelWidth > base._PRE)
                {
                    labelWidth = labelSize.Width;
                }

                if (axisHeight > labelSize.Height)
                {
                    addLabelControl = false;
                    switch (labelTextLocation)
                    {
                        case AxisLabelLocation.First:
                            axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetBottom(label, bottom);
                            lastLabelY = axisHeight - labelSize.Height;
                            labelTextLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            bottom2 = bottom - labelSize.Height / 2;
                            if (bottom2 > ChartConstant.ZERO_D)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;
                                Canvas.SetBottom(label, bottom2);
                                lastLabelY = bottom2 - labelSize.Height;
                            }
                            break;
                        case AxisLabelLocation.Last:
                            if (lastLabelY - labelSize.Height > ChartConstant.ZERO_D)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;
                                Canvas.SetTop(label, ChartConstant.ZERO_D);
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (addLabelControl)
                    {
                        if (base.IsAxisYLeft())
                        {
                            Canvas.SetRight(label, labelTextLineInterval);
                        }
                        else
                        {
                            Canvas.SetLeft(label, labelTextLineInterval);
                        }
                    }
                }

                if (labelTextLocation == AxisLabelLocation.Last)
                {
                    if (this._showLastLabel)
                    {
                        yList.Add(y);
                    }
                    break;
                }
                yList.Add(y);

                value += labelStep;
                if (value >= axisData.MaxValue)
                {
                    labelStepSize = (labelStep - (value - axisData.MaxValue)) * labelStepSize / labelStep;
                    double labelHeight = ChartHelper.MeasureLabelTextSize(this, axisData.MaxValue.ToString()).Height + 10d;
                    if (labelStepSize < labelHeight)
                    {
                        break;
                    }

                    value = axisData.MaxValue;
                    y = ChartConstant.ZERO_D;
                    labelTextLocation = AxisLabelLocation.Last;
                }
                else
                {
                    y -= labelStepSize;
                }

                bottom += labelStepSize;
            }

            axisCanvas.Width = base.CalculateAxisSize(labelWidth);
            return yList;
        }




        /// <summary>
        /// 绘制X轴
        /// </summary>
        /// <param name="axisCanvas">画布</param>
        /// <param name="seriesCollection">Series集合</param>
        /// <returns>Label的X列表</returns>
        protected override List<double> PrimitiveDrawX(Canvas axisCanvas, ChartCollection<ISeries> seriesCollection)
        {
            this._axisData = this.CreateAxisData(seriesCollection);
            return this.DrawX(axisCanvas);
        }

        private List<double> DrawX(Canvas axisCanvas)
        {
            if (this._axisData == null)
            {
                return null;
            }

            List<double> xList;
            switch (base.Orientation)
            {
                case AxisLabelOrientation.LeftToRight:
                    xList = this.DrawXAxisLeftToRight(axisCanvas, this._axisData);
                    break;
                case AxisLabelOrientation.RightToLeft:
                    xList = this.DrawXAxisRightToLeft(axisCanvas, this._axisData);
                    break;
                default:
                    throw new ArgumentException($"未知的{base.Orientation.ToString()}");
            }
            ChartHelper.DrawXAxisLabelLine(this, axisCanvas, xList);
            return xList;
        }

        private List<double> DrawXAxisRightToLeft(Canvas axisCanvas, NumberAxisData axisData)
        {
            List<double> xList = new List<double>();
            double axisWidth = axisCanvas.Width;
            double labelStep = this.CalculateLabelStep(axisData.Area, axisWidth);
            double labelStepSize = ChartHelper.CalculateLabelStepSize(axisData.Area, axisWidth, labelStep);
            double right = ChartConstant.ZERO_D;
            double lastLabelX = axisWidth;
            double x = axisWidth;
            double value = axisData.MinValue;
            TextBlock label;
            Size labelSize;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            bool addLabelControl;
            double offset;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(value));
                labelSize = UITextHelper.MeasureTextSize(label);
                if (axisWidth - labelSize.Width > ChartConstant.LABEL_TEXT_INTERVAL)
                {
                    addLabelControl = false;
                    switch (labelTextLocation)
                    {
                        case AxisLabelLocation.First:
                            axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetRight(label, right);
                            lastLabelX = right + labelSize.Width;
                            labelTextLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            right += labelStepSize;
                            offset = right - labelSize.Width / 2 - lastLabelX;
                            if (offset >= ChartConstant.LABEL_TEXT_INTERVAL)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetRight(label, right - labelSize.Width / 2);
                                lastLabelX = right + labelSize.Width;
                            }
                            else if (offset > 0)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetRight(label, right - labelSize.Width / 2 + offset);
                                lastLabelX = right + labelSize.Width + offset;
                            }
                            break;
                        case AxisLabelLocation.Last:
                            if (right > ChartConstant.ZERO_D && labelSize.Width + ChartConstant.LABEL_TEXT_INTERVAL <= lastLabelX)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;
                                Canvas.SetLeft(label, ChartConstant.ZERO_D);
                                lastLabelX = axisWidth;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (addLabelControl)
                    {
                        if (base.IsAxisXBottom())
                        {
                            Canvas.SetBottom(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                        else
                        {
                            Canvas.SetTop(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                    }
                }

                if (labelTextLocation == AxisLabelLocation.Last)
                {
                    if (this._showLastLabel)
                    {
                        xList.Add(x);
                    }
                    break;
                }
                xList.Add(x);

                value += labelStep;

                if (value >= axisData.MaxValue)
                //if (tmp - axisData.MaxValue > _PRE)
                {
                    value = axisData.MaxValue;
                    x = ChartConstant.ZERO_D;
                    labelTextLocation = AxisLabelLocation.Last;
                }
                else
                {
                    x -= labelStepSize;
                }
            }

            return xList;
        }

        private List<double> DrawXAxisLeftToRight(Canvas axisCanvas, NumberAxisData axisData)
        {
            List<double> xList = new List<double>();
            double axisWidth = axisCanvas.Width;
            double labelStep = this.CalculateLabelStep(axisData.Area, axisWidth);
            double labelStepSize = ChartHelper.CalculateLabelStepSize(axisData.Area, axisWidth, labelStep);
            double left = ChartConstant.ZERO_D;
            double lastLabelX = ChartConstant.ZERO_D;
            double x = ChartConstant.ZERO_D;
            double value = axisData.MinValue;
            TextBlock label;
            Size labelSize;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            bool addLabelControl;
            double offset;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(value));
                labelSize = UITextHelper.MeasureTextSize(label);
                if (axisWidth - labelSize.Width > ChartConstant.LABEL_TEXT_INTERVAL)
                {
                    addLabelControl = false;
                    switch (labelTextLocation)
                    {
                        case AxisLabelLocation.First:
                            axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetLeft(label, left);
                            lastLabelX = left + labelSize.Width;
                            labelTextLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            left += labelStepSize;
                            offset = left - labelSize.Width / 2 - lastLabelX;
                            if (offset >= ChartConstant.LABEL_TEXT_INTERVAL)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetLeft(label, left - labelSize.Width / 2);
                                lastLabelX = left + labelSize.Width;
                            }
                            else if (offset > 0)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetLeft(label, left - labelSize.Width / 2 + offset);
                                lastLabelX = left + labelSize.Width + offset;
                            }
                            break;
                        case AxisLabelLocation.Last:
                            if (lastLabelX + labelSize.Width + ChartConstant.LABEL_TEXT_INTERVAL <= axisWidth)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;
                                Canvas.SetRight(label, ChartConstant.ZERO_D);
                                lastLabelX = axisWidth;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    if (addLabelControl)
                    {
                        if (base.IsAxisXBottom())
                        {
                            Canvas.SetBottom(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                        else
                        {
                            Canvas.SetTop(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                    }
                }

                if (labelTextLocation == AxisLabelLocation.Last)
                {
                    if (this._showLastLabel)
                    {
                        xList.Add(x);
                    }
                    break;
                }
                xList.Add(x);

                value += labelStep;

                if (value >= axisData.MaxValue)
                //if (tmp - axisData.MaxValue > _PRE)
                {
                    value = axisData.MaxValue;
                    x = axisWidth;
                    labelTextLocation = AxisLabelLocation.Last;
                }
                else
                {
                    x += labelStepSize;
                }
            }

            return xList;
        }




        /// <summary>
        /// 获取指定项在X轴的坐标值
        /// </summary>
        /// <param name="item">目标项</param>
        /// <returns>指定项在X轴的坐标值</returns>
        protected override double PrimitiveGetX(IChartItem item)
        {
            return this.GetAxis(item, true, base._axisCanvas.Width);
        }

        /// <summary>
        /// 获取指定项在Y轴的坐标值
        /// </summary>
        /// <param name="item">目标项</param>
        /// <returns>指定项在Y轴的坐标值</returns>
        protected override double PrimitiveGetY(IChartItem item)
        {
            return this.GetAxis(item, false, base._axisCanvas.Height);
        }

        private double GetAxis(IChartItem item, bool x, double axisSize)
        {
            if (this._axisData == null)
            {
                return double.NaN;
            }

            object obj = ChartHelper.GetChartItemAxisValue(item, x);
            if (item == null)
            {
                return double.NaN;
            }

            double value = ChartHelper.ConvertToDouble(obj);
            if (!ChartHelper.DoubleHasValue(value))
            {
                return double.NaN;
            }

            double result = axisSize * (value - this._axisData.MinValue) / this._axisData.Area;
            if (base.Orientation == AxisLabelOrientation.BottomToTop ||
                base.Orientation == AxisLabelOrientation.RightToLeft)
            {
                result = axisSize - result;
            }

            return result;
        }








        /// <summary>
        /// 缩放更新
        /// </summary>
        protected override List<double> Update(Canvas axisCanvas)
        {
            switch (base.AxisType)
            {
                case AxisType.X:
                    return this.DrawX(axisCanvas);
                case AxisType.Y:
                    return this.DrawY(axisCanvas);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal class NumberAxisValueArea
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public NumberAxisValueArea(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }
    }

    internal class NumberAxisData
    {
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }

        public double Area { get; private set; }

        public NumberAxisData(double minValue, double maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Area = maxValue - minValue;
        }
    }
}
