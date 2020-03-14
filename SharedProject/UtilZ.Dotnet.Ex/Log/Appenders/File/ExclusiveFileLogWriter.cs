using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    internal class ExclusiveFileLogWriter : FileLogWriterBase
    {
        private StreamWriter _sw = null;

        public ExclusiveFileLogWriter(FileAppenderConfig fileAppenderConfig, FileAppenderPathManager pathManager) :
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
            DateTime currentTime = DateTime.Now;
            if (this._sw != null &&
                !pathManager.IsFixPath &&
                (fileAppenderConfig.MaxFileLength > 0 &&
                this._sw.BaseStream.Length >= fileAppenderConfig.MaxFileLength ||
                currentTime.Year != createFilePathTime.Year ||
                currentTime.Month != createFilePathTime.Month ||
                currentTime.Day != createFilePathTime.Day))
            {
                this._sw.Close();
                this._sw = null;
            }

            if (this._sw == null)
            {
                string logFilePath = base.GetLogFilePath();
                if (string.IsNullOrWhiteSpace(logFilePath))
                {
                    return;
                }

                this._sw = File.AppendText(logFilePath);
            }

            base.WriteLogToFile(logMessage, this._sw);
        }
    }
}
