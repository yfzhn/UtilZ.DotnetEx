using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.ConstantValueDescription
{
    /// <summary>
    /// 值描述管理类
    /// </summary>
    public class ValueDescriptionManager
    {
        private readonly static ConcurrentDictionary<object, ValueDescriptionGroup> _typeValueDescriptionGroupDic = new ConcurrentDictionary<object, ValueDescriptionGroup>();



        /// <summary>
        /// 注册常量或枚举值描述
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static void Registe<T>()
        {
            Type type = typeof(T);

            ValueDescriptionGroupAttribute valueDescriptionGroupAtt;
            Type valueDescriptionGroupAttributeType = typeof(ValueDescriptionGroupAttribute);
            object[] attrObjArr = type.GetCustomAttributes(valueDescriptionGroupAttributeType, false);
            if (attrObjArr == null || attrObjArr.Length == 0)
            {
                valueDescriptionGroupAtt = null;
            }
            else
            {
                valueDescriptionGroupAtt = (ValueDescriptionGroupAttribute)attrObjArr[0];
            }

            var group = new ValueDescriptionGroup(valueDescriptionGroupAtt);
            FieldInfo[] fieldInfoArr = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            Type displayNameExAttributeType = typeof(DisplayNameExAttribute);
            object key;
            DisplayNameExAttribute value;

            foreach (var fieldInfo in fieldInfoArr)
            {
                attrObjArr = fieldInfo.GetCustomAttributes(displayNameExAttributeType, true);
                if (attrObjArr == null || attrObjArr.Length == 0)
                {
                    //字段无特性,忽略该字段
                    continue;
                }

                key = fieldInfo.GetValue(null);
                value = (DisplayNameExAttribute)attrObjArr[0];
                if (group.ContainsKey(key))
                {
                    throw new ApplicationException($"类型[{type.FullName}]字段值[{key}]重复");
                }

                group.Add(key, value);
            }

            Registe(type, group);
        }

        /// <summary>
        /// 注册描述组
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <param name="group">值描述组</param>
        public static void Registe(object groupId, ValueDescriptionGroup group)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            _typeValueDescriptionGroupDic.AddOrUpdate(groupId, group, (oldKey, oldValue) => { return group; });
        }






        /// <summary>
        /// 根据类型获取值描述组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>值名称映射字典集合</returns>
        public static ValueDescriptionGroup GetValueDescriptionGroup<T>()
        {
            return GetValueDescriptionGroupById(typeof(T));
        }

        /// <summary>
        /// 根据组标识获取值描述组
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <returns>值名称映射字典集合</returns>
        public static ValueDescriptionGroup GetValueDescriptionGroupById(object groupId)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            ValueDescriptionGroup group;
            _typeValueDescriptionGroupDic.TryGetValue(groupId, out group);
            return group;
        }






        /// <summary>
        /// 根据类型移除值描述组,并返回移除的组,不为null则移除成功
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>移除的值名称映射集合</returns>
        public static ValueDescriptionGroup Remove<T>()
        {
            return RemoveById(typeof(T));
        }

        /// <summary>
        /// 根据组标识移除值描述组,并返回移除的组,不为null则移除成功
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <returns>移除的值名称映射集合</returns>
        public static ValueDescriptionGroup RemoveById(object groupId)
        {
            ValueDescriptionGroup group;
            _typeValueDescriptionGroupDic.TryRemove(groupId, out group);
            return group;
        }





        /// <summary>
        /// 清空值描述组
        /// </summary>
        public static void Clear()
        {
            _typeValueDescriptionGroupDic.Clear();
        }






        /// <summary>
        /// 清空指定类型值描述组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Clear<T>()
        {
            Clear(typeof(T));
        }

        /// <summary>
        /// 清空指定组标识值描述组
        /// </summary>
        /// <param name="groupId">组标识</param>
        public static void Clear(object groupId)
        {
            ValueDescriptionGroup group = GetValueDescriptionGroupById(groupId);
            if (group != null)
            {
                group.Clear();
            }
        }






        /// <summary>
        /// 根据值获取值名称
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>值描述</returns>
        public static string GetNameByValue<T>(object value)
        {
            return GetNameByValue(typeof(T), value);
        }

        /// <summary>
        /// 根据组标识和值获取值名称
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <param name="value">值</param>
        /// <returns>值描述</returns>
        public static string GetNameByValue(object groupId, object value)
        {
            ValueDescriptionGroup group = GetValueDescriptionGroupById(groupId);
            if (group != null)
            {
                DisplayNameExAttribute desAtt;
                if (group.TryGetValue(value, out desAtt))
                {
                    return desAtt.DisplayName;
                }
                else
                {
                    var groupDescriptionAtt = group.GroupDescriptionAttribute;
                    if (groupDescriptionAtt != null && groupDescriptionAtt.Extend != null)
                    {
                        return groupDescriptionAtt.Extend.GetName(value, group);
                    }
                    else
                    {
                        throw new ArgumentException($"组标识\"{groupId}\"对应的值\"{value}\"名称不存在");
                    }
                }
            }
            else
            {
                throw new ArgumentException($"组标识\"{groupId}\"未注册");
            }
        }






        /// <summary>
        /// 根据值获取值描述
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>值描述</returns>
        public static string GetDescriptionByValue<T>(object value)
        {
            return GetDescriptionByValue(typeof(T), value);
        }

        /// <summary>
        /// 根据组标识和值获取值描述
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <param name="value">值</param>
        /// <returns>值描述</returns>
        public static string GetDescriptionByValue(object groupId, object value)
        {
            ValueDescriptionGroup group = GetValueDescriptionGroupById(groupId);
            if (group != null)
            {
                DisplayNameExAttribute desAtt;
                if (group.TryGetValue(value, out desAtt))
                {
                    return desAtt.Description;
                }
                else
                {
                    var groupDescriptionAtt = group.GroupDescriptionAttribute;
                    if (groupDescriptionAtt != null && groupDescriptionAtt.Extend != null)
                    {
                        return groupDescriptionAtt.Extend.GetDescription(value, group);
                    }
                    else
                    {
                        throw new ArgumentException($"组标识\"{groupId}\"值\"{value}\"对应的描述不存在");
                    }
                }
            }
            else
            {
                throw new ArgumentException($"组标识\"{groupId}\"未注册");
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
            return GerValueByName(typeof(T), name);
        }

        /// <summary>
        /// 根据名称获取值
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <param name="name">名称</param>
        /// <returns>值</returns>
        public static object GerValueByName(object groupId, string name)
        {
            ValueDescriptionGroup group = GetValueDescriptionGroupById(groupId);
            if (group != null)
            {
                var kvArr = group.ToArray().Where(t => { return string.Equals(t.Value, name); }).ToArray();
                if (kvArr.Length > 0)
                {
                    return kvArr[0].Key;
                }
                else
                {
                    throw new ArgumentException($"组标识\"{groupId}\"名称\"{name}\"对应的值不存在");
                }
            }
            else
            {
                throw new ArgumentException($"组标识\"{groupId}\"未注册");
            }
        }





        /// <summary>
        /// 获取指定类型值数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>类型所有定义常量值数组</returns>
        public static object[] GetValues<T>()
        {
            return GetValues(typeof(T));
        }

        /// <summary>
        /// 获取指定类型值数组
        /// </summary>
        /// <param name="groupId">组标识</param>
        /// <returns>类型所有定义常量值数组</returns>
        public static object[] GetValues(object groupId)
        {
            ValueDescriptionGroup group = GetValueDescriptionGroupById(groupId);
            if (group != null)
            {
                return group.Keys.ToArray();
            }
            else
            {
                throw new ArgumentException($"组标识\"{groupId}\"未注册");
            }
        }



        /// <summary>
        /// 获取指定类型名称数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>类型所有定义常量值数组</returns>
        public static string[] GetNames<T>()
        {
            return GetNames(typeof(T));
        }

        /// <summary>
        /// 获取指定类型名称数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>类型所有定义常量值数组</returns>
        public static string[] GetNames(object groupId)
        {
            ValueDescriptionGroup group = GetValueDescriptionGroupById(groupId);
            if (group != null)
            {
                return group.Values.ToArray().Select(t => { return t.DisplayName; }).ToArray();
            }
            else
            {
                throw new ArgumentException($"组标识\"{groupId}\"未注册");
            }
        }
    }
}
