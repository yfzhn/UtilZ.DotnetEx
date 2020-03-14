using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 值名称获取自定义接口
    /// </summary>
    public interface ICustomerValueName
    {
        /// <summary>
        /// 获取值对应的名称
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="dic">值名称映射字典集合</param>
        /// <returns>值对应的名称</returns>
        string GetName(object value, ValueNameDictionary dic);
    }
}
