using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base.MemoryCache;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// EventWaitHandle扩展类,通知正在等待的线程已发生事件
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class EventWaitHandleEx : IDisposable
    {
        /// <summary>
        /// 通知正在等待的线程已发生事件对象
        /// </summary>
        private readonly EventWaitHandle _eventWaitHandle;

        /// <summary>
        /// 对象是否已释放[true:已释放;false:未释放]
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="mode"></param>
        public EventWaitHandleEx(bool initialState, EventResetMode mode)
        {
            if (mode == EventResetMode.AutoReset)
            {
                this._eventWaitHandle = new AutoResetEvent(initialState);
            }
            else if (mode == EventResetMode.ManualReset)
            {
                this._eventWaitHandle = new ManualResetEvent(initialState);
            }
            else
            {
                throw new NotImplementedException(mode.ToString());
            }
        }

        /// <summary>
        /// 将事件状态设置为非终止，从而导致线程受阻[如果该操作成功，则为 true；否则为 false]
        /// </summary>
        /// <returns>如果该操作成功，则为 true；否则为 false</returns>
        public bool Reset()
        {
            if (this._isDisposed)
            {
                return false;
                //throw new ObjectDisposedException("_eventWaitHandle");
            }

            try
            {
                return this._eventWaitHandle.Reset();
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 将事件状态设置为有信号，从而允许一个或多个等待线程继续执行[如果该操作成功，则为 true；否则为 false]
        /// </summary>
        /// <returns>如果该操作成功，则为 true；否则为 false</returns>
        public bool Set()
        {
            if (this._isDisposed)
            {
                return false;
                //throw new ObjectDisposedException("_eventWaitHandle");
            }

            try
            {
                return this._eventWaitHandle.Set();
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 阻止当前线程，直到当前 System.Threading.WaitHandle 收到信号，同时使用 32 位带符号整数指定时间间隔（以毫秒为单位）
        /// </summary>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 System.Threading.Timeout.Infinite (-1)，表示无限期等待</param>
        /// <returns></returns>
        public bool WaitOne(int millisecondsTimeout = System.Threading.Timeout.Infinite)
        {
            if (this._isDisposed)
            {
                return false;
                //throw new ObjectDisposedException("_eventWaitHandle");
            }

            try
            {
                return this._eventWaitHandle.WaitOne(millisecondsTimeout);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {
            if (this._isDisposed)
            {
                return;
            }

            this._eventWaitHandle.Dispose();
            this._isDisposed = true;
        }
    }
}
