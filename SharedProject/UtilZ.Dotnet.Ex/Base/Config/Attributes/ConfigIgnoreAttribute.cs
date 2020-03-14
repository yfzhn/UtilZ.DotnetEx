using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{

    /// <summary>
    /// 标记配置项为忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigIgnoreAttribute : ConfigAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigIgnoreAttribute()
            : base()
        {

        }
    }
}
