using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 值名称映射字典
    /// </summary>
    public class ValueNameDictionary : Dictionary<object, ValueNameMapFieldAttribute>
    {
        private readonly ValueNameMapTypeAttribute _valueNameMapTypeAttribute;
        /// <summary>
        /// 值名称映射类型特性
        /// </summary>
        public ValueNameMapTypeAttribute ValueNameMapTypeAttribute
        {
            get { return _valueNameMapTypeAttribute; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="valueNameMapTypeAttribute">值名称映射类型特性</param>
        public ValueNameDictionary(ValueNameMapTypeAttribute valueNameMapTypeAttribute)
            : base()
        {
            this._valueNameMapTypeAttribute = valueNameMapTypeAttribute;
        }
    }
}
