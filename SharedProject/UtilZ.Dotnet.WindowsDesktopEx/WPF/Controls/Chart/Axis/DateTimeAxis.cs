using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 日期时间坐标轴
    /// </summary>
    public class DateTimeAxis : AxisAbs
    {
        private TimeSpan? _labelStep = null;
        /// <summary>
        /// 坐标轴刻度值间隔,为null时自动计算
        /// </summary>
        public TimeSpan? LabelStep
        {
            get { return _labelStep; }
            set
            {
                _labelStep = value;
                base.OnRaisePropertyChanged(nameof(LabelStep));
            }
        }


        private DateTime? _minValue = null;
        /// <summary>
        /// 坐标轴刻度最小值,为null时自动计算
        /// </summary>
        public DateTime? MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                base.OnRaisePropertyChanged(nameof(MinValue));
            }
        }

        private DateTime? _maxValue = null;
        /// <summary>
        /// 坐标轴刻度最大值,为null时自动计算
        /// </summary>
        public DateTime? MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                base.OnRaisePropertyChanged(nameof(MaxValue));
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
        public Func<DateTime, string> CustomAxisTextFormatCunc;
        private DateTimeAxisData _axisData = null;


        /// <summary>
        /// 构造函数
        /// </summary>
        public DateTimeAxis()
            : base()
        {

        }


        private Size _labelTextSize;
        /// <summary>
        /// 获取X坐标轴高度
        /// </summary>
        /// <returns>X坐标轴高度</returns>
        protected override double PrimitiveGetXAxisHeight()
        {
            this._labelTextSize = this.MeasureLabelTextSize();
            return base.CalculateAxisSize(this._labelTextSize.Height);
        }

        private Size MeasureLabelTextSize()
        {
            string labelText = this.CreateAxisText(DateTime.Parse("2020-12-12 22:22:22"));
            return ChartHelper.MeasureLabelTextSize(this, labelText);
        }

        private string CreateAxisText(DateTime value)
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

        private DateTimeAxisData CreateAxisData(ChartCollection<ISeries> seriesCollection)
        {
            if (this._minValue == null || this._maxValue == null)
            {
                DateTimeMinAndMaxValue result = this.GetMinAndMaxValue(seriesCollection);
                if (this._minValue != null)
                {
                    result.MinTime = this._minValue.Value;
                }

                if (this._maxValue != null)
                {
                    result.MaxTime = this._maxValue.Value;
                }

                this.CompleteAxisAreaValue(result);
                return this.CreateDateTimeAxisData(result.MinTime, result.MaxTime);
            }
            else
            {
                return this.CreateDateTimeAxisData(this._minValue, this._maxValue);
            }
        }

        private DateTimeAxisData CreateDateTimeAxisData(DateTime? minTime, DateTime? maxTime)
        {
            if (minTime == null || maxTime == null)
            {
                return null;
            }

            TimeSpan area = maxTime.Value - minTime.Value;
            if (area.TotalMilliseconds < base._PRE)
            {
                return null;
            }

            return new DateTimeAxisData(minTime.Value, maxTime.Value);
        }

        private void CompleteAxisAreaValue(DateTimeMinAndMaxValue result)
        {
            if (result.MinTime == null)
            {
                if (result.MaxTime != null && this._labelStep != null)
                {
                    result.MinTime = result.MaxTime.Value.Subtract(this._labelStep.Value);
                }
            }
            else
            {
                if (result.MaxTime == null && this._labelStep != null)
                {
                    result.MaxTime = result.MinTime.Value.Add(this._labelStep.Value);
                }
            }
        }

        private DateTimeMinAndMaxValue GetMinAndMaxValue(ChartCollection<ISeries> seriesCollection)
        {
            if (seriesCollection == null || seriesCollection.Count == 0)
            {
                return new DateTimeMinAndMaxValue(null, null);
            }

            DateTime? min = null, max = null;
            IChartAxisValue chartAxisValue;
            object obj;
            DateTime? time;
            foreach (var series in seriesCollection)
            {
                if (series.AxisX != this && series.AxisY != this ||
                    series.Values == null ||
                    series.Values.Count == 0)
                {
                    continue;
                }

                foreach (var value in series.Values)
                {
                    chartAxisValue = value as IChartAxisValue;
                    if (chartAxisValue == null)
                    {
                        continue;
                    }

                    switch (this.AxisType)
                    {
                        case AxisType.X:
                            obj = chartAxisValue.GetXValue();
                            break;
                        case AxisType.Y:
                            obj = chartAxisValue.GetYValue();
                            break;
                        default:
                            throw new NotImplementedException(this.AxisType.ToString());
                    }

                    time = ChartHelper.ConvertToDateTime(obj);
                    if (time == null)
                    {
                        continue;
                    }

                    if (min == null || time.Value < min.Value)
                    {
                        min = time.Value;
                    }

                    if (max == null || time.Value > max.Value)
                    {
                        max = time.Value;
                    }
                }
            }

            return new DateTimeMinAndMaxValue(min, max);
        }




        private double CalculateXLabelStep(DateTimeAxisData axisData)
        {
            double labelStepMilliseconds = this._labelStep != null ? this._labelStep.Value.TotalMilliseconds : double.NaN;
            if (double.IsNaN(labelStepMilliseconds))
            {
                const double INTERVAL = 20d;
                double labelTextSpace = INTERVAL + this._labelTextSize.Width * 1.5d;
                int count = (int)(this._axisCanvas.Width / labelTextSpace);
                if (count == 0)
                {
                    labelStepMilliseconds = axisData.Area.TotalMilliseconds;
                }
                else
                {
                    TimeSpan intervalTimeLength = TimeSpan.FromMilliseconds(axisData.Area.TotalMilliseconds / count);//一个刻度内时长
                    labelStepMilliseconds = this.AdjustIntervalTimeLength(intervalTimeLength);
                }
            }

            return labelStepMilliseconds;
        }

        private double CalculateYLabelStep(DateTimeAxisData axisData)
        {
            double labelStepMilliseconds = this._labelStep != null ? this._labelStep.Value.TotalMilliseconds : double.NaN;
            if (double.IsNaN(labelStepMilliseconds))
            {
                int labelCount = (int)(this._axisCanvas.Height / ChartConstant.DEFAULT_STEP_SIZE);
                if (this._axisCanvas.Height % ChartConstant.DEFAULT_STEP_SIZE > ChartConstant.ZERO_D)
                {
                    labelCount += 1;
                }

                if (labelCount == 0)
                {
                    labelStepMilliseconds = axisData.Area.TotalMilliseconds;
                }
                else
                {
                    TimeSpan intervalTimeLength = TimeSpan.FromMilliseconds(axisData.Area.TotalMilliseconds / labelCount);//一个刻度内时长
                    labelStepMilliseconds = this.AdjustIntervalTimeLength(intervalTimeLength);
                }
            }

            return labelStepMilliseconds;
        }

        private double AdjustIntervalTimeLength(TimeSpan intervalTimeLength)
        {
            double stepMilliseconds;

            const int YEAR_DAYS = 365;
            const int MONTH_DAYS = 365;
            const int ZERO = 0;
            const double HALF = 0.5d;
            const double DAY_MILLISECONDES = 86400000d;
            const double HOUR_MILLISECONDES = 3600000d;
            const double MINUT_MILLISECONDES = 60000d;
            const double SECOND_MILLISECONDES = 1000d;
            const double OFFSET = 1.0d;

            if (intervalTimeLength.Days >= YEAR_DAYS)
            {
                double totalYears = intervalTimeLength.TotalDays / YEAR_DAYS;
                long intergerYears = (long)totalYears;
                if (totalYears - intergerYears > +HALF)
                {
                    stepMilliseconds = Math.Ceiling(intervalTimeLength.TotalDays / YEAR_DAYS) * YEAR_DAYS * DAY_MILLISECONDES;
                }
                else
                {
                    stepMilliseconds = (HALF + (double)intergerYears) * YEAR_DAYS * DAY_MILLISECONDES;
                }
            }
            else if (intervalTimeLength.Days >= MONTH_DAYS)
            {
                double totalMonths = intervalTimeLength.TotalDays / MONTH_DAYS;
                long intergerMonths = (long)totalMonths;
                if (totalMonths - intergerMonths >= HALF)
                {
                    stepMilliseconds = Math.Ceiling(intervalTimeLength.TotalDays / MONTH_DAYS) * MONTH_DAYS * DAY_MILLISECONDES;
                }
                else
                {
                    stepMilliseconds = (HALF + (double)intergerMonths) * MONTH_DAYS * DAY_MILLISECONDES;
                }
            }
            else if (intervalTimeLength.Days > ZERO)
            {
                if (intervalTimeLength.TotalDays - intervalTimeLength.Days >= HALF)
                {
                    stepMilliseconds = Math.Ceiling(intervalTimeLength.TotalDays) * DAY_MILLISECONDES;
                }
                else
                {
                    stepMilliseconds = HALF * DAY_MILLISECONDES + intervalTimeLength.Days * DAY_MILLISECONDES;
                }
            }
            else if (intervalTimeLength.Hours > ZERO)
            {
                double hours = intervalTimeLength.TotalMilliseconds / HOUR_MILLISECONDES;
                double hoursMilliseconds = hours - intervalTimeLength.Hours;

                if (hoursMilliseconds == ZERO)
                {
                    stepMilliseconds = intervalTimeLength.TotalMilliseconds;
                }
                else if (hoursMilliseconds >= HALF)
                {
                    stepMilliseconds = Math.Ceiling(hours) * HOUR_MILLISECONDES;
                }
                else
                {
                    stepMilliseconds = ((double)intervalTimeLength.Hours + HALF) * HOUR_MILLISECONDES;
                }
            }
            else if (intervalTimeLength.Minutes > ZERO)
            {
                const double HALF_MINUT_MILLISECONDES = 30000d;
                double minutesSurplusMilliseconds = intervalTimeLength.TotalMilliseconds - intervalTimeLength.Minutes * MINUT_MILLISECONDES;
                if (minutesSurplusMilliseconds == ZERO)
                {
                    stepMilliseconds = intervalTimeLength.TotalMilliseconds;
                }
                else if (minutesSurplusMilliseconds >= HALF_MINUT_MILLISECONDES)
                {
                    stepMilliseconds = ((double)intervalTimeLength.Minutes + OFFSET) * MINUT_MILLISECONDES;
                }
                else
                {
                    stepMilliseconds = ((double)intervalTimeLength.Minutes + HALF) * MINUT_MILLISECONDES;
                }
            }
            else if (intervalTimeLength.Seconds > ZERO)
            {
                const double HALF_SECONDE_MILLISECONDES = 500d;
                double secondsSurplusMilliseconds = intervalTimeLength.TotalMilliseconds - intervalTimeLength.Seconds * SECOND_MILLISECONDES;
                if (secondsSurplusMilliseconds == ZERO)
                {
                    stepMilliseconds = intervalTimeLength.TotalMilliseconds;
                }
                else if (secondsSurplusMilliseconds >= HALF_SECONDE_MILLISECONDES)
                {
                    stepMilliseconds = ((double)intervalTimeLength.Minutes + OFFSET) * MINUT_MILLISECONDES;
                }
                else
                {
                    stepMilliseconds = ((double)intervalTimeLength.Minutes + HALF) * MINUT_MILLISECONDES;
                }
            }
            else
            {
                stepMilliseconds = Math.Ceiling(intervalTimeLength.TotalMilliseconds);
                var muilt = ChartHelper.CalDoubleToIntegerMuilt(stepMilliseconds);
                var step2 = ChartHelper.DoubleToCeilingInteger(stepMilliseconds, muilt);
                while (step2 >= intervalTimeLength.TotalMilliseconds && muilt >= 1)
                {
                    muilt = muilt / 10;
                    step2 = ChartHelper.DoubleToCeilingInteger(stepMilliseconds, muilt);
                }

                if (!double.IsNaN(step2))
                {
                    stepMilliseconds = step2;
                }
            }

            return stepMilliseconds;
        }







        /// <summary>
        /// 子类重写此函数时,必须设置Y轴宽度
        /// </summary>
        /// <param name="axisCanvas">画布</param>
        /// <param name="seriesCollection">Series集合</param>
        /// <returns>Label的Y列表</returns>
        protected override List<double> PrimitiveDrawY(Canvas axisCanvas, ChartCollection<ISeries> seriesCollection)
        {
            this._labelTextSize = this.MeasureLabelTextSize();
            axisCanvas.Width = base.CalculateAxisSize(this._labelTextSize.Width);
            this._axisData = this.CreateAxisData(seriesCollection);
            return this.DrawY(axisCanvas);
        }

        private List<double> DrawY(Canvas axisCanvas)
        {
            if (this._axisData == null)
            {
                return null;
            }

            double labelStepMilliseconds = this.CalculateYLabelStep(this._axisData);
            double labelStepSize = ChartHelper.CalculateLabelStepSize(this._axisData.Area.TotalMilliseconds, axisCanvas.Height, labelStepMilliseconds);
            List<double> yList;
            switch (base.Orientation)
            {
                case AxisLabelOrientation.BottomToTop:
                    yList = this.DrawYAxisBottomToTop(axisCanvas, this._axisData, labelStepMilliseconds, labelStepSize);
                    break;
                case AxisLabelOrientation.TopToBottom:
                    yList = this.DrawXAxisTopToBottom(axisCanvas, this._axisData, labelStepMilliseconds, labelStepSize);
                    break;
                default:
                    throw new ArgumentException($"未知的{base.Orientation.ToString()}");
            }

            ChartHelper.DrawYAxisLabelLine(this, axisCanvas, yList);
            return yList;
        }

        private List<double> DrawXAxisTopToBottom(Canvas axisCanvas, DateTimeAxisData axisData, double labelStepMilliseconds, double labelStepSize)
        {
            List<double> yList = new List<double>();
            double axisHeight = axisCanvas.Height;
            double top = ChartConstant.ZERO_D, top2;
            DateTime time = axisData.MinValue;
            double y = ChartConstant.ZERO_D;
            TextBlock label;
            Size labelSize = this._labelTextSize;
            double heightHalf = labelSize.Height / 2;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            bool addLabelControl;
            double lastLabelY = axisHeight;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(time));

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
                            top2 = top - heightHalf;
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
                            Canvas.SetLeft(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                        else
                        {
                            Canvas.SetRight(label, ChartConstant.LABEL_TEXT_INTERVAL);
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

                time = time.AddMilliseconds(labelStepMilliseconds);
                if (time >= axisData.MaxValue)
                //if (tmp - maxValue > _PRE)
                {
                    labelStepSize = (labelStepMilliseconds - (time - axisData.MaxValue).TotalMilliseconds) * labelStepSize / labelStepMilliseconds;
                    double labelHeight = ChartHelper.MeasureLabelTextSize(this, axisData.MaxValue.ToString()).Height + 10d;
                    if (labelStepSize < labelHeight)
                    {
                        break;
                    }

                    time = axisData.MaxValue;
                    y = axisHeight;
                    labelTextLocation = AxisLabelLocation.Last;
                }
                else
                {
                    y += labelStepSize;
                }

                top += labelStepSize;
            }

            return yList;
        }

        private List<double> DrawYAxisBottomToTop(Canvas axisCanvas, DateTimeAxisData axisData, double labelStepMilliseconds, double labelStepSize)
        {
            List<double> yList = new List<double>();
            double axisHeight = axisCanvas.Height;
            double bottom = ChartConstant.ZERO_D, bottom2;
            DateTime time = axisData.MinValue;
            double y = axisHeight;
            TextBlock label;
            Size labelSize = this._labelTextSize;
            double heightHalf = labelSize.Height / 2;
            AxisLabelLocation labelLocation = AxisLabelLocation.First;
            bool addLabelControl;
            double lastLabelY = axisHeight;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(time));

                if (axisHeight > labelSize.Height)
                {
                    addLabelControl = false;
                    switch (labelLocation)
                    {
                        case AxisLabelLocation.First:
                            axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetBottom(label, bottom);
                            lastLabelY = axisHeight - labelSize.Height;
                            labelLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            bottom2 = bottom - heightHalf;
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
                            throw new NotImplementedException(labelLocation.ToString());
                    }

                    if (addLabelControl)
                    {
                        if (base.IsAxisYLeft())
                        {
                            Canvas.SetLeft(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                        else
                        {
                            Canvas.SetRight(label, ChartConstant.LABEL_TEXT_INTERVAL);
                        }
                    }
                }

                if (labelLocation == AxisLabelLocation.Last)
                {
                    if (this._showLastLabel)
                    {
                        yList.Add(y);
                    }
                    break;
                }
                yList.Add(y);

                time = time.AddMilliseconds(labelStepMilliseconds);
                if (time >= axisData.MaxValue)
                {
                    labelStepSize = (labelStepMilliseconds - (time - axisData.MaxValue).TotalMilliseconds) * labelStepSize / labelStepMilliseconds;
                    double labelHeight = ChartHelper.MeasureLabelTextSize(this, axisData.MaxValue.ToString()).Height + 10d;
                    if (labelStepSize < labelHeight)
                    {
                        break;
                    }

                    time = axisData.MaxValue;
                    y = ChartConstant.ZERO_D;
                    labelLocation = AxisLabelLocation.Last;
                }
                else
                {
                    y -= labelStepSize;
                }

                bottom += labelStepSize;
            }

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
            double labelStepMilliseconds = this.CalculateXLabelStep(this._axisData);
            double labelStepSize = ChartHelper.CalculateLabelStepSize(this._axisData.Area.TotalMilliseconds, axisCanvas.Width, labelStepMilliseconds);
            switch (base.Orientation)
            {
                case AxisLabelOrientation.LeftToRight:
                    xList = this.DrawXAxisLeftToRight(axisCanvas, this._axisData, labelStepMilliseconds, labelStepSize);
                    break;
                case AxisLabelOrientation.RightToLeft:
                    xList = this.DrawXAxisRightToLeft(axisCanvas, this._axisData, labelStepMilliseconds, labelStepSize);
                    break;
                default:
                    throw new ArgumentException($"未知的{base.Orientation.ToString()}");
            }
            ChartHelper.DrawXAxisLabelLine(this, axisCanvas, xList);
            return xList;
        }

        private List<double> DrawXAxisRightToLeft(Canvas axisCanvas, DateTimeAxisData axisData, double labelStepMilliseconds, double labelStepSize)
        {
            List<double> xList = new List<double>();
            double axisWidth = axisCanvas.Width;
            double right = ChartConstant.ZERO_D;
            double lastLabelX = axisWidth;
            DateTime time = axisData.MinValue;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            double labelTextWidth = this._labelTextSize.Width;
            double labelTextWidthHalf = labelTextWidth / 2;
            double offset = labelTextWidth / 2;
            TextBlock label;
            bool addLabelControl;
            double x = axisWidth;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(time));

                if (axisWidth - labelTextWidth > ChartConstant.LABEL_TEXT_INTERVAL)
                {
                    addLabelControl = false;

                    switch (labelTextLocation)
                    {
                        case AxisLabelLocation.First:
                            this._axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetRight(label, right);
                            lastLabelX = right + labelTextWidth;
                            labelTextLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            right += labelStepSize;
                            offset = right - labelTextWidthHalf - lastLabelX;
                            if (offset >= ChartConstant.LABEL_TEXT_INTERVAL)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetRight(label, right - labelTextWidthHalf);
                                lastLabelX = right + labelTextWidth;
                            }
                            else if (offset > 0)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetRight(label, right - labelTextWidthHalf + offset);
                                lastLabelX = right + labelTextWidth + offset;
                            }
                            break;
                        case AxisLabelLocation.Last:
                            if (right > ChartConstant.ZERO_D && labelTextWidth + ChartConstant.LABEL_TEXT_INTERVAL <= lastLabelX)
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

                time = time.AddMilliseconds(labelStepMilliseconds);
                if (time >= axisData.MaxValue)
                {
                    x = ChartConstant.ZERO_D;
                    time = axisData.MaxValue;
                    labelTextLocation = AxisLabelLocation.Last;
                }
                else
                {
                    x -= labelStepSize;
                }
            }

            return xList;
        }

        private List<double> DrawXAxisLeftToRight(Canvas axisCanvas, DateTimeAxisData axisData, double labelStepMilliseconds, double labelStepSize)
        {
            List<double> xList = new List<double>();
            double axisWidth = axisCanvas.Width;
            double left = ChartConstant.ZERO_D;
            double lastLabelX = ChartConstant.ZERO_D;
            DateTime time = axisData.MinValue;
            AxisLabelLocation labelTextLocation = AxisLabelLocation.First;
            double labelTextWidth = this._labelTextSize.Width;
            double labelTextWidthHalf = labelTextWidth / 2;
            double offset = labelTextWidth / 2;
            TextBlock label;
            bool addLabelControl;
            double x = left;

            while (true)
            {
                label = ChartHelper.CreateLabelControl(this, this.CreateAxisText(time));

                if (axisWidth - labelTextWidth > ChartConstant.LABEL_TEXT_INTERVAL)
                {
                    addLabelControl = false;

                    switch (labelTextLocation)
                    {
                        case AxisLabelLocation.First:
                            this._axisCanvas.Children.Add(label);
                            addLabelControl = true;
                            Canvas.SetLeft(label, left);
                            lastLabelX = left + labelTextWidth;
                            labelTextLocation = AxisLabelLocation.Middle;
                            break;
                        case AxisLabelLocation.Middle:
                            left += labelStepSize;
                            offset = left - labelTextWidthHalf - lastLabelX;
                            if (offset >= ChartConstant.LABEL_TEXT_INTERVAL)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetLeft(label, left - labelTextWidthHalf);
                                lastLabelX = left + labelTextWidth;
                            }
                            else if (offset > 0)
                            {
                                axisCanvas.Children.Add(label);
                                addLabelControl = true;

                                Canvas.SetLeft(label, left - labelTextWidthHalf + offset);
                                lastLabelX = left + labelTextWidth + offset;
                            }
                            break;
                        case AxisLabelLocation.Last:
                            if (lastLabelX + labelTextWidth + ChartConstant.LABEL_TEXT_INTERVAL <= axisWidth)
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

                time = time.AddMilliseconds(labelStepMilliseconds);
                if (time >= axisData.MaxValue)
                {
                    x = this._axisCanvas.Width;
                    time = axisData.MaxValue;
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

            DateTime? value = ChartHelper.ConvertToDateTime(obj);
            if (value == null)
            {
                return double.NaN;
            }

            //计算AxisOrientation.LeftToRight或TopToBottom
            double result = axisSize * (value.Value - this._axisData.MinValue).TotalMilliseconds / this._axisData.Area.TotalMilliseconds;
            if (base.Orientation == AxisLabelOrientation.RightToLeft ||
                base.Orientation == AxisLabelOrientation.BottomToTop)
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


    internal class DateTimeAxisData
    {
        public DateTime MinValue { get; private set; }
        public DateTime MaxValue { get; private set; }

        public TimeSpan Area { get; private set; }

        public DateTimeAxisData(DateTime minValue, DateTime maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Area = maxValue - minValue;
        }
    }

    internal class DateTimeMinAndMaxValue
    {
        public DateTime? MinTime { get; set; }
        public DateTime? MaxTime { get; set; }

        public DateTimeMinAndMaxValue(DateTime? minTime, DateTime? maxTime)
        {
            this.MinTime = minTime;
            this.MaxTime = maxTime;
        }
    }
}
