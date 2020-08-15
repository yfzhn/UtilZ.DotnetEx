using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Converters
{
    /// <summary>
    /// 值转换基类
    /// </summary>
    public abstract class ValueConverterAbs : IValueConverter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ValueConverterAbs()
        {

        }

        /// <summary>
        /// 内存对象中转换到显示
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 显示转换回内存对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
