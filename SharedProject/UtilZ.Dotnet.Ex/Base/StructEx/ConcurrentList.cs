using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 线程安全列表
    /// </summary>
    /// <typeparam name="T">集合数据类型</typeparam>
    [Serializable]
    public class ConcurrentList<T> : ICollection<T>
    {
        /// <summary>
        /// 内部集合 
        /// </summary>
        private readonly List<T> _items = new List<T>();

        /// <summary>
        /// 线程锁
        /// </summary>
        public readonly object SyncRoot = new object();

        /// <summary>
        /// 获取集合中包含的元素数
        /// </summary>
        public int Count
        {
            get { return this._items.Count; }
        }

        /// <summary>
        /// 集合是否是只读的
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 添加对象到集合尾
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            lock (this.SyncRoot)
            {
                this._items.Add(item);
            }
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                this._items.Clear();
            }
        }

        /// <summary>
        /// 确定某元素是否在集合中
        /// </summary>
        /// <param name="item">指定项</param>
        /// <returns>是否在集合中</returns>
        public bool Contains(T item)
        {
            lock (this.SyncRoot)
            {
                return this._items.Contains(item);
            }
        }

        /// <summary>
        /// 从目标数组的指定索引处开始，将整个 System.Collections.Generic.List`1 复制到兼容的一维数组
        /// </summary>
        /// <param name="array">一维 System.Array，它是从 System.Collections.Generic.List`1 复制的元素的目标。System.Array 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex">array 中从零开始的索引，从此处开始复制</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this.SyncRoot)
            {
                this._items.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// 移除指定的元素
        /// </summary>
        /// <param name="item">指定元素</param>
        /// <returns>移除结果</returns>
        public bool Remove(T item)
        {
            lock (this.SyncRoot)
            {
                return this._items.Remove(item);
            }
        }

        /// <summary>
        /// 返回循环访问的枚举数(注:遍历时需使用集合的SyncRoot字段加上线程锁)
        /// </summary>
        /// <returns>循环访问的枚举数</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}