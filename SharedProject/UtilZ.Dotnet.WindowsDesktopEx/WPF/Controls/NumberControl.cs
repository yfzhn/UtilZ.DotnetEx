using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 数值输入控件
    /// </summary>
    public class NumberControl : TextBox
    {
        private const string _POINT = ".";
        private const string _SUBTRACT = "-";
        private const char _ZEROR = '0';
        private const int _DECIMAL_PLACES_MIN = 1;

        #region 依赖属性
        /// <summary>
        /// 值依赖属性
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(NumberControl),
                new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 值是否允许为NaN依赖属性
        /// </summary>
        public static readonly DependencyProperty AllowNaNProperty =
            DependencyProperty.Register(nameof(AllowNaN), typeof(bool), typeof(NumberControl),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnPropertyChangedCallback)));


        /// <summary>
        /// 最小值依赖属性
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(NumberControl),
                new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 最大值依赖属性
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(NumberControl),
                new FrameworkPropertyMetadata(100d, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 获取或设置整小数显示位数依赖属性
        /// </summary>
        public static readonly DependencyProperty DecimalWidthProperty =
            DependencyProperty.Register(nameof(DecimalWidth), typeof(int), typeof(NumberControl),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 整数显示位数依赖属性
        /// </summary>
        public static readonly DependencyProperty IntegerWidthProperty =
            DependencyProperty.Register(nameof(IntegerWidth), typeof(int), typeof(NumberControl),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChangedCallback)));

        ///// <summary>
        ///// 单击向上或向下按钮时，数字显示框（也称作 up-down 控件）递增或递减的值依赖属性
        ///// </summary>
        //public static readonly DependencyProperty IncrementProperty =
        //    DependencyProperty.Register(nameof(Increment), typeof(double), typeof(NumberControl),
        //        new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnPropertyChangedCallback)));

        /// <summary>
        /// 转换器依赖属性
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register(nameof(Converter), typeof(IValueConverter), typeof(NumberControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChangedCallback)));



        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler<NumberValueChangedArgs> ValueChanged;

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public double Value
        {
            get
            {
                return (double)base.GetValue(ValueProperty);
            }
            set
            {
                if (!double.IsNaN(value))
                {
                    if (value - this.Minimum < this._precision || value - this.Maximum > this._precision)
                    {
                        throw new ArgumentOutOfRangeException($"值超出最大最小值范围{this.Minimum}-{this.Maximum}");
                    }

                    string valueStr = value.ToString();
                    int pointIndex = valueStr.IndexOf(_POINT);
                    int decimalPlaces = this.DecimalWidth;
                    if (decimalPlaces >= _DECIMAL_PLACES_MIN &&
                        pointIndex > 0 &&
                        valueStr.Length - (pointIndex + 1) > decimalPlaces)
                    {
                        valueStr = valueStr.Substring(0, pointIndex + 1 + decimalPlaces);
                        value = double.Parse(valueStr);
                    }
                }

                var oldValue = this.Value;
                base.SetValue(ValueProperty, value);

                var handler = this.ValueChanged;
                if (handler != null)
                {
                    handler(this, new NumberValueChangedArgs(oldValue, value));
                }
            }
        }

        /// <summary>
        /// 获取或设置值是否允许为NaN,true:允许;false:不允许
        /// </summary>
        public bool AllowNaN
        {
            get
            {
                return (bool)base.GetValue(AllowNaNProperty);
            }
            set
            {
                base.SetValue(AllowNaNProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置数字显示框的最小允许值,为double.NaN时无限制
        /// </summary>
        public double Minimum
        {
            get
            {
                return (double)base.GetValue(MinimumProperty);
            }
            set
            {
                base.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置数字显示框的最大允许值,为double.NaN时无限制
        /// </summary>
        public double Maximum
        {
            get
            {
                return (double)base.GetValue(MaximumProperty);
            }
            set
            {
                base.SetValue(MaximumProperty, value);
            }
        }

        private double _precision = 0d;
        /// <summary>
        /// 获取或设置整小数显示位数,小于1表示无限制
        /// </summary>
        public int DecimalWidth
        {
            get
            {
                return (int)base.GetValue(DecimalWidthProperty);
            }
            set
            {
                base.SetValue(DecimalWidthProperty, value);
            }
        }


        /// <summary>
        /// 获取或设置整数显示位数,小于1表示无限制
        /// </summary>
        public int IntegerWidth
        {
            get
            {
                return (int)base.GetValue(IntegerWidthProperty);
            }
            set
            {
                base.SetValue(IntegerWidthProperty, value);
            }
        }

        /// <summary>
        /// 转换器
        /// </summary>
        public IValueConverter Converter
        {
            get
            {
                return (IValueConverter)base.GetValue(ConverterProperty);
            }
            set
            {
                base.SetValue(ConverterProperty, value);
            }
        }

        ///// <summary>
        ///// 获取或设置单击向上或向下按钮时，数字显示框（也称作 up-down 控件）递增或递减的值
        ///// </summary>
        //public double Increment
        //{
        //    get
        //    {
        //        return (double)base.GetValue(IncrementProperty);
        //    }
        //    set
        //    {
        //        base.SetValue(IncrementProperty, value);
        //    }
        //}



        private static void OnPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selfControl = (NumberControl)d;

            if (e.Property == MinimumProperty)
            {
                selfControl.UpdateValueByMinimum((double)e.NewValue);
            }
            else if (e.Property == MaximumProperty)
            {
                selfControl.UpdateValueByMaximum((double)e.NewValue);
            }
            else if (e.Property == AllowNaNProperty)
            {
                selfControl.UpdateValueByAllowNaN((bool)e.NewValue);
            }
            //else if (e.Property == DecimalPlacesProperty)
            //{
            //    selfControl.UpdatePrecision((int)e.NewValue);
            //}


            if (e.Property == ValueProperty ||
                 e.Property == DecimalWidthProperty ||
                 e.Property == DecimalWidthProperty ||
                 e.Property == IntegerWidthProperty ||
                 e.Property == ConverterProperty)
            {
                selfControl.UpdateTextByValue();
            }
        }

        private void UpdateTextByValue()
        {
            this._setShowText = true;
            try
            {
                double value = this.Value;
                if (double.IsNaN(value))
                {
                    this.Text = string.Empty;
                    return;
                }

                if (this.Converter == null)
                {
                    this.Text = value.ToString();
                    this.FormatShowText();
                }
                else
                {
                    object result = this.Converter.Convert(value, null, null, null);
                    if (result != null)
                    {
                        this.Text = result.ToString();
                    }
                    else
                    {
                        this.Text = string.Empty;
                    }
                }
                this.SelectionStart = this.Text.Length;
            }
            finally
            {
                this._setShowText = false;
            }
        }

        private bool _setShowText = false;
        private void FormatShowText()
        {
            int decimalWidth = this.DecimalWidth;
            int integerWidth = this.IntegerWidth;

            if (decimalWidth < 1 && integerWidth < 1)
            {
                return;
            }

            int pointIndex = this.Text.IndexOf('.');
            bool minusNumber = this.Text.StartsWith("-");
            int count;
            string fillStr;
            int insertFillStrIndex;

            if (decimalWidth > 0)
            {
                if (integerWidth > 0)
                {
                    //整数+小数

                    //整数
                    if (pointIndex >= 0)
                    {
                        count = integerWidth - pointIndex;
                    }
                    else
                    {
                        count = integerWidth - this.Text.Length;
                    }

                    insertFillStrIndex = 0;
                    if (minusNumber)
                    {
                        count -= 1;
                        insertFillStrIndex = 1;
                    }

                    if (count > 0)
                    {
                        fillStr = new string('0', count);
                        this.Text = this.Text.Insert(insertFillStrIndex, fillStr);
                    }

                    //小数
                    pointIndex = this.Text.IndexOf('.');
                    if (pointIndex >= 0)
                    {
                        insertFillStrIndex = pointIndex + 1;
                        count = decimalWidth - (this.Text.Length - insertFillStrIndex);
                        if (count > 0)
                        {
                            fillStr = new string('0', count);
                            this.Text = this.Text + fillStr;
                        }
                    }
                    else
                    {
                        fillStr = new string('0', decimalWidth);
                        this.Text = $"{this.Text}.{fillStr}";
                    }
                }
                else
                {
                    //只有小数
                    if (pointIndex < 0)
                    {
                        fillStr = new string('0', decimalWidth);
                        this.Text = $"{this.Text}.{fillStr}";
                    }
                    else
                    {
                        insertFillStrIndex = pointIndex + 1;
                        count = decimalWidth - (this.Text.Length - insertFillStrIndex);
                        if (count > 0)
                        {
                            fillStr = new string('0', count);
                            this.Text = this.Text.Insert(insertFillStrIndex, fillStr);
                        }
                    }
                }
            }
            else
            {
                if (integerWidth > 0)
                {
                    //只有整数
                    if (pointIndex >= 0)
                    {
                        count = integerWidth - pointIndex;
                    }
                    else
                    {
                        count = integerWidth - this.Text.Length;
                    }

                    insertFillStrIndex = 0;
                    if (minusNumber)
                    {
                        count -= 1;
                        insertFillStrIndex = 1;
                    }

                    if (count > 0)
                    {
                        fillStr = new string('0', count);
                        this.Text = this.Text.Insert(insertFillStrIndex, fillStr);
                    }
                }
                else
                {
                    //无限制
                }
            }
        }

        private void UpdatePrecision(int decimalPlaces)
        {
            if (decimalPlaces > 0)
            {
                this._precision = 1d / Math.Pow(10d, (double)(decimalPlaces + 1));
            }
            else
            {
                this._precision = 0d;
            }
        }

        private void UpdateValueByAllowNaN(bool allowNaN)
        {
            if (allowNaN)
            {
                return;
            }

            if (double.IsNaN(this.Value))
            {
                this.Value = this.Minimum;
            }
        }

        private void UpdateValueByMaximum(double value)
        {
            if (!double.IsNaN(value) && this.Value - value > this._precision)
            {
                this.Value = value;
            }
        }

        private void UpdateValueByMinimum(double value)
        {
            if (!double.IsNaN(value) && this.Value - value < this._precision)
            {
                this.Value = value;
            }
        }
        #endregion






        private bool _enablePreviewMouseDown = true;
        private bool _valueNoComit = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public NumberControl()
        {
            this.HorizontalContentAlignment = HorizontalAlignment.Left;
            this.VerticalContentAlignment = VerticalAlignment.Center;
            this.IsInactiveSelectionHighlightEnabled = true;
            this.UndoLimit = 2;
            this.UpdateTextByValue();

            InputMethod.SetPreferredImeState(this, InputMethodState.Off);//禁止输入中文

            var pasteCommand = new CommandBinding(ApplicationCommands.Paste);
            pasteCommand.Executed += PasteCommand_Executed;
            this.CommandBindings.Add(pasteCommand);

            //var copyCommand = new CommandBinding(ApplicationCommands.Copy);
            //copyCommand.Executed += CopyCommand_Executed; ;
            //this.CommandBindings.Add(copyCommand);
        }

        private void CopyCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            object data;
            if (this.SelectionLength == 0)
            {
                data = this.Text;
            }
            else
            {
                data = this.SelectedText;
            }

            Clipboard.SetData(System.Windows.DataFormats.Text, data);
            if (e != null)
            {
                e.Handled = true;
            }
        }

        private void PasteCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            string text = Clipboard.GetText(TextDataFormat.Text);
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            double value;
            if (!double.TryParse(text, out value))
            {
                return;
            }

            if (value - this.Minimum < this._precision || value - this.Maximum > this._precision)
            {
                return;
            }

            this.Value = value;
        }






        /// <summary>
        /// 重写OnPreviewMouseDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (e.Handled)
            {
                return;
            }

            if (this._enablePreviewMouseDown)
            {
                this.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 重写OnGotFocus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            this.SelectAll();
            this._enablePreviewMouseDown = false;
        }

        /// <summary>
        /// 重写OnLostFocus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            this._enablePreviewMouseDown = true;
            this.CommitValue();
        }






        private static Key[] _numberKeyArr = null;
        private static Key[] _allowInputKeyArr = null;
        private bool _lastPressDownCtrl = false;
        /// <summary>
        /// 移除选中内容
        /// </summary>
        private const Key _REMOVE_SELECTION = Key.Clear;
        /// <summary>
        /// 无
        /// </summary>
        private const Key _NONE = Key.None;
        private Key _textChangedKey = _NONE;

        /// <summary>
        /// 重写OnPreviewKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Handled)
            {
                //已验证,不再验证
                return;
            }

            if (this._lastPressDownCtrl)
            {
                switch (e.Key)
                {
                    case Key.C:
                        this.CopyCommand_Executed(this, null);
                        break;
                    case Key.V:
                        this.PasteCommand_Executed(this, null);
                        break;
                    default:
                        break;
                }

                this._lastPressDownCtrl = false;
                return;
            }


            switch (e.Key)
            {
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                    return;
                case Key.Back:
                    if (this.SelectionLength > 0)
                    {
                        if (this.SelectionLength == 1 && this.IsNoNumberChar(this.SelectedText[0]))
                        {
                            this.SelectionLength = 0;
                        }
                        else
                        {
                            this._textChangedKey = _REMOVE_SELECTION;
                            int removeStartIndex = this.SelectionStart - 1;
                            int removeLength = this.SelectionLength;
                            while (removeStartIndex >= 0 && this.IsNoNumberChar(this.Text[removeStartIndex]))
                            {
                                removeLength++;
                                removeStartIndex--;
                            }
                            removeStartIndex += 1;
                            this.Text = this.Text.Remove(removeStartIndex, removeLength);
                            this.SelectionStart = removeStartIndex;
                            this._textChangedKey = _NONE;
                            this.FormatEditContent();
                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        int cursorPreCharIndex = this.SelectionStart - 1;
                        while (cursorPreCharIndex < this.Text.Length &&
                            cursorPreCharIndex >= 0 && this.IsNoNumberChar(this.Text[cursorPreCharIndex]))
                        {
                            cursorPreCharIndex--;
                        }

                        if (cursorPreCharIndex < this.SelectionStart - 1)
                        {
                            this.SelectionStart = cursorPreCharIndex + 1;
                        }

                        this._textChangedKey = e.Key;
                        return;
                    }
                    break;
                case Key.Delete:
                    if (this.SelectionLength > 0)
                    {
                        if (this.SelectionLength == 1 && this.IsNoNumberChar(this.SelectedText[0]))
                        {
                            int selectionStart = this.SelectionStart + 1;
                            if (selectionStart > this.Text.Length)
                            {
                                selectionStart = this.Text.Length - 1;
                            }

                            this.SelectionStart = selectionStart;
                            this.SelectionLength = 0;
                        }
                        else
                        {
                            this._textChangedKey = _REMOVE_SELECTION;
                            int removeStartIndex = this.SelectionStart - 1;
                            int removeLength = this.SelectionLength;
                            while (removeStartIndex >= 0 && this.IsNoNumberChar(this.Text[removeStartIndex]))
                            {
                                removeLength++;
                                removeStartIndex--;
                            }
                            removeStartIndex += 1;
                            this.Text = this.Text.Remove(removeStartIndex, removeLength);
                            this.SelectionStart = removeStartIndex;
                            this._textChangedKey = _NONE;
                            this.FormatEditContent();
                            e.Handled = true;
                            return;
                        }
                    }
                    else
                    {
                        int cursorNextCharIndex = this.SelectionStart;
                        while (cursorNextCharIndex < this.Text.Length &&
                            cursorNextCharIndex >= 0 && this.IsNoNumberChar(this.Text[cursorNextCharIndex]))
                        {
                            cursorNextCharIndex++;
                        }

                        if (cursorNextCharIndex < this.Text.Length && cursorNextCharIndex > this.SelectionStart)
                        {
                            this.SelectionStart = cursorNextCharIndex;
                        }

                        this._textChangedKey = e.Key;
                        return;
                    }
                    break;
                case Key.Enter:
                    this.CommitValue();
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    this._lastPressDownCtrl = true;
                    return;
            }

            if (_allowInputKeyArr == null)
            {
                _numberKeyArr = new Key[]
                {
                    Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,  //小键盘数字
                    Key.D0,Key.D1,Key.D2,Key.D3,Key.D4,Key.D5,Key.D6,Key.D7,Key.D8,Key.D9,  //数字
                };
                _allowInputKeyArr = new Key[]
                {
                    Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9,  //小键盘数字
                    Key.D0,Key.D1,Key.D2,Key.D3,Key.D4,Key.D5,Key.D6,Key.D7,Key.D8,Key.D9,  //数字
                    Key.OemPeriod,Key.Decimal, //小数点
                    Key.OemMinus, Key.Subtract,  //减号
                    Key.Home,Key.End
                };
            }

            if (!_allowInputKeyArr.Contains(e.Key))
            {
                e.Handled = true;
                return;
            }

            if (this.SelectionLength > 0)
            {
                if (_numberKeyArr.Contains(e.Key) && this.SelectionLength == 1 && this.IsNoNumberChar(this.SelectedText[0]))
                {
                    this.SelectionLength = 0;
                    e.Handled = true;
                    return;
                }

                if (this.SelectionLength == this.Text.Length)
                {
                    this.Text = string.Empty;
                }
                else
                {
                    this._textChangedKey = _REMOVE_SELECTION;
                    var selectionStart = this.SelectionStart;
                    this.Text = this.Text.Remove(selectionStart, this.SelectionLength);
                    this.SelectionStart = selectionStart;
                    this._textChangedKey = _NONE;
                }
            }

            this._textChangedKey = e.Key;
        }

        private bool IsNoNumberChar(char ch)
        {
            return ch != '.' && ch < 48 || ch > 57;
        }


        private void CommitValue()
        {
            if (!this._valueNoComit)
            {
                return;
            }

            double value;
            string text = this.GetValueText(this.Text);

            if (string.IsNullOrWhiteSpace(text))
            {
                if (this.AllowNaN)
                {
                    value = double.NaN;
                }
                else
                {
                    value = this.Value;
                }
            }
            else
            {
                if (text.EndsWith(_POINT))
                {
                    text = text.Substring(0, text.Length - 1);
                }

                if (double.TryParse(text, out value))
                {
                    if (value - this.Minimum < this._precision)
                    {
                        value = this.Minimum;
                    }
                    else if (value - this.Maximum > this._precision)
                    {
                        value = this.Maximum;
                    }
                }
                else
                {
                    value = this.Value;
                }
            }

            if (double.IsNaN(this.Value) && double.IsNaN(value) ||
               Math.Abs(this.Value - value) <= this._precision)
            {
                this.UpdateTextByValue();
            }
            else
            {
                this.Value = value;
            }

            this._valueNoComit = false;
        }

        private string GetValueText(string srcText)
        {
            if (this.Converter == null)
            {
                return srcText;
            }

            object result = this.Converter.ConvertBack(srcText, null, null, null);
            if (result != null)
            {
                return result.ToString();
            }
            else
            {
                return string.Empty;
            }
        }





        private static string[] _allowInputStrArr = null;
        /// <summary>
        /// 重写OnPreviewTextInput
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            if (e.Handled)
            {
                //已验证,不再验证
                return;
            }

            if (_allowInputStrArr == null)
            {
                _allowInputStrArr = new string[]
                {
                    "-",".","0","1","2","3","4","5","6","7","8","9"
                };
            }

            if (!_allowInputStrArr.Contains(e.Text))
            {
                //输入了非数字相关的字符
                e.Handled = true;
                return;
            }

            if (this.Maximum >= this._precision)
            {
                if (this.Minimum >= this._precision)
                {
                    //皆为非负数
                    this.OnPreviewTextInput_AllGreaterOrEqualToZero(e);
                }
                else
                {
                    //最大值非负数,最小值负数
                    this.OnPreviewTextInput_MinimumLessThanZero(e);
                }
            }
            else
            {
                //皆为负数
                this.OnPreviewTextInput_AllLessThanZero(e);
            }
        }

        /// <summary>
        /// 皆为负数
        /// </summary>
        /// <param name="e"></param>
        private void OnPreviewTextInput_AllLessThanZero(TextCompositionEventArgs e)
        {
            int decimalPlaces = this.DecimalWidth;
            if (decimalPlaces < _DECIMAL_PLACES_MIN && string.Equals(e.Text, _POINT))
            {
                //小数位数为0,输入了小数点
                e.Handled = true;
                return;
            }

            string text = this.GetPreviewTextInputText(e);
            text = this.GetValueText(text);

            if (string.IsNullOrWhiteSpace(text) || string.Equals(text, _SUBTRACT))
            {
                //没有值呀只有减号,不验证
                return;
            }

            if (!text.StartsWith(_SUBTRACT))
            {
                //不以减号开始
                e.Handled = true;
                return;
            }

            if (text.IndexOf(_SUBTRACT) != text.LastIndexOf(_SUBTRACT))
            {
                //输入了多个减号
                e.Handled = true;
                return;
            }

            if (text.StartsWith(_POINT))
            {
                //以小数点开始
                e.Handled = true;
                return;
            }

            if (text.Length >= 2 && text[0] == _ZEROR && text[1] == _ZEROR)
            {
                //起始输入多个0
                e.Handled = true;
                return;
            }

            double value;
            int decimalPointPostion = text.IndexOf(_POINT);
            if (decimalPointPostion > 0)
            {
                if (decimalPointPostion != text.LastIndexOf(_POINT))
                {
                    //多个小数点
                    e.Handled = true;
                    return;
                }

                if (text.EndsWith(_POINT))
                {
                    if (this.SelectionStart == this.Text.Length)
                    {
                        if (!double.TryParse(this.Text, out value))
                        {
                            //无效值
                            e.Handled = true;
                            return;
                        }

                        if (value - this.Minimum <= this._precision)
                        {
                            //整数部分已经为最小值
                            e.Handled = true;
                            return;
                        }
                        else if (value - this.Maximum > this._precision)
                        {
                            //整数部分大于最大值
                            e.Handled = true;
                            return;
                        }
                    }

                    //以小数结尾,其它不验证
                    return;
                }

                if (text.Length - decimalPointPostion - 1 > decimalPlaces)
                {
                    //小数位数超过设定值
                    e.Handled = true;
                    return;
                }
            }

            if (!double.TryParse(text, out value))
            {
                //无效值
                e.Handled = true;
                return;
            }

            if (value - this.Minimum < this._precision)
            {
                //值小于最小值
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// 最大值非负数,最小值负数
        /// </summary>
        /// <param name="e"></param>
        private void OnPreviewTextInput_MinimumLessThanZero(TextCompositionEventArgs e)
        {
            int decimalPlaces = this.DecimalWidth;
            if (decimalPlaces < _DECIMAL_PLACES_MIN && string.Equals(e.Text, _POINT))
            {
                //小数位数为0,输入了小数点
                e.Handled = true;
                return;
            }

            string text = this.GetPreviewTextInputText(e);
            text = this.GetValueText(text);

            if (string.IsNullOrWhiteSpace(text) || string.Equals(text, _SUBTRACT))
            {
                //没有值呀只有减号,不验证
                return;
            }

            if (text.IndexOf(_SUBTRACT) != text.LastIndexOf(_SUBTRACT))
            {
                //输入了多个减号
                e.Handled = true;
                return;
            }

            if (text.StartsWith(_POINT))
            {
                //以小数点开始
                e.Handled = true;
                return;
            }

            if (text.Length >= 2 && text[0] == _ZEROR && text[1] == _ZEROR)
            {
                //起始输入多个0
                e.Handled = true;
                return;
            }

            double value;
            int decimalPointPostion = text.IndexOf(_POINT);
            if (decimalPointPostion > 0)
            {
                if (decimalPointPostion != text.LastIndexOf(_POINT))
                {
                    //多个小数点
                    e.Handled = true;
                    return;
                }

                if (text.EndsWith(_POINT))
                {
                    if (this.SelectionStart == this.Text.Length)
                    {
                        if (!double.TryParse(this.Text, out value))
                        {
                            //无效值
                            e.Handled = true;
                            return;
                        }

                        if (value - this.Maximum >= this._precision)
                        {
                            //整数部分大于最大值
                            e.Handled = true;
                            return;
                        }

                        if (value - this.Minimum <= this._precision)
                        {
                            //整数部分已经为最小值
                            e.Handled = true;
                            return;
                        }
                    }

                    //以小数结尾,其它不验证
                    return;
                }

                if (text.Length - decimalPointPostion - 1 > decimalPlaces)
                {
                    //小数位数超过设定值
                    e.Handled = true;
                    return;
                }
            }

            if (!double.TryParse(text, out value))
            {
                //无效值
                e.Handled = true;
                return;
            }

            if (value < this._precision)
            {
                if (value - this.Minimum < this._precision)
                {
                    //小于最小值
                    e.Handled = true;
                    return;
                }
            }
            else
            {
                if (value - this.Maximum > this._precision)
                {
                    //大于最大值
                    e.Handled = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 皆为非负数
        /// </summary>
        /// <param name="e"></param>
        private void OnPreviewTextInput_AllGreaterOrEqualToZero(TextCompositionEventArgs e)
        {
            if (string.Equals(e.Text, _SUBTRACT))
            {
                //输入了减号
                e.Handled = true;
                return;
            }

            int decimalPlaces = this.DecimalWidth;
            if (decimalPlaces < _DECIMAL_PLACES_MIN && string.Equals(e.Text, _POINT))
            {
                //小数位数为0,输入了小数点
                e.Handled = true;
                return;
            }

            string text = this.GetPreviewTextInputText(e);
            text = this.GetValueText(text);

            if (string.IsNullOrWhiteSpace(text))
            {
                //没有值,不验证
                return;
            }

            if (text.StartsWith(_POINT))
            {
                //以小数点开始
                e.Handled = true;
                return;
            }

            if (text.Length >= 2 && text[0] == _ZEROR && text[1] == _ZEROR && this.IntegerWidth == 0)
            {
                //起始输入多个0
                e.Handled = true;
                return;
            }

            double value;
            int decimalPointPostion = text.IndexOf(_POINT);
            if (decimalPointPostion > 0)
            {
                if (decimalPointPostion != text.LastIndexOf(_POINT))
                {
                    //多个小数点
                    e.Handled = true;
                    return;
                }

                if (text.EndsWith(_POINT))
                {
                    if (this.SelectionStart == this.Text.Length)
                    {
                        if (!double.TryParse(this.Text, out value))
                        {
                            //无效值
                            e.Handled = true;
                            return;
                        }

                        if (value - this.Minimum < this._precision)
                        {
                            //整数部分已经为最小值
                            e.Handled = true;
                            return;
                        }
                        else if (value - this.Maximum >= this._precision)
                        {
                            //整数部分大于最大值
                            e.Handled = true;
                            return;
                        }
                    }

                    //以小数结尾,其它不验证
                    return;
                }

                if (decimalPlaces > 0 && text.Length - decimalPointPostion - 1 > decimalPlaces)
                {
                    //小数位数超过设定值
                    e.Handled = true;
                    return;
                }
            }

            if (!double.TryParse(text, out value))
            {
                //无效值
                e.Handled = true;
                return;
            }

            if (value - this.Maximum > this._precision)
            {
                //大于最大值
                e.Handled = true;
                return;
            }
        }

        private string GetPreviewTextInputText(TextCompositionEventArgs e)
        {
            return this.Text.Insert(this.SelectionStart, e.Text);
        }





        /// <summary>
        /// 重写OnTextChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (this._setShowText)
            {
                return;
            }

            this._valueNoComit = true;
            if (this._textChangedKey == _REMOVE_SELECTION || this._textChangedKey == _NONE)
            {
                return;
            }

            this.FormatEditContent();
            this._textChangedKey = _NONE;
        }

        private void FormatEditContent()
        {
            if (this.Converter != null)
            {
                int cursorRightContentLenth = this.Text.Length - this.SelectionStart;
                object obj = this.Converter.ConvertBack(this.Text, null, null, null);
                object result = this.Converter.Convert(obj, null, null, null);
                if (result != null)
                {
                    this.Text = result.ToString();
                    int selectionStart = this.Text.Length - cursorRightContentLenth;
                    if (selectionStart < 0)
                    {
                        selectionStart = 0;
                    }

                    this.SelectionStart = selectionStart;
                }
                else
                {
                    this.Text = string.Empty;
                }
            }
        }
    }



    internal enum MinimumMaximumType
    {
        /// <summary>
        /// 最大最小值皆为非负数
        /// </summary>
        AllGreaterOrEqualToZero,

        /// <summary>
        /// 最小值负数,最大值非负数
        /// </summary>
        MinimumLessThanZero,

        /// <summary>
        /// 最大最小值皆为负数
        /// </summary>
        AllLessThanZero
    }


    /// <summary>
    /// number控件值改变事件参数
    /// </summary>
    public class NumberValueChangedArgs : EventArgs
    {
        /// <summary>
        /// 旧值
        /// </summary>
        public double OldValue { get; private set; }

        /// <summary>
        /// 新值
        /// </summary>
        public double NewValue { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        public NumberValueChangedArgs(double oldValue, double newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
