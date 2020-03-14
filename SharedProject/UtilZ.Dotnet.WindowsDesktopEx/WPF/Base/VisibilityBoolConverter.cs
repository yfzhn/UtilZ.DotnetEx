using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Visibility与Bool转换接口
    /// </summary>
    public class VisibilityBoolConverter : ValueConverterAbs
    {
        /// <summary>
        /// 构造函数 
        /// </summary>
        public VisibilityBoolConverter()
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
                return false;
            }

            var visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 将显示的对象转换为后台对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            if ((bool)value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
    }
}
