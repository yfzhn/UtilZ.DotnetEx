using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Xml辅助类
    /// </summary>
    public static class XmlEx
    {
        private static object ConvertValue(string valueStr, Type targetType)
        {
            object result;
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                result = ConvertEx.GetTypeDefaultValue(targetType);
            }
            else
            {
                result = ConvertEx.ConvertToObject(targetType, valueStr);
            }

            return result;
        }

        private static bool TryConvertValue(string valueStr, Type targetType, out object result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                return false;
            }

            try
            {
                result = ConvertEx.ConvertToObject(targetType, valueStr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryConvertValue<T>(string valueStr, out T result)
        {
            object value;
            bool convertResult = TryConvertValue(valueStr, typeof(T), out value);
            if (convertResult)
            {
                result = (T)value;
            }
            else
            {
                result = default(T);
            }

            return convertResult;
        }




        #region Xml
        #region GetXmlNodeValue
        /// <summary>
        /// 获取XmlNode值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="nodeIsNullValueIsNull">元素为null时是否返回null[true:null;false:string.Empty],默认为false</param>
        /// <returns>值</returns>
        public static string GetXmlNodeValue(this XmlNode node, bool nodeIsNullValueIsNull = false)
        {
            if (node == null)
            {
                if (nodeIsNullValueIsNull)
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
                return node.InnerText;
            }
        }

        /// <summary>
        /// 获取XmlNode值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="targetType">数据目标类型</param>
        /// <returns>值</returns>
        public static object GetXmlNodeValue(this XmlNode node, Type targetType)
        {
            string value = GetXmlNodeValue(node, true);
            return ConvertValue(value, targetType);
        }

        /// <summary>
        /// 获取XmlNode值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <returns>值</returns>
        public static T GetXmlNodeValue<T>(this XmlNode node)
        {
            return (T)GetXmlNodeValue(node, typeof(T));
        }


        /// <summary>
        /// 尝试获取并转换XmlNode值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="targetType">数据目标类型</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXmlNodeValue(this XmlNode node, Type targetType, out object result)
        {
            string value = GetXmlNodeValue(node, true);
            return TryConvertValue(value, targetType, out result);
        }

        /// <summary>
        /// 尝试获取并转换XmlNode值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXmlNodeValue<T>(this XmlNode node, out T result)
        {
            string value = GetXmlNodeValue(node, true);
            return TryConvertValue<T>(value, out result);
        }
        #endregion


        #region GetXmlNodeAttributeValue
        /// <summary>
        /// 获取XmlNode指定属性值
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeNotExitValueIsNull">特性不存在时是否返回null值[true:null;false:string.Empty],默认为false</param>
        /// <returns>属性值</returns>
        public static string GetXmlNodeAttributeValue(this XmlNode node, string attributeName, bool attributeNotExitValueIsNull = false)
        {
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            if (node == null)
            {
                if (attributeNotExitValueIsNull)
                {
                    return null;
                }
                else
                {
                    return string.Empty;
                }
            }

            XmlAttribute attri = node.Attributes[attributeName];
            if (attri == null)
            {
                if (attributeNotExitValueIsNull)
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
        /// 获取XmlNode指定属性值
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="targetType">数据目标类型</param>
        /// <returns>属性值</returns>
        public static object GetXmlNodeAttributeValue(this XmlNode node, string attributeName, Type targetType)
        {
            string attiValue = GetXmlNodeAttributeValue(node, attributeName);
            return ConvertValue(attiValue, targetType);
        }

        /// <summary>
        /// 获取XmlNode指定属性值
        /// </summary>
        /// <typeparam name="T">数据目标泛型类型</typeparam>
        /// <param name="node">XmlNode</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>属性值</returns>
        public static T GetXmlNodeAttributeValue<T>(this XmlNode node, string attributeName)
        {
            return (T)GetXmlNodeAttributeValue(node, attributeName, typeof(T));
        }


        /// <summary>
        /// 尝试获取并转换XmlNode指定属性值
        /// </summary>
        /// <param name="node">XmlNode</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="targetType">数据目标类型</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXmlNodeAttributeValue(this XmlNode node, string attributeName, Type targetType, out object result)
        {
            string attiValue = GetXmlNodeAttributeValue(node, attributeName);
            return TryConvertValue(attiValue, targetType, out result);
        }

        /// <summary>
        /// 尝试获取并转换XmlNode指定属性值
        /// </summary>
        /// <typeparam name="T">数据目标泛型类型</typeparam>
        /// <param name="node">XmlNode</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXmlNodeAttributeValue<T>(this XmlNode node, string attributeName, out T result)
        {
            string attiValue = GetXmlNodeAttributeValue(node, attributeName);
            return TryConvertValue<T>(attiValue, out result);
        }
        #endregion


        #region SetXElementAttribute
        /// <summary>
        /// 设置XmlAttribute
        /// </summary>
        /// <param name="node">目标XmlNode元素</param>
        /// <param name="attributeName">特性名称</param>
        /// <param name="value">特性值</param>
        /// <param name="valueIsNullAttributeExit">当特性值为null时,是否存在该特性[true:存在;false:不存在]</param>
        public static void SetXmlNodeAttribute(this XmlNode node, string attributeName, string value, bool valueIsNullAttributeExit = false)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            XmlAttribute attri = node.Attributes[attributeName];
            if (value == null)
            {
                if (valueIsNullAttributeExit)
                {
                    if (attri == null)
                    {
                        attri = node.OwnerDocument.CreateAttribute(attributeName);
                        node.Attributes.Append(attri);
                    }

                    attri.Value = string.Empty;
                }
                else
                {
                    if (attri != null)
                    {
                        node.Attributes.Remove(attri);
                    }
                }
            }
            else
            {
                if (attri == null)
                {
                    attri = node.OwnerDocument.CreateAttribute(attributeName);
                    node.Attributes.Append(attri);
                }

                attri.Value = value;
            }
        }
        #endregion

        #region CreateXElement
        /// <summary>
        /// 创建XmlNode
        /// </summary>
        /// <param name="xdoc">所属XmlDocument</param>
        /// <param name="nodeName">要创建的子节点名称</param>
        /// <param name="attriName">属性名称</param>
        /// <param name="attiValue">属性值</param>
        /// <param name="nodeValue">节点值</param>
        /// <returns>XmlNode</returns>
        public static XmlNode CreateXmlNode(XmlDocument xdoc, string nodeName, string attriName, string attiValue, string nodeValue = null)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }

            if (string.IsNullOrWhiteSpace(attriName))
            {
                throw new ArgumentNullException(nameof(attriName));
            }

            XmlNode node = xdoc.CreateElement(nodeName);
            if (nodeValue != null)
            {
                node.InnerText = nodeValue;
            }

            SetXmlNodeAttribute(node, attriName, attiValue);
            return node;
        }

        /// <summary>
        /// 创建XmlCDataSection
        /// </summary>
        /// <param name="xdoc">所属XmlDocument</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="value">值</param>
        /// <returns>XmlCDataSection</returns>
        public static XmlCDataSection CreateXmlCDataSection(XmlDocument xdoc, string nodeName, string value)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                throw new ArgumentNullException(nameof(nodeName));
            }

            XmlCDataSection node = xdoc.CreateCDataSection(nodeName);
            if (value != null)
            {
                node.InnerText = value;
            }

            return node;
        }
        #endregion
        #endregion




        #region Xml.Linq
        #region GetXElementValue
        /// <summary>
        /// 获取XElement元素节点值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="eleIsNullValueIsNull">元素为null时是否返回null[true:null;false:string.Empty],默认为false</param>
        /// <returns>值</returns>
        public static string GetXElementValue(this XElement ele, bool eleIsNullValueIsNull = false)
        {
            if (ele == null)
            {
                if (eleIsNullValueIsNull)
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
            return (T)GetXElementValue(ele, typeof(T));
        }

        /// <summary>
        /// 获取XElement元素节点值[节点为null时返回空字符串]
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="targetType">数据目标类型</param>
        /// <returns>值</returns>
        public static object GetXElementValue(this XElement ele, Type targetType)
        {
            string value = null;
            if (ele != null)
            {
                value = ele.Value;
            }

            return ConvertValue(value, targetType);
        }


        /// <summary>
        /// 尝试获取并转换XElement元素节点值
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryTryGetXElementValue<T>(this XElement ele, out T result)
        {
            string value = GetXElementValue(ele, true);
            return TryConvertValue<T>(value, out result);
        }

        /// <summary>
        /// 尝试获取并转换XElement元素节点值
        /// </summary>
        /// <param name="ele">XElement节点</param>
        /// <param name="targetType">数据目标类型</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXElementValue(this XElement ele, Type targetType, out object result)
        {
            string value = GetXElementValue(ele, true);
            return TryConvertValue(value, targetType, out result);
        }
        #endregion

        #region GetXElementAttributeValue
        /// <summary>
        /// 获取XElement指定属性值
        /// </summary>
        /// <param name="ele">XElement</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="attributeNotExitValueIsNull">特性不存在时是否返回null[true:null;false:string.Empty],默认为false</param>
        /// <returns>属性值</returns>
        public static string GetXElementAttributeValue(this XElement ele, string attributeName, bool attributeNotExitValueIsNull = false)
        {
            if (string.IsNullOrEmpty(attributeName))
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            if (ele == null)
            {
                if (attributeNotExitValueIsNull)
                {
                    return null;
                }
                else
                {
                    return string.Empty;
                }
            }

            XAttribute attri = ele.Attribute(attributeName);
            if (attri == null)
            {
                if (attributeNotExitValueIsNull)
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
        /// 获取XElement指定属性值
        /// </summary>
        /// <param name="ele">XElement</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="targetType">数据目标类型</param>
        /// <returns>属性值</returns>
        public static object GetXElementAttributeValue(this XElement ele, string attributeName, Type targetType)
        {
            string attiValue = GetXElementAttributeValue(ele, attributeName);
            return ConvertValue(attiValue, targetType);
        }

        /// <summary>
        /// 获取XElement指定属性值
        /// </summary>
        /// <typeparam name="T">数据目标泛型类型</typeparam>
        /// <param name="ele">XElement</param>
        /// <param name="attributeName">属性名称</param>
        /// <returns>属性值</returns>
        public static T GetXElementAttributeValue<T>(this XElement ele, string attributeName)
        {
            return (T)GetXElementAttributeValue(ele, attributeName, typeof(T));
        }

        /// <summary>
        /// 尝试获取并转换XElement指定属性值
        /// </summary>
        /// <param name="ele">XElement</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="targetType">数据目标类型</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXElementAttributeValue(this XElement ele, string attributeName, Type targetType, out object result)
        {
            string attiValue = GetXElementAttributeValue(ele, attributeName);
            return TryConvertValue(attiValue, targetType, out result);
        }

        /// <summary>
        /// 尝试获取并转换XElement指定属性值
        /// </summary>
        /// <typeparam name="T">数据目标泛型类型</typeparam>
        /// <param name="ele">XElement</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="result">结果值</param>
        /// <returns>转换结果</returns>
        public static bool TryGetXElementAttributeValue<T>(this XElement ele, string attributeName, out T result)
        {
            string attiValue = GetXElementAttributeValue(ele, attributeName);
            return TryConvertValue<T>(attiValue, out result);
        }
        #endregion

        #region SetXElementAttribute
        /// <summary>
        /// 设置XAttribute
        /// </summary>
        /// <param name="ele">目标元素</param>
        /// <param name="attributeName">特性名称</param>
        /// <param name="value">特性值</param>
        /// <param name="valueIsNullAttributeExit">当特性值为null时,是否存在该特性[true:存在;false:不存在]</param>
        public static void SetXElementAttribute(this XElement ele, string attributeName, string value, bool valueIsNullAttributeExit = false)
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
                if (valueIsNullAttributeExit)
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
        #endregion

        #region CreateXElement
        /// <summary>
        /// 创建特性值xml元素节点
        /// </summary>
        /// <param name="eleName">节点名称</param>
        /// <param name="attriName">属性名称</param>
        /// <param name="attiValue">属性值</param>
        /// <param name="eleValue">节点值</param>
        /// <returns>特性值xml元素节点</returns>
        public static XElement CreateXElement(string eleName, string attriName, string attiValue, string eleValue = null)
        {
            if (string.IsNullOrWhiteSpace(eleName))
            {
                throw new ArgumentNullException(nameof(eleName));
            }

            XElement ele = new XElement(eleName);
            if (eleValue != null)
            {
                ele.Value = eleValue;
            }

            SetXElementAttribute(ele, attriName, attiValue, true);
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
            if (value != null)
            {
                ele.Add(new XCData(value));
            }

            return ele;
        }
        #endregion
        #endregion
    }
}
