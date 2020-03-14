using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 进程扩展方法
    /// </summary>
    public partial class ProcessEx
    {
        /// <summary>
        /// 根据进程ID获取该进程的子进程列表
        /// </summary>
        /// <param name="id">进程ID</param>
        /// <returns>子进程列表</returns>
        public static List<Process> GetChildProcessListById(int id)
        {
            if (!ValidaProcessId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            var childProcessList = new List<Process>();
            int repeatCount = 0, maxRepeatCount = 3;
            Exception innerException = null;

            while (repeatCount < maxRepeatCount)
            {
                try
                {
                    repeatCount++;
                    childProcessList.Clear();
                    using (ManagementObjectSearcher search = new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessId={id}"))
                    {
                        using (ManagementObjectCollection moc = search.Get())
                        {
                            foreach (ManagementObject mo in moc)
                            {
                                childProcessList.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
                            }
                        }
                    }

                    return childProcessList;
                }
                catch (ManagementException me)
                {
                    innerException = me;
                }
                catch (System.Runtime.InteropServices.COMException come)
                {
                    innerException = come;
                }
            }

            throw new Exception("获取该进程的子进程列表异常", innerException);
        }

        /// <summary>
        /// 根据进程ID获取父进程
        /// </summary>
        /// <param name="id">进程ID</param>
        /// <returns>父进程</returns>
        public static Process GetParentProcessById(int id)
        {
            if (!ValidaProcessId(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            //try
            //{
            var processName = Process.GetProcessById(id).ProcessName;
            var processByName = Process.GetProcessesByName(processName);
            string processIndexName = null;
            for (var i = 0; i < processByName.Length; i++)
            {
                processIndexName = i == 0 ? processName : processName + "#" + i;
                var processIdPc = new PerformanceCounter("Process", "ID Process", processIndexName);
                if ((int)processIdPc.NextValue() == id)
                {
                    break;
                }
            }

            var parentIdPc = new PerformanceCounter("Process", "Creating Process ID", processIndexName);
            var parentId = (int)parentIdPc.NextValue();
            var pro = Process.GetProcessById(parentId);
            return pro;
            //}
            //catch (ArgumentException ae)
            //{
            //    if (ae.HResult == -2147024809)
            //    {
            //        return null;
            //    }

            //    throw ae;
            //}
        }

        /// <summary>
        /// 获取指定进程ID的顶级进程(explorer的下一级)
        /// </summary>
        /// <param name="id">进程ID</param>
        /// <returns>顶级进程</returns>
        public static Process GetTopProcessById(int id)
        {
            if (!ValidaProcessId(id))
            {
                return null;
            }

            string explorerFileFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            Process topProcess = null;
            try
            {
                Process lastProcess = Process.GetProcessById(id);
                while (!string.Equals(lastProcess.MainModule.FileName, explorerFileFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    topProcess = lastProcess;
                    lastProcess = GetParentProcessById(lastProcess.Id);
                }
            }
            catch
            { }

            return topProcess;
        }




        /// <summary>
        /// 根据进程ID杀死该进程以及所有子孙进程和父进程
        /// </summary>
        /// <param name="id">进程ID</param>
        public static void KillProcessTreeById(int id)
        {
            try
            {
                if (!ValidaProcessId(id))
                {
                    return;
                }

                Process process = GetTopProcessById(id);
                KillProcessTreeById(process);
            }
            catch
            { }
        }


        /// <summary>
        /// 根据进程ID杀死该进程以及所有子孙进程
        /// </summary>
        /// <param name="process">进程</param>
        public static void KillProcessTreeById(Process process)
        {
            Process hostProcess = Process.GetCurrentProcess();
            PrimitiveKillProcessTreeById(process, hostProcess.Id);
        }

        /// <summary>
        /// 根据进程ID杀死该进程以及所有子孙进程
        /// </summary>
        /// <param name="process">进程</param>
        /// <param name="hostProcessId">宿主进程ID(当前主进程ID)</param>
        private static void PrimitiveKillProcessTreeById(Process process, int hostProcessId)
        {
            try
            {
                if (process == null)
                {
                    return;
                }

                PrimitiveKill(process, hostProcessId);

                List<Process> childProcessList = GetChildProcessListById(process.Id);
                foreach (var childProcess in childProcessList)
                {
                    PrimitiveKill(childProcess, hostProcessId);
                    KillProcessTreeById(childProcess);
                }
            }
            catch
            { }
        }
    }
}
