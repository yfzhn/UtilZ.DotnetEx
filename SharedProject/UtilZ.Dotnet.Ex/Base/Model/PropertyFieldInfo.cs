using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 属性字段信息
    /// </summary>
    public class PropertyFieldInfo
    {
        /// <summary>
        /// 获取或设置显示名称
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 数据标识
        /// </summary>
        public object Tag { get; private set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PropertyFieldInfo()
            : this(string.Empty, null, string.Empty, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="value">值</param>
        public PropertyFieldInfo(string displayName, object value)
            : this(displayName, value, string.Empty, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="value">值</param>
        /// <param name="description">项描述</param>
        public PropertyFieldInfo(string displayName, object value, string description)
            : this(displayName, value, description, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="displayName">显示文本</param>
        /// <param name="value">值</param>
        /// <param name="description">项描述</param>
        /// <param name="tag">数据标识</param>
        public PropertyFieldInfo(string displayName, object value, string description, object tag)
        {
            this.DisplayName = displayName;
            this.Value = value;
            this.Description = description;
            this.Tag = tag;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>下拉框显示文本</returns>
        public override string ToString()
        {
            return this.DisplayName;
        }

        /// <summary>
        /// 转换泛型集合为DropdownBindingItem列表
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="srcItems">原始泛型集合</param>
        /// <param name="displayMember">显示的成员,属性名或字段名,当为null时调用成员的ToString方法的值作为显示值[默认值为null]</param>
        /// <returns>DropdownBindingItem列表</returns>
        public static List<PropertyFieldInfo> GenericToDropdownBindingItems<T>(IEnumerable<T> srcItems, string displayMember = null) where T : class
        {
            if (srcItems == null || srcItems.Count() == 0)
            {
                return new List<PropertyFieldInfo>();
            }

            List<PropertyFieldInfo> items;
            if (string.IsNullOrWhiteSpace(displayMember))
            {
                items = srcItems.Select(t => { return new PropertyFieldInfo(t?.ToString(), t); }).ToList();
            }
            else
            {
                Type type = typeof(T);
                PropertyInfo proInfo = type.GetProperty(displayMember);
                if (proInfo != null)
                {
                    items = srcItems.Select(t => { return new PropertyFieldInfo(proInfo.GetValue(t, null)?.ToString(), t); }).ToList();
                }
                else
                {
                    FieldInfo fieldInfo = type.GetField(displayMember);
                    if (fieldInfo != null)
                    {
                        items = srcItems.Select(t => { return new PropertyFieldInfo(fieldInfo.GetValue(t)?.ToString(), t); }).ToList();
                    }
                    else
                    {
                        items = srcItems.Select(t => { return new PropertyFieldInfo(t?.ToString(), t); }).ToList();
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// 转换泛型集合为DropdownBindingItem列表
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="srcItems">原始泛型集合</param>
        /// <param name="displayFun">显示转换委托</param>
        /// <returns>DropdownBindingItem列表</returns>
        public static List<PropertyFieldInfo> GenericToDropdownBindingItems<T>(IEnumerable<T> srcItems, Func<T, string> displayFun = null) where T : class
        {
            if (srcItems == null || srcItems.Count() == 0)
            {
                return new List<PropertyFieldInfo>();
            }

            List<PropertyFieldInfo> items;
            if (displayFun == null)
            {
                items = srcItems.Select(t => { return new PropertyFieldInfo(t?.ToString(), t); }).ToList();
            }
            else
            {
                items = srcItems.Select(t => { return new PropertyFieldInfo(displayFun(t), t); }).ToList();
            }
            return items;
        }
    }
}
