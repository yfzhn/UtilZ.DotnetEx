using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 标记注释文本项
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigCommentAttribute : ConfigAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigCommentAttribute()
            : base()
        {

        }
    }
}
