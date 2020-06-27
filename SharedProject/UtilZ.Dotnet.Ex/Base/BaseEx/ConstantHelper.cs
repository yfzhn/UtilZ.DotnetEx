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
        /// 获取常量字段特性信息列表
        /// </summary>
        /// <typeparam name="T">常量定义类</typeparam>
        /// <param name="ignoreNoAttibute">忽略未标记的字段[true:忽略;false:使用字段名。默认为true]</param>
        /// <returns>PropertyFieldInfo列表</returns>
        public static List<PropertyFieldInfo> GetConstantPropertyFieldInfoList<T>(bool ignoreNoAttibute = true)
        {
            return GetConstantPropertyFieldInfoList(typeof(T), ignoreNoAttibute);
        }

        /// <summary>
        /// 获取常量字段特性信息列表
        /// </summary>
        /// <param name="type">类类型</param>
        /// <param name="ignoreNoAttibute">忽略未标记的字段[true:忽略;false:使用字段名。默认为true]</param>
        /// <returns>PropertyFieldInfo列表</returns>
        public static List<PropertyFieldInfo> GetConstantPropertyFieldInfoList(Type type, bool ignoreNoAttibute = true)
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

            var list = new List<PropertyFieldInfo>();
            foreach (var enumAttriItem in attriItems)
            {
                list.Add(new PropertyFieldInfo(enumAttriItem.Item1.DisplayName, enumAttriItem.Item2, enumAttriItem.Item1.Description, enumAttriItem.Item1));
            }

            return list;
        }
    }
}
