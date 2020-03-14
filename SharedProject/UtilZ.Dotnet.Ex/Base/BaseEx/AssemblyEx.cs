using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 程序集管理辅助类
    /// </summary>
    public static class AssemblyEx
    {
        private readonly static ConcurrentDictionary<string, Assembly> _targetAssemblyDic = new ConcurrentDictionary<string, Assembly>();

        /// <summary>
        /// 是否内部加载数据库访问组件
        /// </summary>
        private static bool _enable = false;

        /// <summary>
        /// 获取或设置是否内部加载数据库访问组件,true:内部加载,false:外部加载
        /// </summary>
        public static bool Enable
        {
            get
            {
                return _enable;
            }
            set
            {
                if (_enable == value)
                {
                    return;
                }

                _enable = value;
                if (_enable)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += ManualLoadAssembly;
                }
                else
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= ManualLoadAssembly;
                }
            }
        }

        /// <summary>
        /// 查找范围[true:主程序及所有了子目录;false:仅RequestingAssembly所在目录]
        /// </summary>
        private static bool _findArea = false;

        /// <summary>
        /// 获取或设置查找范围[true:主程序及所有了子目录;false:仅RequestingAssembly所在目录]
        /// </summary>
        public static bool FindArea
        {
            get
            {
                return _findArea;
            }
            set
            {
                _findArea = value;
            }
        }

        /// <summary>
        /// 手动加载程序集委托列表
        /// </summary>
        private readonly static ConcurrentDictionary<string, Func<object, ResolveEventArgs, System.Reflection.Assembly>> _manualLoadAssemblyFuncDic = new ConcurrentDictionary<string, Func<object, ResolveEventArgs, Assembly>>();

        /// <summary>
        /// 加载数组库访问程序集
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="args">需要的程序集名称信息参数</param>
        private static System.Reflection.Assembly ManualLoadAssembly(object sender, ResolveEventArgs args)
        {
            Assembly targetAssembly;
            //验证是否存在已查找过的集合中,如果已存在则直接取值返回
            if (_targetAssemblyDic.TryGetValue(args.Name, out targetAssembly))
            {
                return _targetAssemblyDic[args.Name];
            }

            string assemblyFullPath;
            targetAssembly = FindAssembly(args.Name);
            if (targetAssembly != null)
            {
                return targetAssembly;
            }

            string assemblyName = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";
            if (args.RequestingAssembly != null)
            {
                //先找请求的程序集目录
                assemblyFullPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(args.RequestingAssembly.Location), assemblyName);
                if (!System.IO.File.Exists(assemblyFullPath))
                {
                    //如果请求的程序集目录中没有程序集,则在工作目录中查找
                    assemblyFullPath = System.IO.Path.Combine(DirectoryInfoEx.CurrentAssemblyDirectory, assemblyName);
                }

                if (System.IO.File.Exists(assemblyName))
                {
                    targetAssembly = Assembly.LoadFrom(assemblyName);
                }

                //添加到已加载的程序集字典集合中
                if (targetAssembly != null)
                {
                    return targetAssembly;
                }
            }

            if (_findArea)
            {
                //在主程序目录以及所有子目录中查找目标程序集
                string[] filePathArr = System.IO.Directory.GetFiles(DirectoryInfoEx.CurrentAssemblyDirectory, assemblyName, System.IO.SearchOption.AllDirectories);
                if (filePathArr.Length == 0)
                {
                    return null;
                }

                try
                {
                    targetAssembly = Assembly.LoadFrom(filePathArr.First());
                    return targetAssembly;
                }
                catch
                { }
            }

            //手动外部加载
            foreach (var manualLoadAssemblyFunc in _manualLoadAssemblyFuncDic.Values.ToArray())
            {
                targetAssembly = manualLoadAssemblyFunc(sender, args);
                if (targetAssembly != null)
                {
                    return targetAssembly;
                }
            }

            return targetAssembly;
        }

        /// <summary>
        /// 添加一个手动加载程序集委托
        /// </summary>
        /// <param name="key">委托key</param>
        /// <param name="func">委托,找着了返回目标程序集,未找着返回null</param>
        public static void AddManualLoadAssemblyFunc(string key, Func<object, ResolveEventArgs, System.Reflection.Assembly> func)
        {
            _manualLoadAssemblyFuncDic[key] = func;
        }

        /// <summary>
        /// 移除一个手动加载程序集委托
        /// </summary>
        public static void RemoveManualLoadAssemblyFunc(string key)
        {
            Func<object, ResolveEventArgs, System.Reflection.Assembly> func;
            _manualLoadAssemblyFuncDic.TryRemove(key, out func);
        }

        /// <summary>
        /// 清空手动加载程序集委托
        /// </summary>
        public static void ClearManualLoadAssemblyFunc()
        {
            _manualLoadAssemblyFuncDic.Clear();
        }

        /// <summary>
        /// 从当前应用程序域中查找指定名称的程序集[找到返回目标程序集,没找到返回null]
        /// </summary>
        /// <param name="name">程序集名称或全名</param>
        /// <returns>找到返回目标程序集,没找到返回null</returns>
        public static Assembly FindAssembly(string name)
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            //return (from assembly in assemblys where string.Equals(name, assembly.FullName) || string.Equals(name, assembly.GetName().Name) select assembly).FirstOrDefault();
            Assembly targetAssembly = (from assembly in assemblys where string.Equals(name, assembly.FullName) select assembly).FirstOrDefault();
            if (targetAssembly != null)
            {
                return targetAssembly;
            }

            targetAssembly = (from assembly in assemblys where string.Equals(name, assembly.GetName().Name) select assembly).FirstOrDefault();
            return targetAssembly;
        }

        /// <summary>
        /// 添加一个程序集
        /// </summary>
        /// <param name="assembly">程序集</param>
        public static void AddAssembly(System.Reflection.Assembly assembly)
        {
            _targetAssemblyDic[assembly.FullName] = assembly;
            _targetAssemblyDic[assembly.GetName().Name] = assembly;
        }

        /// <summary>
        /// 添加一个程序集集合
        /// </summary>
        /// <param name="assemblies">程序集集合</param>
        public static void AddAssembly(IEnumerable<System.Reflection.Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                AddAssembly(assembly);
            }
        }

        /// <summary>
        /// 获取目标程序集集合
        /// </summary>
        public static Assembly[] TargetAssemblys
        {
            get
            {
                return _targetAssemblyDic.Values.Distinct().ToArray();
            }
        }

        /// <summary>
        /// 目标程序集中是否包含指定名称的程序集[存在返回:true,否则返回false]
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns>存在返回:true,否则返回false</returns>
        public static bool ContainsAssembly(string assemblyName)
        {
            return _targetAssemblyDic.ContainsKey(assemblyName);
        }

        /// <summary>
        /// 移除目标程序集
        /// </summary>
        /// <param name="assembly"></param>
        public static void RemoveAssembly(Assembly assembly)
        {
            Assembly assembly2;
            _targetAssemblyDic.TryRemove(assembly.FullName, out assembly2);
            _targetAssemblyDic.TryRemove(assembly.GetName().Name, out assembly2);
        }

        /// <summary>
        /// 清空目标程序集集合
        /// </summary>
        public static void ClearAssembly()
        {
            _targetAssemblyDic.Clear();
        }



        /// <summary>
        /// 获取程序集名称[获取失败或非.net程序集,则返回null]
        /// </summary>
        /// <param name="assemblyPath">程序集路径</param>
        /// <returns>程序集名称</returns>
        public static string GetAssemblyName(string assemblyPath)
        {
            try
            {
                if (System.IO.File.Exists(assemblyPath))
                {
                    return AssemblyName.GetAssemblyName(assemblyPath).FullName;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }





        /// <summary>
        /// 判断dll文件是64位还是32位[true:64位;false:32位;.NET AnyCpu程序集判断为32位]
        /// </summary>
        /// <param name="dllFilePath">dll文件路径</param>
        /// <returns>[true:64位;false:32位;.NET AnyCpu程序集判断为32位]</returns>
        public static bool IsX64OrX86(string dllFilePath)
        {
            using (System.IO.FileStream fStream = new System.IO.FileStream(dllFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                System.IO.BinaryReader bReader = new System.IO.BinaryReader(fStream);
                if (bReader.ReadUInt16() != 23117) //check the MZ signature
                {
                    throw new Exception("不识别的dll");
                }

                fStream.Seek(0x3A, System.IO.SeekOrigin.Current); //seek to e_lfanew.
                fStream.Seek(bReader.ReadUInt32(), System.IO.SeekOrigin.Begin); //seek to the start of the NT header.
                if (bReader.ReadUInt32() != 17744) //check the PE\0\0 signature.
                {
                    throw new Exception("不识别的dll");
                }

                fStream.Seek(20, System.IO.SeekOrigin.Current); //seek past the file header,
                ushort architecture = bReader.ReadUInt16(); //read the magic number of the optional header.
                //523 64位    267 32位
                switch (architecture)
                {
                    case 523:
                        return true;
                    case 267:
                        return false;
                    default:
                        throw new Exception("不识别的dll类型");
                }
            }
        }




        /*
        /// <summary>
        /// 编码程序集[返回编码结果]
        /// </summary>
        /// <param name="sourceFilePaths">源码文件路径数组</param>
        /// <param name="referencedAssemblyFilePaths">所有引用程序集路径数组</param>
        /// <param name="outAssemblyFilePath">输出程序集路径</param>
        /// <returns>CompilerResults</returns>
        public static CompilerResults CompileAssembly(string[] sourceFilePaths, string[] referencedAssemblyFilePaths, string outAssemblyFilePath)
        {
            if (sourceFilePaths == null || sourceFilePaths.Length == 0)
            {
                throw new ArgumentNullException("sourceFilePaths");
            }

            string sourceFileExtension = System.IO.Path.GetExtension(sourceFilePaths[0].ToUpper(System.Globalization.CultureInfo.InvariantCulture));
            string extension;
            foreach (var sourceFilePath in sourceFilePaths)
            {
                extension = System.IO.Path.GetExtension(sourceFilePaths[0].ToUpper(System.Globalization.CultureInfo.InvariantCulture));
                if (!string.Equals(sourceFileExtension, extension))
                {
                    throw new ArgumentException("源文件扩展名必须一致");
                }
            }

            CodeDomProvider provider;
            switch (sourceFileExtension)
            {
                case ".CS":
                    provider = CodeDomProvider.CreateProvider("CSharp");
                    break;
                case ".VB":
                    provider = CodeDomProvider.CreateProvider("VisualBasic");
                    break;
                default:
                    throw new ArgumentException("源文件扩展名必须为.cs或.vb");
            }

            var cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.OutputAssembly = outAssemblyFilePath;
            cp.GenerateInMemory = false;
            cp.TreatWarningsAsErrors = false;
            cp.ReferencedAssemblies.AddRange(referencedAssemblyFilePaths);
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceFilePaths);
            return cr;
        }
        */




        /// <summary>
        /// 获取程序集GUID
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns>程序集GUID</returns>
        public static string GetAssemblyGUID(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttributes(true).Where(t => t is GuidAttribute).Cast<GuidAttribute>().Select(t => t.Value).FirstOrDefault();
        }
    }
}
