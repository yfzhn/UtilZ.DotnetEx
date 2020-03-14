using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTLV属性或字段值序列化编解码转换接口
    /// </summary>
    public class TTLVSerializeConverter : TTLVConverterBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TTLVSerializeConverter()
            : base()
        {

        }

        private TTLVSerializeType GetTTLVSerializeType(object converterPara)
        {
            TTLVSerializeType ttlvSerializeType = TTLVSerializeType.Json;
            if (converterPara != null)
            {
                if (!(converterPara is TTLVSerializeType))
                {
                    throw new ArgumentNullException($"转换器[{typeof(TTLVSerializeConverter).FullName}]转换参数必须为枚举类型[{typeof(TTLVSerializeType).FullName}]的值之一,当前参数值无效");
                }

                ttlvSerializeType = (TTLVSerializeType)converterPara;
            }

            return ttlvSerializeType;
        }

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
            throw new NotImplementedException();
            //if (obj == null)
            //{
            //    return new byte[0];
            //}

            //TTLVSerializeType ttlvSerializeType = this.GetTTLVSerializeType(converterPara);
            //byte[] buffer;
            //switch (ttlvSerializeType)
            //{
            //    case TTLVSerializeType.Bin:
            //        buffer = SerializeEx.BinarySerialize(obj);
            //        break;
            //    case TTLVSerializeType.Json:
            //        string json = SerializeEx.WebScriptJsonSerializerObject(obj);
            //        buffer = System.Text.Encoding.UTF8.GetBytes(json);
            //        break;
            //    case TTLVSerializeType.Xml:
            //        string xmlStr = SerializeEx.XmlSerializer(obj);
            //        buffer = System.Text.Encoding.UTF8.GetBytes(xmlStr);
            //        break;
            //    default:
            //        throw new NotImplementedException("未实现的序列化方式");
            //}

            //return buffer;
        }

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
            throw new NotImplementedException();
            //if (buffer == null || buffer.Length == 0)
            //{
            //    return null;
            //}

            //TTLVSerializeType ttlvSerializeType = this.GetTTLVSerializeType(converterPara);
            //object value;
            //switch (ttlvSerializeType)
            //{
            //    case TTLVSerializeType.Bin:
            //        value = SerializeEx.BinaryDeserialize(buffer);
            //        break;
            //    case TTLVSerializeType.Json:
            //        string json = System.Text.Encoding.UTF8.GetString(buffer);
            //        value = SerializeEx.WebScriptJsonDeserializeObject(json, targetType);
            //        break;
            //    case TTLVSerializeType.Xml:
            //        string xmlStr = System.Text.Encoding.UTF8.GetString(buffer);
            //        value = SerializeEx.XmlDeserializerFromString(xmlStr, targetType);
            //        break;
            //    default:
            //        throw new NotImplementedException("未实现的序列化方式");
            //}

            //return value;
        }
    }

    /// <summary>
    /// TTLVSerializeConverter序列化类型
    /// </summary>
    public enum TTLVSerializeType
    {
        /// <summary>
        /// Json
        /// </summary>
        [DisplayNameEx("Json")]
        Json,

        /// <summary>
        /// Xml,目标类型需要标记为可序列化,且类型必须是public类型
        /// </summary>
        [DisplayNameEx("Xml")]
        Xml,

        /// <summary>
        /// 二进制,目标类型需要标记为可序列化
        /// </summary>
        [DisplayNameEx("二进制")]
        Bin
    }
}
