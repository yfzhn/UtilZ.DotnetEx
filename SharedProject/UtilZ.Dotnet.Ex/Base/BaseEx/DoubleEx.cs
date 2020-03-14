using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Double扩展方法
    /// </summary>
    public class DoubleEx
    {
        /// <summary>
        /// 判断double值是否有效[有效返回true;无效返回false]
        /// </summary>
        /// <param name="value">目标值</param>
        /// <returns>有效返回true;无效返回false</returns>
        public static bool Valid(double value)
        {
            if (double.IsInfinity(value) ||
                double.IsNaN(value) ||
                double.IsPositiveInfinity(value) ||
                double.IsNegativeInfinity(value))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 将object转换为double,转换失败返回double.NaN
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <returns>转换结果</returns>
        public static double ConvertToDouble(object obj)
        {
            if (obj == null)
            {
                return double.NaN;
            }

            double value;
            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                try
                {
                    value = Convert.ToDouble(obj);
                }
                catch
                {
                    value = double.NaN;
                }
            }

            return value;
        }
    }
}
