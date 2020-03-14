using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 表示一个可用于显示或编辑数字格式文本的控件
    /// </summary>
    [ContentProperty("NumberTextBox")]
    [Localizability(LocalizationCategory.Text)]
    public class NumberTextBox : TextBox, ISupportInitialize
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NumberTextBox()
            : base()
        {
            this.WordWrap = false;
            this.Multiline = false;
        }

        /// <summary>
        /// 用信号通知对象初始化即将开始
        /// </summary>
        public void BeginInit()
        {

        }

        /// <summary>
        /// 用信号通知对象初始化已完成
        /// </summary>
        public void EndInit()
        {

        }

        private int _decimalPlaces = 0;
        /// <summary>
        /// 获取或设置数字显示框（也称作 up-down 控件）中要显示的十进制位数。默认值为 0。
        /// 异常:
        /// T:System.ArgumentOutOfRangeException:所分配的值小于 0。- 或 - 所赋的值大于 99。
        /// </summary>
        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get { return this._decimalPlaces; }
            set
            {
                if (value < 0 || value > 99)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"值{value}超出有效值[0-99]范围");
                }

                this._decimalPlaces = value;
                this.OnTextChanged(null);
            }
        }

        ///// <summary>
        ///// 获取或设置单击向上或向下按钮时，数字显示框（也称作 up-down 控件）递增或递减的值。
        ///// 异常:
        ///// T:System.ArgumentOutOfRangeException:所赋的值不大于或等于零。
        ///// </summary>
        //public decimal Increment { get; set; } = 1;

        private decimal _maximum = 100;
        /// <summary>
        /// 获取或设置数字显示框（也称作 up-down 控件）的最大值。默认值为 100。
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        public decimal Maximum
        {
            get { return this._maximum; }
            set
            {
                if (value < this._minimum)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"最大值不能小于最小{this._minimum}");
                }

                this._maximum = value;
                if (this._value > this._maximum)
                {
                    this.SetValue(value);
                }
            }
        }

        private decimal _minimum = 0;
        /// <summary>
        /// 获取或设置数字显示框（也称作 up-down 控件）的最小允许值。默认值为 0。
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        public decimal Minimum
        {
            get { return this._minimum; }
            set
            {
                if (value > this._maximum)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"最小值不能大于最大值{this._maximum}");
                }

                this._minimum = value;
                if (this._value < this._minimum)
                {
                    this.SetValue(value);
                }
            }
        }

        private decimal _value = 0;
        /// <summary>
        /// 获取或设置赋给数字显示框（也称作 up-down 控件）的值。
        /// 异常:
        /// T:System.ArgumentOutOfRangeException:所赋的值小于 System.Windows.Forms.NumericUpDown.Minimum 属性值。- 或 - 所赋的值大于 System.Windows.Forms.NumericUpDown.Maximum属性值。
        /// </summary>
        [Bindable(true)]
        public decimal Value
        {
            get { return this._value; }
            set
            {
                if (value > this._maximum || value < this._minimum)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"值{value}必须介于[{this._minimum}-{this._maximum}]之间");
                }

                this.SetValue(value);
            }
        }

        private void SetValue(decimal value)
        {
            base.Text = value.ToString();
            this.PrimitiveSetValue(value);
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler ValueChanged;

        private void PrimitiveSetValue(decimal value)
        {
            if (this._decimalPlaces > 0)
            {
                decimal pre = (decimal)(1 / Math.Pow(10, this._decimalPlaces));
                if (Math.Abs(this._value - value) < pre)
                {
                    return;
                }
            }
            else
            {
                if ((long)this._value == (long)value)
                {
                    return;
                }
            }

            this._value = value;
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void SetDefaultValue()
        {
            base.Text = this._value.ToString();
        }

        private const string _REGSTR_POINT = @"^\d+$|^\d+\.$";
        private const string _REGSTR = @"^\d+$|^\d+\.\d+$";
        private bool _flag = false;

        /// <summary>
        /// OnLostFocus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            this.FormatDigitsValue(base.Text, true);
            base.OnLostFocus(e);
        }

        /// <summary>
        /// OnTextChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            if (this._flag)
            {
                return;
            }

            this._flag = true;
            try
            {
                if (this._decimalPlaces > 0)
                {
                    if (Regex.IsMatch(base.Text, _REGSTR_POINT))
                    {
                        //以小数点结尾,忽略
                        return;
                    }
                    else
                    {
                        this.FormatDigitsValue(base.Text, false);
                    }
                }
                else
                {
                    if (!Regex.IsMatch(base.Text, _REGSTR))
                    {
                        this.SetDefaultValue();
                        return;
                    }

                    this.FormatIntegerValue(base.Text);
                }
            }
            finally
            {
                this._flag = false;
                base.OnTextChanged(e);
            }
        }

        private void FormatIntegerValue(string text)
        {
            decimal value;
            if (decimal.TryParse(base.Text, out value))
            {
                if (value < this._minimum || value > this._maximum)
                {
                    this.SetDefaultValue();
                }
                else
                {
                    base.Text = ((long)value).ToString();
                }
            }
            else
            {
                this.SetDefaultValue();
            }

            this.PrimitiveSetValue(decimal.Parse(base.Text));
        }

        private void FormatDigitsValue(string text, bool lostFocus)
        {
            decimal value;
            if (decimal.TryParse(base.Text, out value))
            {
                if (value < this._minimum || value > this._maximum)
                {
                    this.SetDefaultValue();
                }
                else
                {
                    string valueStr = value.ToString();
                    int pointIndex = valueStr.IndexOf('.');
                    if (pointIndex >= 0)
                    {
                        int len = pointIndex + this._decimalPlaces + 1;
                        if (len <= valueStr.Length)
                        {
                            base.Text = valueStr.Substring(0, len).ToString();
                        }
                        else
                        {
                            base.Text = valueStr;
                        }
                    }
                    else
                    {
                        base.Text = ((long)value).ToString();
                    }
                }
            }
            else
            {
                this.SetDefaultValue();
            }

            this.PrimitiveSetValue(decimal.Parse(base.Text));
        }
    }
}
