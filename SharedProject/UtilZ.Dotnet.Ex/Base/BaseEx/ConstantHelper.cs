using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 常量辅助类
    /// </summary>
    public class ConstantHelper
    {
        /// <summary>
        /// 断言类型T为类类型
        /// </summary>
        /// <param name="type">类型</param>
        public static void AssertClass(Type type)
        {
            if (!type.IsClass)
            {
                throw new ArgumentException(string.Format("类型:{0}不是类类型", type.FullName));
            }
        }

        /// <summary>
        /// 获取常量字段特性转换成的DropdownBindingItem列表
        /// </summary>
        /// <param name="ignoreNoAttibute">忽略未标记的字段</param>
        /// <returns>绑定列表集合</returns>
        public static List<DropdownBindingItem> GetDisplayNameExAttributeItemList<T>(bool ignoreNoAttibute = true)
        {
            return GetDisplayNameExAttributeItemList(typeof(T), ignoreNoAttibute);
        }

        /// <summary>
        /// 获取常量字段特性转换成的DropdownBindingItem列表
        /// </summary>
        /// <param name="type">类类型</param>
        /// <param name="ignoreNoAttibute">忽略未标记的字段</param>
        /// <returns>绑定列表集合</returns>
        public static List<DropdownBindingItem> GetDisplayNameExAttributeItemList(Type type, bool ignoreNoAttibute = true)
        {
            AssertClass(type);

            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Type enumAttriType = typeof(DisplayNameExAttribute);
            object[] csAttris = null;

            List<Tuple<DisplayNameExAttribute, object>> attriItems = new List<Tuple<DisplayNameExAttribute, object>>();
            object value = null;
            DisplayNameExAttribute dneAtt = null;

            foreach (var field in fields)
            {
                csAttris = field.GetCustomAttributes(enumAttriType, false);
                value = field.GetValue(null);

                if (csAttris.Length == 0)
                {
                    if (ignoreNoAttibute)
                    {
                        continue;
                    }

                    dneAtt = new DisplayNameExAttribute(value.ToString(), value == null ? null : value.ToString());
                }
                else
                {
                    dneAtt = (DisplayNameExAttribute)csAttris[0];
                }

                attriItems.Add(Tuple.Create<DisplayNameExAttribute, object>(dneAtt, value));
            }

            attriItems = attriItems.OrderBy(new Func<Tuple<DisplayNameExAttribute, object>, int>((item) => { return item.Item1.OrderIndex; })).ToList();

            List<DropdownBindingItem> dbiItems = new List<DropdownBindingItem>();
            foreach (var enumAttriItem in attriItems)
            {
                dbiItems.Add(new DropdownBindingItem(enumAttriItem.Item1.DisplayName, enumAttriItem.Item2, enumAttriItem.Item1.Description, enumAttriItem.Item1));
            }

            return dbiItems;
        }
    }
}
