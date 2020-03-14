using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志操作公共类
    /// </summary>
    public static class LogUtil
    {
        /// <summary>
        /// 当前程序集所在目录
        /// </summary>
        private static readonly string _currentAssemblyDirectory;

        static LogUtil()
        {
            _currentAssemblyDirectory = Path.GetDirectoryName(typeof(LogUtil).Assembly.Location);
        }

        /// <summary>
        /// 获取节点指定特性值
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="attriName"></param>
        /// <returns></returns>
        public static string GetAttributeValue(this XElement ele, string attriName)
        {
            string value;
            var attri = ele.Attribute(attriName);
            if (attri != null)
            {
                value = attri.Value;
            }
            else
            {
                value = string.Empty;
            }

            return value;
        }

        /// <summary>
        /// 获取节点下指定名称子节点特性值
        /// </summary>
        /// <param name="ele"></param>
        /// <param name="childName"></param>
        /// <param name="attriName"></param>
        /// <returns></returns>
        public static string GetChildXElementValue(this XElement ele, string childName, string attriName = null)
        {
            string value;
            var childEle = ele.XPathSelectElement(string.Format(@"param[@name='{0}']", childName));
            if (childEle == null)
            {
                value = string.Empty;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(attriName))
                {
                    attriName = "value";
                }

                value = GetAttributeValue(childEle, attriName);
            }

            return value;
        }

        /// <summary>
        /// 根据字符串信息获取程序集中的类型
        /// </summary>
        /// <param name="typeFullName">类型名称[格式:类型名,程序集命名.例如:Oracle.ManagedDataAccess.Client.OracleConnection,Oracle.ManagedDataAccess, Version=4.121.1.0, Culture=neutral, PublicKeyToken=89b483f429c47342]</param>
        /// <returns>实例</returns>
        public static Type GetType(string typeFullName)
        {
            if (string.IsNullOrWhiteSpace(typeFullName))
            {
                return null;
            }

            string[] segs = typeFullName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (segs.Length < 2)
            {
                throw new NotSupportedException(string.Format("不支持的格式{0}", typeFullName));
            }

            string assemblyFileName = segs[1].Trim();//程序集文件名称
            string assemblyPath;
            if (string.IsNullOrEmpty(Path.GetPathRoot(assemblyFileName)))
            {
                //相对工作目录的路径
                var currentAssemblyDirectory = Path.GetDirectoryName(typeof(LogUtil).Assembly.Location);
                assemblyPath = Path.Combine(currentAssemblyDirectory, assemblyFileName);
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

            string assemblyName = AssemblyName.GetAssemblyName(assemblyPath).FullName;
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            Assembly assembly = (from item in assemblys where assemblyName.Equals(item.FullName) select item).FirstOrDefault();
            if (assembly == null)
            {
                assembly = Assembly.LoadFile(assemblyPath);
            }

            Type type = assembly.GetType(segs[0].Trim(), false, true);
            if (type == null)
            {
                return null;
            }

            return type;
        }

        /// <summary>
        /// 获取指定层级调用堆栈(格式:[类全名.方法名].[类全名.方法名].xxx)
        /// </summary>
        /// <param name="stackCount">要获取的堆栈数</param>
        /// <returns>指定层级调用堆栈</returns>
        public static string GetStackTrace(int stackCount = 1)
        {
            var stackTrace = new System.Diagnostics.StackTrace(2, true);
            var frames = stackTrace.GetFrames();
            if (frames.Length < stackCount)
            {
                stackCount = frames.Length;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < stackCount; i++)
            {
                var sf = frames[i];
                string fileName = sf.GetFileName();
                int lineNo = sf.GetFileLineNumber();
                //int colNo = sf.GetFileColumnNumber();
                MethodBase methodBase = sf.GetMethod();
                var sogger = methodBase.DeclaringType.FullName;
                string mn = methodBase.Name;

                if (sb.Length > 0)
                {
                    sb.Append(".");
                }

                sb.Append(string.Format("[{0}.{1}]", methodBase.DeclaringType.FullName, methodBase.Name));
            }

            return sb.ToString();
        }
    }
}
