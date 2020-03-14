using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志志属异步队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class LogAsynQueue<T>
    {
        /// <summary>
        /// 异步队列线程
        /// </summary>
        private readonly Thread _thread = null;

        /// <summary>
        /// 线程取消通知对象
        /// </summary>
        private readonly CancellationTokenSource _cts = null;

        /// <summary>
        /// Queue
        /// </summary>
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        /// <summary>
        /// 入队消息通知
        /// </summary>
        private readonly AutoResetEvent _enqueueEventHandler = new AutoResetEvent(false);

        /// <summary>
        /// 数据处理委托
        /// </summary>
        private readonly Action<T> _processAction;

        /// <summary>
        /// 是否已释放过资源[true:已释放过;false:未释放过]
        /// </summary>
        private bool _isDispose = false;




        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="processAction">数据处理委托</param>
        /// <param name="threadName">异步队列线程名称</param>
        public LogAsynQueue(Action<T> processAction, string threadName)
        {
            if (processAction == null)
            {
                throw new ArgumentNullException(nameof(processAction), "数据处理回调不能为null");
            }

            this._processAction = processAction;
            this._cts = new CancellationTokenSource();
            this._thread = new Thread(this.LogThreadMethod);
            this._thread.IsBackground = false;
            this._thread.Name = threadName;
            this._thread.Start();
        }

        /// <summary>
        /// 线程方法
        /// </summary>
        private void LogThreadMethod()
        {
            try
            {
                T item;
                var token = this._cts.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (this._queue.Count == 0)
                        {
                            try
                            {
                                this._enqueueEventHandler.WaitOne();
                                continue;
                            }
                            catch (ObjectDisposedException)
                            {
                                break;
                            }
                        }

                        if (this._queue.TryDequeue(out item))
                        {
                            this._processAction(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogSysInnerLog.OnRaiseLog(this, ex);
                    }
                }
            }
            catch (Exception e)
            {
                LogSysInnerLog.OnRaiseLog(this, e);
            }
        }

        /// <summary>
        /// 将对象添加到队列的结尾处
        /// </summary>
        /// <param name="item">待添加的对象</param>
        public void Enqueue(T item)
        {
            this._queue.Enqueue(item);
            this._enqueueEventHandler.Set();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this._isDispose)
            {
                return;
            }

            this._cts.Cancel();
            this._enqueueEventHandler.Set();
            this._cts.Dispose();
            this._enqueueEventHandler.Dispose();
            this._isDispose = true;
        }
    }
}
