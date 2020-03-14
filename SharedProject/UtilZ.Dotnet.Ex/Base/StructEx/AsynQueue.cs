using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 异步队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class AsynQueue<T> : IDisposable
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
        /// 线程取消通知对象
        /// </summary>
        private CancellationTokenSource _cts = null;

        /// <summary>
        /// 是否是后台线程[true:后台线程，false:前台线程]
        /// </summary>
        private readonly bool _isBackground = true;

        /// <summary>
        /// Queue
        /// </summary>
        private readonly Queue<T> _queue = new Queue<T>();

        /// <summary>
        /// 空队列等待线程消息通知
        /// </summary>
        private readonly AutoResetEvent _emptyQueueWaitEventHandle = new AutoResetEvent(false);

        private void EmptyQueueWaitEventHandleSet()
        {
            try
            {
                this._emptyQueueWaitEventHandle.Set();
            }
            catch (ObjectDisposedException)
            { }
        }

        /// <summary>
        /// 空队列等待超时时间
        /// </summary>
        private readonly int _emptyQueueWaitTimeout = 10000;

        /// <summary>
        /// 停止线程消息通知
        /// </summary>
        private readonly AutoResetEvent _stopAutoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 对象是否已释放[true:已释放;false:未释放]
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 异步队列线程名称
        /// </summary>
        private string _threadName = "异步队列线程";
        /// <summary>
        /// 异步队列线程名称
        /// </summary>
        public string ThreadName
        {
            get { return _threadName; }
        }

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

        private readonly int _capity;
        /// <summary>
        /// 获取队列容量[如果设置的容量小于当前已有队列长度,则丢弃掉队列头的项.直到队列长度与目标容量一致]
        /// </summary>
        public int Capity
        {
            get { return this._capity; }
        }

        /// <summary>
        /// 是否每次抛出多项
        /// </summary>
        private readonly bool _isDequeueMuiltItem;

        /// <summary>
        /// 批量处理最大项数
        /// </summary>
        private readonly int _batchCount = 0;

        /// <summary>
        /// 当队列中的项数少于批量处理最大项数时的等待时间,单位毫秒
        /// </summary>
        private readonly int _millisecondsTimeout;

        /// <summary>
        /// 数据处理委托
        /// </summary>
        public Action<T> ProcessAction;

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="item">待处理数据项</param>
        private void OnRaiseProcess(T item)
        {
            var handler = this.ProcessAction;
            if (handler != null)
            {
                handler(item);
            }
        }

        /// <summary>
        /// 数据处理委托
        /// </summary>
        public Action<List<T>> ProcessAction2;

        /// <summary>
        /// 调用数据处理委托
        /// </summary>
        /// <param name="items"></param>
        private void OnRaiseProcessAction2(List<T> items)
        {
            var handler = this.ProcessAction2;
            handler?.Invoke(items);//数据处理
        }

        /// <summary>
        /// 同步操作对象
        /// </summary>
        public readonly object SyncRoot = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isDequeueMuiltItem">是否每次抛出多项</param>
        /// <param name="threadName">异步队列线程名称</param>
        /// <param name="isBackground">是否是后台线程[true:后台线程，false:前台线程]</param>
        /// <param name="isAutoStart">是否自动启动线程</param>
        /// <param name="capcity">队列容量</param>
        private AsynQueue(bool isDequeueMuiltItem, string threadName, bool isBackground, bool isAutoStart, int capcity)
        {
            this._isDequeueMuiltItem = isDequeueMuiltItem;
            this._isBackground = isBackground;
            this._threadName = threadName;
            if (capcity < 1)
            {
                throw new ArgumentException("capcity");
            }

            this._capity = capcity;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="processAction">数据处理委托</param>
        /// <param name="threadName">异步队列线程名称</param>
        /// <param name="isBackground">是否是后台线程[true:后台线程，false:前台线程]</param>
        /// <param name="isAutoStart">是否自动启动线程</param>
        /// <param name="capcity">队列容量</param>
        public AsynQueue(Action<T> processAction, string threadName = null, bool isBackground = true, bool isAutoStart = false, int capcity = int.MaxValue) :
            this(false, threadName, isBackground, isAutoStart, capcity)
        {
            this.ProcessAction = processAction;
            if (isAutoStart)
            {
                this.Start();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="processAction">数据处理委托</param>
        /// <param name="batchCount">批量处理最大项数</param>
        /// <param name="millisecondsTimeout">当队列中的项数少于批量处理最大项数时的等待时间,单位毫秒</param>
        /// <param name="threadName">异步队列线程名称</param>
        /// <param name="isBackground">是否是后台线程[true:后台线程，false:前台线程]</param>
        /// <param name="isAutoStart">是否自动启动线程</param>
        /// <param name="capcity">队列容量</param>
        public AsynQueue(Action<List<T>> processAction, int batchCount = 10, int millisecondsTimeout = 10, string threadName = null, bool isBackground = true, bool isAutoStart = false, int capcity = int.MaxValue) :
            this(true, threadName, isBackground, isAutoStart, capcity)
        {
            if (batchCount < 1)
            {
                throw new ArgumentException(string.Format("批量处理最大项数不能小于1,值:{0}无效", batchCount));
            }

            this.ProcessAction2 = processAction;
            this._batchCount = batchCount;
            this._millisecondsTimeout = millisecondsTimeout;
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
                this._thread.IsBackground = this._isBackground;
                this._thread.SetApartmentState(apartmentState);
                this._thread.Name = this._threadName;
                this._thread.Start(this._cts.Token);
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
            this._status = false;

            var cts = this._cts;
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                this._cts = null;
            }

            this.EmptyQueueWaitEventHandleSet();
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
        private void RunThreadQueueProcessMethod(object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            if (this._isDequeueMuiltItem)
            {
                this.RunThreadQueueMuiltProcessMethod(token);
            }
            else
            {
                this.RunThreadQueueSingleProcessMethod(token);
            }
        }

        /// <summary>
        /// 线程队列处理方法
        /// </summary>
        private void RunThreadQueueSingleProcessMethod(CancellationToken token)
        {
            int count;
            T item;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    lock (this.SyncRoot)
                    {
                        count = this._queue.Count;
                    }

                    if (count == 0)
                    {
                        try
                        {
                            this._emptyQueueWaitEventHandle.WaitOne(this._emptyQueueWaitTimeout);
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }
                    }
                    else
                    {
                        lock (this.SyncRoot)
                        {
                            item = this._queue.Dequeue();
                        }

                        //数据处理
                        this.OnRaiseProcess(item);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                this.ThreadRunFinish();
                return;
            }

            this.ThreadRunFinish();
        }

        /// <summary>
        /// 线程队列处理方法
        /// </summary>
        private void RunThreadQueueMuiltProcessMethod(CancellationToken token)
        {
            List<T> items = new List<T>();
            List<T> items2;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    lock (this.SyncRoot)
                    {
                        while (this._queue.Count > 0 && items.Count < this._batchCount)
                        {
                            items.Add(this._queue.Dequeue());
                        }
                    }

                    try
                    {
                        if (items.Count < this._batchCount && this._emptyQueueWaitEventHandle.WaitOne(this._millisecondsTimeout))
                        {
                            lock (this.SyncRoot)
                            {
                                while (this._queue.Count > 0 && items.Count < this._batchCount)
                                {
                                    items.Add(this._queue.Dequeue());
                                }
                            }
                        }

                        if (items.Count == 0)
                        {
                            this._emptyQueueWaitEventHandle.WaitOne(this._emptyQueueWaitTimeout);
                        }
                        else
                        {
                            //数据处理
                            items2 = items.ToList();
                            items.Clear();
                            this.OnRaiseProcessAction2(items2);
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
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
                this._thread = null;

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
        /// 将对象添加到队列的结尾处
        /// </summary>
        /// <param name="item">待添加的对象</param>
        public bool Enqueue(T item)
        {
            lock (this.SyncRoot)
            {
                if (this._queue.Count < this._capity)
                {
                    this._queue.Enqueue(item);
                    this.EmptyQueueWaitEventHandleSet();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 移除位于开始处的指定个数对象
        /// </summary>
        /// <param name="count">要移除的项数</param>
        /// <returns>移除项集合</returns>
        public List<T> Remove(int count)
        {
            var items = new List<T>();
            lock (this.SyncRoot)
            {
                while (items.Count < count && this._queue.Count > 0)
                {
                    items.Add(this._queue.Dequeue());
                }
            }

            return items;
        }

        /// <summary>
        /// 移除满足条件的元素
        /// </summary>
        /// <param name="predicate">用于定义要移除的元素应满足的条件</param>
        /// <returns>移除项集合</returns>
        public IEnumerable<T> Remove(Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            lock (this.SyncRoot)
            {
                T[] array = this._queue.ToArray();
                var removeItems = array.Where(predicate);
                if (removeItems.Count() > 0)
                {
                    this._queue.Clear();
                    foreach (var item in array)
                    {
                        if (removeItems.Contains(item))
                        {
                            continue;
                        }
                        else
                        {
                            this._queue.Enqueue(item);
                        }
                    }
                }

                return removeItems;
            }
        }

        /// <summary>
        /// 获取队列中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._queue.Count;
                }
            }
        }

        /// <summary>
        /// 将队列中存储的元素复制到新数组中
        /// </summary>
        /// <returns>新数组</returns>
        public T[] ToArray()
        {
            lock (this.SyncRoot)
            {
                return this._queue.ToArray();
            }
        }

        /// <summary>
        /// 清空队列,必须在停止时执行,否则后果未知
        /// </summary>
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                this._queue.Clear();
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

                this.PrimitiveStop(false, false, 5000);
                this._emptyQueueWaitEventHandle.Dispose();
                this._stopAutoResetEvent.Dispose();
                this._isDisposed = true;
            }
        }
    }
}
