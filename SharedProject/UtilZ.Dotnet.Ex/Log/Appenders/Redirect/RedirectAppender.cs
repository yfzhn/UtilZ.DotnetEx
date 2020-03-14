using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 重定向日志输出追加器
    /// </summary>
    public class RedirectAppender : AppenderBase
    {
        private readonly RedirectAppendConfig _redirectAppendConfig;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public RedirectAppender(XElement ele) 
            : base(ele)
        {
            this._redirectAppendConfig = (RedirectAppendConfig)base._config;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置对象</param>
        public RedirectAppender(BaseConfig config)
            : base(config)
        {
            this._redirectAppendConfig = (RedirectAppendConfig)base._config;
        }

        /// <summary>
        /// 创建配置对象实例
        /// </summary>
        /// <param name="ele">配置元素</param>
        /// <returns>配置对象实例</returns>
        protected override BaseConfig CreateConfig(XElement ele)
        {
            return new RedirectAppendConfig(ele);
        }

        /// <summary>
        /// 重定向输出事件
        /// </summary>
        public event EventHandler<RedirectOuputArgs> RedirectOuput;

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="item">日志项</param>
        protected override void PrimitiveWriteLog(LogItem item)
        {
            try
            {
                if (this._redirectAppendConfig == null ||
                    !base.Validate(this._redirectAppendConfig, item))
                {
                    return;
                }

                var handler = this.RedirectOuput;
                if (handler != null)
                {
                    handler(this, new RedirectOuputArgs(item));
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }
        }
    }
}
