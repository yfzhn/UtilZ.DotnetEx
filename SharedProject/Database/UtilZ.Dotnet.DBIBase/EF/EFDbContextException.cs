using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.DBIBase.EF
{
    /// <summary>
    ///  EF上下文异常
    /// </summary>
    [Serializable]
    public class EFDbContextException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EFDbContextException()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        public EFDbContextException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="ex">异常信息</param>
        public EFDbContextException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
