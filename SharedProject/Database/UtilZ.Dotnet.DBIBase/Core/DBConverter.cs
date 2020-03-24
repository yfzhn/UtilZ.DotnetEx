using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.DBIBase.Model;

namespace UtilZ.Dotnet.DBIBase.Core
{
    /// <summary>
    /// 数据库类型转换类
    /// </summary>
    public class DBConverter
    {
        /// <summary>
        /// 将数据库字段值转换为clr类型值
        /// </summary>
        /// <typeparam name="T">clr类型</typeparam>
        /// <param name="obj">数据库字段值</param>
        /// <returns>clr类型值</returns>
        public static T? ConvertToDBValue<T>(object obj) where T : struct, IConvertible
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }

            T value;
            if (obj is T)
            {
                value = (T)obj;
            }
            else
            {
                value = (T)Convert.ChangeType(obj, typeof(T));
            }

            return value;
        }

        /// <summary>
        /// 将数据库字段值转换为byte
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static byte? ToByte(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is byte)
            {
                return (byte)obj;
            }
            else
            {
                return Convert.ToByte(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为short
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static short? ToInt16(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is short)
            {
                return (short)obj;
            }
            else
            {
                return Convert.ToInt16(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为ushort
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static ushort? ToUInt16(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is ushort)
            {
                return (ushort)obj;
            }
            else
            {
                return Convert.ToUInt16(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为int
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static int? ToInt32(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is int)
            {
                return (int)obj;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为uint
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static uint? ToUInt32(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is uint)
            {
                return (uint)obj;
            }
            else
            {
                return Convert.ToUInt32(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为long
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static long? ToInt64(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is long)
            {
                return (long)obj;
            }
            else
            {
                return Convert.ToInt64(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为ulong
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static ulong? ToUInt64(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is ulong)
            {
                return (ulong)obj;
            }
            else
            {
                return Convert.ToUInt64(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为float
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <param name="digits">转换精度,小于等于0为数据库中的全值</param>
        /// <returns>转换结果</returns>
        public static float? ToSingle(object obj, int digits = DBConstant.DIGITS)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            float value;
            if (obj is float)
            {
                value = (float)obj;
            }
            else
            {
                value = Convert.ToSingle(obj);
            }

            if (digits >= 0)
            {
                value = (float)Math.Round((double)value, digits);
            }

            return value;
        }

        /// <summary>
        /// 将数据库字段值转换为double
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <param name="digits">转换精度,小于等于0为数据库中的全值</param>
        /// <returns>转换结果</returns>
        public static double? ToDouble(object obj, int digits = DBConstant.DIGITS)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            double value;
            if (obj is double)
            {
                value = (double)obj;
            }
            else
            {
                value = Convert.ToDouble(obj);
            }

            if (digits >= 0)
            {
                value = Math.Round(value, digits);
            }

            return value;
        }

        /// <summary>
        /// 将数据库字段值转换为decimal
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <param name="digits">转换精度,小于等于0为数据库中的全值</param>
        /// <returns>转换结果</returns>
        public static decimal? ToDecimal(object obj, int digits = DBConstant.DIGITS)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            decimal value;
            if (obj is decimal)
            {
                value = (decimal)obj;
            }
            else
            {
                value = Convert.ToDecimal(obj);
            }

            if (digits >= 0)
            {
                value = Math.Round(value, digits);
            }

            return value;
        }

        /// <summary>
        /// 将数据库字段值转换为DateTime
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static DateTime? ToDateTime(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            else
            {
                return Convert.ToDateTime(obj);
            }
        }

        /// <summary>
        /// 将数据库字段值转换为String
        /// </summary>
        /// <param name="obj">数据库字段值</param>
        /// <returns>转换结果</returns>
        public static string ToString(object obj)
        {
            if (obj == DBNull.Value)
            {
                return null;
            }

            if (obj is string)
            {
                return (string)obj;
            }
            else
            {
                return obj.ToString();
            }
        }








        /// <summary>
        /// 以地浮点数做四舍五入运算
        /// </summary>
        /// <typeparam name="T">clr类型</typeparam>
        /// <param name="value">目标值</param>
        /// <param name="digits">浮点数据四舍五入运算,当T为浮点数时且digits大于等于0做此运算</param>
        /// <returns>结果值</returns>
        public static T Round<T>(T value, int digits = DBConstant.DIGITS) where T : struct, IConvertible
        {
            if (digits < 0)
            {
                return value;
            }

            Type type = typeof(T);
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    value = (T)Convert.ChangeType(Math.Round(Convert.ToDouble(value), digits), type);
                    break;
                case TypeCode.Decimal:
                    value = (T)Convert.ChangeType(Math.Round(Convert.ToDecimal(value), digits), type);
                    break;
            }

            return value;
        }






        /// <summary>
        /// 将clr类型值转换为数据库字段值
        /// </summary>
        /// <typeparam name="T">clr类型</typeparam>
        /// <param name="value">clr类型值</param>        
        /// <returns>数据库字段值</returns>
        public static object GetDBValue<T>(T? value) where T : struct, IConvertible
        {
            if (value.HasValue)
            {
                return value.Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        /// <summary>
        /// 将clr类型值转换为数据库字段值
        /// </summary>
        /// <typeparam name="T">clr类型</typeparam>
        /// <param name="value">clr类型值</param>
        /// <returns>数据库字段值</returns>
        public static object GetDBValue(object value)
        {
            if (value != null)
            {
                return value;
            }
            else
            {
                return DBNull.Value;
            }
        }
    }
}
