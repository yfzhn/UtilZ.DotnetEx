using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    ///  Type类型扩展方法类
    /// </summary>
    public static class TypeEx
    {
        /// <summary>
        /// 已创建过后类型[key:类型名称;value:Type]
        /// </summary>
        private static ConcurrentDictionary<string, Type> _typeDic = null;
        private static string[] _extensionArr = null;

        /// <summary>
        /// 根据类型全名称转换为类型
        /// </summary>
        /// <param name="assemblyQualifiedName">System.Type.AssemblyQualifiedName
        /// 例如:Oracle.ManagedDataAccess.Client.OracleConnection,Oracle.ManagedDataAccess, Version=4.121.1.0, Culture=neutral, PublicKeyToken=89b483f429c47342</param>
        /// <param name="hasCache">是否缓存转换之后的类型[true:缓存;false:不缓存]</param>
        public static Type GetType(string assemblyQualifiedName, bool hasCache = false)
        {
            if (string.IsNullOrWhiteSpace(assemblyQualifiedName))
            {
                return null;
            }

            Type type;
            if (hasCache)
            {
                if (_typeDic == null)
                {
                    _typeDic = new ConcurrentDictionary<string, Type>();
                }

                if (_typeDic.TryGetValue(assemblyQualifiedName, out type))
                {
                    return type;
                }
            }

            string[] strArr = assemblyQualifiedName.Split(',');
            if (strArr.Length < 2)
            {
                throw new NotSupportedException($"不支持的格式\"{assemblyQualifiedName}\"");
            }

            string assemblyName = strArr[1].Trim();//程序集名称
            Assembly assembly = AssemblyEx.FindAssembly(assemblyName);

            if (assembly == null)
            {
                assembly = GetAssemblyByName(assemblyName);
            }

            if (assembly == null)
            {
                return null;
            }

            type = assembly.GetType(strArr[0].Trim(), false, true);

            if (type != null && hasCache)
            {
                _typeDic.TryAdd(assemblyQualifiedName, type);
            }

            return type;
        }

        /// <summary>
        /// 根据程序集名称获取程序集,成功返回Assembly,失败返回null
        /// </summary>
        /// <param name="assemblyName">可带路径的程序集名称</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByName(string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                return null;
            }

            string assemblyPath;//不带扩展名的全路径
            if (string.IsNullOrEmpty(Path.GetPathRoot(assemblyName)))
            {
                //相对工作目录的路径
                assemblyPath = Path.GetFullPath(assemblyName);
            }
            else
            {
                //全路径
                assemblyPath = assemblyName;
            }

            if (File.Exists(assemblyPath))
            {
                try
                {
                    return Assembly.LoadFile(assemblyPath);
                }
                catch
                {
                    return null;
                }
            }

            if (_extensionArr == null)
            {
                _extensionArr = new string[] { ".dll", ".exe" };
            }

            string tmpAssemblyPath;
            foreach (var extension in _extensionArr)
            {
                tmpAssemblyPath = assemblyPath + extension;
                if (File.Exists(tmpAssemblyPath))
                {
                    try
                    {
                        return Assembly.LoadFile(tmpAssemblyPath);
                    }
                    catch
                    { }
                }
            }

            return null;
        }

        /// <summary>
        /// 确定当前的类型是继承自指定的接口[true:继承自接口;false:未继承自接口]
        /// </summary>
        /// <param name="type">当前的类型</param>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>true:继承自接口;false:未继承自接口</returns>
        public static bool IsSubInterfaceOf(this Type type, Type interfaceType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            return type.GetInterface(interfaceType.FullName) != null;
        }


#if NET4_0
        /// <summary>
        /// 在派生类中重写时，返回由 System.Type 标识的自定义特性的数组
        /// 异常:
        /// T:System.TypeLoadException:无法加载自定义特性类型。
        /// T:System.ArgumentNullException:如果 attributeType 为 null。
        /// T:System.InvalidOperationException:该成员属于加载到仅反射上下文的类型。请参见How to: Load Assemblies into the Reflection-Only Context。
        /// </summary>
        /// <param name="memberInfo">目标成员</param>
        /// <param name="attributeType">要搜索的特性类型。只返回可分配给此类型的特性</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性</param>
        /// <returns>应用于此成员的自定义特性</returns>
        public static Attribute GetCustomAttribute(this MemberInfo memberInfo, Type attributeType, bool inherit)
        {
            object[] attriArr = memberInfo.GetCustomAttributes(attributeType, inherit);
            if (attriArr == null || attriArr.Length == 0)
            {
                return null;
            }

            return (Attribute)attriArr[0];
        }
#endif

        /// <summary>
        /// 检查类型是否有无参构造函数[true:有无参构造函数;false:没有无参构造函数]
        /// </summary>
        /// <param name="type">被检查类型</param>
        /// <returns>true:有无参构造函数;false:没有无参构造函数</returns>
        public static bool TypeHasNoParaConstructors(Type type)
        {
            return type.IsClass &&
               !type.IsAbstract &&
               type.GetConstructors().Where(t => { return t.GetParameters().Length == 0; }).Count() > 0;
        }





        #region GetTypeName
        /// <summary>
        /// 获取 System.Type.AssemblyQualifiedName 的程序集限定名，其中包括从中加载 System.Type.AssemblyQualifiedName 的程序集的名称
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <returns>System.Type.AssemblyQualifiedName</returns>
        public static string GetTypeName<T>()
        {
            return GetTypeName(typeof(T));
        }

        /// <summary>
        /// 获取 System.Type.AssemblyQualifiedName 的程序集限定名，其中包括从中加载 System.Type.AssemblyQualifiedName 的程序集的名称
        /// </summary>
        /// <param name="obj">获取类型的名称的对象</param>
        /// <returns>System.Type.AssemblyQualifiedName</returns>
        public static string GetTypeName(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return GetTypeName(obj.GetType());
        }

        /// <summary>
        /// 获取 System.Type.AssemblyQualifiedName 的程序集限定名，其中包括从中加载 System.Type.AssemblyQualifiedName 的程序集的名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>System.Type.AssemblyQualifiedName</returns>
        public static string GetTypeName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.AssemblyQualifiedName;
        }
        #endregion
    }
}
