using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 注册表操作类
    /// </summary>
    public static class RegistryKeyEx
    {
        /// <summary>
        /// 打开注册表
        /// </summary>
        /// <param name="registryHive">注册表巢类型</param>
        /// <returns>打开的注册表</returns>
        public static RegistryKey OpeBaseKey(RegistryHive registryHive)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            return RegistryKey.OpenBaseKey(registryHive, registryView);
        }

        /// <summary>
        /// 设置开机自启动
        /// </summary>
        /// <param name="filePath">程序路径</param>
        /// <param name="isAutoRun">true开机启动/false删除开机启动</param>
        public static void SetAutoRun(string filePath, bool isAutoRun)
        {
            RegistryKey localMachineRegistry = RegistryKeyEx.OpeBaseKey(RegistryHive.LocalMachine);
            RegistryKey runRegKey = null;
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    throw new Exception(filePath + "应用程序不存在!");
                }

                string name = System.IO.Path.GetFileName(filePath);
                runRegKey = localMachineRegistry.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runRegKey == null)
                {
                    runRegKey = localMachineRegistry.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                }

                //是否开机自动启动
                if (isAutoRun)
                {
                    runRegKey.SetValue(name, filePath);
                }
                else
                {
                    runRegKey.SetValue(name, false);
                }
            }
            finally
            {
                RegistryKeyEx.CloseRegistryKey(runRegKey);
            }
        }

        /// <summary>
        /// 设置开机自动登录
        /// </summary>
        /// <param name="userName">系统登录用户名</param>
        /// <param name="password">系统登录密码</param>
        public static void SetAutoLogin(string userName, string password)
        {
            RegistryKey localMachineRegistry = RegistryKeyEx.OpeBaseKey(RegistryHive.LocalMachine);

            //在注册表中设置开机自动登录程序
            string winlogonRegKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
            RegistryKey autoLogOnRegKey = null;
            try
            {
                autoLogOnRegKey = localMachineRegistry.OpenSubKey(winlogonRegKeyPath, true);
                if (autoLogOnRegKey == null)
                {
                    //如果子键节点不存在,则创建之
                    autoLogOnRegKey = Registry.LocalMachine.CreateSubKey(winlogonRegKeyPath);
                }
                //在注册表中设置自启动程序
                autoLogOnRegKey.SetValue("AutoAdminLogon", "1");
                autoLogOnRegKey.SetValue("DefaultUserName", userName);
                autoLogOnRegKey.SetValue("DefaultPassword", password);
            }
            finally
            {
                RegistryKeyEx.CloseRegistryKey(autoLogOnRegKey);
            }
        }

        /// <summary>
        /// 异常关机（断电重启）以后是否出现关闭事件跟踪程序
        /// </summary>
        /// <param name="value">0:不出现关闭事件跟踪程序,1:出现关闭事件跟踪程序</param>
        public static void SetNotEventTrace(int value)
        {
            RegistryKey localMachineRegistry = RegistryKeyEx.OpeBaseKey(RegistryHive.LocalMachine);

            string reliabilityRegKeyPath = @"SOFTWARE\Policies\Microsoft\Windows NT\Reliability";
            RegistryKey exEventRegKey = null;
            try
            {
                exEventRegKey = localMachineRegistry.OpenSubKey(reliabilityRegKeyPath, true);
                if (exEventRegKey == null)
                {
                    //如果子键节点不存在,则创建之
                    exEventRegKey = Registry.LocalMachine.CreateSubKey(reliabilityRegKeyPath);
                }
                exEventRegKey.SetValue("ShutdownReasonOn", value);
            }
            finally
            {
                RegistryKeyEx.CloseRegistryKey(exEventRegKey);
            }
        }

        /// <summary>
        /// 关闭注册表
        /// </summary>
        /// <param name="key">要关闭的注册表键</param>
        private static void CloseRegistryKey(RegistryKey key)
        {
            if (key != null)
            {
                key.Close();
            }
        }
    }
}
