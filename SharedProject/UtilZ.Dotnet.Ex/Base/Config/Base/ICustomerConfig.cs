using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 自定义配置项读写接口
    /// </summary>
    public interface ICustomerConfig
    {
        /// <summary>
        /// 写配置项
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo</param>
        /// <param name="value">属性值</param>
        /// <param name="element">存放配置的XElement</param>
        /// <param name="attri">ConfigItemCustomerAttribute</param>
        void Write(PropertyInfo propertyInfo, object value, XElement element, ConfigItemCustomerAttribute attri);

        /// <summary>
        /// 读配置项
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo</param>
        /// <param name="element">存放配置的XElement</param>
        /// <param name="attri">ConfigItemCustomerAttribute</param>
        /// <returns>属性值</returns>
        object Read(PropertyInfo propertyInfo, XElement element, ConfigItemCustomerAttribute attri);
    }
}
