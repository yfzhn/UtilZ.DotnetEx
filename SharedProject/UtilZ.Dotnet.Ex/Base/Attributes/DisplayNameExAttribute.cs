using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 显示名称特性
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class DisplayNameExAttribute : DisplayNameAttribute
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DisplayNameExAttribute()
            : this(string.Empty)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        public DisplayNameExAttribute(string displayName)
            : this(displayName, 0)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="tag">标识</param>
        public DisplayNameExAttribute(string displayName, object tag)
            : this(displayName, null, tag)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="description">描述</param>
        public DisplayNameExAttribute(string displayName, string description)
            : this(displayName, 0, description)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="description">描述</param>
        /// <param name="tag">标识</param>
        public DisplayNameExAttribute(string displayName, string description, object tag)
            : this(displayName, 0, description, tag)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="orderIndex">项显示顺序</param>
        public DisplayNameExAttribute(string displayName, int orderIndex)
            : this(displayName, orderIndex, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="orderIndex">项显示顺序</param>
        /// <param name="description">描述</param>
        public DisplayNameExAttribute(string displayName, int orderIndex, string description)
            : this(displayName, orderIndex, description, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="orderIndex">项显示顺序</param>
        /// <param name="description">描述</param>
        /// <param name="tag">标识</param>
        public DisplayNameExAttribute(string displayName, int orderIndex, string description, object tag)
            : base(displayName)
        {
            this.OrderIndex = orderIndex;
            this.Description = description;
            this.Tag = tag;
        }

        /// <summary>
        /// 项显示顺序
        /// </summary>
        public int OrderIndex { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 获取或设置标识
        /// </summary>
        public object Tag { get; private set; }

        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns>返回特性文本</returns>
        public override string ToString()
        {
            return base.DisplayName;
        }
    }
}
