using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 日期时间显示格式化器
    /// </summary>
    public class DateTimeFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// 日期转换器实例
        /// </summary>
        private readonly static DateTimeFormatProvider _instance = new DateTimeFormatProvider();

        /// <summary>
        /// 获取日期转换器实例
        /// </summary>
        public static DateTimeFormatProvider Instance
        {
            get { return DateTimeFormatProvider._instance; }
        } 

        /// <summary>
        /// 构造函数
        /// </summary>
        public DateTimeFormatProvider()
        {

        }

        /// <summary>
        /// 获取格式类型
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 格式转换
        /// </summary>
        /// <param name="format">转换格式字符串</param>
        /// <param name="arg">要转换的参数</param>
        /// <param name="formatProvider">转换器</param>
        /// <returns>转换后的字符串</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return string.Empty;
            }
            else if (arg is DateTime)
            {
                if (string.IsNullOrEmpty(format))
                {
                    if (formatProvider == null)
                    {
                        return ((DateTime)arg).ToString(@"yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        return ((DateTime)arg).ToString(formatProvider);
                    }
                }
                else
                {
                    return ((DateTime)arg).ToString(format);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
