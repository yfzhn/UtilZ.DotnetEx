using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model
{
    /// <summary>
    /// 异步等待执行结果
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    public class PartAsynExcuteResult<T, TResult>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="para">异步执行参数</param>
        /// <param name="status">结果状态</param>
        /// <param name="result">异步执行返回值</param>
        /// <param name="exception">当执行异步委托出现异常时的异常信息</param>
        public PartAsynExcuteResult(T para, PartAsynExcuteStatus status, TResult result, Exception exception)
        {
            this.Para = para;
            this.Status = status;
            this.Result = result;
            this.Exception = exception;
        }

        /// <summary>
        /// 异步执行参数
        /// </summary>
        public T Para { get; private set; }

        /// <summary>
        /// 结果状态
        /// </summary>
        public PartAsynExcuteStatus Status { get; internal set; }

        /// <summary>
        /// 异步执行返回值
        /// </summary>
        public TResult Result { get; internal set; }

        /// <summary>
        /// 当执行异步委托出现异常时的异常信息
        /// </summary>
        public Exception Exception { get; internal set; }
    }
}
