using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 属性改变事件扩展参数
    /// </summary>
    public class PropertyChangedExEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// 旧值
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        public PropertyChangedExEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            this.OldValue = OldValue;
            this.NewValue = NewValue;
        }
    }
}
