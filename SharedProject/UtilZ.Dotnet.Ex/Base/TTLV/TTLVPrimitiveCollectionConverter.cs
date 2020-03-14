using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// 基元类型集合(包括非基元类型的字符串集合)TTLV属性值编解码转换接口
    /// </summary>
    public class TTLVPrimitiveCollectionConverter : TTLVConverterBase
    {
        private readonly byte[] _stringNullLengthBuffer;
        /// <summary>
        /// 构造函数
        /// </summary>
        public TTLVPrimitiveCollectionConverter()
            : base()
        {
            int length = -1;
            this._stringNullLengthBuffer = BitConverter.GetBytes(length);
        }

        private bool IsByteArray(TypeCode typeCode, Type targetType)
        {
            if (typeCode == TypeCode.Byte && targetType.IsArray)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region ConvertToBytes
        /// <summary>
        /// 目标对象转换为bytes
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">转换对象</param>
        /// <param name="targetType">目标属性或字段类型</param>
        /// <param name="converterPara">转换器参数</param>
        /// <returns>转换结果</returns>
        public override byte[] ConvertToBytes(int tag, object obj, Type targetType, object converterPara)
        {
            Type eleType;
            TTLVType ttlvType;
            TTLVCommon.GetCollectionElementType(targetType, out eleType, out ttlvType);

            TypeCode typeCode = Type.GetTypeCode(eleType);
            if (this.IsByteArray(typeCode, targetType))
            {
                return (byte[])obj;
            }
            else
            {
                var collection = (IEnumerable)obj;
                List<byte> buffer = new List<byte>();
                switch (typeCode)
                {
                    case TypeCode.String:
                        this.PrimitiveStringCollectionConvertToBytes(buffer, collection, typeCode);
                        break;
                    default:
                        this.PrimitiveConvertToBytes(buffer, collection, typeCode);
                        break;
                }

                return buffer.ToArray();
            }
        }

        private void PrimitiveStringCollectionConvertToBytes(List<byte> buffer, IEnumerable collection, TypeCode typeCode)
        {
            foreach (var item in collection)
            {
                if (item == null)
                {
                    buffer.AddRange(this._stringNullLengthBuffer);
                }
                else
                {
                    byte[] tmpBuffer = TTLVCommon.ConvertToBytes(typeCode, item);
                    buffer.AddRange(BitConverter.GetBytes(tmpBuffer.Length));
                    if (tmpBuffer.Length > 0)
                    {
                        buffer.AddRange(tmpBuffer);
                    }
                }
            }
        }

        private void PrimitiveConvertToBytes(List<byte> buffer, IEnumerable collection, TypeCode typeCode)
        {
            foreach (var item in collection)
            {
                byte[] tmpBuffer = TTLVCommon.ConvertToBytes(typeCode, item);
                buffer.AddRange(tmpBuffer);
            }
        }
        #endregion

        #region ConvertBackObject
        /// <summary>
        /// 转换bytes到目标对象
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="buffer">byte[]</param>
        /// <param name="memberValue">目标属性或字段值</param>
        /// <param name="targetType">目标属性或字段类型</param>
        /// <param name="converterPara">转换器参数</param>
        /// <returns>解析结果对象</returns>
        public override object ConvertBackObject(int tag, byte[] buffer, object memberValue, Type targetType, object converterPara)
        {
            Type eleType;
            TTLVType ttlvType;
            TTLVCommon.GetCollectionElementType(targetType, out eleType, out ttlvType);

            TypeCode typeCode = Type.GetTypeCode(eleType);
            if (this.IsByteArray(typeCode, targetType))
            {
                return buffer;
            }

            object value;
            switch (ttlvType)
            {
                case TTLVType.IList:
                    var list = (IList)memberValue;
                    if (list == null)
                    {
                        list = (IList)Activator.CreateInstance(targetType);
                    }

                    this.ReadValue(buffer, eleType, list);
                    value = list;
                    break;
                case TTLVType.Array:
                    IList arraList = new List<object>();
                    this.ReadValue(buffer, eleType, arraList);
                    Array array = Array.CreateInstance(eleType, arraList.Count);
                    for (int i = 0; i < arraList.Count; i++)
                    {
                        array.SetValue(arraList[i], i);
                    }

                    value = array;
                    break;
                default:
                    throw new NotImplementedException($"未实现类型[{ttlvType.ToString()}]");
            }

            return value;
        }

        private void ReadValue(byte[] buffer, Type eleType, IList list)
        {
            TypeCode typeCode = Type.GetTypeCode(eleType);
            switch (typeCode)
            {
                case TypeCode.String:
                    this.PrimitiveReadStringCollectionValue(buffer, eleType, list, typeCode);
                    break;
                default:
                    this.PrimitiveReadValue(buffer, TTLVCommon.GetConvertTypeLength(typeCode), list, typeCode);
                    break;
            }
        }

        private void PrimitiveReadStringCollectionValue(byte[] buffer, Type eleType, IList list, TypeCode typeCode)
        {
            int index = 0;
            int length;
            object item;

            while (index < buffer.Length)
            {
                length = BitConverter.ToInt32(buffer, index);
                index += 4;

                if (length == 0)
                {
                    list.Add(string.Empty);
                }
                else if (length > 0)
                {
                    item = TTLVCommon.ConvterBack(buffer, typeCode, index, length);
                    index += length;
                    list.Add(item);
                }
                else
                {
                    list.Add(null);
                }
            }
        }

        private void PrimitiveReadValue(byte[] buffer, int eleSize, IList list, TypeCode typeCode)
        {
            int index = 0;
            object item;

            while (index < buffer.Length)
            {
                item = TTLVCommon.ConvterBack(buffer, typeCode, index, eleSize);
                index += eleSize;
                list.Add(item);
            }
        }
        #endregion
    }
}
