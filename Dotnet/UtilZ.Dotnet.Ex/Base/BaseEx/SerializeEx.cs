using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 序列化扩展类[注:当对象中有继承后重写父类的属性时,不适用]
    /// </summary>
    public static partial class SerializeEx
    {
        #region JavaScriptSerializer-JasonSerializer
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <returns>json序列化之后的字符串</returns>
        public static string JsonSerializerObject(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <typeparam name="T">反序列化之类的类型</typeparam>
        /// <param name="json">待反序列化的json字符串</param>
        /// <returns>反序列化之后的对象</returns>
        public static T JsonDeserializeObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException("json");
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        /// <param name="json">待反序列化的json字符串</param>
        /// <param name="targetType">反序列化之类的类型</param>
        /// <returns>反序列化之后的对象</returns>
        public static object JsonDeserializeObject(string json, Type targetType)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentNullException("json");
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize(json, targetType);
        }
        #endregion
    }
}
