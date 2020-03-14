using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 线程安全集合提供阻止和限制功能
    /// </summary>
    /// <typeparam name="T">集合中的元素类型</typeparam>
    [ComVisible(false)]
    public class BlockingCollectionEx<T> : IDisposable
    {
        private Queue<T> _queue = new Queue<T>();
        private readonly object _queueLock = new object();
        private const int _DEFAULT_TIMEOUT = 10;
        private readonly AutoResetEvent _queueChangedEventHandle = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _isDisposabled = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BlockingCollectionEx()
        {

        }

        /// <summary>
        /// 获取项数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this._queueLock)
                {
                    return this._queue.Count;
                }
            }
        }

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            lock (this._queueLock)
            {
                this._queue.Enqueue(item);
                try
                {
                    if (this._isDisposabled)
                    {
                        return;
                    }

                    this._queueChangedEventHandle.Set();
                }
                catch (ObjectDisposedException)
                { }
            }
        }

        /// <summary>
        /// 从集合中中获取一项,如果没有,则阻塞
        /// </summary>
        /// <returns>从该集合中移除的项</returns>
        public T Take()
        {
            return this.Take(this._cts.Token);
        }

        /// <summary>
        /// 从集合中中获取一项,如果没有,则阻塞
        /// </summary>
        /// <param name="token">操作取消通知</param>
        /// <returns>从该集合中移除的项</returns>
        public T Take(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (this._queueLock)
                {
                    if (this._queue.Count > 0)
                    {
                        return this._queue.Dequeue();
                    }
                }

                if (this._isDisposabled)
                {
                    break;
                }

                try
                {
                    this._queueChangedEventHandle.WaitOne(_DEFAULT_TIMEOUT);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }

            throw new OperationCanceledException();
        }

        /// <summary>
        /// 从集合中中获取一项,如果没有,则阻塞
        /// </summary>
        /// <param name="millisecondsTimeout">阻塞超时时长,单位毫秒</param>
        /// <returns>从该集合中移除的项</returns>
        public T Take(int millisecondsTimeout)
        {
            return this.Take(millisecondsTimeout, this._cts.Token);
        }

        /// <summary>
        /// 从集合中中获取一项,如果没有,则阻塞
        /// </summary>
        /// <param name="millisecondsTimeout">阻塞超时时长,单位毫秒</param>
        /// <param name="token">操作取消通知</param>
        /// <returns>从该集合中移除的项</returns>
        public T Take(int millisecondsTimeout, CancellationToken token)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            T item = default(T);

            try
            {
                int timeout = _DEFAULT_TIMEOUT;

                while (!token.IsCancellationRequested)
                {
                    lock (this._queueLock)
                    {
                        if (this._queue.Count > 0)
                        {
                            item = this._queue.Dequeue();
                            break;
                        }
                    }

                    if (millisecondsTimeout != Timeout.Infinite)
                    {
                        timeout = millisecondsTimeout - watch.Elapsed.Milliseconds;
                        if (timeout < 1)
                        {
                            break;
                        }

                        if (timeout > _DEFAULT_TIMEOUT)
                        {
                            timeout = _DEFAULT_TIMEOUT;
                        }
                    }

                    try
                    {
                        if (this._isDisposabled || !this._queueChangedEventHandle.WaitOne(timeout))
                        {
                            break;
                        }
                    }
                    catch (ObjectDisposedException)
                    { }
                }

                if (token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                return item;
            }
            finally
            {
                watch.Stop();
            }
        }



        /// <summary>
        /// 在观察取消标记时，尝试在指定的时间内从集合中移除某个项
        /// 异常:
        /// System.OperationCanceledException:如果集合释放。
        /// System.ArgumentOutOfRangeException:millisecondsTimeout 是一个非 -1 的负数，而 -1 表示无限期超时。
        /// </summary>
        /// <param name="item">从该集合中移除的项</param>
        /// <returns>如果在指定的时间内可以从集合中移除某个项，则为 true；否则为 false</returns>
        public bool TryTake(out T item)
        {
            CancellationToken token;
            try
            {
                token = _cts.Token;
            }
            catch (ObjectDisposedException)
            {
                item = default(T);
                return false;
            }

            return this.TryTake(out item, Timeout.Infinite, token);
        }

        /// <summary>
        /// 在观察取消标记时，尝试在指定的时间内从集合中移除某个项
        /// 异常:
        /// System.OperationCanceledException:如果集合释放。
        /// System.ArgumentOutOfRangeException:millisecondsTimeout 是一个非 -1 的负数，而 -1 表示无限期超时。
        /// </summary>
        /// <param name="item">从该集合中移除的项</param>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 System.Threading.Timeout.Infinite (-1)，表示无限期等待</param>
        /// <returns>如果在指定的时间内可以从集合中移除某个项，则为 true；否则为 false</returns>
        public bool TryTake(out T item, int millisecondsTimeout)
        {
            CancellationToken token;
            try
            {
                token = _cts.Token;
            }
            catch (ObjectDisposedException)
            {
                item = default(T);
                return false;
            }

            return this.TryTake(out item, millisecondsTimeout, token);
        }

        /// <summary>
        /// 在观察取消标记时，尝试在指定的时间内从集合中移除某个项
        /// 异常:
        /// System.OperationCanceledException:如果 System.Threading.CancellationToken已取消。
        /// System.ArgumentOutOfRangeException:millisecondsTimeout 是一个非 -1 的负数，而 -1 表示无限期超时。
        /// </summary>
        /// <param name="item">从该集合中移除的项</param>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 System.Threading.Timeout.Infinite (-1)，表示无限期等待</param>
        /// <param name="token">要观察的取消标记</param>
        /// <returns>如果在指定的时间内可以从集合中移除某个项，则为 true；否则为 false</returns>
        public bool TryTake(out T item, int millisecondsTimeout, CancellationToken token)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                int timeout = _DEFAULT_TIMEOUT;

                while (!token.IsCancellationRequested)
                {
                    lock (this._queueLock)
                    {
                        if (this._queue.Count > 0)
                        {
                            item = this._queue.Dequeue();
                            return true;
                        }
                    }

                    if (millisecondsTimeout != Timeout.Infinite)
                    {
                        timeout = millisecondsTimeout - watch.Elapsed.Milliseconds;
                        if (timeout < 1)
                        {
                            break;
                        }

                        if (timeout > _DEFAULT_TIMEOUT)
                        {
                            timeout = _DEFAULT_TIMEOUT;
                        }
                    }

                    try
                    {
                        if (this._isDisposabled || !this._queueChangedEventHandle.WaitOne(timeout))
                        {
                            break;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }

                item = default(T);
                return false;
            }
            finally
            {
                watch.Stop();
            }
        }



        /// <summary>
        /// 将项从集合中实例复制到新数组中
        /// </summary>
        /// <returns>包含集合元素的副本的数组</returns>
        public T[] ToArray()
        {
            lock (this._queueLock)
            {
                return this._queue.ToArray();
            }
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            lock (this._queueLock)
            {
                if (this._queue.Count > 0)
                {
                    this._queue = new Queue<T>();
                }
            }
        }


        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            lock (this._queueLock)
            {
                try
                {
                    if (this._isDisposabled)
                    {
                        return;
                    }

                    this._cts.Cancel();
                    this._cts.Dispose();
                    this._queueChangedEventHandle.Dispose();
                    this._isDisposabled = true;

                }
                catch (Exception ex)
                {
                    Loger.Error(ex, "Dispose异常");
                }
            }
        }
    }
}
