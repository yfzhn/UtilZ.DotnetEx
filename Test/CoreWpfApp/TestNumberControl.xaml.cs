using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoreWpfApp
{
    /// <summary>
    /// TestNumberControl.xaml 的交互逻辑
    /// </summary>
    public partial class TestNumberControl : Window
    {
        public TestNumberControl()
        {
            InitializeComponent();
        }

        private void NumberControl_ValueChanged(object sender, UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.NumberValueChangedArgs e)
        {
            txt.Text = e.NewValue.ToString();
        }
    }

    internal class TestNumberControlVM : UtilZ.Dotnet.Ex.Base.NotifyPropertyChangedAbs
    {
        //private long _freq = 123456789L;
        private long _freq = 95500000L;
        /// <summary>
        /// 频率
        /// </summary>
        public long Freq
        {
            get
            {
                return _freq;
            }
            set
            {
                _freq = value;
                base.OnRaisePropertyChanged(nameof(Freq));
            }
        }

        private double _freq2 = 95.500000d;
        /// <summary>
        /// 频率
        /// </summary>
        public double Freq2
        {
            get
            {
                return _freq2;
            }
            set
            {
                _freq2 = value;
                base.OnRaisePropertyChanged(nameof(Freq2));
            }
        }

        public TestNumberControlVM()
        {

        }

    }

    [ValueConversion(typeof(long), typeof(string))]
    internal class FreqConverter : IValueConverter
    {
        public FreqConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            long freq = System.Convert.ToInt64(value);
            string freqText = string.Format("{0:N0}", freq);
            return freqText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return text.Replace(",", string.Empty);
        }
    }
}
