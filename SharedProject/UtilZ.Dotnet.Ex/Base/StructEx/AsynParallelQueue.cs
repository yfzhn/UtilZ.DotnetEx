using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 异步队列
    /// </summary>
    /// <typeparam name="T">输入数据类型</typeparam>
    /// <typeparam name="TResult">处理后输出数据类型</typeparam>
    public class AsynParallelQueue<T, TResult> : IDisposable
    {
        /// <summary>
        /// 异步队列线程
        /// </summary>
        private Thread _thread = null;

        /// <summary>
        /// 线程操作监视锁
        /// </summary>
        private readonly object _threadMonitor = new object();

        /// <summary>
        /// 当前并行运行对象停止状态通知
        /// </summary>
        private readonly ConcurrentList<ParallelLoopState> _parallelLoopStates = new ConcurrentList<ParallelLoopState>();

        /// <summary>
        /// 对象是否已释放[true:已释放;false:未释放]
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 线程取消通知对象
        /// </summary>
        private CancellationTokenSource _cts = null;

        /// <summary>
        /// BlockingCollection
        /// </summary>
        private readonly BlockingCollection<T> _blockingCollection;

        /// <summary>
        /// 入队列线程消息通知
        /// </summary>
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 空队列等待超时时间
        /// </summary>
        private readonly int _emptyQueueWaitTimeout = 10000;

        /// <summary>
        /// 停止线程消息通知
        /// </summary>
        private readonly AutoResetEvent _stopAutoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 队列线程状态[true:线程正在运行;false:线程未运行]
        /// </summary>
        private bool _status = false;
        /// <summary>
        /// 获取队列线程状态[true:线程正在运行;false:线程未运行]
        /// </summary>
        public bool Status
        {
            get { return _status; }
        }

        /// <summary>
        /// 获取队列容量[如果设置的容量小于当前已有队列长度,则丢弃掉队列头的项.直到队列长度与目标容量一致]
        /// </summary>
        public int Capity
        {
            get { return this._blockingCollection.BoundedCapacity; }
        }

        /// <summary>
        /// 并行处理最大线程数
        /// </summary>
        private readonly int _maxThreadCount = 0;

        /// <summary>
        /// 当队列中的项数少于批量处理最大项数时的等待时间,单位毫秒
        /// </summary>
        private readonly int _millisecondsTimeout;

        /// <summary>
        /// 数据处理委托
        /// </summary>
        public Func<T, CancellationToken, TResult> ProcessAction;

        /// <summary>
        /// 处理结果输出委托
        /// </summary>
        public Action<List<TResult>> ProcessResultAction;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="processAction">数据处理委托</param>
        /// <param name="processResultAction">处理结果输出委托</param>
        /// <param name="maxThreadCount">并行处理最大线程数,建议值Environment.ProcessorCount,线程运行时并不会一直占用CPU时间片</param>
        /// <param name="millisecondsTimeout">当队列中的项数少于批量处理最大项数时的等待时间,-1表示无限制等待,单位毫秒</param>
        /// <param name="isAutoStart">是否自动启动线程</param>
        /// <param name="capcity">队列容量</param>
        public AsynParallelQueue(Func<T, CancellationToken, TResult> processAction, Action<List<TResult>> processResultAction,
            int maxThreadCount, int millisecondsTimeout = 10, bool isAutoStart = false, int capcity = int.MaxValue)
        {
            if (maxThreadCount < 1)
            {
                throw new ArgumentException("并行处理最大线程数不能小于1", "maxThreadCount");
            }

            if (capcity < maxThreadCount)
            {
                throw new ArgumentException("队列容量不能小于并行处理最大线程数", "capcity");
            }

            if (millisecondsTimeout < -1)
            {
                throw new ArgumentException("当队列中的项数少于批量处理最大项数时的等待时间不能小于-1", "capcity");
            }
            else
            {
                this._millisecondsTimeout = millisecondsTimeout;
            }

            this._maxThreadCount = maxThreadCount;
            this._blockingCollection = new BlockingCollection<T>(new ConcurrentQueue<T>(), capcity);
            this.ProcessAction = processAction;
            this.ProcessResultAction = processResultAction;
            if (isAutoStart)
            {
                this.Start();
            }
        }

        /// <summary>
        /// 启动子类无参数工作线程
        /// </summary>
        /// <param name="apartmentState">指定的单元状态 System.Threading.Thread</param>
        public void Start(ApartmentState apartmentState = ApartmentState.Unknown)
        {
            lock (this._threadMonitor)
            {
                if (this._isDisposed)
                {
                    throw new ObjectDisposedException(string.Empty, "对象已释放");
                }

                if (this._status)
                {
                    return;
                }

                this._cts = new CancellationTokenSource();
                this._thread = new Thread(this.RunThreadQueueProcessMethod);
                this._thread.IsBackground = true;
                this._thread.SetApartmentState(apartmentState);
                this._thread.Start();
                this._status = true;
            }
        }

        /// <summary>
        /// 停止工作线程
        /// </summary>       
        /// <param name="isAbort">是否立即终止处理方法[true:立即终止;false:等待方法执行完成;默认false]</param>
        /// <param name="isSync">是否同步停止[true:同步停止;false:异常停止];注:注意线程死锁,典型场景:刷新UI,在UI上执行同步停止</param>
        /// <param name="synMillisecondsTimeout">同步超时时间,-1表示无限期等待,单位/毫秒[isSycn为true时有效]</param>
        public void Stop(bool isAbort = false, bool isSync = false, int synMillisecondsTimeout = -1)
        {
            lock (this._threadMonitor)
            {
                this.PrimitiveStop(isAbort, isSync, synMillisecondsTimeout);
            }
        }

        /// <summary>
        /// 停止工作线程
        /// </summary>       
        /// <param name="isAbort">是否立即终止处理方法[true:立即终止;false:等待方法执行完成;默认false]</param>
        /// <param name="isSync">是否同步停止[true:同步停止;false:异常停止];注:注意线程死锁,典型场景:刷新UI,在UI上执行同步停止</param>
        /// <param name="synMillisecondsTimeout">同步超时时间,-1表示无限期等待,单位/毫秒[isSycn为true时有效]</param>
        private void PrimitiveStop(bool isAbort, bool isSync, int synMillisecondsTimeout)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (!this._status)
            {
                return;
            }

            this._cts.Cancel();
            if (isAbort)
            {
                this._thread.Abort();
            }

            if (isSync)
            {
                if (!this._stopAutoResetEvent.WaitOne(synMillisecondsTimeout))
                {
                    if (!isAbort)
                    {
                        this._thread.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// 线程队列处理方法
        /// </summary>
        private void RunThreadQueueProcessMethod()
        {
            CancellationToken token = this._cts.Token;
            List<T> items = new List<T>();
            List<TResult> results = new List<TResult>();
            T item;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    while (items.Count < this._maxThreadCount)
                    {
                        try
                        {
                            if (this._blockingCollection.TryTake(out item, this._millisecondsTimeout, token))
                            {
                                items.Add(item);
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (ObjectDisposedException)
                        {
                            return;//已释放
                        }
                        catch (ArgumentNullException)
                        { }
                    }

                    if (items.Count == 0)
                    {
                        try
                        {
                            this._autoResetEvent.WaitOne(this._emptyQueueWaitTimeout);
                            continue;
                        }
                        catch (ObjectDisposedException)
                        {
                            return;
                        }
                    }

                    var processHandler = this.ProcessAction;
                    if (processHandler == null)
                    {
                        continue;
                    }

                    this._parallelLoopStates.Clear();
                    //并行处理
                    Parallel.ForEach(items, (t, parallelLoopState) =>
                    {
                        lock (this._threadMonitor)
                        {
                            if (this._status)
                            {
                                this._parallelLoopStates.Add(parallelLoopState);
                            }
                            else
                            {
                                parallelLoopState.Stop();
                                return;
                            }
                        }

                        results.Add(processHandler(t, token));
                    });
                    items.Clear();

                    //处理结果输出
                    var processResultHandler = this.ProcessResultAction;
                    if (processResultHandler != null)
                    {
                        processResultHandler(results.ToList());
                    }

                    results.Clear();
                }
            }
            catch (ThreadAbortException)
            {
                this.ThreadRunFinish();
                return;
            }

            this.ThreadRunFinish();
        }

        private void ThreadRunFinish()
        {
            lock (this._threadMonitor)
            {
                if (this._isDisposed)
                {
                    return;
                }

                if (!this._status)
                {
                    return;
                }

                foreach (var parallelLoopState in this._parallelLoopStates)
                {
                    parallelLoopState.Stop();
                }
                this._parallelLoopStates.Clear();

                this._thread = null;
                this._status = false;
                try
                {
                    if (!this._isDisposed)
                    {
                        this._stopAutoResetEvent.Set();
                    }
                }
                catch (ObjectDisposedException)
                { }
            }
        }

        /// <summary>
        /// 将对象添加到队列的结尾处[如果在指定的时间内可以将 item 添加到集合中，则为 true；否则为 false]
        /// </summary>
        /// <param name="item">待添加的对象</param>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 System.Threading.Timeout.Infinite (-1)，表示无限期等待</param>
        /// <param name="overflowItems">当队列超出策略为Dequeue时,先进入队列中移除项输出集合,如果为null则不输出</param>
        /// <returns>如果在指定的时间内可以将 item 添加到集合中，则为 true；否则为 false</returns>
        public bool Enqueue(T item, int millisecondsTimeout = System.Threading.Timeout.Infinite, List<T> overflowItems = null)
        {
            var ret = this._blockingCollection.TryAdd(item, millisecondsTimeout);
            if (ret)
            {
                this._autoResetEvent.Set();
            }

            return ret;
        }

        /// <summary>
        /// 移除位于开始处的指定个数对象
        /// </summary>
        /// <param name="count">要移除的项数</param>
        public List<T> Remove(int count = 1)
        {
            var items = new List<T>();
            T result;
            int removeCount = 0;
            while (removeCount < count && this._blockingCollection.Count > 0)
            {
                //移除一项
                if (this._blockingCollection.TryTake(out result))
                {
                    items.Add(result);
                    removeCount++;
                }
            }

            return items;
        }

        /// <summary>
        /// 获取队列中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return this._blockingCollection.Count;
            }
        }

        /// <summary>
        /// 将队列中存储的元素复制到新数组中
        /// </summary>
        /// <returns>新数组</returns>
        public T[] ToArray()
        {
            return this._blockingCollection.ToArray();
        }

        /// <summary>
        /// 从指定数组索引开始将 System.Collections.Concurrent.ConcurrentQueue`1 元素复制到现有一维 System.Array中
        /// 异常:
        /// T:System.ArgumentNullException:array 为 null 引用（在 Visual Basic 中为 Nothing）。
        /// T:System.ArgumentOutOfRangeException:index 小于零。
        /// T:System.ArgumentException:index 等于或大于该长度的 array -源中的元素数目 System.Collections.Concurrent.ConcurrentQueue`1大于从的可用空间 index 目标从头到尾 array。
        /// </summary>
        /// <param name="array">一维 System.Array，用作从 System.Collections.Concurrent.ConcurrentQueue`1 所复制的元素的目标数组。System.Array 必须具有从零开始的索引。</param>
        /// <param name="index">array 中从零开始的索引，从此处开始复制</param>
        public void CopyTo(T[] array, int index)
        {
            this._blockingCollection.CopyTo(array, index);
        }

        /// <summary>
        /// 清空队列,必须在停止时执行,否则后果未知
        /// </summary>
        public void Clear()
        {
            T result;
            while (this._blockingCollection.Count > 0)
            {
                this._blockingCollection.TryTake(out result);
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
            lock (this._threadMonitor)
            {
                if (this._isDisposed)
                {
                    return;
                }
                this._isDisposed = true;

                this.PrimitiveStop(false, false, 5000);
                if (this._cts != null)
                {
                    this._cts.Dispose();
                    this._cts = null;
                }

                this._blockingCollection.Dispose();
                this._stopAutoResetEvent.Dispose();
            }
        }
    }
}
