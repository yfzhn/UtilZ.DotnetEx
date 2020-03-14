using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 值名称映射类型特性
    /// </summary>
    public class ValueNameMapTypeAttribute : Attribute
    {
        private readonly string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        private ICustomerValueName _customerValueName;
        /// <summary>
        /// 自定义值名称获取接口
        /// </summary>
        public ICustomerValueName CustomerValueName
        {
            get { return _customerValueName; }
        }

        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="name"></param>
        public ValueNameMapTypeAttribute(string name)
            : this(name, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="customerValueNameType"></param>
        public ValueNameMapTypeAttribute(string name, Type customerValueNameType)
        {
            this._name = name;
            if (customerValueNameType == null)
            {
                this._customerValueName = new DefaultCustomerValueName();
            }
            else
            {
                this._customerValueName = (ICustomerValueName)Activator.CreateInstance(customerValueNameType);
            }
        }
    }
}
