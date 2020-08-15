using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 数量与Bool转换接口
    /// </summary>
    public class CountToVisConverter : ValueConverterAbs
    {
        /// <summary>
        /// 构造函数 
        /// </summary>
        public CountToVisConverter()
        {

        }

        /// <summary>
        /// 将后台对象转换为显示的对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            long count = (long)value;
            if (count > 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
