using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTLVCommon
    /// </summary>
    public class TTLVCommon
    {
        /// <summary>
        /// 获取转换类型bytes长度
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static int GetConvertTypeLength(TypeCode typeCode)
        {
            int length;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    length = sizeof(bool);
                    break;
                case TypeCode.Char:
                    length = sizeof(char);
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                    length = sizeof(Int16);
                    break;
                case TypeCode.UInt16:
                    length = sizeof(UInt16);
                    break;
                case TypeCode.Int32:
                    length = sizeof(Int32);
                    break;
                case TypeCode.UInt32:
                    length = sizeof(UInt32);
                    break;
                case TypeCode.Int64:
                    length = sizeof(Int64);
                    break;
                case TypeCode.UInt64:
                    length = sizeof(UInt64);
                    break;
                case TypeCode.Single:
                    length = sizeof(float);
                    break;
                case TypeCode.Double:
                    length = sizeof(double);
                    break;
                case TypeCode.Decimal:
                    length = sizeof(decimal);
                    break;
                case TypeCode.DateTime:
                    length = 8;
                    break;
                case TypeCode.String:
                default:
                    throw new ArgumentException(string.Format("不支持的类型[{0}]", typeCode.ToString()));
            }

            return length;
        }

        /// <summary>
        /// 获取基元类型数据转换为Bytes
        /// </summary>
        /// <param name="typeCode">TypeCode</param>
        /// <param name="objValue">object</param>
        public static byte[] ConvertToBytes(TypeCode typeCode, object objValue)
        {
            byte[] valueBuffer;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    valueBuffer = BitConverter.GetBytes(Convert.ToBoolean(objValue));
                    break;
                case TypeCode.Char:
                    valueBuffer = BitConverter.GetBytes(Convert.ToChar(objValue));
                    break;
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                    valueBuffer = BitConverter.GetBytes(Convert.ToInt16(objValue));
                    break;
                case TypeCode.UInt16:
                    valueBuffer = BitConverter.GetBytes(Convert.ToUInt16(objValue));
                    break;
                case TypeCode.Int32:
                    valueBuffer = BitConverter.GetBytes(Convert.ToInt32(objValue));
                    break;
                case TypeCode.UInt32:
                    valueBuffer = BitConverter.GetBytes(Convert.ToUInt32(objValue));
                    break;
                case TypeCode.Int64:
                    valueBuffer = BitConverter.GetBytes(Convert.ToInt64(objValue));
                    break;
                case TypeCode.UInt64:
                    valueBuffer = BitConverter.GetBytes(Convert.ToUInt64(objValue));
                    break;
                case TypeCode.Single:
                    valueBuffer = BitConverter.GetBytes(Convert.ToSingle(objValue));
                    break;
                case TypeCode.Double:
                    valueBuffer = BitConverter.GetBytes(Convert.ToDouble(objValue));
                    break;
                case TypeCode.Decimal:
                    int[] bits = Decimal.GetBits(Convert.ToDecimal(objValue));
                    valueBuffer = new byte[bits.Length * 4];
                    using (var ms = new MemoryStream(valueBuffer))
                    {
                        var bw = new BinaryWriter(ms);
                        foreach (var bit in bits)
                        {
                            bw.Write(bit);
                        }

                        bw.Flush();
                    }
                    break;
                case TypeCode.DateTime:
                    valueBuffer = BitConverter.GetBytes(Convert.ToDateTime(objValue).ToFileTime());
                    break;
                case TypeCode.String:
                    valueBuffer = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(objValue));
                    break;
                default:
                    throw new ArgumentException(string.Format("不支持的类型[{0}]", typeCode.ToString()));
            }

            return valueBuffer;
        }

        /// <summary>
        /// 获取byte[]转换为基元类型数据
        /// </summary>
        /// <param name="valueBuffer"></param>
        /// <param name="typeCode"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static object ConvterBack(byte[] valueBuffer, TypeCode typeCode, int startIndex, int count)
        {
            object value;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    value = BitConverter.ToBoolean(valueBuffer, startIndex);
                    break;
                case TypeCode.Char:
                    value = BitConverter.ToChar(valueBuffer, startIndex);
                    break;
                case TypeCode.SByte:
                    value = Convert.ToSByte(BitConverter.ToInt16(valueBuffer, startIndex));
                    break;
                case TypeCode.Byte:
                    value = Convert.ToByte(BitConverter.ToInt16(valueBuffer, startIndex));
                    break;
                case TypeCode.Int16:
                    value = BitConverter.ToInt16(valueBuffer, startIndex);
                    break;
                case TypeCode.UInt16:
                    value = BitConverter.ToUInt16(valueBuffer, startIndex);
                    break;
                case TypeCode.Int32:
                    value = BitConverter.ToInt32(valueBuffer, startIndex);
                    break;
                case TypeCode.UInt32:
                    value = BitConverter.ToUInt32(valueBuffer, startIndex);
                    break;
                case TypeCode.Int64:
                    value = BitConverter.ToInt64(valueBuffer, startIndex);
                    break;
                case TypeCode.UInt64:
                    value = BitConverter.ToUInt64(valueBuffer, startIndex);
                    break;
                case TypeCode.Single:
                    value = BitConverter.ToSingle(valueBuffer, startIndex);
                    break;
                case TypeCode.Double:
                    value = BitConverter.ToDouble(valueBuffer, startIndex);
                    break;
                case TypeCode.Decimal:
                    int bitLen = valueBuffer.Length / 4;
                    int[] bits = new int[bitLen];
                    for (int i = 0; i < bitLen; i++)
                    {
                        bits[i] = BitConverter.ToInt32(valueBuffer, startIndex);
                        startIndex += 4;
                    }

                    value = new Decimal(bits);
                    break;
                case TypeCode.DateTime:
                    value = DateTime.FromFileTime(BitConverter.ToInt64(valueBuffer, startIndex));
                    break;
                case TypeCode.String:
                    value = System.Text.Encoding.UTF8.GetString(valueBuffer, startIndex, count);
                    break;
                default:
                    throw new ArgumentException(string.Format("不支持的类型[{0}]", typeCode.ToString()));
            }

            return value;
        }

        /// <summary>
        /// 判断是否有无参构造函数,且可反射调用无参构造函数实例化对象
        /// </summary>
        /// <param name="type"></param>
        public static void CheckHasNoParaConstructor(Type type)
        {
            if (type.IsAbstract)
            {
                throw new InvalidOperationException($"类型[{type.FullName}]为抽象类,创建实例失败,TTVL解析异常");
            }

            if (type.GetConstructors().Where(t => { return t.GetParameters().Length == 0; }).Count() == 0)
            {
                throw new InvalidOperationException($"类型[{type.FullName}]没有无参构造函数,创建实例失败,TTVL解析异常");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionType"></param>
        /// <param name="eleType"></param>
        /// <param name="ttlvType"></param>
        public static void GetCollectionElementType(Type collectionType, out Type eleType, out TTLVType ttlvType)
        {
            if (collectionType.IsArray)
            {
                ttlvType = TTLVType.Array;
                eleType = collectionType.GetElementType();
            }
            else if (collectionType.GetInterface(typeof(IList).FullName) != null)
            {
                ttlvType = TTLVType.IList;
                //eleType = propertyInfo.PropertyType.GenericTypeArguments[0];//4.5支持此种用法,4.0不支持
                eleType = collectionType.GetGenericArguments()[0];
            }
            else
            {
                throw new NotImplementedException($"集合类型[{collectionType.FullName}]不支持TTLV编码");
            }
        }
    }
}
