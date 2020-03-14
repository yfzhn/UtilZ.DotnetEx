using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Base;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.TypeConverters
{
    /// <summary>
    /// PropertyGrid排序Converter
    /// </summary>
    public class PropertyGridSortConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// 使用指定的上下文返回此对象是否支持可以从列表中选取的标准值集
        /// </summary>
        /// <param name="context">提供格式上下文</param>
        /// <returns>如果应调用 GetStandardValues 来查找对象支持的一组公共值，则为 true；否则，为 false</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// 获取由值参数指定的对象类型的属性集合
        /// </summary>
        /// <param name="context">一个 System.ComponentModel.ITypeDescriptorContext，用于提供格式上下文</param>
        /// <param name="value">System.Object ，它指定要获取其属性的对象类型</param>
        /// <param name="attributes">类型的数组 System.Attribute ，将用作筛选器</param>
        /// <returns>一个 System.ComponentModel.PropertyDescriptorCollection 与组件公开的属性或 null 是否存在任何属性</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (context == null || context.Instance == null)
            {
                return base.GetProperties(context, value, attributes);
            }

            PropertyDescriptorCollection pdc = base.GetProperties(context, value, attributes);
            string[] orderedPropertyNames;
            if (context.Instance.GetType().GetInterface(typeof(IPropertyGridOrder).FullName) != null)
            {
                IPropertyGridOrder propertyGridOrder = (IPropertyGridOrder)context.Instance;
                if (propertyGridOrder.OrderType == PropertyGridOrderType.Custom)
                {
                    var srcOrderedPropertyNames = new List<string>();
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        srcOrderedPropertyNames.Add(pd.Name);
                    }

                    orderedPropertyNames = propertyGridOrder.GetCustomSortPropertyName(srcOrderedPropertyNames);
                }
                else
                {
                    orderedPropertyNames = this.GetOrderedPropertyNames(pdc, propertyGridOrder.OrderType == PropertyGridOrderType.Ascending);
                }
            }
            else
            {
                orderedPropertyNames = this.GetOrderedPropertyNames(pdc, true);
            }

            return pdc.Sort(orderedPropertyNames);
        }

        /// <summary>
        /// 获取属性名称排序列表
        /// </summary>
        /// <param name="pdc">PropertyDescriptorCollection</param>
        /// <param name="orderFlag">true:升序;false:降序</param>
        /// <returns></returns>
        private string[] GetOrderedPropertyNames(PropertyDescriptorCollection pdc, bool orderFlag)
        {
            Type orderType = typeof(PropertyGridOrderAttribute);
            //PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
            var orderPropertyDescriptors = new List<Tuple<int, PropertyDescriptor>>();
            var noOrderPropertyNames = new List<string>();
            foreach (PropertyDescriptor pd in pdc)
            {
                Attribute attribute = pd.Attributes[orderType];
                if (attribute != null)
                {
                    PropertyGridOrderAttribute poa = (PropertyGridOrderAttribute)attribute;
                    orderPropertyDescriptors.Add(new Tuple<int, PropertyDescriptor>(poa.Order, pd));
                }
                else
                {
                    noOrderPropertyNames.Add(pd.Name);
                }
            }

            List<string> orderedProperties;
            if (orderFlag)
            {
                orderedProperties = (from tmpItem in orderPropertyDescriptors orderby tmpItem.Item1 ascending select tmpItem.Item2.Name).ToList();
            }
            else
            {
                orderedProperties = (from tmpItem in orderPropertyDescriptors orderby tmpItem.Item1 descending select tmpItem.Item2.Name).ToList();
            }

            if (noOrderPropertyNames.Count > 0)
            {
                noOrderPropertyNames = (from tmpItem in noOrderPropertyNames orderby tmpItem ascending select tmpItem).ToList();
                orderedProperties.AddRange(noOrderPropertyNames);
            }

            return orderedProperties.ToArray();
        }
    }
}
