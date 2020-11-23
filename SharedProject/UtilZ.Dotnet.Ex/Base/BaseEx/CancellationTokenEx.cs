using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// CancellationToken扩展类
    /// </summary>
    public class CancellationTokenEx
    {
        private readonly CancellationToken _token;
        /// <summary>
        /// 线程取消通知Token
        /// </summary>
        public CancellationToken Token
        {
            get { return _token; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="token">CancellationToken</param>
        public CancellationTokenEx(CancellationToken token)
        {
            this._token = token;
        }

        /// <summary>
        /// 对Token,WaitHandle方法的包装,Blocks the current thread until the current System.Threading.WaitHandle receives a signal. 
        /// </summary>
        /// <returns>true if the current instance receives a signal. 
        /// If the current instance is never signaled, 
        /// System.Threading.WaitHandle.WaitOne(System.Int32,System.Boolean) never returns.</returns>
        public bool WaitOne()
        {
            try
            {
                return this._token.WaitHandle.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// 对Token,WaitHandle方法的包装,Blocks the current thread until the current System.Threading.WaitHandle receives a signal, using a 32-bit signed integer to specify the time interval in milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or System.Threading.Timeout.Infinite (-1)to wait indefinitely</param>
        /// <returns>true if the current instance receives a signal; otherwise, false</returns>
        public bool WaitOne(int millisecondsTimeout)
        {
            try
            {
                return this._token.WaitHandle.WaitOne(millisecondsTimeout);
            }
            catch (AbandonedMutexException)
            {
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
