using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 标记基元配置项
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigItemAttribute : ConfigAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigItemAttribute()
            : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="des">描述</param>
        /// <param name="allowNullValueElement">值为null时节点是否存在[true:存在;false:不存在]</param>
        public ConfigItemAttribute(string name, string des = null, bool allowNullValueElement = false)
            : base(name, des, allowNullValueElement)
        {

        }
    }
}
