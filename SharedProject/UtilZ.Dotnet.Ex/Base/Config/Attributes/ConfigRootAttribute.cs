using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 标记配置根
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ConfigRootAttribute : ConfigAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="des">描述</param>
        /// <param name="allowNullValueElement">值为null时节点是否存在[true:存在;false:不存在]</param>
        public ConfigRootAttribute(string name, string des = null, bool allowNullValueElement = false)
            : base(name, des, allowNullValueElement)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigRootAttribute()
            : base()
        {

        }


        internal static ConfigRootAttribute GetRootConfigRootAttribute(Type configType, ConfigAttributeTypes configAttributeTypes)
        {
            ConfigRootAttribute configAttribute;
            Attribute attri = configType.GetCustomAttribute(configAttributeTypes.RootAttributeType, false);
            if (attri == null)
            {
                //未标记特性,创建默认值
                configAttribute = new ConfigRootAttribute(configType.Name, null, true);
            }
            else
            {
                configAttribute = (ConfigRootAttribute)attri;
            }

            return configAttribute;
        }
    }
}
