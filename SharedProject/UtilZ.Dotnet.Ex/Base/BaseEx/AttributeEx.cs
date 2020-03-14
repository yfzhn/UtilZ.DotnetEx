using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 特性辅助类
    /// </summary>
    public class AttributeEx
    {
        /// <summary>
        /// 将T类型valueObj对象中通过W特性标记的属性的值设置到T类型的targetObj对象中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="W">特性类型</typeparam>
        /// <param name="valueObj">值对象</param>
        /// <param name="targetObj">目标对象</param>
        public static void UpdateValue<T, W>(T valueObj, T targetObj)
            where T : class
            where W : Attribute
        {
            if (valueObj == null)
            {
                throw new ArgumentNullException(nameof(valueObj));
            }

            if (targetObj == null)
            {
                throw new ArgumentNullException(nameof(targetObj));
            }

            System.Reflection.PropertyInfo[] propertyInfos = valueObj.GetType().GetProperties();
            Type attributeType = typeof(W);
            foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetCustomAttributes(attributeType, false).Count() > 0)
                {
                    propertyInfo.SetValue(targetObj, propertyInfo.GetValue(valueObj, null), null);
                }
            }
        }
    }
}
