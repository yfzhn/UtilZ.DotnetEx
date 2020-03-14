using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 标记配置项集合
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigCollectionAttribute : ConfigAttribute
    {
        /// <summary>
        /// 元素项名称
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigCollectionAttribute()
            : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="eleName">元素项名称</param>
        /// <param name="des">描述</param>
        /// <param name="allowNullValueElement">值为null时节点是否存在[true:存在;false:不存在]</param>
        public ConfigCollectionAttribute(string name = null, string eleName = null, string des = null, bool allowNullValueElement = false)
            : base(name, des, allowNullValueElement)
        {
            this.ElementName = eleName;
        }

        internal string GetElementName(Type eleType)
        {
            string childName = this.ElementName;
            if (string.IsNullOrWhiteSpace(childName))
            {
                childName = eleType.Name;
            }

            return childName;
        }
    }
}
