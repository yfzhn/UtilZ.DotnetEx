using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTLV属性或字段值序列化编解码转换接口
    /// </summary>
    public abstract class TTLVConverterBase : ITTLVConverter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TTLVConverterBase()
        {

        }

        /// <summary>
        /// 目标对象转换为bytes
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="obj">转换对象</param>
        /// <param name="targetType">目标属性或字段类型</param>
        /// <param name="converterPara">转换器参数</param>
        /// <returns>转换结果</returns>
        public abstract byte[] ConvertToBytes(int tag, object obj, Type targetType, object converterPara);

        /// <summary>
        /// 转换bytes到目标对象
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="buffer">byte[]</param>
        /// <param name="memberValue">目标属性或字段值</param>
        /// <param name="targetType">目标属性或字段类型</param>
        /// <param name="converterPara">转换器参数</param>
        /// <returns>解析结果对象</returns>
        public abstract object ConvertBackObject(int tag, byte[] buffer, object memberValue, Type targetType, object converterPara);
    }
}
