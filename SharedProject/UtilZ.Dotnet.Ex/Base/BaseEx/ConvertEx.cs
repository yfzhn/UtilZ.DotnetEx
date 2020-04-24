using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Convert类型扩展方法类
    /// </summary>
    public static class ConvertEx
    {
        #region 类型转换
        /// <summary>
        /// 转换数据到
        /// </summary>
        /// <typeparam name="T">要待转换的目标类型</typeparam>
        /// <typeparam name="TResult">目标类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值,存放在object中,如果转换失败为目标类型的默认值</returns>
        public static TResult ConvertTo<T, TResult>(T value) where T : IConvertible
        {
            return (TResult)ConvertToObject(typeof(TResult), value);
        }

        /// <summary>
        /// 尝试转换数据到
        /// </summary>
        /// <typeparam name="T">要待转换的目标类型</typeparam>
        /// <typeparam name="TResult">目标类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryConvertTo<T, TResult>(T value, out TResult result) where T : IConvertible
        {
            object obj;
            bool ret = TryConvertToObject(typeof(TResult), value, out obj);
            if (ret)
            {
                result = (TResult)obj;
            }
            else
            {
                result = default(TResult);
            }

            return ret;
        }

        /// <summary>
        /// 转换数据到
        /// </summary>
        /// <param name="targetValueType">要待转换的目标类型</param>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值,存放在object中,如果转换失败为目标类型的默认值</returns>
        public static object ConvertToObject(Type targetValueType, object value)
        {
            object resultValue = null;
            if (value != null)
            {
                if (targetValueType.IsEnum)
                {
                    resultValue = Enum.Parse(targetValueType, value.ToString(), true);
                }
                else
                {
                    resultValue = System.Convert.ChangeType(value, targetValueType);
                }
            }
            else
            {
                resultValue = value;
            }

            return resultValue;
        }

        /// <summary>
        /// 尝试转换数据到指定类型
        /// </summary>
        /// <param name="targetValueType">要待转换的目标类型</param>
        /// <param name="value">要转换的值</param>
        /// <param name="result">转换后的值</param>
        /// <returns></returns>
        public static bool TryConvertToObject(Type targetValueType, object value, out object result)
        {
            result = null;
            if (value == null)
            {
                return false;
            }

            try
            {
                if (targetValueType.IsEnum)
                {
                    result = Enum.Parse(targetValueType, value.ToString(), true);
                    return true;
                }

                result = System.Convert.ChangeType(value, targetValueType);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 字符串转换为指定类型数值类型
        /// <summary>
        /// 字符串转换为指定类型数值类型
        /// </summary>
        /// <typeparam name="T">目标数值类型</typeparam>
        /// <param name="value">待转换字符串</param>
        /// <param name="fromBase">值中数字基数(value是何种进制的字符串),必须是2,8,10,16</param>
        /// <returns>目标数值</returns>
        public static T ConvertTo<T>(string value, byte fromBase = 10)
        {
            return (T)ConvertTo(typeof(T), value, fromBase);
        }

        /// <summary>
        /// 各种进制字符串转换为数据类型,转换失败返回默认值,包括枚举
        /// </summary>
        /// <typeparam name="T">目标数值类型</typeparam>
        /// <param name="value">要转换的值</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <param name="fromBase">值中数字基数(value是何种进制的字符串),必须是2,8,10,16</param>
        /// <param name="formatProvider">一个提供区域性特定的格式设置信息的对象</param>
        /// <returns>转换后的值,存放在object中,如果转换失败为目标类型的默认值</returns>
        public static T ConvertTo<T>(string value, T defaultValue, byte fromBase = 10, IFormatProvider formatProvider = null)
        {
            T resultValue;
            try
            {
                resultValue = (T)ConvertTo(typeof(T), value, fromBase, formatProvider);
            }
            catch
            {
                resultValue = defaultValue;
            }

            return resultValue;
        }

        /// <summary>
        /// 将字符串转换为值类型数据,包括枚举
        /// </summary>
        /// <param name="targetValueType">要待转换的目标类型</param>
        /// <param name="value">要转换的值</param>
        /// <param name="fromBase">值中数字基数(value是何种进制的字符串),必须是2,8,10,16</param>
        /// <param name="formatProvider">一个提供区域性特定的格式设置信息的对象</param>
        /// <returns>转换后的值,存放在object中,如果转换失败为目标类型的默认值</returns>
        public static object ConvertTo(Type targetValueType, string value, byte fromBase = 10, IFormatProvider formatProvider = null)
        {
            if (targetValueType == null)
            {
                throw new ArgumentNullException(nameof(targetValueType));
            }

            if (!targetValueType.IsValueType)
            {
                throw new ArgumentException(string.Format("目标类型:{0}不是值类型", targetValueType.FullName), nameof(targetValueType));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (targetValueType.IsEnum)
            {
                return Enum.Parse(targetValueType, value, true);
            }

            if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase == 16)
            {
                throw new ArgumentException("值中数字基数(value是何种进制的字符串),必须是2,8,10,16", nameof(fromBase));
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            object resultValue;
            TypeCode code = Type.GetTypeCode(targetValueType);
            switch (code)
            {
                case TypeCode.Boolean:
                    resultValue = System.Convert.ToBoolean(value, formatProvider);
                    break;
                case TypeCode.Char:
                    resultValue = System.Convert.ToChar(value, formatProvider);
                    break;
                case TypeCode.SByte:
                    resultValue = System.Convert.ToSByte(value, fromBase);
                    break;
                case TypeCode.Byte:
                    resultValue = System.Convert.ToByte(value, fromBase);
                    break;
                case TypeCode.Int16:
                    resultValue = System.Convert.ToInt16(value, fromBase);
                    break;
                case TypeCode.UInt16:
                    resultValue = System.Convert.ToUInt16(value, fromBase);
                    break;
                case TypeCode.Int32:
                    resultValue = System.Convert.ToInt32(value, fromBase);
                    break;
                case TypeCode.UInt32:
                    resultValue = System.Convert.ToUInt32(value, fromBase);
                    break;
                case TypeCode.Int64:
                    resultValue = System.Convert.ToInt64(value, fromBase);
                    break;
                case TypeCode.UInt64:
                    resultValue = System.Convert.ToUInt64(value, fromBase);
                    break;
                case TypeCode.Decimal:
                    resultValue = System.Convert.ToDecimal(value, formatProvider);
                    break;
                case TypeCode.Double:
                    resultValue = System.Convert.ToDouble(value, formatProvider);
                    break;
                case TypeCode.Single:
                    resultValue = System.Convert.ToSingle(value, formatProvider);
                    break;
                case TypeCode.DateTime:
                    resultValue = System.Convert.ToDateTime(value, formatProvider);
                    break;
                case TypeCode.String:
                    resultValue = value;
                    break;
                default:
                    throw new NotSupportedException(string.Format("不支持类型:{0}", targetValueType.FullName));
            }

            return resultValue;
        }

        /// <summary>
        /// 尝试将字符串转换为值类型数据,包括枚举
        /// </summary>
        /// <param name="targetValueType">要待转换的目标类型</param>
        /// <param name="value">要转换的值</param>
        /// <param name="result">转换结果</param>
        /// <returns>转换后的值,存放在object中,如果转换失败为目标类型的默认值</returns>
        public static bool TryConvertTo(Type targetValueType, string value, out object result)
        {
            result = null;
            if (targetValueType == null)
            {
                return false;
            }

            if (!targetValueType.IsValueType)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (targetValueType.IsEnum)
            {
                try
                {
                    result = Enum.Parse(targetValueType, value, true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            IConvertible convertible = value as IConvertible;
            if (convertible == null)
            {
                return false;
            }

            bool resultValue;
            TypeCode code = Type.GetTypeCode(targetValueType);
            switch (code)
            {
                case TypeCode.Boolean:
                    bool boolResult;
                    resultValue = bool.TryParse(value, out boolResult);
                    if (resultValue)
                    {
                        result = boolResult;
                    }
                    break;
                case TypeCode.Char:
                    char charResult;
                    resultValue = char.TryParse(value, out charResult);
                    if (resultValue)
                    {
                        result = charResult;
                    }
                    break;
                case TypeCode.SByte:
                    sbyte sbyteResult;
                    resultValue = sbyte.TryParse(value, out sbyteResult);
                    if (resultValue)
                    {
                        result = sbyteResult;
                    }
                    break;
                case TypeCode.Byte:
                    byte byteResult;
                    resultValue = byte.TryParse(value, out byteResult);
                    if (resultValue)
                    {
                        result = byteResult;
                    }
                    break;
                case TypeCode.Int16:
                    Int16 int16Result;
                    resultValue = Int16.TryParse(value, out int16Result);
                    if (resultValue)
                    {
                        result = int16Result;
                    }
                    break;
                case TypeCode.UInt16:
                    UInt16 uint16Result;
                    resultValue = UInt16.TryParse(value, out uint16Result);
                    if (resultValue)
                    {
                        result = uint16Result;
                    }
                    break;
                case TypeCode.Int32:
                    Int32 int32Result;
                    resultValue = Int32.TryParse(value, out int32Result);
                    if (resultValue)
                    {
                        result = int32Result;
                    }
                    break;
                case TypeCode.UInt32:
                    UInt32 uint32Result;
                    resultValue = UInt32.TryParse(value, out uint32Result);
                    if (resultValue)
                    {
                        result = uint32Result;
                    }
                    break;
                case TypeCode.Int64:
                    Int64 int64Result;
                    resultValue = Int64.TryParse(value, out int64Result);
                    if (resultValue)
                    {
                        result = int64Result;
                    }
                    break;
                case TypeCode.UInt64:
                    UInt64 uint64Result;
                    resultValue = UInt64.TryParse(value, out uint64Result);
                    if (resultValue)
                    {
                        result = uint64Result;
                    }
                    break;
                case TypeCode.Decimal:
                    decimal decimalResult;
                    resultValue = decimal.TryParse(value, out decimalResult);
                    if (resultValue)
                    {
                        result = decimalResult;
                    }
                    break;
                case TypeCode.Double:
                    double doubleResult;
                    resultValue = double.TryParse(value, out doubleResult);
                    if (resultValue)
                    {
                        result = doubleResult;
                    }
                    break;
                case TypeCode.Single:
                    Single singleResult;
                    resultValue = Single.TryParse(value, out singleResult);
                    if (resultValue)
                    {
                        result = singleResult;
                    }
                    break;
                case TypeCode.DateTime:
                    DateTime dateTimeResult;
                    resultValue = DateTime.TryParse(value, out dateTimeResult);
                    if (resultValue)
                    {
                        result = dateTimeResult;
                    }
                    break;
                case TypeCode.String:
                    result = value;
                    resultValue = true;
                    break;
                default:
                    try
                    {
                        result = convertible.ToType(targetValueType, CultureInfo.CurrentCulture);
                        resultValue = true;
                    }
                    catch
                    {
                        resultValue = false;
                    }
                    break;
            }

            return resultValue;
        }
        #endregion

        /// <summary>
        /// 获得类型默认值
        /// </summary>
        /// <param name="targetType">要获取默认值的目标类型</param>
        /// <returns>类型默认值</returns>
        public static object GetTypeDefaultValue(Type targetType)
        {
            object defaultValue;
            if (targetType.IsValueType)
            {
                defaultValue = Activator.CreateInstance(targetType);
            }
            else
            {
                defaultValue = null;
            }

            return defaultValue;
        }
    }
}
