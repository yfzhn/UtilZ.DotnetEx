using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// BlockingCollectionEx线程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlockingCollectionExThread<T> : IDisposable
    {
        /// <summary>
        /// 默认添加处理线程阈值条件
        /// </summary>
        private const int ADD_PROCESS_THREAD_THRESHOLD = 100;

        //private readonly BlockingCollectionEx<T> _blockingCollection = new BlockingCollectionEx<T>();
        private System.Collections.Concurrent.BlockingCollection<T> _blockingCollection = new System.Collections.Concurrent.BlockingCollection<T>();
        private readonly object _blockingCollectionLock = new object();
        /// <summary>
        /// 添加处理线程条件类型[true:外部条件;false:线程数]
        /// </summary>
        private bool _addProcessThreadConditionType;
        private readonly Func<int, bool> _addProcessThreadFunc;
        private readonly int _maxThreadCount;
        private readonly string _threadNamePre;
        private readonly Action<T> _process;
        private readonly int _addProcessThreadThreshold;
        private readonly List<ThreadEx> _threadList = new List<ThreadEx>();
        private readonly object _threadLock = new object();
        private bool _allowAddThread = true;
        private bool _isDisposed = false;

        /// <summary>
        /// 获取项数
        /// </summary>
        public int Count
        {
            get
            {
                return this._blockingCollection.Count;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="addProcessThreadFunc">添加线程条件,参数为当前处理的线程数,返回值true表示可增加线程处理;false表示不再增加线程处理</param>
        /// <param name="threadNamePre">线程名前续</param>
        /// <param name="process">处理回调</param>
        /// <param name="addProcessThreadThreshold">新添加处理线程阈值条件,当未处理集合中的项数超过此值时会新添加一个处理线程</param>
        public BlockingCollectionExThread(Func<int, bool> addProcessThreadFunc, string threadNamePre, Action<T> process, int addProcessThreadThreshold = ADD_PROCESS_THREAD_THRESHOLD)
            : this(threadNamePre, process, addProcessThreadThreshold, true)
        {
            this._addProcessThreadFunc = addProcessThreadFunc;
            this.CreateProcessThread();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxThreadCount">最大并发处理线程数,小于1无限制,最多为CPU核心数</param>
        /// <param name="threadNamePre">线程名前续</param>
        /// <param name="process">处理回调</param>
        /// <param name="addProcessThreadThreshold">新添加处理线程阈值条件,当未处理集合中的项数超过此值时会新添加一个处理线程</param>
        public BlockingCollectionExThread(int maxThreadCount, string threadNamePre, Action<T> process, int addProcessThreadThreshold = ADD_PROCESS_THREAD_THRESHOLD)
            : this(threadNamePre, process, addProcessThreadThreshold, false)
        {
            this._maxThreadCount = maxThreadCount;
            this.CreateProcessThread();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="threadNamePre">线程名前续</param>
        /// <param name="process">处理回调</param>
        /// <param name="addProcessThreadThreshold">新添加处理线程阈值条件,当未处理集合中的项数超过此值时会新添加一个处理线程</param>
        /// <param name="addProcessThreadConditionType">添加处理线程条件类型[true:外部条件;false:线程数]</param>
        private BlockingCollectionExThread(string threadNamePre, Action<T> process, int addProcessThreadThreshold, bool addProcessThreadConditionType)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (addProcessThreadThreshold < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(addProcessThreadThreshold), "新添加处理线程阈值不能小于1");
            }

            this._threadNamePre = threadNamePre;
            this._process = process;
            this._addProcessThreadThreshold = addProcessThreadThreshold;
            this._addProcessThreadConditionType = addProcessThreadConditionType;
        }

        private void CreateProcessThread()
        {
            if (!this._allowAddThread)
            {
                return;
            }

            lock (this._threadLock)
            {
                if (!this._allowAddThread)
                {
                    return;
                }

                if (this._isDisposed)
                {
                    this._allowAddThread = false;
                    return;
                }

                if (this._threadList.Count >= Environment.ProcessorCount)
                {
                    this._allowAddThread = false;
                    return;
                }

                if (this._addProcessThreadConditionType)
                {
                    if (!this._addProcessThreadFunc(this._threadList.Count))
                    {
                        this._allowAddThread = false;
                        return;
                    }
                }
                else
                {
                    if (this._maxThreadCount > 0 && this._threadList.Count >= this._maxThreadCount)
                    {
                        this._allowAddThread = false;
                        return;
                    }
                }

                string threadName = $"{this._threadNamePre}{this._threadList.Count}";
                ThreadEx thread = new ThreadEx(this.ProcessThreadMethod, threadName, true);
                this._threadList.Add(thread);
                thread.Start();
            }
        }


        private void ProcessThreadMethod(CancellationToken token)
        {
            try
            {
                const int millisecondsTimeout = 1000;
                T item;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        try
                        {
                            if (!this._blockingCollection.TryTake(out item, millisecondsTimeout, token))
                            {
                                continue;
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (ArgumentNullException)
                        {
                            continue;
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }

                        this.AddNewProcessThread();
                        this._process(item);
                    }
                    catch (Exception exi)
                    {
                        Loger.Error(exi);
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private void AddNewProcessThread()
        {
            if (this._blockingCollection.Count <= this._addProcessThreadThreshold)
            {
                return;
            }

            this.CreateProcessThread();
        }


        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (this._isDisposed)
            {
                return;
            }

            lock (this._blockingCollectionLock)
            {
                this._blockingCollection.Add(item);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            lock (this._threadLock)
            {
                this.ClearThread();

                lock (this._blockingCollectionLock)
                {
                    this._blockingCollection.Dispose();
                    this._blockingCollection = new System.Collections.Concurrent.BlockingCollection<T>();
                }

                //this._blockingCollection.Clear();
                this._allowAddThread = true;
                this.CreateProcessThread();
            }
        }

        private void ClearThread()
        {
            if (this._threadList.Count == 0)
            {
                return;
            }

            foreach (var thread in this._threadList)
            {
                thread.Stop();
                thread.Dispose();
            }

            this._threadList.Clear();
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            try
            {
                lock (this._threadLock)
                {
                    if (this._isDisposed)
                    {
                        return;
                    }
                    this._isDisposed = true;

                    this.ClearThread();

                    lock (this._blockingCollectionLock)
                    {
                        this._blockingCollection.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
