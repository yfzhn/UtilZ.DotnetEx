using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// CLR系统类型类
    /// </summary>
    public class ClrSystemType
    {
        #region 值类型
        /// <summary>
        /// bool类型
        /// </summary>
        public static readonly Type BoolType = typeof(bool);

        /// <summary>
        /// byte类型
        /// </summary>
        public static readonly Type ByteType = typeof(byte);

        /// <summary>
        /// char类型
        /// </summary>
        public static readonly Type CharType = typeof(char);

        /// <summary>
        /// DateTime类型
        /// </summary>
        public static readonly Type DateTimeType = typeof(DateTime);

        /// <summary>
        /// decimal类型
        /// </summary>
        public static readonly Type DecimalType = typeof(decimal);

        /// <summary>
        /// double类型
        /// </summary>
        public static readonly Type DoubleType = typeof(double);

        /// <summary>
        /// Int16类型
        /// </summary>
        public static readonly Type Int16Type = typeof(Int16);

        /// <summary>
        /// Int32类型
        /// </summary>
        public static readonly Type Int32Type = typeof(Int32);

        /// <summary>
        /// Int64类型
        /// </summary>
        public static readonly Type Int64Type = typeof(Int64);

        /// <summary>
        /// sbyte类型
        /// </summary>
        public static readonly Type SbyteType = typeof(sbyte);

        /// <summary>
        /// float类型
        /// </summary>
        public static readonly Type FloatType = typeof(float);

        /// <summary>
        /// string类型
        /// </summary>
        public static readonly Type StringType = typeof(string);

        /// <summary>
        /// UInt16类型
        /// </summary>
        public static readonly Type UInt16Type = typeof(UInt16);

        /// <summary>
        /// UInt32类型
        /// </summary>
        public static readonly Type UInt32Type = typeof(UInt32);

        /// <summary>
        /// UInt64类型
        /// </summary>
        public static readonly Type UInt64Type = typeof(UInt64);
        #endregion

        #region 扩展类型
        /// <summary>
        /// Object类型
        /// </summary>
        public static readonly Type ObjectType = typeof(Object);

        /// <summary>
        /// TimeSpan类型
        /// </summary>
        public static readonly Type TimeSpanType = typeof(TimeSpan);

        /// <summary>
        /// DateTimeOffset类型
        /// </summary>
        public static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);

        /// <summary>
        /// Guid类型
        /// </summary>
        public static readonly Type GuidType = typeof(Guid);

        /// <summary>
        /// byte[]类型
        /// </summary>
        public static readonly Type BytesType = typeof(byte[]);
        #endregion

        /// <summary>
        /// 获取系统类型缩写名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>系统类型缩写名称</returns>
        public static string GetSystemTypeBriefName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            string typeStr;
            TypeCode code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Boolean:
                    typeStr = "bool";
                    break;
                case TypeCode.Byte:
                    typeStr = "byte";
                    break;
                case TypeCode.Char:
                    typeStr = "char";
                    break;
                case TypeCode.DateTime:
                    typeStr = "DateTime";
                    break;
                case TypeCode.Decimal:
                    typeStr = "decimal";
                    break;
                case TypeCode.Double:
                    typeStr = "double";
                    break;
                case TypeCode.Int16:
                    typeStr = "short";
                    break;
                case TypeCode.Int32:
                    typeStr = "int";
                    break;
                case TypeCode.Int64:
                    typeStr = "long";
                    break;
                case TypeCode.SByte:
                    typeStr = "sbyte";
                    break;
                case TypeCode.Single:
                    typeStr = "float";
                    break;
                case TypeCode.String:
                    typeStr = "string";
                    break;
                case TypeCode.UInt16:
                    typeStr = "ushort";
                    break;
                case TypeCode.UInt32:
                    typeStr = "uint";
                    break;
                case TypeCode.UInt64:
                    typeStr = "ulong";
                    break;
                case TypeCode.Empty:
                    typeStr = string.Empty;
                    break;
                case TypeCode.Object:
                    if (type == ClrSystemType.BytesType)
                    {
                        typeStr = "byte[]";
                    }
                    else
                    {
                        typeStr = type.FullName;//默认类型全名
                    }
                    break;
                case TypeCode.DBNull:
                    typeStr = "DBNull";
                    break;
                default:
                    typeStr = type.FullName;//默认类型全名
                    break;
            }

            return typeStr;
        }

        /// <summary>
        /// 判断目标类型是否是整形[包括int,uint等]
        /// </summary>
        /// <param name="targetValueType">目标类型</param>
        /// <returns>true:是;false:否</returns>
        public static bool IsIntegerType(Type targetValueType)
        {
            TypeCode code = Type.GetTypeCode(targetValueType);
            if (code == TypeCode.Byte ||
                code == TypeCode.Int16 ||
                code == TypeCode.Int32 ||
                code == TypeCode.Int64 ||
                code == TypeCode.SByte ||
                code == TypeCode.UInt16 ||
                code == TypeCode.UInt32 ||
                code == TypeCode.UInt64)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断目标类型是否是无符号整形[包括int,uint等]
        /// </summary>
        /// <param name="targetValueType">目标类型</param>
        /// <returns>true:是;false:否</returns>
        public static bool IsUIntegerType(Type targetValueType)
        {
            TypeCode code = Type.GetTypeCode(targetValueType);
            if (code == TypeCode.SByte ||
                code == TypeCode.UInt16 ||
                code == TypeCode.UInt32 ||
                code == TypeCode.UInt64)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断目标类型是否是有符号整形[包括int,uint等]
        /// </summary>
        /// <param name="targetValueType">目标类型</param>
        /// <returns>true:是;false:否</returns>
        public static bool IsHIntegerType(Type targetValueType)
        {
            TypeCode code = Type.GetTypeCode(targetValueType);
            if (code == TypeCode.SByte ||
                code == TypeCode.Int16 ||
                code == TypeCode.Int32 ||
                code == TypeCode.Int64)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断目标类型是否是浮点形[包括int,uint等;true:是;false:否]
        /// </summary>
        /// <param name="targetValueType">目标类型</param>
        /// <returns>true:是;false:否</returns>
        public static bool IsFloatType(Type targetValueType)
        {
            TypeCode code = Type.GetTypeCode(targetValueType);
            if (code == TypeCode.Decimal ||
                code == TypeCode.Double ||
                code == TypeCode.Single)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断目标类型是否是系统数值类型[true:是;false:否]
        /// </summary>
        /// <param name="targetValueType">目标类型</param>
        /// <returns>true:是;false:否</returns>
        public static bool IsSystemNumberType(Type targetValueType)
        {
            TypeCode code = Type.GetTypeCode(targetValueType);
            if (code == TypeCode.Byte ||
                code == TypeCode.Decimal ||
                code == TypeCode.Double ||
                code == TypeCode.Single ||
                code == TypeCode.Int16 ||
                code == TypeCode.Int32 ||
                code == TypeCode.Int64 ||
                code == TypeCode.SByte ||
                code == TypeCode.UInt16 ||
                code == TypeCode.UInt32 ||
                code == TypeCode.UInt64)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
