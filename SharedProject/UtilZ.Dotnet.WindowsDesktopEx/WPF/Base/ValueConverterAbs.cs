using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Base
{
    /// <summary>
    /// ValueConverter基类
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
        /// 获取主程序目录
        /// </summary>
        /// <returns></returns>
        protected string GetEntryAssemblyDirectory()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        /// <summary>
        /// 将后台对象转换为显示的对象
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
        /// 将显示的对象转换为后台对象
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
