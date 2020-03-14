using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 文件日志追加器
    /// </summary>
    public class FileAppender : AppenderBase
    {
        private FileAppenderConfig _fileAppenderConfig;
        private FileLogWriterBase _fileLogWriter = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public FileAppender(XElement ele) : base(ele)
        {
            this.Init();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置对象</param>
        public FileAppender(BaseConfig config) : base(config)
        {
            this.Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this._fileLogWriter = null;
            var fileAppenderConfig = (FileAppenderConfig)base._config;
            this._fileAppenderConfig = fileAppenderConfig;
            var pathManager = new FileAppenderPathManager(this._fileAppenderConfig);

            switch (this._fileAppenderConfig.LockingModel)
            {
                case LockingModel.Exclusive:
                    this._fileLogWriter = new ExclusiveFileLogWriter(fileAppenderConfig, pathManager);
                    break;
                case LockingModel.Minimal:
                    this._fileLogWriter = new MinimalFileLogWriter(fileAppenderConfig, pathManager);
                    break;
                case LockingModel.InterProcess:
                    this._fileLogWriter = new InterProcessFileLogWriter(fileAppenderConfig, pathManager);
                    break;
                default:
                    LogSysInnerLog.OnRaiseLog(this, new NotSupportedException(string.Format("不支持的锁模型:{0}", this._fileAppenderConfig.LockingModel.ToString())));
                    break;
            }
        }

        /// <summary>
        /// 创建配置对象实例
        /// </summary>
        /// <param name="ele">配置元素</param>
        /// <returns>配置对象实例</returns>
        protected override BaseConfig CreateConfig(XElement ele)
        {
            return new FileAppenderConfig(ele);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="item">日志项</param>
        protected override void PrimitiveWriteLog(LogItem item)
        {
            try
            {
                if (this._fileAppenderConfig == null ||
                this._fileLogWriter == null ||
                !base.Validate(this._fileAppenderConfig, item) ||
                !this._fileAppenderConfig.Enable)
                {
                    return;
                }

                string logMsg = base.LayoutLog(item);
                this._fileLogWriter.WriteLog(new LogMessageItem(item.LogerName, logMsg));
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }
    }
}
