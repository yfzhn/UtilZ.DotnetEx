using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.ConstantValueDescription
{
    /// <summary>
    /// 值描述组
    /// </summary>
    public class ValueDescriptionGroup : Dictionary<object, DisplayNameExAttribute>
    {
        private readonly ValueDescriptionGroupAttribute _groupDescriptionAttribute;
        /// <summary>
        /// 获取组描述特性
        /// </summary>
        public ValueDescriptionGroupAttribute GroupDescriptionAttribute
        {
            get { return _groupDescriptionAttribute; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="groupDescriptionAttribute">组描述特性</param>
        public ValueDescriptionGroup(ValueDescriptionGroupAttribute groupDescriptionAttribute)
        {
            this._groupDescriptionAttribute = groupDescriptionAttribute;
        }
    }
}
