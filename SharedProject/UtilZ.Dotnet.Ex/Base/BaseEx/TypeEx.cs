using System;
using System.Collections;
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
        private static readonly Hashtable _htTypes = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 根据类型全名称转换为类型
        /// </summary>
        /// <param name="typeFullName">类型名称[格式:类型名,程序集命名.例如:Oracle.ManagedDataAccess.Client.OracleConnection,Oracle.ManagedDataAccess, Version=4.121.1.0, Culture=neutral, PublicKeyToken=89b483f429c47342]</param>
        /// <param name="hasCache">是否缓存转换之后的类型[true:缓存;false:不缓存]</param>
        public static Type ConvertTypeByTypeFullName(string typeFullName, bool hasCache = false)
        {
            if (string.IsNullOrWhiteSpace(typeFullName))
            {
                return null;
            }

            Type type;
            if (hasCache)
            {
                type = _htTypes[typeFullName] as Type;
            }
            else
            {
                type = null;
            }

            if (type == null)
            {
                string[] segs = typeFullName.Split(',');
                if (segs.Length < 2)
                {
                    throw new NotSupportedException(string.Format("不支持的格式{0}", typeFullName));
                }

                string assemblyFileName = segs[1].Trim();//程序集文件名称
                string assemblyPath;
                if (string.IsNullOrEmpty(Path.GetPathRoot(assemblyFileName)))
                {
                    //相对工作目录的路径
                    assemblyPath = Path.Combine(DirectoryInfoEx.CurrentAssemblyDirectory, assemblyFileName);
                }
                else
                {
                    //全路径
                    assemblyPath = assemblyFileName;
                }

                if (!File.Exists(assemblyPath))
                {
                    string srcExtension = Path.GetExtension(assemblyPath).ToLower();
                    List<string> extensions = new List<string> { ".dll", ".exe" };
                    if (extensions.Contains(srcExtension))
                    {
                        return null;
                    }

                    bool isFind = false;
                    string tmpAssemblyPath;
                    foreach (var extension in extensions)
                    {
                        tmpAssemblyPath = assemblyPath + extension;
                        if (File.Exists(tmpAssemblyPath))
                        {
                            assemblyPath = tmpAssemblyPath;
                            isFind = true;
                            break;
                        }
                    }

                    if (!isFind)
                    {
                        return null;
                    }
                }

                string assemblyName = AssemblyEx.GetAssemblyName(assemblyPath);
                Assembly assembly = AssemblyEx.FindAssembly(assemblyName);
                if (assembly == null)
                {
                    assembly = Assembly.LoadFile(assemblyPath);
                }

                type = assembly.GetType(segs[0].Trim(), false, true);

                if (hasCache)
                {
                    TypeEx._htTypes[typeFullName] = type;
                }
            }

            return type;
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
    }
}
