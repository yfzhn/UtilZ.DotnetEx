using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Log
{
    internal class InterProcessFileLogWriter : FileLogWriterBase
    {
        public InterProcessFileLogWriter(FileAppenderConfig fileAppenderConfig, FileAppenderPathManager pathManager) :
            base(fileAppenderConfig, pathManager)
        {


        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="fileAppenderConfig">配置</param>
        /// <param name="pathManager">路由管理器</param>
        /// <param name="createFilePathTime">创建时间</param>
        /// <param name="logMessage">日志信息项</param>
        protected override void WriteLog(FileAppenderConfig fileAppenderConfig, FileAppenderPathManager pathManager, DateTime createFilePathTime, LogMessageItem logMessage)
        {
            Mutex mutex = null;
            try
            {
                string logFilePath = base.GetLogFilePath();
                if (string.IsNullOrWhiteSpace(logFilePath))
                {
                    return;
                }

                mutex = this.GetMutex(logFilePath);
                using (var sw = File.AppendText(logFilePath))
                {
                    //日志处理
                    base.WriteLogToFile(logMessage, sw);
                }
            }
            finally
            {
                this.ReleaseMutex(mutex);
            }
        }

        /// <summary>
        /// 获取进程锁
        /// </summary>
        /// <returns>进程锁</returns>
        private Mutex GetMutex(string logFilePath)
        {
            string mutexName = logFilePath.Replace("\\", "_").Replace(":", "_").Replace("/", "_");
            Mutex mutex = null;
            while (mutex == null)
            {
                try
                {
                    //如果此命名互斥对象已存在则请求打开
                    try
                    {
                        //如果此命名互斥对象已存在则请求打开
                        mutex = Mutex.OpenExisting(mutexName);
                    }
                    catch (WaitHandleCannotBeOpenedException)
                    {
                        //打开失败则创建一个
                        mutex = new Mutex(false, mutexName);
                    }

                    mutex.WaitOne();
                }
                catch (Exception ex)
                {
                    LogSysInnerLog.OnRaiseLog(this, ex);
                    Thread.Sleep(10);
                }
            }

            return mutex;
        }

        /// <summary>
        /// 释放进程锁
        /// </summary>
        /// <param name="mutex">进程锁</param>
        private void ReleaseMutex(Mutex mutex)
        {
            try
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }
    }
}
