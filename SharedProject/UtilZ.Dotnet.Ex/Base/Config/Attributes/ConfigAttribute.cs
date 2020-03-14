using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 配置Attribute基类
    /// </summary>
    public abstract class ConfigAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// 描述
        /// </summary>
        public string Des { get; set; } = null;

        /// <summary>
        /// 值为null时节点是否存在[true:存在;false:不存在]
        /// </summary>
        public bool AllowNullValueElement { get; set; } = false;

        /// <summary>
        /// 配置值转换对象
        /// </summary>
        internal IConfigValueConverter Converter { get; private set; } = null;

        private Type _converterType = null;
        /// <summary>
        /// 配置值转换类型
        /// </summary>
        public Type ConverterType
        {
            get { return _converterType; }
            set
            {
                _converterType = value;
                if (_converterType != null)
                {
                    this.Converter = ActivatorEx.CreateInstance<IConfigValueConverter>(_converterType);
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="des">描述</param>
        /// <param name="allowNullValueElement">值为null时节点是否存在[true:存在;false:不存在]</param>
        public ConfigAttribute(string name, string des, bool allowNullValueElement)
        {
            this.Name = name;
            this.Des = des;
            this.AllowNullValueElement = allowNullValueElement;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigAttribute()
        {

        }


        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <param name="type">项类型</param>
        /// <returns>参数名称</returns>
        internal string GetName(Type type)
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                return type.Name;
            }

            return this.Name;
        }

        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <param name="propertyInfo">项PropertyInfo</param>
        /// <returns>参数名称</returns>
        internal string GetName(PropertyInfo propertyInfo)
        {
            var name = this.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = propertyInfo.Name;
            }

            return name;
        }
    }
}
