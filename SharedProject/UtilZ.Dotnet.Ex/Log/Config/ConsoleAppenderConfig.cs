using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志追加器配置
    /// </summary>
    [Serializable]
    public class ConsoleAppenderConfig : BaseConfig
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public ConsoleAppenderConfig(XElement ele)
            : base(ele)
        {

        }
    }
}
