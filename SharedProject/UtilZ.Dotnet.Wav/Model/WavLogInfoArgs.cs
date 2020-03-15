using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// Wav日志信息类
    /// </summary>
    public class WavLogInfoArgs : EventArgs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        public WavLogInfoArgs(string errorMessage)
            : this(errorMessage, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exception">错误异常</param>
        public WavLogInfoArgs(Exception exception)
            : this(null, exception)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="exception">错误异常</param>
        public WavLogInfoArgs(string errorMessage, Exception exception)
        {
            Time = DateTime.Now;
            this.ErrorMessage = errorMessage;
            this.Exception = exception;
        }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 错误异常
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 重写GetHashCode
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            string msg = ErrorMessage;
            if (string.IsNullOrEmpty(msg))
            {
                if (this.Exception != null)
                {
                    msg = this.Exception.ToString();
                }
                else
                {
                    msg = string.Empty;
                }
            }
            else
            {
                if (this.Exception != null)
                {
                    msg = string.Format("{0},{1}", msg, this.Exception);
                }
            }

            return msg;
        }
    }
}
