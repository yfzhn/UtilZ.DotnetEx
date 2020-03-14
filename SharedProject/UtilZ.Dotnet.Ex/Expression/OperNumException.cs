using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.Basepression
{
    /// <summary>
    /// 操作数异常
    /// </summary>
    [Serializable]
    public class OperNumException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        public OperNumException(string message)
            : base(message)
        { }
    }
}
