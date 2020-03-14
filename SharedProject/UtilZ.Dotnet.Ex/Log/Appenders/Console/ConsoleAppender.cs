using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 控制台日志输出追加器
    /// </summary>
    public class ConsoleAppender : AppenderBase
    {
        private readonly ConsoleAppenderConfig _consoleAppenderConfig;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public ConsoleAppender(XElement ele) : base(ele)
        {
            this._consoleAppenderConfig = (ConsoleAppenderConfig)base._config;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置对象</param>
        public ConsoleAppender(BaseConfig config) : base(config)
        {
            this._consoleAppenderConfig = (ConsoleAppenderConfig)base._config;
        }

        /// <summary>
        /// 创建配置对象实例
        /// </summary>
        /// <param name="ele">配置元素</param>
        /// <returns>配置对象实例</returns>
        protected override BaseConfig CreateConfig(XElement ele)
        {
            return new ConsoleAppenderConfig(ele);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="item">日志项</param>
        protected override void PrimitiveWriteLog(LogItem item)
        {
            try
            {
                if (this._consoleAppenderConfig == null || !base.Validate(this._consoleAppenderConfig, item))
                {
                    return;
                }

                string logMsg = base.LayoutLog(item);
                Console.WriteLine(logMsg);
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }
    }
}
