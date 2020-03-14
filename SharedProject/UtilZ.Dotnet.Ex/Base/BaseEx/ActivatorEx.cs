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
    ///  Activator类型扩展方法类
    /// </summary>
    public static class ActivatorEx
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="typeFullName">类型名称[格式:类型名,程序集命名.例如:Oracle.ManagedDataAccess.Client.OracleConnection,Oracle.ManagedDataAccess, Version=4.121.1.0, Culture=neutral, PublicKeyToken=89b483f429c47342]</param>
        /// <param name="args">构造函数参数</param>
        public static object CreateInstance(string typeFullName, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(typeFullName))
            {
                return null;
            }

            Type type = TypeEx.ConvertTypeByTypeFullName(typeFullName);
            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type, args);
        }

        ///// <summary>
        ///// 创建实例
        ///// </summary>
        ///// <param name="typeFullName">类型名称[格式:类型名,程序集命名.例如:Oracle.ManagedDataAccess.Client.OracleConnection,Oracle.ManagedDataAccess, Version=4.121.1.0, Culture=neutral, PublicKeyToken=89b483f429c47342]</param>
        ///// <param name="appDomain">应用程序域,默认null为当前域</param>
        //private static object CreateInstance_bk(string typeFullName, AppDomain appDomain = null)
        //{
        //    if (string.IsNullOrEmpty(typeFullName))
        //    {
        //        return null;
        //    }

        //    if (appDomain == null)
        //    {
        //        appDomain = AppDomain.CurrentDomain;
        //    }

        //    string[] segs = typeFullName.Split(',');
        //    if (segs.Length != 5)
        //    {
        //        throw new NotSupportedException(string.Format("不支持的格式{0}", typeFullName));
        //    }

        //    int index = typeFullName.IndexOf(',');
        //    if (index == -1 || typeFullName.Length == index + 1)
        //    {
        //        return null;
        //    }

        //    string assemblyName = string.Join(",", segs, 1, segs.Length - 1);
        //    System.Runtime.Remoting.ObjectHandle objHandle = Activator.CreateInstance(appDomain, assemblyName, segs[0]);
        //    return objHandle.Unwrap();
        //}


        /// <summary>
        /// 创建指定接口类型实例 
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <param name="type">实现该接口的类</param>
        /// <param name="args">构造函数个数数组</param>
        /// <returns>接口实例</returns>
        public static T CreateInstance<T>(Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!type.IsClass ||
                type.IsAbstract ||
                type.GetConstructors().Where(t => { return t.GetParameters().Length == 0; }).Count() == 0 ||
                type.GetInterface(typeof(T).FullName) == null)
            {
                throw new ArgumentException($"类型{type.FullName}需要有无参构造函数的可实例化类型,且该类型必须实现接口{typeof(T).FullName}");
            }

            if (args != null && args.Length > 0)
            {
                return (T)Activator.CreateInstance(type, args);
            }
            else
            {
                return (T)Activator.CreateInstance(type);
            }
        }
    }
}
