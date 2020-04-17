using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Xml辅助类
    /// </summary>
    public static class XmlEx
    {
        #region ConvertValue
        private static T ConvertValue<T>(string attiValue)
        {
            if (string.IsNullOrWhiteSpace(attiValue))
            {
                return default(T);
            }

            return (T)ConvertEx.ToObject(typeof(T), attiValue);
        }

        private static object ConvertValue(string attiValue, Type targetType)
        {
            object value;
            if (string.IsNullOrWhiteSpace(attiValue))
            {
                value = ConvertEx.GetTypeDefaultValue(targetType);
            }
            else
            {
                value = ConvertEx.ToObject(targetType, attiValue);
            }

            return value;
        }
        #endregion

        #region GetXElementValue
        /// <summary>
        /// 获取XElement元素节点值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <returns>值</returns>
        public static string GetXElementValue(this XElement ele)
        {
            if (ele == null)
            {
                return string.Empty;
            }
            else
            {
                return ele.Value;
            }
        }

        /// <summary>
        /// 获取XElement元素节点值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <returns>值</returns>
        public static T GetXElementValue<T>(this XElement ele)
        {
            string valueStr;
            if (ele == null)
            {
                valueStr = null;
            }
            else
            {
                valueStr = ele.Value;
            }

            return ConvertValue<T>(valueStr);
        }

        /// <summary>
        /// 获取XElement元素节点值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="targetType">数据目标类型</param>
        /// <returns>值</returns>
        public static object GetXElementValue(this XElement ele, Type targetType)
        {
            string valueStr;
            if (ele == null)
            {
                valueStr = null;
            }
            else
            {
                valueStr = ele.Value;
            }

            return ConvertValue(valueStr, targetType);
        }
        #endregion

        #region GetXElementAttributeValue
        /// <summary>
        /// 获取XElement元素属性值
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="noValueNull">特性不存在时返回null值[true:人财两空null;false:返回string.Empty],默认为false</param>
        /// <returns>值</returns>
        public static string GetXElementAttributeValue(this XElement ele, string attributeName, bool noValueNull = false)
        {
            if (ele == null)
            {
                if (noValueNull)
                {
                    return null;
                }
                else
                {
                    return string.Empty;
                }
            }

            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            XAttribute attri = ele.Attribute(attributeName);
            if (attri == null)
            {
                if (noValueNull)
                {
                    return null;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return attri.Value;
            }
        }

        /// <summary>
        /// 获取XElement元素属性值
        /// </summary>
        /// <typeparam name="T">数据目标泛型类型</typeparam>
        /// <param name="ele">XElement节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns></returns>
        public static T GetXElementAttributeValue<T>(this XElement ele, string attributeName)
        {
            string attiValue = GetXElementAttributeValue(ele, attributeName);
            return ConvertValue<T>(attiValue);
        }

        /// <summary>
        /// 获取XElement元素属性值
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="targetType">数据目标类型</param>
        /// <returns></returns>
        public static object GetXElementAttributeValue(this XElement ele, string attributeName, Type targetType)
        {
            string attiValue = GetXElementAttributeValue(ele, attributeName);
            return ConvertValue(attiValue, targetType);
        }
        #endregion

        /// <summary>
        /// 设置XAttribute
        /// </summary>
        /// <param name="ele">目标元素</param>
        /// <param name="attributeName">特性名称</param>
        /// <param name="value">特性值</param>
        /// <param name="valueNullExitAttribute">当特性值为null时,是否存在该特性[true:存在;false:不存在]</param>
        public static void SetXElementAttribute(this XElement ele, string attributeName, string value, bool valueNullExitAttribute = false)
        {
            if (ele == null)
            {
                throw new ArgumentNullException(nameof(ele));
            }

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            XAttribute attribute = ele.Attribute(attributeName);
            if (value == null)
            {
                if (valueNullExitAttribute)
                {
                    if (attribute == null)
                    {
                        attribute = new XAttribute(attributeName, string.Empty);
                        ele.Add(attribute);
                    }
                    else
                    {
                        attribute.Value = string.Empty;
                    }
                }
                else
                {
                    if (attribute != null)
                    {
                        attribute.Remove();
                    }
                }
            }
            else
            {
                if (attribute == null)
                {
                    attribute = new XAttribute(attributeName, value);
                    ele.Add(attribute);
                }
                else
                {
                    attribute.Value = value;
                }
            }
        }

        #region CreateXElement
        /// <summary>
        /// 创建特性值xml元素节点
        /// </summary>
        /// <param name="eleName">节点名称</param>
        /// <param name="attriName">属性名称</param>
        /// <param name="attiValue">属性值</param>
        /// <param name="eleValue">节点值</param>
        /// <returns>特性值xml元素节点</returns>
        public static XElement CreateXElement(string eleName, string attriName, object attiValue, string eleValue = null)
        {
            if (string.IsNullOrWhiteSpace(eleName))
            {
                throw new ArgumentNullException(nameof(eleName));
            }

            if (string.IsNullOrWhiteSpace(attriName))
            {
                throw new ArgumentNullException(nameof(attriName));
            }

            XElement ele = new XElement(eleName);
            if (eleValue != null)
            {
                ele.Value = eleValue;
            }

            ele.Add(new XAttribute(attriName, attiValue));
            return ele;
        }

        /// <summary>
        /// 创建CDataxml元素节点
        /// </summary>
        /// <param name="eleName">节点名称</param>
        /// <param name="value">值</param>
        /// <returns>CDataxml元素节点</returns>
        public static XElement CreateXCDataXElement(string eleName, string value)
        {
            XElement ele = new XElement(eleName);
            ele.Add(new XCData(value));
            return ele;
        }
        #endregion
    }
}
