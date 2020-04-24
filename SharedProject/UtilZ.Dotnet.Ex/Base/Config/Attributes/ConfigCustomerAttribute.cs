using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 标记配置项自定义读写
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigCustomerAttribute : ConfigAttribute
    {
        /// <summary>
        /// 自定义配置项读写
        /// </summary>
        internal IConfigCustomerRW CustomerConfig { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="customerConfigType">自定义配置项读写类型</param>
        public ConfigCustomerAttribute(Type customerConfigType)
            : base()
        {
            this.CustomerConfig = ActivatorEx.CreateInstance<IConfigCustomerRW>(customerConfigType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="customerConfigType">自定义配置项读写类型</param>
        /// <param name="name">名称</param>
        /// <param name="des">描述</param>
        /// <param name="allowNullValueElement">值为null时节点是否存在[true:存在;false:不存在]</param>
        public ConfigCustomerAttribute(Type customerConfigType, string name, string des = null, bool allowNullValueElement = false)
            : base(name, des, allowNullValueElement)
        {
            this.CustomerConfig = ActivatorEx.CreateInstance<IConfigCustomerRW>(customerConfigType);
        }
    }
}
