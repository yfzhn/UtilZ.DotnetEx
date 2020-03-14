using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 常量字段值名称映射特性
    /// </summary>
    public class ValueNameMapFieldAttribute : Attribute
    {
        private readonly string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="name"></param>
        public ValueNameMapFieldAttribute(string name)
        {
            this._name = name;
        }
    }
}
