using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav
{
    // WAV播放控件-事件
    public partial class WavePlayer
    {
        #region Wave日志事件
        /// <summary>
        /// Wav日志事件
        /// </summary>
        private event EventHandler<WavLogInfoArgs> _wavLog;

        /// <summary>
        /// Wav日志事件属性
        /// </summary>
        public event EventHandler<WavLogInfoArgs> WavLog
        {
            add { this._wavLog += value; }
            remove
            {
                if (value != null)
                {
                    this._wavLog -= value;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        private void OnRaiseLog(string errorMessage)
        {
            this.OnRaiseLog(errorMessage, null);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exception">错误异常</param>
        private void OnRaiseLog(Exception exception)
        {
            this.OnRaiseLog(null, exception);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="exception">错误异常</param>
        private void OnRaiseLog(string errorMessage, Exception exception)
        {
            //两文参数都为null则不输出错误信息
            if (string.IsNullOrEmpty(errorMessage) && exception == null)
            {
                return;
            }

            var logArgs = new WavLogInfoArgs(errorMessage, exception);
            EventHandler<WavLogInfoArgs> handler = this._wavLog;
            if (handler == null)
            {
                System.Diagnostics.Debug.WriteLine(logArgs.ToString());
            }
            else
            {
                handler(this, logArgs);
            }
        }
        #endregion

        /// <summary>
        /// 播放结束事件
        /// </summary>
        public event EventHandler PlayEnd;

        /// <summary>
        /// 触发播放结束事件
        /// </summary>
        private void OnRaisePlayEnd()
        {
            EventHandler handler = this.PlayEnd;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
