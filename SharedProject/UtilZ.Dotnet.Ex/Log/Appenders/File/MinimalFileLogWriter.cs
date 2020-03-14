using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    internal class MinimalFileLogWriter : FileLogWriterBase
    {
        public MinimalFileLogWriter(FileAppenderConfig fileAppenderConfig, FileAppenderPathManager pathManager) :
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
            string logFilePath = base.GetLogFilePath();
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                return;
            }

            using (var sw = File.AppendText(logFilePath))
            {
                base.WriteLogToFile(logMessage, sw);
            }
        }
    }
}
