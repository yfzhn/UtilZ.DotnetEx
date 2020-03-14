using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 优先级队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class PriorityQueue<T> : IEnumerable<KeyValuePair<int, Queue<T>>>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PriorityQueue()
        {

        }

        /// <summary>
        /// 等待处理的信号指令队列
        /// </summary>
        private readonly SortedList<int, Queue<T>> _priorityQuenue = new SortedList<int, Queue<T>>();

        /// <summary>
        /// 多线程锁
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// 将对象添加到队列结尾处
        /// </summary>
        /// <param name="priorityLevel">优先级[值越小优先级越高,越大越低]</param>
        /// <param name="item">对象,该值可以为null</param>
        public void Enqueue(int priorityLevel, T item)
        {
            lock (this._monitor)
            {
                if (this._priorityQuenue.ContainsKey(priorityLevel))
                {
                    this._priorityQuenue[priorityLevel].Enqueue(item);
                }
                else
                {
                    Queue<T> quenu = new Queue<T>();
                    quenu.Enqueue(item);
                    this._priorityQuenue.Add(priorityLevel, quenu);
                }
            }
        }

        /// <summary>
        /// 移除并返回指定优先级开始处的对象
        /// </summary>
        /// <param name="priorityLevel">优先级[值越小优先级越高,越大越低]</param>
        /// <returns>开始处的对象</returns>
        public T Dequeue(int priorityLevel)
        {
            lock (this._monitor)
            {
                if (this._priorityQuenue.Count == 0 || !this._priorityQuenue.ContainsKey(priorityLevel) || this._priorityQuenue[priorityLevel].Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }

                T value = this._priorityQuenue[priorityLevel].Dequeue();
                if (this._priorityQuenue[priorityLevel].Count == 0)
                {
                    this._priorityQuenue.Remove(priorityLevel);
                }

                return value;
            }
        }

        /// <summary>
        /// 移除并返回开始处的对象,根据优先级依次移除,优先级高的先移除
        /// </summary>
        /// <returns>开始处的对象</returns>
        public T Dequeue()
        {
            lock (this._monitor)
            {
                if (this._priorityQuenue.Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }

                int priorityLevel = this._priorityQuenue.Keys[0];
                T value = this._priorityQuenue[priorityLevel].Dequeue();
                if (this._priorityQuenue[priorityLevel].Count == 0)
                {
                    this._priorityQuenue.Remove(priorityLevel);
                }

                return value;
            }
        }

        /// <summary>
        /// 返回指定优先级开始处的对象,但不移除
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        /// <returns>开始处的对象</returns>
        public T Peek(int priorityLevel)
        {
            lock (this._monitor)
            {
                if (this._priorityQuenue.Count == 0 || !this._priorityQuenue.ContainsKey(priorityLevel) || this._priorityQuenue[priorityLevel].Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }

                return this._priorityQuenue[priorityLevel].Peek();
            }
        }

        /// <summary>
        /// 返回开始处的对象,但不移除,根据优先级依次返回,优先级高的先返回
        /// </summary>
        /// <returns>开始处的对象</returns>
        public T Peek()
        {
            lock (this._monitor)
            {
                if (this._priorityQuenue.Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }

                int priorityLevel = this._priorityQuenue.Keys[0];
                return this._priorityQuenue[priorityLevel].Peek();
            }
        }

        /// <summary>
        /// 移除队列中的前N项
        /// </summary>
        /// <param name="count">项数</param>
        public void RemoveRange(long count)
        {
            lock (this._monitor)
            {
                if (count > this.GetCount())
                {
                    throw new ArgumentOutOfRangeException(string.Format("当前队列中的项数小于要移除的项数"));
                }

                while (count-- > 0)
                {
                    int priorityLevel = this._priorityQuenue.Keys[0];
                    this._priorityQuenue[priorityLevel].Dequeue();

                    if (this._priorityQuenue[priorityLevel].Count == 0)
                    {
                        this._priorityQuenue.Remove(priorityLevel);
                    }
                }
            }
        }

        /// <summary>
        /// 移除队列中的前N项
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        /// <param name="count">项数</param>
        public void RemoveRange(int priorityLevel, long count)
        {
            lock (this._monitor)
            {
                if (count > this.GetPriorityCount(priorityLevel))
                {
                    throw new ArgumentOutOfRangeException(string.Format("当前队列中的项数小于要移除的项数"));
                }

                while (count-- > 0)
                {
                    this._priorityQuenue[priorityLevel].Dequeue();
                }

                if (this._priorityQuenue[priorityLevel].Count == 0)
                {
                    this._priorityQuenue.Remove(priorityLevel);
                }
            }
        }

        /// <summary>
        /// 清空全部队列
        /// </summary>
        public void Clear()
        {
            lock (this._monitor)
            {
                this._priorityQuenue.Clear();
            }
        }

        /// <summary>
        /// 清空指定优先级的队列
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        public void Clear(int priorityLevel)
        {
            lock (this._monitor)
            {
                if (this._priorityQuenue.Count == 0 || !this._priorityQuenue.ContainsKey(priorityLevel) || this._priorityQuenue[priorityLevel].Count == 0)
                {
                    return;
                }

                this._priorityQuenue.Remove(priorityLevel);
            }
        }

        /// <summary>
        /// 获取队列中包含的元素数
        /// </summary>
        private long GetCount()
        {
            if (this._priorityQuenue.Count == 0)
            {
                return 0;
            }

            long count = 0;
            foreach (var item in this._priorityQuenue)
            {
                count += item.Value.Count;
            }

            return count;
        }

        /// <summary>
        /// 获取队列中指定优先级的元素数
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        /// <returns>元素数</returns>
        private int GetPriorityCount(int priorityLevel)
        {
            int count = 0;
            if (this._priorityQuenue.Count != 0 && this._priorityQuenue.ContainsKey(priorityLevel))
            {
                count = this._priorityQuenue[priorityLevel].Count;
            }

            return count;
        }

        /// <summary>
        /// 获取队列中包含的元素数
        /// </summary>
        public long Count
        {
            get
            {
                lock (this._monitor)
                {
                    return this.GetCount();
                }
            }
        }

        /// <summary>
        /// 获取队列中指定优先级的元素数
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        /// <returns>元素数</returns>
        public int GetCountByPriority(int priorityLevel)
        {
            lock (this._monitor)
            {
                return this.GetPriorityCount(priorityLevel);
            }
        }

        /// <summary>
        /// 获取当前存储的优先级类型数
        /// </summary>
        public int PriorityCount
        {
            get
            {
                lock (this._monitor)
                {
                    return this._priorityQuenue.Count;
                }
            }
        }

        /// <summary>
        /// 返回循环的遍历枚举
        /// </summary>
        /// <returns>循环的遍历枚举</returns>
        public IEnumerator<KeyValuePair<int, Queue<T>>> GetEnumerator()
        {
            return this._priorityQuenue.GetEnumerator();
        }

        /// <summary>
        /// 返回循环的遍历枚举
        /// </summary>
        /// <returns>循环的遍历枚举</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
