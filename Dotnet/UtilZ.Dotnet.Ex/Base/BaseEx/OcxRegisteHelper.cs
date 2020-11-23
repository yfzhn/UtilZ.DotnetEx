using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Ocx插件注册辅助类
    /// </summary>
    public class OcxRegisteHelper
    {
        /// <summary>
        /// Ocx插件注册和卸载委托
        /// </summary>
        /// <returns></returns>
        private delegate int OcxDllRegisterHandle();


        /// <summary>
        /// Ocx插件是否已注册(已注册返回true;否则返回false)
        /// </summary>
        /// <param name="ocxFilePath">ocx插件文件路径</param>
        /// <param name="guid">ocx插件标识</param>
        /// <returns>已注册返回true;否则返回false</returns>
        public static bool IsRegisted(string ocxFilePath, string guid)
        {
            if (string.IsNullOrWhiteSpace(ocxFilePath))
            {
                throw new ArgumentNullException(nameof(ocxFilePath));
            }

            if (!File.Exists(ocxFilePath))
            {
                throw new FileNotFoundException("ocx插件文件不存在", ocxFilePath);
            }

            if (string.IsNullOrWhiteSpace(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }

            using (RegistryKey clasrootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default))
            {
                //RegistryKey ocxKey = clasrootKey.OpenSubKey(@"CLSID\{A004D140-B636-49E8-9309-41DB74C166F6}");
                //CLSID\{A004D140-B636-49E8-9309-41DB74C166F6}\1.0\0\win32

                using (RegistryKey ocxKey = clasrootKey.OpenSubKey($@"TypeLib\{guid}"))
                {
                    if (ocxKey == null)
                    {
                        return false;
                    }

                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(ocxFilePath);
                    string ver = $"{fileVersionInfo.ProductMajorPart}.{fileVersionInfo.ProductMinorPart}";
                    RegistryKey win32Key = ocxKey.OpenSubKey($@"{ver}\0\win32");
                    if (win32Key == null)
                    {
                        return false;
                    }

                    string registedOcxFilePath = win32Key.GetValue(string.Empty).ToString();
                    win32Key.Close();

                    if (System.IO.File.Exists(registedOcxFilePath))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 注册Ocx插件
        /// </summary>
        /// <param name="ocxFilePath">ocx插件文件路径</param>
        /// <param name="guid">ocx插件标识</param>
        public static void Registe(string ocxFilePath, string guid)
        {
            if (string.IsNullOrWhiteSpace(ocxFilePath))
            {
                throw new ArgumentNullException(nameof(ocxFilePath));
            }

            if (!File.Exists(ocxFilePath))
            {
                throw new FileNotFoundException("ocx插件文件不存在", ocxFilePath);
            }

            if (string.IsNullOrWhiteSpace(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }

            if (IsRegisted(ocxFilePath, guid))
            {
                return;
            }

            IntPtr ptr = UnManagedDll.LoadEx(ocxFilePath);
            var handle = (OcxDllRegisterHandle)UnManagedDll.GetProcDelegate(ptr, "DllRegisterServer", typeof(OcxDllRegisterHandle), CharSet.Ansi);
            int ret = handle();
            if (ret < 0)
            {
                throw new ArgumentException($"注册失败,{ret}");
            }

            UnManagedDll.Free(ptr);
        }

        /// <summary>
        /// 卸载Ocx插件
        /// </summary>
        /// <param name="ocxFilePath">ocx插件文件路径</param>
        /// <param name="guid">ocx插件标识</param>
        public static void Unregiste(string ocxFilePath, string guid)
        {
            if (string.IsNullOrWhiteSpace(ocxFilePath))
            {
                throw new ArgumentNullException(nameof(ocxFilePath));
            }

            if (!File.Exists(ocxFilePath))
            {
                throw new FileNotFoundException("ocx插件文件不存在", ocxFilePath);
            }

            if (string.IsNullOrWhiteSpace(guid))
            {
                throw new ArgumentNullException(nameof(guid));
            }

            if (!IsRegisted(ocxFilePath, guid))
            {
                return;
            }

            IntPtr ptr = UnManagedDll.LoadEx(ocxFilePath);
            var handle = (OcxDllRegisterHandle)UnManagedDll.GetProcDelegate(ptr, "DllUnregisterServer", typeof(OcxDllRegisterHandle), CharSet.Ansi);
            int ret = handle();
            if (ret < 0)
            {
                throw new ArgumentException($"卸载失败,{ret}");
            }

            UnManagedDll.Free(ptr);
        }
    }
}
