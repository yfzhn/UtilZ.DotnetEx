using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.TypeConverters
{
    /// <summary>
    /// 属性表格密码编辑转换器
    /// </summary>
    public class PropertyGridPasswordConverter : TypeConverter
    {
        /// <summary>
        /// 字符串类型
        /// </summary>
        private readonly Type _stringType = typeof(string);

        /// <summary>
        /// 表格密码设置接口类型
        /// </summary>
        private readonly Type _ipropertyGridPasswordType = typeof(IPropertyGridPassword);

        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型[是否从显示文本转换为真实对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="sourceType">表示要转换的类型</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context.PropertyDescriptor.PropertyType == this._stringType)
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的对象转换为此转换器的类型[从显示文本转换为真实对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="culture">用作当前区域性的 CultureInfo</param>
        /// <param name="value">要转换的 Object</param>
        /// <returns>表示转换的 value 的 Object</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return value;
        }

        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型[是否能转换为显示对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="destinationType">表示要转换到的类型</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (context.PropertyDescriptor.PropertyType == this._stringType)
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定的值对象转换为指定的类型[转换为显示对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="culture">如果传递 null，则采用当前区域性</param>
        /// <param name="value">要转换的 Object</param>
        /// <param name="destinationType">value 参数要转换成的 Type</param>
        /// <returns>表示转换的 value 的 Object</returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (context == null || Type.GetTypeCode(context.PropertyDescriptor.PropertyType) != TypeCode.String)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            try
            {
                char passwordChar;
                if (context.Instance.GetType().GetInterface(this._ipropertyGridPasswordType.FullName) == null)
                {
                    passwordChar = '*';
                }
                else
                {
                    passwordChar = ((IPropertyGridPassword)context.Instance).GetPasswordChar(context.PropertyDescriptor.Name);
                }

                string valueStr = value == null ? string.Empty : value.ToString();
                if (string.IsNullOrEmpty(valueStr))
                {
                    return string.Empty;
                }
                else
                {
                    return new string(passwordChar, valueStr.Length);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return null;
            }
        }
    }
}
