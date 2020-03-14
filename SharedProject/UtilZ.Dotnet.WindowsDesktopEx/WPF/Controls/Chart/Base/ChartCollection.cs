using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChartCollection<T> : NotifyPropertyChangedAbs, IChartCollection<T>
    {
        private readonly object _sourceLock = new object();
        private readonly List<T> _source = new List<T>();


        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartCollection()
        {

        }


        /// <summary>
        /// 集合改变事件
        /// </summary>

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnRaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Chart集合改变事件
        /// </summary>
        public event EventHandler<ChartCollectionChangedEventArgs<T>> ChartCollectionChanged;
        private void OnRaiseChartCollectionChanged(ChartCollectionChangedEventArgs<T> e)
        {
            var handler = this.ChartCollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }



        /// <summary>
        /// 获取或设置指定索引的值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                lock (this._sourceLock)
                {
                    return _source[index];
                }
            }
            set
            {
                lock (this._sourceLock)
                {
                    var oldItem = _source[index];
                    _source[index] = value;
                    this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
                    this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Replace, value, oldItem));
                }
            }
        }

        /// <summary>
        /// 获取集合中的元素个数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this._sourceLock)
                {
                    return _source.Count;
                }
            }
        }

        /// <summary>
        /// 集合是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }










        /// <summary>
        /// 添加项到集合中
        /// </summary>
        /// <param name="item">新项</param>
        public void Add(T item)
        {
            lock (this._sourceLock)
            {
                this._source.Add(item);
                this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Add, item, default(T)));
            }
        }


        /// <summary>
        /// 添加集合到本集合中
        /// </summary>
        /// <param name="items">目标集合</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
            {
                return;
            }

            lock (this._sourceLock)
            {
                this._source.AddRange(items);
                foreach (var item in items)
                {
                    this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                }
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Add, items.ToList(), null));
            }
        }

        /// <summary>
        /// 插入项到指定位置
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="item">要插入的项</param>
        public void Insert(int index, T item)
        {
            lock (this._sourceLock)
            {
                this._source.Insert(index, item);
                this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Add, item, default(T)));
            }
        }
        /// <summary>
        /// 插入集合到指定位置
        /// </summary>
        /// <param name="index">插入位置</param>
        /// <param name="items">要插入的集合</param>
        public void InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
            {
                return;
            }

            lock (this._sourceLock)
            {
                this._source.InsertRange(index, items);
                foreach (var item in items)
                {
                    this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                }
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Add, items.ToList(), null));
            }
        }




        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="item">要移除的项</param>
        /// <returns>移除结果</returns>
        public bool Remove(T item)
        {
            lock (this._sourceLock)
            {
                var index = this._source.IndexOf(item);
                bool ret = this._source.Remove(item);
                if (ret)
                {
                    this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                    this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Remove, default(T), item));
                }

                return ret;
            }
        }
        /// <summary>
        /// 移除满足匹配条件的项
        /// </summary>
        /// <param name="match">匹配条件</param>
        /// <returns>移除的项数</returns>
        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
            {
                return 0;
            }

            List<T> list = null;
            lock (this._sourceLock)
            {
                foreach (var item in this._source.ToArray())
                {
                    if (match(item))
                    {
                        if (list == null)
                        {
                            list = new List<T>();
                        }

                        list.Add(item);
                        this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                        this._source.Remove(item);
                    }
                }
            }

            if (list != null)
            {
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Remove, null, list));
                return list.Count;
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 移除指定索引位置的项
        /// </summary>
        /// <param name="index">目标索引</param>
        public void RemoveAt(int index)
        {
            lock (this._sourceLock)
            {
                var oldItem = this._source.ElementAt(index);
                this._source.RemoveAt(index);
                this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Remove, default(T), oldItem));
            }
        }


        /// <summary>
        /// 清空集合
        /// </summary>
        public void Clear()
        {
            lock (this._sourceLock)
            {
                var list = this._source.ToList();
                this._source.Clear();
                this.OnRaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null));
                this.OnRaiseChartCollectionChanged(new ChartCollectionChangedEventArgs<T>(ChartCollectionChangedAction.Remove, null, list));
            }
        }


        /// <summary>
        /// 集合中是否包含某项
        /// </summary>
        /// <param name="item">目标项</param>
        /// <returns>包含结果</returns>
        public bool Contains(T item)
        {
            lock (this._sourceLock)
            {
                return this._source.Contains(item);
            }
        }


        /// <summary>
        /// 复制集合到指定数据
        /// </summary>
        /// <param name="array">目标数据</param>
        /// <param name="arrayIndex">起始位置</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this._sourceLock)
            {
                this._source.CopyTo(array, arrayIndex);
            }
        }


        /// <summary>
        /// 检索某项所在位置
        /// </summary>
        /// <param name="item">目标项</param>
        /// <returns>目标项在集合中的位置</returns>
        public int IndexOf(T item)
        {
            lock (this._sourceLock)
            {
                return this._source.IndexOf(item);
            }
        }



        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (_sourceLock)
            {
                return new List<T>(_source).GetEnumerator();
            }
        }

        /// <summary>
        /// IEnumerable.GetEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
