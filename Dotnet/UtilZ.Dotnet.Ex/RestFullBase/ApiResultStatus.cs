using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.RestFullBase
{
    /// <summary>
    /// Api结果状态
    /// </summary>
    public enum ApiResultStatus : byte
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succes = 1,

        /// <summary>
        /// 失败
        /// </summary>
        Fail = 2,

        /// <summary>
        /// 异常
        /// </summary>
        Exception = 3
    }
}
