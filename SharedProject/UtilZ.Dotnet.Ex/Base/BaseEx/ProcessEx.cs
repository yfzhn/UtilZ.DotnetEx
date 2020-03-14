using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 进程扩展方法
    /// </summary>
    public partial class ProcessEx
    {
        /// <summary>
        /// 进程互斥对象
        /// </summary>
        private static Mutex _processMutex = null;

        /// <summary>
        /// 多线程锁
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 单进程检测[创建进程互斥对象成功返回true;否则返回false]
        /// </summary>
        /// <param name="rangeFlag">单进程检测范围[true:所有用户;false:仅当前用户]</param>
        /// <param name="mutexName">互斥变量名称</param>
        /// <returns>创建进程互斥对象成功返回true;否则返回false</returns>
        public static bool SingleProcessCheck(bool rangeFlag, string mutexName = null)
        {
            lock (_lock)
            {
                if (_processMutex != null)
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(mutexName))
                {
                    mutexName = Assembly.GetEntryAssembly().GetCustomAttributes(true).Where(t => t is GuidAttribute).Cast<GuidAttribute>().Select(t => t.Value).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(mutexName))
                    {
                        //mutexName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;
                        mutexName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
                    }
                }

                if (rangeFlag)
                {
                    mutexName = @"Global\" + mutexName;
                }

                bool createNew;
                try
                {
                    _processMutex = new Mutex(true, mutexName, out createNew);
                }
                catch
                {
                    createNew = false;
                }

                return createNew;
            }
        }

        /// <summary>
        /// 同步执行程序并返回该程序的执行输出结果
        /// </summary>
        /// <param name="appPath">应用程序路径</param>
        /// <param name="args">启动参数</param>
        /// <param name="millisecondsTimeout">执行超时时长-1无限时长</param>
        /// <returns>执行输出结果</returns>
        public static string SynExcuteCmd(string appPath, string args, int millisecondsTimeout = Timeout.Infinite)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            try
            {
                Process pro = new Process();
                pro.StartInfo = new ProcessStartInfo();
                pro.StartInfo.FileName = appPath;
                pro.StartInfo.Arguments = args;
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pro.StartInfo.RedirectStandardOutput = true;
                pro.StartInfo.UseShellExecute = false;
                pro.EnableRaisingEvents = true;
                pro.Exited += (s, e) =>
                {
                    autoResetEvent.Set();
                };

                pro.Start();
                if (autoResetEvent.WaitOne(millisecondsTimeout))
                {
                    return pro.StandardOutput.ReadToEnd();
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            finally
            {
                autoResetEvent.Dispose();
            }
        }



        /// <summary>
        /// 验证进程ID是否有效[有效返回true;无效返回false]
        /// </summary>
        /// <param name="id">进程ID</param>
        /// <returns>有效返回true;无效返回false</returns>
        public static bool ValidaProcessId(int id)
        {
            return id > 0;
        }

        /// <summary>
        /// 根据程序路径查找进程
        /// </summary>
        /// <param name="appExeFilePath">根据程序路</param>
        /// <returns>进程列表</returns>
        public static List<Process> FindProcessByFilePath(string appExeFilePath)
        {
            var resultPros = new List<Process>();
            Process[] pros = Process.GetProcesses();
            foreach (var pro in pros)
            {
                try
                {
                    if (string.Equals(appExeFilePath, pro.MainModule.FileName, StringComparison.OrdinalIgnoreCase))
                    {
                        resultPros.Add(pro);
                    }
                }
                catch
                { }
            }

            return resultPros;
        }

        private static void PrimitiveKill(Process process, int hostProcessId)
        {
            try
            {
                process.Refresh();
                if (process == null || process.HasExited)
                {
                    return;
                }

                if (process.Id == hostProcessId)
                {
                    return;
                }

                process.Kill();
            }
            catch
            { }
        }

        /// <summary>
        /// 通过指定应用程序的名称和一组命令行参数来启动一个进程资源，并将该资源与新的 System.Diagnostics.Process 组件相关联
        /// </summary>
        /// <param name="fileName">要在该进程中运行的应用程序文件的名称</param>
        /// <param name="arguments">启动该进程时传递的命令行实参</param>
        /// <returns>与该进程关联的新的 System.Diagnostics.Process 组件；如果没有启动进程资源（例如，如果重用了现有进程），则为 null</returns>
        public static Process Start(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;
            return Process.Start(startInfo);
        }
    }
}
