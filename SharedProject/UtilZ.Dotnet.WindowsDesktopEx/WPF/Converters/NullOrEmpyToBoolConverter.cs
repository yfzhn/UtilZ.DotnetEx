using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Converters
{
    /// <summary>
    /// null或空值NullOrEmpyToBoolConverter
    /// </summary>
    public class NullOrEmpyToBoolConverter : ValueConverterAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NullOrEmpyToBoolConverter()
            : base()
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
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value is string && string.IsNullOrEmpty(((string)value)))
            {
                return false;
            }

            if (value is IEnumerable)
            {
                var enumerable = (IEnumerable)value;
                int count = 0;
                foreach (var item in enumerable)
                {
                    count++;
                }

                if (count == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
