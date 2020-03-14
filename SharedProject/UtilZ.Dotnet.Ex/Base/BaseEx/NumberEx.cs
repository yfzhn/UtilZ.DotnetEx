using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 数值类型扩展方法类
    /// </summary>
    public static class NumberEx
    {
        /// <summary>
        /// 转换为十六进制字符串
        /// </summary>
        /// <typeparam name="T">具体的某种整形值类型</typeparam>
        /// <param name="value">十进制整形数值</param>
        /// <returns>0x开头的十六进制字符串</returns>
        public static string ToHexadecimalString<T>(this T value) where T : struct, IFormattable
        {
            return string.Format("0x{0}", value.ToString("x4", null));
        }

        /// <summary>
        /// 数值类型数据转换为百分比字符串
        /// </summary>
        /// <typeparam name="T">具体的某种整形值类型</typeparam>
        /// <param name="value">十进制整形数值</param>
        /// <param name="dpCount">百分比保留的小数点位数</param>
        /// <returns>百分比字符串</returns>
        public static string ToPersentString<T>(this T value, byte dpCount) where T : struct, IFormattable, IConvertible
        {
            decimal dvalue = 0M;
            if (!decimal.TryParse(value.ToString(), out dvalue))
            {
                throw new Exception(string.Format("值:{0}不是有效的数值类型数据", value));
            }

            dvalue = dvalue * 100;
            string str = dvalue.ToString();
            if (str.Contains("."))
            {
                int pLocation = str.LastIndexOf('.');
                if (dpCount > 0)
                {
                    int subLength = pLocation + dpCount + 1;
                    if (str.Length >= subLength)
                    {
                        str = str.Substring(0, subLength);
                    }
                    else
                    {
                        int appendCount = dpCount - (str.Length - pLocation - 1);
                        StringBuilder sb = new StringBuilder();
                        sb.Append(str);
                        sb.Append('0', appendCount);
                        str = sb.ToString();
                    }
                }
                else
                {
                    str = str = str.Substring(0, pLocation);
                }
            }
            else
            {
                if (dpCount > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(str);
                    sb.Append('.');
                    sb.Append('0', dpCount);
                    str = sb.ToString();
                }
            }

            return string.Format("{0}%", str);
        }

        /// <summary>
        /// 转换内存字节数为GB字符串
        /// </summary>
        /// <param name="value">字节数</param>
        /// <returns>GB字符串</returns>
        public static string ToGBString<T>(this T value) where T : struct, IFormattable, IConvertible
        {
            float size = 0;
            if (!float.TryParse(value.ToString(), out size))
            {
                throw new Exception(string.Format("值:{0}不是有效的数值类型数据", value));
            }

            if (size < 0)
            {
                throw new Exception(string.Format("值:{0}不能是负值", size));
            }

            float gbSize = (float)size / 1024 / 1024 / 1024;
            if (gbSize - (int)gbSize > 0)
            {
                gbSize = gbSize * 100;
                gbSize = (float)((int)gbSize) / 100;
            }
            return string.Format("{0}GB", gbSize);
        }
    }
}
