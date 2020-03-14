using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 默认值名称获取自定义类
    /// </summary>
    public class DefaultCustomerValueName : CustomerValueNameAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultCustomerValueName()
            : base()
        {

        }

        private readonly Func<object, ValueNameDictionary, string> _getNameFunc = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public DefaultCustomerValueName(Func<object, ValueNameDictionary, string> getNameFunc)
            : this()
        {
            this._getNameFunc = getNameFunc;
        }

        /// <summary>
        /// 获取值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="dic">值名称映射字典集合</param>
        /// <returns>值对应的名称</returns>
        protected override string PrimitiveGetName(object value, ValueNameDictionary dic)
        {
            string name;
            if (this._getNameFunc == null)
            {
                name = $"未知的{dic.ValueNameMapTypeAttribute.Name}[{value}]";
            }
            else
            {
                name = this._getNameFunc(value, dic);
            }

            return name;
        }
    }
}
