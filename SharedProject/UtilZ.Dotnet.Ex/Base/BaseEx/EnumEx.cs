using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static class EnumEx
    {
        /// <summary>
        /// 断言类型T为枚举类型
        /// </summary>
        /// <typeparam name="T">类型T</typeparam>
        public static void AssertEnum<T>()
        {
            AssertEnum(typeof(T));
        }

        /// <summary>
        /// 断言类型T为枚举类型
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        public static void AssertEnum(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("类型:{0}不是枚举类型", enumType.FullName));
            }
        }




        /// <summary>
        /// 根据枚举DisplayNameExAttribute特性文本获取对应的枚举项
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="displayName">显示文本</param>
        /// <returns>枚举项</returns>
        public static object GetEnumByDisplayNameExAttributeDisplayName(Type enumType, string displayName)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException(nameof(enumType));
            }

            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentNullException(nameof(displayName), "NEnumAttribute显示文本值不能为空或null");
            }

            AssertEnum(enumType);
            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;
            object enumValue = null;
            List<System.Reflection.FieldInfo> noNEnumAttributeFields = new List<System.Reflection.FieldInfo>();

            //有特性返回特性文本对应的值
            foreach (var field in fields)
            {
                enumValue = field.GetValue(null);
                csAttris = field.GetCustomAttributes(enumAttriType, false);
                if (csAttris.Length == 0)
                {
                    noNEnumAttributeFields.Add(field);
                    continue;
                }

                if (string.Equals(displayName, ((DisplayNameExAttribute)csAttris[0]).DisplayName))
                {
                    return enumValue;
                }
            }

            //无特性标注的返回枚举字符串对应的值
            foreach (var field in noNEnumAttributeFields)
            {
                enumValue = field.GetValue(null);
                if (string.Equals(displayName, enumValue.ToString()))
                {
                    return enumValue;
                }
            }

            return null;
        }



        /// <summary>
        /// 获取枚举字段特性信息列表
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>PropertyFieldInfo列表</returns>
        public static List<FieldInfoEx> GetEnumFieldInfoExList<T>() where T : Enum
        {
            return GetEnumFieldInfoExList(typeof(T));
        }

        /// <summary>
        /// 获取枚举字段特性信息列表
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>PropertyFieldInfo列表</returns>
        public static List<FieldInfoEx> GetEnumFieldInfoExList(Type enumType)
        {
            AssertEnum(enumType);

            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;

            List<Tuple<DisplayNameExAttribute, object>> enumAttriItems = new List<Tuple<DisplayNameExAttribute, object>>();
            object value = null;

            foreach (var field in fields)
            {
                DisplayNameExAttribute dneAtt = null;
                csAttris = field.GetCustomAttributes(enumAttriType, false);
                value = field.GetValue(null);

                if (csAttris.Length == 0)
                {
                    dneAtt = new DisplayNameExAttribute(value.ToString(), value == null ? null : value.ToString());
                }
                else
                {
                    dneAtt = (DisplayNameExAttribute)csAttris[0];
                }

                enumAttriItems.Add(Tuple.Create<DisplayNameExAttribute, object>(dneAtt, value));
            }

            enumAttriItems = enumAttriItems.OrderBy(new Func<Tuple<DisplayNameExAttribute, object>, int>((item) => { return item.Item1.OrderIndex; })).ToList();

            var list = new List<FieldInfoEx>();
            foreach (var enumAttriItem in enumAttriItems)
            {
                list.Add(new FieldInfoEx(enumAttriItem.Item1.DisplayName, enumAttriItem.Item2, enumAttriItem.Item1.Description, enumAttriItem.Item1));
            }

            return list;
        }



        /// <summary>
        /// 获取枚举特性转换成的字典集合[key:枚举值;value:枚举项上标记的特性(多项取第一项)]
        /// </summary>
        /// <typeparam name="ET">枚举类型</typeparam>
        /// <typeparam name="AT">枚举上对应的特性类型</typeparam>
        /// <returns>枚举特性转换成的字典集合</returns>
        public static Dictionary<ET, AT> GetEnumItemAttribute<ET, AT>() where AT : Attribute
        {
            Type enumType = typeof(ET);
            AssertEnum(enumType);

            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type attriType = typeof(AT);
            object[] csAttris = null;
            ET value;
            AT attri;
            var itemAttriDic = new Dictionary<ET, AT>();
            foreach (var field in fields)
            {
                csAttris = field.GetCustomAttributes(attriType, false);
                value = (ET)field.GetValue(null);
                if (csAttris.Length == 0)
                {
                    attri = null;
                }
                else
                {
                    attri = (AT)csAttris[0];
                }

                itemAttriDic.Add(value, attri);
            }

            return itemAttriDic;
        }




        #region 获取枚举项上的特性显示文本
        /// <summary>
        /// 获取枚举项上的特性显示文本
        /// </summary>
        /// <param name="enumItem">枚举值</param>
        /// <returns>特性显示文本</returns>
        public static string GetEnumItemDisplayName(object enumItem)
        {
            if (enumItem == null)
            {
                return string.Empty;
            }

            return PrimitiveGetEnumItemDisplayName(enumItem.GetType(), enumItem);
        }

        /// <summary>
        /// 获取枚举项上的特性显示文本
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumItem">枚举值</param>
        /// <returns>特性显示文本</returns>
        public static string GetEnumItemDisplayName<T>(T enumItem) where T : struct, IComparable, IFormattable, IConvertible
        {
            Type enumType = typeof(T);
            return PrimitiveGetEnumItemDisplayName(enumType, enumItem);
        }

        /// <summary>
        /// 获取枚举项上的显示文本
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumItem">枚举项</param>
        /// <returns>显示文本</returns>
        private static string PrimitiveGetEnumItemDisplayName(Type enumType, object enumItem)
        {
            AssertEnum(enumType);
            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;
            object enumValue = null;

            foreach (var field in fields)
            {
                enumValue = field.GetValue(null);
                if (enumItem.Equals(enumValue))
                {
                    csAttris = field.GetCustomAttributes(enumAttriType, false);
                    if (csAttris.Length == 0)
                    {
                        return enumValue.ToString();
                    }
                    else
                    {
                        return ((DisplayNameExAttribute)csAttris[0]).DisplayName;
                    }
                }
            }

            return string.Empty;
        }
        #endregion




        #region 获取枚举字段上的标识
        /// <summary>
        /// 获取枚举项上的标识
        /// </summary>
        /// <param name="enumItem">枚举值</param>
        /// <returns>枚举项上的标识</returns>
        public static object GetEnumItemTag(object enumItem)
        {
            Type enumType = enumItem.GetType();
            return PrimitiveGetEnumItemTag(enumType, enumItem);
        }

        /// <summary>
        /// 获取枚举项上的标识
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumItem">枚举值</param>
        /// <returns>枚举项上的标识</returns>
        public static object GetEnumItemTag<T>(T enumItem) where T : struct, IComparable, IFormattable, IConvertible
        {
            Type enumType = typeof(T);
            return PrimitiveGetEnumItemTag(enumType, enumItem);
        }

        /// <summary>
        /// 获取枚举项上的标识
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumItem">枚举值</param>
        /// <returns>枚举项上的标识</returns>
        private static object PrimitiveGetEnumItemTag(Type enumType, object enumItem)
        {
            AssertEnum(enumType);
            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;
            object enumValue = null;

            foreach (var field in fields)
            {
                enumValue = field.GetValue(null);
                if (enumItem.Equals(enumValue))
                {
                    csAttris = field.GetCustomAttributes(enumAttriType, false);
                    if (csAttris.Length == 0)
                    {
                        return enumValue.ToString();
                    }
                    else
                    {
                        return ((DisplayNameExAttribute)csAttris[0]).Tag;
                    }
                }
            }

            return string.Empty;
        }
        #endregion




        #region 获取枚举字段上的描述
        /// <summary>
        /// 获取枚举项上的描述
        /// </summary>
        /// <param name="enumItem">枚举值</param>
        /// <returns>枚举项上的标识</returns>
        public static string GetEnumItemDescription(object enumItem)
        {
            Type enumType = enumItem.GetType();
            return PrimitiveGetEnumItemDescription(enumType, enumItem);
        }

        /// <summary>
        /// 获取枚举项上的描述
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumItem">枚举值</param>
        /// <returns>枚举项上的标识</returns>
        public static string GetEnumItemDescription<T>(T enumItem) where T : struct, IComparable, IFormattable, IConvertible
        {
            Type enumType = typeof(T);
            return PrimitiveGetEnumItemDescription(enumType, enumItem);
        }

        /// <summary>
        /// 获取枚举项上的描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumItem">枚举值</param>
        /// <returns>枚举项上的标识</returns>
        private static string PrimitiveGetEnumItemDescription(Type enumType, object enumItem)
        {
            AssertEnum(enumType);
            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;
            object enumValue = null;

            foreach (var field in fields)
            {
                enumValue = field.GetValue(null);
                if (enumItem.Equals(enumValue))
                {
                    csAttris = field.GetCustomAttributes(enumAttriType, false);
                    if (csAttris.Length == 0)
                    {
                        return enumValue.ToString();
                    }
                    else
                    {
                        return ((DisplayNameExAttribute)csAttris[0]).Description;
                    }
                }
            }

            return string.Empty;
        }
        #endregion




        #region 获取枚举字段特性显示文本
        /// <summary>
        /// 获取枚举项上的特性
        /// </summary>
        /// <param name="enumItem">枚举值</param>
        /// <returns>特性</returns>
        public static DisplayNameExAttribute GetEnumItemDisplayNameExAttribute(object enumItem)
        {
            if (enumItem == null)
            {
                return null;
            }

            return PrimitiveGetEnumItemDisplayNameExAttribute(enumItem.GetType(), enumItem);
        }

        /// <summary>
        /// 获取枚举项上的特性
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumItem">枚举值</param>
        /// <returns>特性显示文本</returns>
        public static DisplayNameExAttribute GetEnumItemDisplayNameExAttribute<T>(T enumItem) where T : struct, IComparable, IFormattable, IConvertible
        {
            Type enumType = typeof(T);
            return PrimitiveGetEnumItemDisplayNameExAttribute(enumType, enumItem);
        }

        /// <summary>
        /// 获取枚举项上的特性
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumItem">枚举项</param>
        /// <returns>显示文本</returns>
        private static DisplayNameExAttribute PrimitiveGetEnumItemDisplayNameExAttribute(Type enumType, object enumItem)
        {
            AssertEnum(enumType);
            var fields = enumType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;
            object enumValue = null;

            foreach (var field in fields)
            {
                enumValue = field.GetValue(null);
                if (enumItem.Equals(enumValue))
                {
                    csAttris = field.GetCustomAttributes(enumAttriType, false);
                    if (csAttris.Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return (DisplayNameExAttribute)csAttris[0];
                    }
                }
            }

            return null;
        }
        #endregion

    }
}
