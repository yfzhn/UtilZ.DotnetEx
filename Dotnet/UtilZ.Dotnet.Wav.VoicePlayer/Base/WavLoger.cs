using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Wav.VoicePlayer.Base
{
    /// <summary>
    /// 音频日志类
    /// </summary>
    public class WavLoger
    {
        #region Wave日志事件
        /// <summary>
        /// Wav日志事件
        /// </summary>
        private static event EventHandler<WavLogInfoArgs> _wavLog;

        /// <summary>
        /// Wav日志事件属性
        /// </summary>
        public static event EventHandler<WavLogInfoArgs> WavLog
        {
            add { _wavLog += value; }
            remove
            {
                if (value != null)
                {
                    _wavLog -= value;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender">日志发生者</param>
        /// <param name="errorMessage">错误信息</param>
        public static void OnRaiseLog(object sender, string errorMessage)
        {
            OnRaiseLog(sender, errorMessage, null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender">日志发生者</param>
        /// <param name="exception">错误异常</param>
        public static void OnRaiseLog(object sender, Exception exception)
        {
            OnRaiseLog(sender, null, exception);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender">日志发生者</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="exception">错误异常</param>
        public static void OnRaiseLog(object sender, string errorMessage, Exception exception)
        {
            //两文参数都为null则不输出错误信息
            if (string.IsNullOrEmpty(errorMessage) && exception == null)
            {
                return;
            }

            var logArgs = new WavLogInfoArgs(errorMessage, exception);
            EventHandler<WavLogInfoArgs> handler = _wavLog;
            if (handler == null)
            {
                System.Diagnostics.Debug.WriteLine(logArgs.ToString());
            }
            else
            {
                handler(sender, logArgs);
            }
        }
        #endregion
    }
}
