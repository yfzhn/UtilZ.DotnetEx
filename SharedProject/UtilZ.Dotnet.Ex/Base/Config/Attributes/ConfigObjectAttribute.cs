using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 标记配置项是对象类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigObjectAttribute : ConfigAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigObjectAttribute()
            : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="des">描述</param>
        /// <param name="allowNullValueElement">值为null时节点是否存在[true:存在;false:不存在]</param>
        public ConfigObjectAttribute(string name, string des = null, bool allowNullValueElement = false)
            : base(name, des, allowNullValueElement)
        {

        }
    }
}
