using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    internal abstract class FileLogWriterBase
    {
        private readonly FileAppenderConfig _fileAppenderConfig;
        private readonly FileAppenderPathManager _pathManager;

        /// <summary>
        /// 日志安全策略
        /// </summary>
        private ILogSecurityPolicy _securityPolicy = null;
        private string _filePath;
        private long _fileSize = long.MinValue;
        private DateTime _createFilePathTime = DateTime.Now.AddMonths(-1);

        public FileLogWriterBase(FileAppenderConfig fileAppenderConfig, FileAppenderPathManager pathManager)
        {
            this._fileAppenderConfig = fileAppenderConfig;
            this._pathManager = pathManager;
            try
            {
                Type type = LogUtil.GetType(fileAppenderConfig.SecurityPolicy);
                if (type != null && type.GetInterface(typeof(ILogSecurityPolicy).FullName) != null)
                {
                    this._securityPolicy = Activator.CreateInstance(type) as ILogSecurityPolicy;
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logMessage">日志信息项</param>
        public void WriteLog(LogMessageItem logMessage)
        {
            this.WriteLog(this._fileAppenderConfig, this._pathManager, this._createFilePathTime, logMessage);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="fileAppenderConfig">配置</param>
        /// <param name="pathManager">路由管理器</param>
        /// <param name="createFilePathTime">创建时间</param>
        /// <param name="logMessage">日志信息项</param>
        protected abstract void WriteLog(FileAppenderConfig fileAppenderConfig, FileAppenderPathManager pathManager, DateTime createFilePathTime, LogMessageItem logMessage);

        /// <summary>
        /// 写日志到文件
        /// </summary>
        /// <param name="logMessage">日志信息项</param>
        /// <param name="sw">StreamWriter</param>
        protected void WriteLogToFile(LogMessageItem logMessage, StreamWriter sw)
        {
            string logMsg = logMessage.LogMsg;
            if (this._securityPolicy != null)
            {
                logMsg = this._securityPolicy.Encryption(logMsg);
            }

            sw.WriteLine(logMsg);
            sw.Flush();
            this._fileSize = sw.BaseStream.Length;
        }

        /// <summary>
        /// 获得日志文件路径
        /// </summary>
        /// <returns>日志文件路径</returns>
        protected string GetLogFilePath()
        {
            /********************************************************************
            * Log\*yyyy-MM-dd_HH_mm_ss*_flow.log  =>  Log\2018-08-19_17_05_12_flow.log
            * *yyyy-MM-dd*\info.log  =>  2018-08-19\info_1.log 或 2018-08-19\info_n.log
            * *yyyy-MM-dd*\*yyyy-MM-dd_HH_mm_ss*_flow.log  =>  2018-08-19\2018-08-19_17_05_12_flow.log
            * 或
            * *yyyy-MM-dd*\*HH_mm_ss*_flow.log  =>  2018-08-19\17_05_12_flow.log
            ********************************************************************/

            DateTime currentTime = DateTime.Now;
            if (this._pathManager.IsFixPath ||
                !string.IsNullOrWhiteSpace(this._filePath) &&
                (this._fileAppenderConfig.MaxFileLength > 0 &&
                this._fileSize < this._fileAppenderConfig.MaxFileLength &&
                currentTime.Year == this._createFilePathTime.Year &&
                currentTime.Month == this._createFilePathTime.Month &&
                currentTime.Day == this._createFilePathTime.Day))
            {
                //前一次写入的文件名尚可用
                return this._filePath;
            }

            this._fileSize = 0;
            this._filePath = this._pathManager.CreateLogFilePath();
            this._createFilePathTime = currentTime;
            return this._filePath;
        }
    }
}
