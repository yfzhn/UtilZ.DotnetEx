using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.TTLV
{
    /// <summary>
    /// TTLV特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TTLVAttribute : Attribute
    {
        /// <summary>
        /// 标签
        /// </summary>
        internal int Tag { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal Type ConverterType { get; private set; }

        internal object ConverterPara { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal TTLVPropertyType PropertyType { get; private set; }

        internal object[] Index { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="propertyType">属性类型[true:单个对象;false;集合(支持类型:IList子类和数组);如果为集合,则属性类型不只能是具体类型,不能是抽象类类型]</param>
        /// <param name="index">索引化属性的可选索引值。对于非索引化属性，此值应为 null</param>
        public TTLVAttribute(int tag, bool propertyType = true, object[] index = null) :
            this(tag, null, propertyType ? TTLVPropertyType.SingleObject : TTLVPropertyType.Collection, index, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="converterType">属性值转换器类型[该类型必须实现ITTLVConverter接口,且有无参构造函数]</param>
        /// <param name="index">索引化属性的可选索引值。对于非索引化属性，此值应为 null</param>
        /// <param name="converterPara">转换器参数</param>
        public TTLVAttribute(int tag, Type converterType, object[] index = null, object converterPara = null)
            : this(tag, converterType, TTLVPropertyType.Converter, index, converterPara)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="converterType">属性值转换器类型[该类型必须实现ITTLVConverter接口,且有无参构造函数]</param>
        /// <param name="propertyType">属性类型[如果为Collection]</param>
        /// <param name="index">索引化属性的可选索引值。对于非索引化属性，此值应为 null</param>
        /// <param name="converterPara">转换器参数</param>
        private TTLVAttribute(int tag, Type converterType, TTLVPropertyType propertyType, object[] index, object converterPara = null)
        {
            this.Tag = tag;
            this.ConverterType = converterType;
            this.ConverterPara = converterPara;
            this.PropertyType = propertyType;
            this.Index = index;
        }
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum TTLVPropertyType
    {
        /// <summary>
        /// 单对象
        /// </summary>
        SingleObject,

        /// <summary>
        /// 集合(仅支持类型,IList子类和数组)
        /// </summary>
        Collection,

        /// <summary>
        /// 自定义转换器
        /// </summary>
        Converter
    }
}
