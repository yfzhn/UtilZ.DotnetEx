using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 值名称获取自定义基类
    /// </summary>
    public abstract class CustomerValueNameAbs : ICustomerValueName
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerValueNameAbs()
        {

        }

        /// <summary>
        /// 获取值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="dic">值名称映射字典集合</param>
        /// <returns>值对应的名称</returns>
        public string GetName(object value, ValueNameDictionary dic)
        {
            return this.PrimitiveGetName(value, dic);
        }

        /// <summary>
        /// 获取值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="dic">值名称映射字典集合</param>
        /// <returns>值对应的名称</returns>
        protected abstract string PrimitiveGetName(object value, ValueNameDictionary dic);
    }
}
