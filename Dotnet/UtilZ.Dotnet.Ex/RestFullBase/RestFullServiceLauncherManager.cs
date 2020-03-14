using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// RestFullServiceLauncher管理类
    /// </summary>
    public class RestFullServiceLauncherManager
    {
        private static List<IRestFullServiceLauncher> _serviceLauncherList = new List<IRestFullServiceLauncher>();
        private static readonly object _serviceLauncherListLock = new object();

        /// <summary>
        /// 智能查找当前应用程序域里所有程序集中包含的RestFullServiceLauncher
        /// </summary>
        public static void SmartFindRestFullServiceLauncherManager()
        {
            Type iServiceLauncherType = typeof(IRestFullServiceLauncher);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (var type in types)
                {
                    try
                    {
                        if (!type.IsClass
                            || type.IsAbstract
                            || type.GetInterface(iServiceLauncherType.FullName) == null
                            || type.GetConstructors().Where(t => { return t.GetParameters().Length == 0; }).Count() < 1)

                        {
                            //不是类、抽象类、没有实现IServiceLauncher接口、无参构造函数个数为0的类型,忽略
                            continue;
                        }

                        var serviceLauncher = (IRestFullServiceLauncher)Activator.CreateInstance(type);
                        _serviceLauncherList.Add(serviceLauncher);
                    }
                    catch (Exception ex)
                    {
                        Loger.Error(ex, "RestFullServiceLauncher");
                    }
                }
            }
        }

        /// <summary>
        /// 注册RestFullServiceLauncher
        /// </summary>
        /// <param name="restFullServiceLauncher">目标RestFullServiceLauncher </param>
        public static void RegistRestFullServiceLauncher(IRestFullServiceLauncher restFullServiceLauncher)
        {
            if (restFullServiceLauncher == null)
            {
                return;
            }

            lock (_serviceLauncherListLock)
            {
                _serviceLauncherList.Add(restFullServiceLauncher);
            }
        }

        /// <summary>
        /// 移除RestFullServiceLauncher
        /// </summary>
        /// <param name="restFullServiceLauncher">目标RestFullServiceLauncher </param>
        public static void RemoveRestFullServiceLauncher(IRestFullServiceLauncher restFullServiceLauncher)
        {
            if (restFullServiceLauncher == null)
            {
                return;
            }

            lock (_serviceLauncherListLock)
            {
                _serviceLauncherList.Remove(restFullServiceLauncher);
            }
        }

        /// <summary>
        /// 启动全部RestFullServiceLauncher
        /// </summary>
        public static void Start()
        {
            lock (_serviceLauncherListLock)
            {
                foreach (var serviceLauncher in _serviceLauncherList)
                {
                    serviceLauncher.Start();
                }
            }
        }

        /// <summary>
        /// 停止全部RestFullServiceLauncher
        /// </summary>
        public static void Stop()
        {
            lock (_serviceLauncherListLock)
            {
                foreach (var serviceLauncher in _serviceLauncherList)
                {
                    serviceLauncher.Stop();
                    serviceLauncher.Dispose();
                }
            }
        }
    }
}
