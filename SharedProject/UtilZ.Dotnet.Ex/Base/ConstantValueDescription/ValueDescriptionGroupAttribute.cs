using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.ConstantValueDescription
{
    /// <summary>
    /// 值描述组特性
    /// </summary>
    public class ValueDescriptionGroupAttribute : DisplayNameExAttribute
    {
        private IExtendDescription _extend;
        /// <summary>
        /// 扩展描述类类型
        /// </summary>
        public IExtendDescription Extend
        {
            get { return _extend; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        public ValueDescriptionGroupAttribute(string displayName)
            : this(displayName, null, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="extendDescriptionType">扩展描述类类型,该类型需要实现IExtendDescription接口</param>
        public ValueDescriptionGroupAttribute(string displayName, Type extendDescriptionType)
            : this(displayName, null, extendDescriptionType)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="description">描述</param>
        /// <param name="extendDescriptionType">扩展描述类类型,该类型需要实现IExtendDescription接口</param>
        public ValueDescriptionGroupAttribute(string displayName, string description, Type extendDescriptionType)
        : base(displayName, description)
        {
            if (extendDescriptionType == null)
            {
                this._extend = null;
            }
            else
            {
                this._extend = (IExtendDescription)Activator.CreateInstance(extendDescriptionType);
            }
        }
    }
}
