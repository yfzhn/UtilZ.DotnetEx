using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 非托管程序集扩展类
    /// </summary>
    public class UnManagedDll
    {
        #region C#动态调用C++编写的DLL函数
        private const string _KERNEL32 = "Kernel32";
        /// <summary>
        /// 添加目录到DLL搜索路径
        /// </summary>
        /// <param name="dir">The directory to be added to the search path. 
        /// If this parameter is an empty string (""), 
        /// the call removes the current directory from the default DLL search order. 
        /// If this parameter is NULL, the function restores the default search order</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport(_KERNEL32, CharSet = CharSet.Unicode)]
        private static extern bool SetDllDirectory(string dir);

        /// <summary>
        /// 加载C++ dll
        /// </summary>
        /// <param name="dllPath">dll路径</param>
        /// <returns>库句柄</returns>
        [DllImport(_KERNEL32, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string dllPath);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/ms684179(v=vs.85).aspx
        /// <summary>
        /// 加载C++ dll
        /// </summary>
        /// <param name="dllPath">dll路径</param>
        /// <param name="hFile">This parameter is reserved for future use. It must be NULL.</param>
        /// <param name="dwFlags">The action to be taken when loading the module. If no flags are specified, the behavior of this function is identical to that of the LoadLibrary function. This parameter can be one of the following values.</param>
        /// <returns>库句柄</returns>
        [DllImport(_KERNEL32, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryEx(string dllPath, int hFile = 0, int dwFlags = 0x00000008);

        /// <summary>
        /// 获取方法句柄Unicode
        /// </summary>
        /// <param name="libHandle">库句柄</param>
        /// <param name="funcName">方法名称</param>
        /// <returns>方法句柄</returns>
        [DllImport(_KERNEL32, EntryPoint = "GetProcAddress", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetProcAddressUnicode(IntPtr libHandle, string funcName);

        /// <summary>
        /// 获取方法句柄Ansi
        /// </summary>
        /// <param name="libHandle">库句柄</param>
        /// <param name="funcName">方法名称</param>
        /// <returns>方法句柄</returns>
        [DllImport(_KERNEL32, EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddressAnsi(IntPtr libHandle, string funcName);

        /// <summary>
        /// 获取方法句柄Auto
        /// </summary>
        /// <param name="libHandle">库句柄</param>
        /// <param name="funcName">方法名称</param>
        /// <returns>方法句柄</returns>
        [DllImport(_KERNEL32, EntryPoint = "GetProcAddress", CharSet = CharSet.Auto)]
        private static extern IntPtr GetProcAddressAuto(IntPtr libHandle, string funcName);

        /// <summary>
        /// 释放库
        /// </summary>
        /// <param name="libHandle">库句柄</param>
        /// <returns>释放结果</returns>
        [DllImport(_KERNEL32)]
        private static extern int FreeLibrary(IntPtr libHandle);
        #endregion


        //帮助文章:http://www.cnblogs.com/fdyang/p/4015396.html

        /// <summary>
        /// 添加目录到DLL搜索路径[代码中第一行一般都是SetDllDirectory(""),目的是为了防DLL挟持]
        /// The SetDllDirectory function affects all subsequent calls to the LoadLibrary and LoadLibraryEx functions.It also effectively disables safe DLL search mode while the specified directory is in the search path.
        /// After calling SetDllDirectory, the standard DLL search path is:
        /// The directory from which the application loaded.
        /// The directory specified by the lpPathName parameter.
        /// The system directory.Use the GetSystemDirectory function to get the path of this directory.The name of this directory is System32.
        /// The 16-bit system directory.There is no function that obtains the path of this directory, but it is searched.The name of this directory is System.
        /// The Windows directory.Use the GetWindowsDirectory function to get the path of this directory.
        /// The directories that are listed in the PATH environment variable.
        /// </summary>
        /// <param name="dir">The directory to be added to the search path. 
        /// If this parameter is an empty string (""), 
        /// the call removes the current directory from the default DLL search order. 
        /// If this parameter is NULL, the function restores the default search order</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        public static bool SetDllDir(string dir)
        {
            //https://msdn.microsoft.com/en-us/library/windows/desktop/ms686203(v=vs.85).aspx
            //http://blog.csdn.net/zvall/article/details/51770853
            return SetDllDirectory(dir);
        }

        /// <summary>
        /// 加载C++ dll
        /// </summary>
        /// <param name="dllPath">dll路径</param>
        /// <returns>库句柄</returns>
        public static IntPtr Load(string dllPath)
        {
            return LoadLibrary(dllPath);
        }

        /// <summary>
        /// 加载C++ dll
        /// </summary>
        /// <param name="dllPath">dll路径</param>
        /// <param name="hFile">This parameter is reserved for future use. It must be NULL.</param>
        /// <param name="dwFlags">The action to be taken when loading the module. If no flags are specified, the behavior of this function is identical to that of the LoadLibrary function. This parameter can be one of the following values.</param>
        /// <returns>库句柄</returns>
        public static IntPtr LoadEx(string dllPath, int hFile = 0, int dwFlags = 0x00000008)
        {
            return LoadLibraryEx(dllPath, hFile, dwFlags);
        }

        /// <summary>
        /// 获取方法委托
        /// </summary>
        /// <param name="libHandle">库句柄</param>
        /// <param name="funcName">方法名称</param>
        /// <param name="delegateType">与非托管dll中方法定义对应的委托类型</param>
        /// <param name="charSet">规定封送字符串应使用何种字符集</param>
        /// <returns>方法句柄</returns>
        public static Delegate GetProcDelegate(IntPtr libHandle, string funcName, Type delegateType, CharSet charSet = CharSet.Ansi)
        {
            IntPtr address;
            switch (charSet)
            {
                case CharSet.Ansi:
                    address = GetProcAddressAnsi(libHandle, funcName);
                    break;
                case CharSet.Unicode:
                    address = GetProcAddressUnicode(libHandle, funcName);
                    break;
                case CharSet.Auto:
                    address = GetProcAddressAuto(libHandle, funcName);
                    break;
                case CharSet.None:
                default:
                    throw new NotSupportedException($"不支持的封送字符串应使用何种字符集\"{charSet.ToString()}\"");
            }

            if (address == IntPtr.Zero)
            {
                throw new ArgumentException($"方法\"{funcName}\"不存在");
            }

            //通过非托管函数名转换为对应的委托
            return Marshal.GetDelegateForFunctionPointer(address, delegateType);
        }

        /// <summary>
        /// 释放库
        /// </summary>
        /// <param name="libHandle">库句柄</param>
        /// <returns>释放结果</returns>
        public static int Free(IntPtr libHandle)
        {
            return FreeLibrary(libHandle);
        }
    }
}
