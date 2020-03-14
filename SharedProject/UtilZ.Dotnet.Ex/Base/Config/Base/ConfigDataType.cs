using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.Config
{
    /// <summary>
    /// 配置数据类型
    /// </summary>
    internal enum ConfigDataType
    {
        /// <summary>
        /// 类型可与字符串相互转换
        /// </summary>
        Basic,

        /// <summary>
        /// 实现IDictionary接口的集合
        /// </summary>
        IDictionary,

        /// <summary>
        /// 实现IList接口的集合
        /// </summary>
        IList,

        /// <summary>
        /// 对象
        /// </summary>
        Object
    }
}
