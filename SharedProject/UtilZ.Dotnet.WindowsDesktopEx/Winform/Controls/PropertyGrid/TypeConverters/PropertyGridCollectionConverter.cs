using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.TypeConverters
{
    /// <summary>
    /// PropertyGrid集合显示信息接口
    /// </summary>
    public class PropertyGridCollectionConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型[转换为显示对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="culture">如果传递 null，则采用当前区域性</param>
        /// <param name="value">要转换的 Object</param>
        /// <param name="destType">value 参数要转换成的 Type</param>
        /// <returns>表示转换的 value 的 Object</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (context == null || context.Instance == null)
            {
                return base.ConvertTo(context, culture, value, destType);
            }

            // Type type = context.PropertyDescriptor.ComponentType;
            Type type = context.Instance.GetType();
            if (type.GetInterface(typeof(IPropertyGridCollection).FullName) != null)
            {
                return ((IPropertyGridCollection)context.Instance).GetCollectionDisplayInfo(context.PropertyDescriptor.Name);
            }

            return base.ConvertTo(context, culture, value, destType);
        }
    }

    /// <summary>
    /// PropertyGrid集合项显示信息接口
    /// </summary>
    public class PropertyGridCollectionItemConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型[转换为显示对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="culture">如果传递 null，则采用当前区域性</param>
        /// <param name="value">要转换的 Object</param>
        /// <param name="destType">value 参数要转换成的 Type</param>
        /// <returns>表示转换的 value 的 Object</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (context == null || context.Instance == null)
            {
                return base.ConvertTo(context, culture, value, destType);
            }

            if (context.PropertyDescriptor.PropertyType.GetInterface(typeof(IPropertyGridCollectionItem).FullName) != null)
            {
                return ((IPropertyGridCollectionItem)value).GetItemInfo();
            }

            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
