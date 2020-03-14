using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 配置值转换接口
    /// </summary>
    public interface IConfigValueConverter
    {
        /// <summary>
        /// 将属性值转换为字符串
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo</param>
        /// <param name="value">属性值</param>
        /// <returns>字符串</returns>
        string ConvertTo(PropertyInfo propertyInfo, object value);

        /// <summary>
        /// 将字符串转换为属性值
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo</param>
        /// <param name="value">字符串</param>
        /// <returns>属性值</returns>
        object ConvertFrom(PropertyInfo propertyInfo, string value);
    }
}
