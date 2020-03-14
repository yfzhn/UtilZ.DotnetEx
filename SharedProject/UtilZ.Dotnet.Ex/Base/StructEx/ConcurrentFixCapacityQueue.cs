using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 固定容量集合
    /// </summary>
    /// <typeparam name="T">集合项类型</typeparam>
    [Serializable]
    public class ConcurrentFixCapacityQueue<T> : IEnumerable<T>, ICollection, IEnumerable
    {
        /// <summary>
        /// 内容队列
        /// </summary>
        private readonly Queue<T> _queue = new Queue<T>();

        /// <summary>
        /// 线程锁
        /// </summary>
        public readonly object _syncRoot = new object();

        /// <summary>
        /// 集合容量
        /// </summary>
        private int _capacity;

        /// <summary>
        /// 获取或设置集合容量
        /// </summary>
        public int Capacity
        {
            get { return _capacity; }
            set
            {
                lock (this._syncRoot)
                {
                    this._capacity = value;
                    this.CheckCapcity();
                }
            }
        }

        /// <summary>
        /// 当队列生产大于消费时,队列超出容量之后会将最先入队列的项溢出通知
        /// </summary>
        private Action<List<T>> _overflowedNotify;

        /// <summary>
        /// 触发队列溢出通知
        /// </summary>
        /// <param name="items">溢出项集合</param>
        private void OnRaiseOverflowedNotify(List<T> items)
        {
            var handler = this._overflowedNotify;
            if (handler != null)
            {
                handler(items);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">集合容量</param>
        public ConcurrentFixCapacityQueue(int capacity)
            : this(capacity, null, null)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">集合容量</param>
        /// <param name="overflowedNotify">当队列生产大于消费时,队列超出容量之后会将最先入队列的项溢出通知</param>
        public ConcurrentFixCapacityQueue(int capacity, Action<List<T>> overflowedNotify)
            : this(capacity, null, overflowedNotify)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">集合容量</param>
        /// <param name="items">初始化集合</param>
        /// <param name="overflowedNotify">当队列生产大于消费时,队列超出容量之后会将最先入队列的项溢出通知</param>
        public ConcurrentFixCapacityQueue(int capacity, IEnumerable<T> items, Action<List<T>> overflowedNotify)
        {
            if (items != null)
            {
                if (items.Count() > capacity)
                {
                    throw new ArgumentOutOfRangeException("初始化集合中的项数大于容量");
                }

                foreach (var item in items)
                {
                    this._queue.Enqueue(item);
                }
            }

            this._capacity = capacity;
            this._overflowedNotify = overflowedNotify;
        }

        /// <summary>
        /// 从特定的 Array 索引处开始，将 ICollection 的元素复制到一个 Array 中
        /// </summary>
        /// <param name="array">作为从 ICollection 复制的元素的目标的一维 Array。 Array 必须具有从零开始的索引</param>
        /// <param name="index">array 中从零开始的索引，从此索引处开始进行复制</param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取队列集合中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._queue.Count;
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值指示是否同步对 ICollection 的访问（线程安全）。
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// 获取可用于同步对 ICollection 的访问的对象
        /// </summary>
        public object SyncRoot
        {
            get { return this._syncRoot; }
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>一个循环访问集合的枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this._syncRoot)
            {
                return this._queue.GetEnumerator();
            }
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举数
        /// </summary>
        /// <returns>一个循环访问集合的枚举数</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 将对象添加到队列结尾
        /// </summary>
        /// <param name="item">要添加的对象</param>
        public virtual void Enqueue(T item)
        {
            lock (this._syncRoot)
            {
                this._queue.Enqueue(item);
                this.CheckCapcity();
            }
        }

        /// <summary>
        /// 检查容量
        /// </summary>
        private void CheckCapcity()
        {
            var overflowedItems = new List<T>();
            while (this._queue.Count > this._capacity)
            {
                var overflowedItem = this._queue.Dequeue();
                overflowedItems.Add(overflowedItem);
            }

            this.OnRaiseOverflowedNotify(overflowedItems);
        }

        /// <summary>
        /// 将对象添加到队列结尾
        /// </summary>
        /// <param name="items">要添加的对象集合</param>
        public virtual void EnqueueRange(IEnumerable<T> items)
        {
            lock (this._syncRoot)
            {
                foreach (var item in items)
                {
                    this._queue.Enqueue(item);
                }

                this.CheckCapcity();
            }
        }

        /// <summary>
        /// 返回位于队列集合开始处的对象
        /// </summary>
        /// <returns>位于队列集合开始处的对象</returns>
        public virtual T Peek()
        {
            lock (this._syncRoot)
            {
                if (this._queue.Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }

                return this._queue.Peek();
            }
        }

        /// <summary>
        /// 返回位于队列集合开始处的若干对象列表
        /// </summary>
        /// <param name="peekCount">要Peek项数</param>
        /// <returns>位于队列集合开始处的若干对象列表</returns>
        public virtual List<T> Peek(int peekCount)
        {
            List<T> items = new List<T>();
            lock (this._syncRoot)
            {
                int count = this._queue.Count;
                if (count > peekCount)
                {
                    count = peekCount;
                }

                for (int i = 0; i < count; i++)
                {
                    items.Add(this._queue.Peek());
                }
            }

            return items;
        }

        /// <summary>
        /// 移除并返回位于队列集合开始处的对象
        /// </summary>
        /// <returns>位于队列集合开始处的对象</returns>
        public virtual T Dequeue()
        {
            lock (this._syncRoot)
            {
                if (this._queue.Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }

                return this._queue.Dequeue();
            }
        }

        /// <summary>
        /// 移除并返回位于队列集合开始处的若干对象列表
        /// </summary>
        /// <param name="dequeueCount">要Dequeue项数</param>
        /// <returns>位于队列集合开始处的若干对象列表</returns>
        public virtual List<T> Dequeue(int dequeueCount)
        {
            List<T> items = new List<T>();
            lock (this._syncRoot)
            {
                int count = this._queue.Count;
                if (count > dequeueCount)
                {
                    count = dequeueCount;
                }

                for (int i = 0; i < count; i++)
                {
                    items.Add(this._queue.Dequeue());
                }
            }

            return items;
        }

        /// <summary>
        /// 从队列中移除所有对象
        /// </summary>
        public virtual void Clear()
        {
            lock (this._syncRoot)
            {
                this._queue.Clear();
            }
        }

        /// <summary>
        /// 获取当前内部集合项列表
        /// </summary>
        /// <returns>当前内部集合项列表</returns>
        public List<T> Items
        {
            get
            {
                lock (this._syncRoot)
                {
                    return this._queue.ToList();
                }
            }
        }
    }
}
