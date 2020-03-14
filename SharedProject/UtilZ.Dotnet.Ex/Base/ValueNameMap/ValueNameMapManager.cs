using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.ValueNameMap
{
    /// <summary>
    /// 值名称映射管理类
    /// </summary>
    public class ValueNameMapManager
    {
        private readonly static ConcurrentDictionary<Type, ValueNameDictionary> _typeValueNameDictionaryDic = new ConcurrentDictionary<Type, ValueNameDictionary>();
        private readonly static object _typeFieldDicLock = new object();



        /// <summary>
        /// 注册常量类
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static void RegisteType<T>()
        {
            Type type = typeof(T);
            Type valueNameMapTypeAttributeType = typeof(ValueNameMapTypeAttribute);

            object[] attrObjArr = type.GetCustomAttributes(valueNameMapTypeAttributeType, false);
            if (attrObjArr == null || attrObjArr.Length == 0)
            {
                throw new ApplicationException($"映射字段定义类型类[{type.FullName}]未标记[{valueNameMapTypeAttributeType.FullName}]特性");
            }

            var valueNameMapTypeAttribute = (ValueNameMapTypeAttribute)attrObjArr[0];
            var dic = new ValueNameDictionary(valueNameMapTypeAttribute);
            FieldInfo[] fieldInfoArr = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            Type valueNameMapFieldAttributeType = typeof(ValueNameMapFieldAttribute);
            object key;
            ValueNameMapFieldAttribute value;

            foreach (var fieldInfo in fieldInfoArr)
            {
                attrObjArr = fieldInfo.GetCustomAttributes(valueNameMapFieldAttributeType, true);
                if (attrObjArr == null || attrObjArr.Length == 0)
                {
                    //字段无特性,忽略该字段
                    continue;
                }

                key = fieldInfo.GetValue(null);
                value = (ValueNameMapFieldAttribute)attrObjArr[0];
                if (dic.ContainsKey(key))
                {
                    throw new ApplicationException($"类型[{type.FullName}]字段值[{key}]重复");
                }

                dic.Add(key, value);
            }

            RegisteType(type, dic);
        }

        /// <summary>
        /// 注册常量类
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="valueNameDic">值名称字典集合</param>
        public static void RegisteType<T>(ValueNameDictionary valueNameDic)
        {
            RegisteType(typeof(T), valueNameDic);
        }

        /// <summary>
        /// 注册指定类型的值名称映射
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <param name="valueNameDic">值名称字典集合</param>
        public static void RegisteType(Type type, ValueNameDictionary valueNameDic)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (valueNameDic == null)
            {
                throw new ArgumentNullException(nameof(valueNameDic));
            }

            _typeValueNameDictionaryDic[type] = valueNameDic;
        }




        /// <summary>
        /// 获取指定类型的值名称映射字典集合
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>值名称映射字典集合</returns>
        public static ValueNameDictionary GetValueNameDictionary<T>()
        {
            return GetValueNameDictionary(typeof(T));
        }

        /// <summary>
        /// 获取指定类型的值名称映射字典集合
        /// </summary>
        /// <param name="type">目标类型</param>
        /// <returns>值名称映射字典集合</returns>
        public static ValueNameDictionary GetValueNameDictionary(Type type)
        {
            ValueNameDictionary valueNameDic;
            _typeValueNameDictionaryDic.TryGetValue(type, out valueNameDic);
            return valueNameDic;
        }




        /// <summary>
        /// 移除指定类型的映射
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>移除的值名称映射集合</returns>
        public static ValueNameDictionary Remove<T>()
        {
            ValueNameDictionary dic;
            _typeValueNameDictionaryDic.TryRemove(typeof(T), out dic);
            return dic;
        }

        /// <summary>
        /// 清空映射所有映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Clear<T>()
        {
            ValueNameDictionary dic;
            if (_typeValueNameDictionaryDic.TryGetValue(typeof(T), out dic))
            {
                dic.Clear();
            }
        }

        /// <summary>
        /// 根据值获取名称
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>名称</returns>
        public static string GetNameByValue<T>(object value)
        {
            Type type = typeof(T);
            ValueNameDictionary dic;
            if (_typeValueNameDictionaryDic.TryGetValue(type, out dic))
            {
                ValueNameMapFieldAttribute valueNameMapFieldAttribute;
                if (dic.TryGetValue(value, out valueNameMapFieldAttribute))
                {
                    return valueNameMapFieldAttribute.Name;
                }
                else
                {
                    return dic.ValueNameMapTypeAttribute.CustomerValueName.GetName(value, dic);
                }
            }
            else
            {
                throw new ArgumentException($"类型[{type.FullName}]未注册");
            }
        }

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public static object GerValueByName<T>(string name)
        {
            Type type = typeof(T);
            ValueNameDictionary dic;
            if (_typeValueNameDictionaryDic.TryGetValue(type, out dic))
            {
                var kvArr = dic.Where(t => { return string.Equals(t.Value, name); }).ToArray();
                if (kvArr.Length > 0)
                {
                    return kvArr[0].Key;
                }
                else
                {
                    throw new ArgumentException($"名称[{name}]对应的值不存在");
                }
            }
            else
            {
                throw new ArgumentException($"类型[{type.FullName}]未注册");
            }
        }

        /// <summary>
        /// 获取指定类型值数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>类型所有定义常量值数组</returns>
        public static object[] GetValues<T>()
        {
            Type type = typeof(T);
            ValueNameDictionary dic;
            if (_typeValueNameDictionaryDic.TryGetValue(type, out dic))
            {
                return dic.Keys.ToArray();
            }
            else
            {
                throw new ArgumentException($"类型[{type.FullName}]未注册");
            }
        }

        /// <summary>
        /// 获取指定类型名称数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>类型所有定义常量值数组</returns>
        public static string[] GetNames<T>()
        {
            Type type = typeof(T);
            ValueNameDictionary dic;
            if (_typeValueNameDictionaryDic.TryGetValue(type, out dic))
            {
                return dic.Values.Select(t => { return t.Name; }).ToArray();
            }
            else
            {
                throw new ArgumentException($"类型[{type.FullName}]未注册");
            }
        }
    }
}
