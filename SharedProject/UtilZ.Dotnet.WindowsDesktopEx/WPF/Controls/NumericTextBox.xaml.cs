using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF
{
    /// <summary>
    /// NumericTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class NumericTextBox : TextBox
    {
        /// <summary>
        /// 最大值依赖属性
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.RegisterAttached("Maximum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(0.0d, PropertyChanged));

        /// <summary>
        /// 最小值依赖属性
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.RegisterAttached("Minimum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(100.0d, PropertyChanged));

        /// <summary>
        /// 小数位数依赖属性
        /// </summary>
        public static readonly DependencyProperty DigitsProperty = DependencyProperty.RegisterAttached("Digits", typeof(int), typeof(NumericTextBox), new PropertyMetadata(0, PropertyChanged));

        /// <summary>
        /// 获取或设置最大值
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("最大值")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double Maximum
        {
            get
            {
                return (double)this.GetValue(MaximumProperty);
            }
            set
            {
                if (this.Minimum > value)
                {
                    value = this.Minimum;
                }

                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置最小值
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("最小值")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public double Minimum
        {
            get
            {
                return (double)this.GetValue(MinimumProperty);
            }
            set
            {
                if (this.Maximum < value)
                {
                    value = this.Maximum;
                }

                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置小数位数
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Description("小数位数")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Digits
        {
            get
            {
                return (int)this.GetValue(DigitsProperty);
            }
            set
            {
                if (value < 0)
                {
                    return;
                }

                this.SetValue(DigitsProperty, value);
            }
        }

        /// <summary>
        /// NumericTextBox依赖属性值改变事件
        /// </summary>
        /// <param name="d">依赖父控件</param>
        /// <param name="e">属性参数</param>
        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBox control = d as NumericTextBox;
            var value = Convert.ToDouble(control.Text);
            if (e.Property == MaximumProperty)
            {
                var maximum = (double)e.NewValue;
                if (value > maximum)
                {
                    control.Text = maximum.ToString();
                }
            }
            else if (e.Property == MinimumProperty)
            {
                var minimum = (double)e.NewValue;
                if (value > minimum)
                {
                    control.Text = minimum.ToString();
                }
            }
            else if (e.Property == DigitsProperty)
            {
                int digits = (int)e.NewValue;
                control.Text = Math.Round(value, digits).ToString();
            }
        }

       

        /// <summary>
        /// 构造函数
        /// </summary>
        public NumericTextBox() : base()
        {
            DataObject.AddPastingHandler(this, Text_Pasting);
            this.Text = "0";
        }

        private void Text_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var pastValue = e.DataObject.GetData(typeof(string));
            if (!(pastValue is string))
            {
                //非字符串禁止Pasting
                e.CancelCommand();
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                 e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right ||
                 (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
                 || (e.Key >= Key.D0 && e.Key <= Key.D9 && e.KeyboardDevice.Modifiers != ModifierKeys.Shift))
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
