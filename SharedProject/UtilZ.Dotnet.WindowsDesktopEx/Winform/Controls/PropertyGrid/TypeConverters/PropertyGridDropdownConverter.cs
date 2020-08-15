using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.TypeConverters
{
    /// <summary>
    /// 属性表格编辑下拉框转换器(用于枚举和基元集合或复杂对象集合下拉选择)
    /// </summary>
    [Serializable]
    public class PropertyGridDropdownConverter : TypeConverter
    {
        /// <summary>
        /// IPropertyGridDropDownList类型
        /// </summary>
        private static readonly Type _ipropertyGridDropDownListType = typeof(IPropertyGridDropDown);

        /// <summary>
        /// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <returns>如果应调用 GetStandardValues 来查找对象支持的一组公共值，则为 true；否则，为 false</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            try
            {
                if (context == null)
                {
                    return base.GetStandardValuesSupported(context);
                }

                return context.PropertyDescriptor.PropertyType.IsEnum || context.Instance.GetType().GetInterface(_ipropertyGridDropDownListType.FullName) != null;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取下拉框的显示枚举集合
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <returns>包含标准有效值集的 TypeConverter.StandardValuesCollection；如果数据类型不支持标准值集，则为null</returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            try
            {
                if (context == null)
                {
                    return base.GetStandardValues(context);
                }

                if (context.PropertyDescriptor.PropertyType.IsEnum)
                {
                    List<FieldInfoEx> dbiItems = EnumEx.GetEnumFieldInfoExList(context.PropertyDescriptor.PropertyType);
                    var enumItems = (from item in dbiItems select item.Value).ToArray();
                    return new StandardValuesCollection(enumItems);
                }
                else
                {

                    if (context.Instance.GetType().GetInterface(_ipropertyGridDropDownListType.FullName) != null && context.Instance != null)
                    {
                        System.Collections.ICollection collection = ((IPropertyGridDropDown)context.Instance).GetPropertyGridDropDownItems(context.PropertyDescriptor.Name);
                        if (collection != null)
                        {
                            return new StandardValuesCollection(collection);
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回该转换器是否可以使用指定的上下文将给定类型的对象转换为此转换器的类型[是否从显示文本转换为真实对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="sourceType">表示要转换的类型</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            try
            {
                if (context == null || sourceType == null)
                {
                    return base.CanConvertFrom(context, sourceType);
                }

                return context.PropertyDescriptor.PropertyType.IsEnum || context.Instance.GetType().GetInterface(_ipropertyGridDropDownListType.FullName) != null;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return false;
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
            try
            {
                if (context == null)
                {
                    return base.ConvertFrom(context, culture, value);
                }

                if (value == null)
                {
                    return value;
                }

                string valueStr = value.ToString();
                if (context.PropertyDescriptor.PropertyType.IsEnum)
                {
                    if (string.IsNullOrWhiteSpace(valueStr))
                    {
                        return base.ConvertFrom(context, culture, value);
                    }
                    else
                    {
                        return EnumEx.GetEnumByDisplayNameExAttributeDisplayName(context.PropertyDescriptor.PropertyType, valueStr);
                    }
                }
                else
                {
                    if (context.Instance.GetType().GetInterface(_ipropertyGridDropDownListType.FullName) == null)
                    {
                        return value;
                    }

                    IPropertyGridDropDown ipropertyGridDropDownList = (IPropertyGridDropDown)context.Instance;
                    System.Collections.ICollection collection = ipropertyGridDropDownList.GetPropertyGridDropDownItems(context.PropertyDescriptor.Name);
                    if (collection == null || collection.Count == 0)
                    {
                        return value;
                    }

                    Type instanceType = null;
                    foreach (var item in collection)
                    {
                        instanceType = item.GetType();
                        break;
                    }

                    if (instanceType.IsPrimitive)
                    {
                        if (value.GetType() != instanceType)
                        {
                            value = Convert.ChangeType(value, instanceType);
                        }

                        return value;
                    }
                    else
                    {
                        string displayPropertyName = ipropertyGridDropDownList.GetPropertyGridDisplayName(context.PropertyDescriptor.Name);
                        if (string.IsNullOrWhiteSpace(displayPropertyName))
                        {
                            //如果显示属性名称为空或null则直接用原始数据作比较,比如:字符串集合
                            foreach (var item in collection)
                            {
                                if (object.Equals(valueStr, item.ToString()))
                                {
                                    return item;
                                }
                            }

                            return null;
                        }
                        else
                        {
                            System.Reflection.PropertyInfo propertyInfo = instanceType.GetProperty(displayPropertyName);
                            if (propertyInfo == null)
                            {
                                System.Reflection.FieldInfo fieldInfo = instanceType.GetField(displayPropertyName);
                                if (fieldInfo == null)
                                {
                                    //如果属性名或字段名不正确,则也调用父类方法
                                    foreach (var item in collection)
                                    {
                                        if (object.Equals(value, item))
                                        {
                                            return item;
                                        }
                                    }

                                    return null;
                                }
                                else
                                {
                                    //通过字段反射获取值
                                    foreach (var item in collection)
                                    {
                                        if (object.Equals(fieldInfo.GetValue(item), value))
                                        {
                                            return item;
                                        }
                                    }

                                    return null;
                                }
                            }
                            else
                            {
                                //通过属性反射获取值
                                foreach (var item in collection)
                                {
                                    if (object.Equals(propertyInfo.GetValue(item, null), value))
                                    {
                                        return item;
                                    }
                                }

                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 返回此转换器是否可以使用指定的上下文将该对象转换为指定的类型[是否能转换为显示对象]
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <param name="destinationType">表示要转换到的类型</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            try
            {
                if (context == null || destinationType == null)
                {
                    return base.CanConvertTo(context, destinationType);
                }

                return context.PropertyDescriptor.PropertyType.IsEnum || context.Instance.GetType().GetInterface(_ipropertyGridDropDownListType.FullName) != null;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
                return false;
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
            try
            {
                if (context == null)
                {
                    return base.ConvertTo(context, culture, value, destinationType);
                }

                if (value == null)
                {
                    return value;
                }

                if (context.PropertyDescriptor.PropertyType.IsEnum)
                {
                    return EnumEx.GetEnumItemDisplayName(value);
                }
                else
                {
                    if (context.Instance.GetType().GetInterface(_ipropertyGridDropDownListType.FullName) == null)
                    {
                        return value;
                    }

                    Type valueType = value.GetType();
                    if (valueType.IsPrimitive)
                    {
                        return value;
                    }

                    IPropertyGridDropDown ipropertyGridDropDownList = (IPropertyGridDropDown)context.Instance;
                    string displayPropertyName = ipropertyGridDropDownList.GetPropertyGridDisplayName(context.PropertyDescriptor.Name);
                    //如果显示属性名称为空或null则调用父类方法
                    if (string.IsNullOrEmpty(displayPropertyName))
                    {
                        return value.ToString();
                    }

                    object retValue = null;
                    System.Reflection.PropertyInfo propertyInfo = valueType.GetProperty(displayPropertyName);
                    if (propertyInfo == null)
                    {
                        System.Reflection.FieldInfo fieldInfo = valueType.GetField(displayPropertyName);
                        if (fieldInfo == null)
                        {
                            //如果属性名或字段名不正确,则也调用父类方法
                            retValue = value;
                        }
                        else
                        {
                            //通过字段反射获取值
                            retValue = fieldInfo.GetValue(value);
                        }
                    }
                    else
                    {
                        //通过属性反射获取值
                        retValue = propertyInfo.GetValue(value, null);
                    }

                    return retValue;
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
