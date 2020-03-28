using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 配置读写类
    /// </summary>
    internal class ConfigCore
    {
        private const string _DIC_ITEM_ELE_NAME = "KeyValuePair";
        private const string _KEY = "Key";
        private const string _VALUE = "Value";




        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigCore()
        {

        }




        #region Common
        private Dictionary<Type, PropertyInfo[]> _typePropertyInfoArrDic = null;
        private PropertyInfo[] GetTypePropertyInfos(Type type)
        {
            if (this._typePropertyInfoArrDic == null)
            {
                this._typePropertyInfoArrDic = new Dictionary<Type, PropertyInfo[]>();
            }

            PropertyInfo[] propertyInfoArr;
            if (!this._typePropertyInfoArrDic.TryGetValue(type, out propertyInfoArr))
            {
                propertyInfoArr = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                this._typePropertyInfoArrDic.Add(type, propertyInfoArr);
            }

            return propertyInfoArr;
        }


        private Dictionary<Type, ConfigDataType> _typeConfigDataTypeDic = null;
        private ConfigDataType GetDataType(Type type)
        {
            if (this._typeConfigDataTypeDic == null)
            {
                this._typeConfigDataTypeDic = new Dictionary<Type, ConfigDataType>();
            }

            ConfigDataType configDataType;
            if (!this._typeConfigDataTypeDic.TryGetValue(type, out configDataType))
            {
                Type targetType = Nullable.GetUnderlyingType(type);
                if (targetType == null)
                {
                    targetType = type;
                }

                if (targetType.IsPrimitive || targetType.IsEnum)
                {
                    //基元类型: Boolean、 Byte、 SByte、 Int16、 UInt16、 Int32、 UInt32、 Int64、 UInt64、 IntPtr、 UIntPtr、 Char、 Double 和 Single。
                    //系统基元类型、枚举
                    configDataType = ConfigDataType.Basic;
                }
                else
                {
                    TypeCode typeCode = Type.GetTypeCode(targetType);
                    if (typeCode == TypeCode.String ||
                        typeCode == TypeCode.DateTime)
                    {
                        //string、DateTime
                        configDataType = ConfigDataType.Basic;
                    }
                    else
                    {
                        if (targetType.GetInterface(typeof(IDictionary).FullName) != null)
                        {
                            configDataType = ConfigDataType.IDictionary;
                        }
                        else if (targetType.GetInterface(typeof(IList).FullName) != null)
                        {
                            configDataType = ConfigDataType.IList;
                        }
                        else
                        {
                            configDataType = ConfigDataType.Object;
                        }
                    }
                }

                this._typeConfigDataTypeDic.Add(type, configDataType);
            }

            return configDataType;
        }


        private Dictionary<Type, Type> _typeElementTypeDic = null;
        private Type GetIListElementType(Type type)
        {
            if (this._typeElementTypeDic == null)
            {
                this._typeElementTypeDic = new Dictionary<Type, Type>();
            }

            Type eleType;

            if (!this._typeElementTypeDic.TryGetValue(type, out eleType))
            {
                if (type.IsArray)
                {
                    string eleTypeFullName = type.FullName.Substring(0, type.FullName.Length - 2);
                    eleType = type.Assembly.GetType(eleTypeFullName);
                    //eleType = Type.GetType(eleTypeFullName);
                }
                else if (type.IsGenericType)
                {
                    eleType = type.GetGenericArguments().First();
                }
                else
                {
                    throw new NotImplementedException(type.FullName);
                }

                this._typeElementTypeDic.Add(type, eleType);
            }

            return eleType;
        }

        private string GetIDictionaryName(string name, PropertyInfo propertyInfo)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = propertyInfo.Name;
            }
            return name;
        }

        private string GetIDictionaryElementName(string eleName)
        {
            if (string.IsNullOrWhiteSpace(eleName))
            {
                eleName = _DIC_ITEM_ELE_NAME;
            }

            return eleName;
        }

        private string GetConfigItemName(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }

        private Dictionary<Type, string> _typeIListElementNameDic = null;
        private string GetIListElementName(Type eleType)
        {
            if (this._typeIListElementNameDic == null)
            {
                this._typeIListElementNameDic = new Dictionary<Type, string>();
            }

            string iListElementName;
            if (!this._typeIListElementNameDic.TryGetValue(eleType, out iListElementName))
            {
                const char _GENERIC_TYIPE_NAME_SPLIT = '`';
                int index = eleType.Name.IndexOf(_GENERIC_TYIPE_NAME_SPLIT);
                if (index < 0)
                {
                    iListElementName = eleType.Name;
                }
                else
                {
                    iListElementName = eleType.Name.Substring(0, index);
                }

                this._typeIListElementNameDic.Add(eleType, iListElementName);
            }

            return iListElementName;
        }
        #endregion






        #region Write
        /// <summary>
        /// 写配置到XDocument
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns>配置XDocument</returns>
        public XDocument WriteConfigToXDocument(object config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            ConfigAttributeTypes configAttributeTypes = new ConfigAttributeTypes();
            XDocument xdoc = new XDocument();
            Type configType = config.GetType();
            ConfigRootAttribute configRootAttribute = ConfigRootAttribute.GetRootConfigRootAttribute(configType, configAttributeTypes);
            XElement rootEle = new XElement(configRootAttribute.GetName(configType));
            this.AddDes(rootEle, configRootAttribute.Des);

            PropertyInfo[] propertyInfoArr = this.GetTypePropertyInfos(configType);
            this.WriteConfigToXml(rootEle, propertyInfoArr, config, configAttributeTypes);
            xdoc.Add(rootEle);
            return xdoc;
        }

        private void WriteConfigToXml(XElement ownerEle, PropertyInfo[] ownerObjPropertyInfoArr, object ownerObj, ConfigAttributeTypes configAttributeTypes)
        {
            if (ownerObj == null)
            {
                return;
            }

            ConfigAttribute attri;
            object propertyValue;
            string name, eleName, comment;
            Type eleType;
            ConfigDataType configDataType;

            foreach (var propertyInfo in ownerObjPropertyInfoArr)
            {
                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.IgnoreAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //忽略该属性
                    continue;
                }

                propertyValue = propertyInfo.GetValue(ownerObj, null);
                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.ConfigCommentAttributeType, false) as ConfigCommentAttribute;
                if (attri != null)
                {
                    //注释
                    if (propertyValue != null)
                    {
                        if (propertyValue is string)
                        {
                            comment = (string)propertyValue;
                        }
                        else
                        {
                            comment = propertyValue.ToString();
                        }

                        ownerEle.Add(new XComment(comment));
                    }

                    continue;
                }


                configDataType = this.GetDataType(propertyInfo.PropertyType);

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.CustomerAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //自定义
                    var configItemCustomerAttribute = (ConfigItemCustomerAttribute)attri;
                    configItemCustomerAttribute.CustomerConfig.Write(propertyInfo, propertyValue, ownerEle, configItemCustomerAttribute);
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.CollectionAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //集合
                    var collectionAtt = (ConfigCollectionAttribute)attri;
                    name = collectionAtt.GetName(propertyInfo);
                    if (configDataType == ConfigDataType.IDictionary)
                    {
                        eleName = this.GetIDictionaryElementName(collectionAtt.ElementName);
                        this.WriteIDictionary(ownerEle, (IDictionary)propertyValue, propertyInfo.PropertyType, name, eleName, collectionAtt.Des, configAttributeTypes);
                    }
                    else if (configDataType == ConfigDataType.IList)
                    {
                        if (collectionAtt != null && propertyValue == null && !collectionAtt.AllowNullValueElement)
                        {
                            continue;
                        }

                        eleType = this.GetIListElementType(propertyInfo.PropertyType);
                        eleName = collectionAtt.GetElementName(eleType);
                        this.WriteIList(name, eleName, collectionAtt.Des, ownerEle, (IList)propertyValue, propertyInfo.PropertyType, configAttributeTypes);
                    }
                    else
                    {
                        throw new NotImplementedException(configDataType.ToString());
                    }
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.ObjectAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //复合对象
                    this.WriteObject(ownerEle, configAttributeTypes, attri, propertyValue, propertyInfo);
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.ItemAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //基元项
                    this.WriteItem(ownerEle, (ConfigItemAttribute)attri, propertyValue, propertyInfo);
                    continue;
                }


                //未标记项====================================================================================================


                switch (configDataType)
                {
                    case ConfigDataType.IDictionary:
                        name = this.GetIDictionaryName(null, propertyInfo);
                        eleName = this.GetIDictionaryElementName(null);
                        this.WriteIDictionary(ownerEle, (IDictionary)propertyValue, propertyInfo.PropertyType, name, eleName, null, configAttributeTypes);
                        break;
                    case ConfigDataType.IList:
                        name = this.GetConfigItemName(propertyInfo);
                        eleType = this.GetIListElementType(propertyInfo.PropertyType);
                        eleName = this.GetIListElementName(eleType);
                        this.WriteIList(name, eleName, null, ownerEle, (IList)propertyValue, propertyInfo.PropertyType, configAttributeTypes);
                        break;
                    case ConfigDataType.Object:
                        this.WriteObject(ownerEle, configAttributeTypes, propertyValue, propertyInfo);
                        break;
                    case ConfigDataType.Basic:
                        this.WriteItem(ownerEle, propertyValue, propertyInfo);
                        break;
                    default:
                        throw new NotImplementedException(configDataType.ToString());
                }
            }
        }


        private void WriteIList(string name, string eleName, string des, XElement ownerEle, IList list, Type listType, ConfigAttributeTypes configAttributeTypes)
        {
            XElement collectionEle = new XElement(name);
            this.AddDes(collectionEle, des);
            ownerEle.Add(collectionEle);

            var eleType = this.GetIListElementType(listType);
            this.WriteIList(eleName, collectionEle, list, eleType, configAttributeTypes);
        }


        private void WriteIList(string name, XElement collectionEle, IList list, Type eleType, ConfigAttributeTypes configAttributeTypes)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            string eleName = this.GetIListElementName(eleType), eleName2 = null;
            ConfigDataType configDataType = this.GetDataType(eleType);
            switch (configDataType)
            {
                case ConfigDataType.IDictionary:
                    eleName2 = this.GetIDictionaryElementName(null);
                    foreach (var item in list)
                    {
                        this.WriteIDictionary(collectionEle, (IDictionary)item, eleType, eleName, eleName2, null, configAttributeTypes);
                    }
                    break;
                case ConfigDataType.IList:
                    var itemType2 = this.GetIListElementType(eleType);
                    eleName = this.GetIListElementName(itemType2);
                    foreach (var item in list)
                    {
                        XElement itemEle = new XElement(name);
                        collectionEle.Add(itemEle);
                        this.WriteIList(eleName, itemEle, (IList)item, itemType2, configAttributeTypes);
                    }
                    break;
                case ConfigDataType.Object:
                    PropertyInfo[] objPropertyInfoArr = this.GetTypePropertyInfos(eleType);
                    foreach (var item in list)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        XElement childEle = new XElement(name);
                        this.WriteConfigToXml(childEle, objPropertyInfoArr, item, configAttributeTypes);
                        collectionEle.Add(childEle);
                    }
                    break;
                case ConfigDataType.Basic:
                    foreach (var item in list)
                    {
                        this.WriteItem(collectionEle, name, item, null);
                    }
                    break;
                default:
                    throw new NotImplementedException(configDataType.ToString());
            }
        }


        private void WriteIDictionary(XElement ownerEle, IDictionary dic, Type dicType, string name, string eleName, string des, ConfigAttributeTypes configAttributeTypes)
        {
            if (dic == null || dic.Count == 0)
            {
                return;
            }

            XElement dicEle = new XElement(name);
            this.AddDes(dicEle, des);
            ownerEle.Add(dicEle);

            object value;
            Type[] argsTypeArr = dicType.GetGenericArguments();
            ConfigDataType keyDataType = this.GetDataType(argsTypeArr[0]);
            ConfigDataType valueDataType = this.GetDataType(argsTypeArr[1]);
            PropertyInfo[] objPropertyInfoArr = null;
            string name2 = null, eleName2 = null, eleName3 = null;
            Type eleType = null;

            foreach (var key in dic.Keys)
            {
                XElement eleEle = new XElement(eleName);
                dicEle.Add(eleEle);

                //key
                XElement keyEle = new XElement(_KEY);
                switch (keyDataType)
                {
                    case ConfigDataType.Basic:
                        keyEle.Add(new XAttribute(_VALUE, key.ToString()));
                        break;
                    case ConfigDataType.IDictionary:
                    case ConfigDataType.IList:
                    case ConfigDataType.Object:
                    default:
                        throw new NotSupportedException($"字典集合的key只能为基础类型数据,类型{keyDataType.ToString()}无效");
                }
                eleEle.Add(keyEle);


                value = dic[key];
                XElement valueEle = new XElement(_VALUE);
                switch (valueDataType)
                {
                    case ConfigDataType.Basic:
                        if (value != null)
                        {
                            valueEle.Add(new XAttribute(_VALUE, value.ToString()));
                        }
                        break;
                    case ConfigDataType.IDictionary:
                        if (name2 == null)
                        {
                            name2 = this.GetIListElementName(argsTypeArr[1]);
                        }

                        if (eleName2 == null)
                        {
                            eleName2 = this.GetIDictionaryElementName(null);
                        }

                        this.WriteIDictionary(valueEle, (IDictionary)value, argsTypeArr[1], name2, eleName2, null, configAttributeTypes);
                        break;
                    case ConfigDataType.IList:
                        if (eleType == null)
                        {
                            eleType = this.GetIListElementType(argsTypeArr[1]);
                        }

                        if (eleName2 == null)
                        {
                            eleName2 = this.GetIListElementName(argsTypeArr[1]);
                        }

                        if (eleName3 == null)
                        {
                            eleName3 = this.GetIListElementName(eleType);
                        }
                        XElement listChildEle = new XElement(eleName2);
                        this.WriteIList(eleName3, listChildEle, (IList)value, eleType, configAttributeTypes);
                        valueEle.Add(listChildEle);
                        break;
                    case ConfigDataType.Object:
                        if (objPropertyInfoArr == null)
                        {
                            objPropertyInfoArr = this.GetTypePropertyInfos(argsTypeArr[1]);
                        }

                        if (eleName2 == null)
                        {
                            eleName2 = this.GetIListElementName(argsTypeArr[1]);
                        }

                        XElement childEle = new XElement(eleName2);
                        this.WriteConfigToXml(childEle, objPropertyInfoArr, value, configAttributeTypes);
                        valueEle.Add(childEle);
                        break;
                    default:
                        throw new NotSupportedException(valueDataType.ToString());
                }

                eleEle.Add(valueEle);
            }
        }






        private void WriteObject(XElement ownerEle, ConfigAttributeTypes configAttributeTypes, object propertyValue, PropertyInfo propertyInfo)
        {
            string complexEleName = this.GetConfigItemName(propertyInfo);
            XElement complexEle = new XElement(complexEleName);
            ownerEle.Add(complexEle);

            if (propertyValue != null)
            {
                PropertyInfo[] complexPropertyInfoArr = this.GetTypePropertyInfos(propertyInfo.PropertyType);
                this.WriteConfigToXml(complexEle, complexPropertyInfoArr, propertyValue, configAttributeTypes);
            }
        }

        private void WriteObject(XElement ownerEle, ConfigAttributeTypes configAttributeTypes, ConfigAttribute attri, object propertyValue, PropertyInfo propertyInfo)
        {
            if (!attri.AllowNullValueElement && propertyValue == null)
            {
                return;
            }

            XElement complexEle = new XElement(attri.GetName(propertyInfo));
            this.AddDes(complexEle, attri.Des);
            ownerEle.Add(complexEle);

            if (propertyValue != null)
            {
                PropertyInfo[] complexPropertyInfoArr = this.GetTypePropertyInfos(propertyInfo.PropertyType);
                this.WriteConfigToXml(complexEle, complexPropertyInfoArr, propertyValue, configAttributeTypes);
            }
        }


        private void WriteItem(XElement ownerEle, object propertyInfoValue, PropertyInfo propertyInfo)
        {
            string value = this.GetStringValue(null, propertyInfoValue, propertyInfo);
            string childEleName = this.GetConfigItemName(propertyInfo);
            this.WriteItem(ownerEle, childEleName, propertyInfoValue, null);
        }


        private void WriteItem(XElement ownerEle, ConfigItemAttribute attri, object propertyValue, PropertyInfo propertyInfo)
        {
            if (!attri.AllowNullValueElement && propertyValue == null)
            {
                return;
            }

            string value = this.GetStringValue(attri.Converter, propertyValue, propertyInfo);
            string childEleName = attri.GetName(propertyInfo);
            this.WriteItem(ownerEle, childEleName, value, attri.Des);
        }

        private void WriteItem(XElement ownerEle, string eleName, object value, string des)
        {
            if (string.IsNullOrWhiteSpace(eleName))
            {
                throw new ArgumentNullException(nameof(eleName));
            }

            XElement ele = new XElement(eleName);
            if (value != null)
            {
                ele.Add(new XAttribute(_VALUE, value));
            }

            this.AddDes(ele, des);
            ownerEle.Add(ele);
        }

        private string GetStringValue(IConfigValueConverter converter, object propertyInfoValue, PropertyInfo propertyInfo)
        {
            string strValue;
            if (converter != null)
            {
                strValue = converter.ConvertTo(propertyInfo, propertyInfoValue);
            }
            else
            {
                if (propertyInfoValue == null)
                {
                    strValue = string.Empty;
                }
                else
                {
                    if (propertyInfoValue is string)
                    {
                        strValue = (string)propertyInfoValue;
                    }
                    else
                    {
                        strValue = propertyInfoValue.ToString();
                    }
                }
            }

            return strValue;
        }

        private void AddDes(XElement ele, string des)
        {
            if (!string.IsNullOrWhiteSpace(des))
            {
                ele.Add(new XAttribute(nameof(ConfigItemAttribute.Des), des));
            }
        }
        #endregion





        #region Read
        /// <summary>
        /// 从xml配置中读取配置到指定的配置对象中
        /// </summary>
        /// <param name="xdoc">xml配置</param>
        /// <param name="config">指定的配置对象</param>
        public void ReadConfigFromXDocument(XDocument xdoc, ref object config)
        {
            ConfigAttributeTypes configAttributeTypes = new ConfigAttributeTypes();
            Type configType = config.GetType();
            ConfigRootAttribute configRootAttribute = ConfigRootAttribute.GetRootConfigRootAttribute(configType, configAttributeTypes);
            XElement rootEle = xdoc.XPathSelectElement(configRootAttribute.GetName(configType));
            if (rootEle == null)
            {
                return;
            }

            PropertyInfo[] propertyInfoArr = this.GetTypePropertyInfos(configType);
            this.ReadConfigToXml(rootEle, propertyInfoArr, ref config, configAttributeTypes);
        }




        private Dictionary<string, XElement> GetElementsDic(XElement ownerEle)
        {
            var eleDic = new Dictionary<string, XElement>();
            foreach (var ele in ownerEle.Elements())
            {
                eleDic[ele.Name.LocalName] = ele;
            }

            return eleDic;
        }

        private void ReadConfigToXml(XElement ownerEle, PropertyInfo[] propertyInfoArr, ref object ownerObj, ConfigAttributeTypes configAttributeTypes)
        {
            ConfigAttribute attri;
            Dictionary<string, XElement> eleDic = this.GetElementsDic(ownerEle);
            ConfigDataType configDataType;
            string name, eleName;
            object customerValue = null;
            XElement ele;
            Type eleType;

            foreach (var propertyInfo in propertyInfoArr)
            {
                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.IgnoreAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //忽略该属性
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.ConfigCommentAttributeType, false) as ConfigCommentAttribute;
                if (attri != null)
                {
                    //注释
                    continue;
                }

                configDataType = this.GetDataType(propertyInfo.PropertyType);
                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.CustomerAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    var configItemCustomerAttribute = (ConfigItemCustomerAttribute)attri;
                    customerValue = configItemCustomerAttribute.CustomerConfig.Read(propertyInfo, ownerEle, configItemCustomerAttribute);
                    propertyInfo.SetValue(ownerObj, customerValue, null);
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.CollectionAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    var collectionAtt = (ConfigCollectionAttribute)attri;
                    name = collectionAtt.GetName(propertyInfo);
                    if (!eleDic.TryGetValue(name, out ele))
                    {
                        //属性对应的配置项不存在,忽略
                        continue;
                    }

                    if (configDataType == ConfigDataType.IDictionary)
                    {
                        eleName = this.GetIDictionaryElementName(collectionAtt.ElementName);
                        this.ReadIDictionary(ref ownerObj, ele.Elements(eleName).ToArray(), propertyInfo, configAttributeTypes);
                    }
                    else
                    {
                        eleType = this.GetIListElementType(propertyInfo.PropertyType);
                        eleName = collectionAtt.GetElementName(eleType);
                        this.ReadIList(ref ownerObj, propertyInfo, ele.Elements(eleName).ToArray(), configAttributeTypes);
                    }
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.ObjectAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //复合对象
                    name = attri.GetName(propertyInfo);
                    if (!eleDic.TryGetValue(name, out ele))
                    {
                        //属性对应的配置项不存在,忽略
                        continue;
                    }

                    this.ReadObject(ele, configAttributeTypes, propertyInfo, ref ownerObj);
                    continue;
                }

                attri = propertyInfo.GetCustomAttribute(configAttributeTypes.ItemAttributeType, false) as ConfigAttribute;
                if (attri != null)
                {
                    //基元项
                    name = attri.GetName(propertyInfo);
                    if (!eleDic.TryGetValue(name, out ele))
                    {
                        //属性对应的配置项不存在,忽略
                        continue;
                    }

                    this.ReadItem(ele, ((ConfigItemAttribute)attri).Converter, propertyInfo, ownerObj);
                    continue;
                }


                //未标记项====================================================================================================


                switch (configDataType)
                {
                    case ConfigDataType.IDictionary:
                        name = this.GetIDictionaryName(null, propertyInfo);
                        if (!eleDic.TryGetValue(name, out ele))
                        {
                            //属性对应的配置项不存在,忽略
                            continue;
                        }

                        eleName = this.GetIDictionaryElementName(null);
                        this.ReadIDictionary(ref ownerObj, ele.Elements(eleName).ToArray(), propertyInfo, configAttributeTypes);
                        break;
                    case ConfigDataType.IList:
                        eleName = this.GetConfigItemName(propertyInfo);
                        if (!eleDic.TryGetValue(eleName, out ele))
                        {
                            //属性对应的配置项不存在,忽略
                            continue;
                        }

                        eleType = this.GetIListElementType(propertyInfo.PropertyType);
                        eleName = this.GetIListElementName(eleType);
                        this.ReadIList(ref ownerObj, propertyInfo, ele.Elements(eleName).ToArray(), configAttributeTypes);
                        break;
                    case ConfigDataType.Object:
                        string complexEleName = this.GetConfigItemName(propertyInfo);
                        if (!eleDic.TryGetValue(complexEleName, out ele))
                        {
                            //属性对应的配置项不存在,忽略
                            continue;
                        }

                        this.ReadObject(ele, configAttributeTypes, propertyInfo, ref ownerObj);
                        break;
                    case ConfigDataType.Basic:
                        eleName = this.GetConfigItemName(propertyInfo);
                        if (!eleDic.TryGetValue(eleName, out ele))
                        {
                            //属性对应的配置项不存在,忽略
                            continue;
                        }

                        this.ReadItem(ele, null, propertyInfo, ownerObj);
                        break;
                    default:
                        throw new NotImplementedException(configDataType.ToString());
                }
            }
        }

        private void ReadIDictionary(ref object ownerObj, XElement[] eleEleArr, PropertyInfo propertyInfo, ConfigAttributeTypes configAttributeTypes)
        {
            var dic = (IDictionary)propertyInfo.GetValue(ownerObj, null);
            if (dic == null)
            {
                if (!propertyInfo.CanWrite)
                {
                    throw new ArgumentException($"属性{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}不支持set操作");
                }

                dic = ActivatorEx.CreateInstance<IDictionary>(propertyInfo.PropertyType);
                propertyInfo.SetValue(ownerObj, dic, null);
            }

            this.ReadIDictionary(dic, eleEleArr, propertyInfo.PropertyType, configAttributeTypes);
        }

        private void ReadIDictionary(IDictionary dic, XElement[] eleEleArr, Type eleType, ConfigAttributeTypes configAttributeTypes)
        {
            XElement keyEle, valueEle;
            string keyStr, valueStr;
            object key, value = null;
            Type[] argsTypeArr = eleType.GetGenericArguments();
            ConfigDataType configDataType = this.GetDataType(argsTypeArr[1]);
            string name2 = null, eleName2 = null, eleName3 = null;
            Type eleType2 = null;
            PropertyInfo[] objPropertyInfoArr = null;
            XElement[] eleEleArr2 = null;

            foreach (var childEle in eleEleArr)
            {
                keyEle = childEle.Element(_KEY);
                keyStr = XmlEx.GetXElementAttributeValue(keyEle, _VALUE, false);
                key = this.StringToObject(keyStr, argsTypeArr[0]);

                valueEle = childEle.Element(_VALUE);
                switch (configDataType)
                {
                    case ConfigDataType.Basic:
                        valueStr = XmlEx.GetXElementAttributeValue(valueEle, _VALUE, true);
                        if (valueStr == null)
                        {
                            value = null;
                        }
                        else
                        {
                            value = this.StringToObject(valueStr, argsTypeArr[1]);
                        }
                        break;
                    case ConfigDataType.IDictionary:
                        if (name2 == null)
                        {
                            name2 = this.GetIListElementName(argsTypeArr[1]);
                        }

                        if (eleName2 == null)
                        {
                            eleName2 = this.GetIDictionaryElementName(null);
                        }

                        value = Activator.CreateInstance(argsTypeArr[1]);
                        this.ReadIDictionary((IDictionary)value, valueEle.XPathSelectElements($"{name2}/{eleName2}").ToArray(), argsTypeArr[1], configAttributeTypes);
                        break;
                    case ConfigDataType.IList:
                        if (eleType2 == null)
                        {
                            eleType2 = this.GetIListElementType(argsTypeArr[1]);
                        }

                        if (eleName2 == null)
                        {
                            eleName2 = this.GetIListElementName(argsTypeArr[1]);
                        }

                        if (eleName3 == null)
                        {
                            eleName3 = this.GetIListElementName(eleType2);
                        }

                        eleEleArr2 = valueEle.XPathSelectElements($"{eleName2}/{eleName3}").ToArray();
                        value = this.CreateIList(argsTypeArr[1], eleEleArr2.Length);
                        this.ReadIList((IList)value, eleType2, eleEleArr2, configAttributeTypes);
                        break;
                    case ConfigDataType.Object:
                        if (objPropertyInfoArr == null)
                        {
                            objPropertyInfoArr = this.GetTypePropertyInfos(argsTypeArr[1]);
                        }

                        if (eleName2 == null)
                        {
                            eleName2 = this.GetIListElementName(argsTypeArr[1]);
                        }

                        value = Activator.CreateInstance(argsTypeArr[1]);
                        this.ReadConfigToXml(valueEle.Element(eleName2), objPropertyInfoArr, ref value, configAttributeTypes);
                        break;
                    default:
                        throw new NotSupportedException(configDataType.ToString());
                }

                dic[key] = value;
            }
        }



        private void ReadIList(ref object ownerObj, PropertyInfo propertyInfo, XElement[] eleEleArr, ConfigAttributeTypes configAttributeTypes)
        {
            //非字典集合类型
            var list = (IList)propertyInfo.GetValue(ownerObj, null);
            if (list == null)
            {
                if (!propertyInfo.CanWrite)
                {
                    throw new ArgumentException($"属性{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}不支持set操作");
                }

                list = this.CreateIList(propertyInfo.PropertyType, eleEleArr.Length);
                propertyInfo.SetValue(ownerObj, list, null);
            }

            Type eleType = this.GetIListElementType(propertyInfo.PropertyType);
            this.ReadIList(list, eleType, eleEleArr, configAttributeTypes);
        }

        private void ReadIList(IList list, Type eleType, XElement[] eleEleArr, ConfigAttributeTypes configAttributeTypes)
        {
            ConfigDataType configDataType = this.GetDataType(eleType);
            string value, eleName = null;
            object obj;

            switch (configDataType)
            {
                case ConfigDataType.IDictionary:
                    foreach (var eleEle in eleEleArr)
                    {
                        obj = Activator.CreateInstance(eleType);
                        this.ReadIDictionary((IDictionary)obj, eleEle.Elements(eleName).ToArray(), eleType, configAttributeTypes);
                        list.Add(obj);
                    }
                    break;
                case ConfigDataType.IList:
                    var itemType2 = this.GetIListElementType(eleType);
                    eleName = this.GetIListElementName(itemType2);
                    XElement[] eleEleArr2;
                    foreach (var eleEle in eleEleArr)
                    {
                        eleEleArr2 = eleEle.Elements(eleName).ToArray();
                        obj = this.CreateIList(eleType, eleEleArr2.Length);
                        this.ReadIList((IList)obj, itemType2, eleEleArr2, configAttributeTypes);
                        list.Add(obj);
                    }
                    break;
                case ConfigDataType.Object:
                    PropertyInfo[] objPropertyInfoArr = this.GetTypePropertyInfos(eleType);
                    int index = 0;
                    foreach (var eleEle in eleEleArr)
                    {
                        obj = Activator.CreateInstance(eleType);
                        this.ReadConfigToXml(eleEle, objPropertyInfoArr, ref obj, configAttributeTypes);
                        if (list is Array)
                        {
                            list[index++] = obj;
                        }
                        else
                        {
                            list.Add(obj);
                        }
                    }
                    break;
                case ConfigDataType.Basic:
                    foreach (var eleEle in eleEleArr)
                    {
                        value = XmlEx.GetXElementAttributeValue(eleEle, _VALUE, true);
                        list.Add(this.StringToObject(value, eleType));
                    }
                    break;
                default:
                    throw new NotImplementedException(configDataType.ToString());
            }
        }

        private IList CreateIList(Type ilistType, int eleCount)
        {
            IList list;
            if (ilistType.IsArray)
            {
                Type eleType = this.GetIListElementType(ilistType);
                list = Array.CreateInstance(eleType, eleCount);
            }
            else
            {
                list = ActivatorEx.CreateInstance<IList>(ilistType);
            }

            return list;
        }


        private void ReadObject(XElement ele, ConfigAttributeTypes configAttributeTypes, PropertyInfo propertyInfo, ref object ownerObj)
        {
            object value = propertyInfo.GetValue(ownerObj, null);
            if (value == null)
            {
                if (!propertyInfo.CanWrite)
                {
                    throw new ArgumentException($"属性{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}不支持set操作");
                }

                value = Activator.CreateInstance(propertyInfo.PropertyType);
                if (!propertyInfo.PropertyType.IsValueType)
                {
                    propertyInfo.SetValue(ownerObj, value, null);
                }
            }

            PropertyInfo[] complexProInfoArr = this.GetTypePropertyInfos(propertyInfo.PropertyType);
            this.ReadConfigToXml(ele, complexProInfoArr, ref value, configAttributeTypes);

            if (propertyInfo.PropertyType.IsValueType)
            {
                propertyInfo.SetValue(ownerObj, value, null);
            }
        }


        private void ReadItem(XElement ele, IConfigValueConverter converter, PropertyInfo propertyInfo, object ownerObj)
        {
            if (!propertyInfo.CanWrite)
            {
                throw new ArgumentException($"属性{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}不支持set操作");
            }

            object value;
            string valueStr = XmlEx.GetXElementAttributeValue(ele, _VALUE, true);
            if (valueStr == null)
            {
                value = null;
            }
            else
            {
                if (converter != null)
                {
                    value = converter.ConvertFrom(propertyInfo, valueStr);
                }
                else
                {
                    value = this.StringToObject(valueStr, propertyInfo.PropertyType);
                }
            }

            propertyInfo.SetValue(ownerObj, value, null);
        }


        private object StringToObject(string str, Type targetType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            object value = str;
            if (Type.GetTypeCode(targetType) != TypeCode.String)
            {
                value = ConvertEx.ToObject(targetType, str);
            }

            return value;
        }
        #endregion
    }
}
