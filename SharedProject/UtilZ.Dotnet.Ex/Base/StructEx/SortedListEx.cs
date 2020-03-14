using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 顺序列表[支持多线程,内部已做数据同步处理]
    /// </summary>
    public class SortedListEx<T> : ICollection<T> where T : IComparable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SortedListEx()
        {

        }

        /// <summary>
        /// 集合
        /// </summary>
        private readonly List<T> _items = new List<T>();

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            //小于零:此实例在排序顺序中位于 obj 之前
            //零:此实例在排序顺序中的位置与 obj 相同
            //大于零:此实例在排序顺序中位于 obj 之后
            lock (this)
            {
                //如果当前集合为空或是集合的要添加的项大于等于最后一项,则添加到集合最后
                if (this._items.Count == 0 || item.CompareTo(this._items[this._items.Count - 1]) >= 0)
                {
                    this._items.Add(item);
                    return;
                }

                //如果要添加的项小于第一项,则应该插入到第一项的位置
                if (item.CompareTo(this._items[0]) < 0)
                {
                    this._items.Insert(0, item);
                    return;
                }

                //二分查找法插入
                int segmentBeginIndex = 0;//二分查找段起始索引,默认为0
                int segmentEndIndex = this._items.Count - 1;//二分查找段结束索引,默认为集合数减1
                int segmentIndexSize = 0;//当前范围段大小
                int hafSegmentCurrentIndex = -1;//二分查找段当前索引,默认为-1
                int compareResult = 0;//比较结果

                while (true)
                {
                    segmentIndexSize = segmentEndIndex - segmentBeginIndex;
                    if (segmentIndexSize < 3)
                    {
                        for (int i = segmentBeginIndex; i <= segmentEndIndex; i++)
                        {
                            if (item.CompareTo(this._items[i]) < 0)
                            {
                                this._items.Insert(i, item);
                                return;
                            }
                        }

                        this._items.Insert(segmentEndIndex, item);
                        break;
                    }
                    else
                    {
                        hafSegmentCurrentIndex = (int)Math.Floor((double)segmentIndexSize / 2);
                        compareResult = item.CompareTo(this._items[segmentBeginIndex + hafSegmentCurrentIndex]);

                        if (compareResult < 0)
                        {
                            segmentEndIndex = segmentEndIndex - hafSegmentCurrentIndex;
                        }
                        else
                        {
                            segmentBeginIndex = segmentBeginIndex + hafSegmentCurrentIndex;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将指定的集合添加到顺序列表中
        /// </summary>
        /// <param name="items">指定的集合</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                this._items.Clear();
            }
        }

        /// <summary>
        /// 确定是否包含特定值
        /// </summary>
        /// <param name="item">值</param>
        /// <returns>包含返回true,否则返回false</returns>
        public bool Contains(T item)
        {
            lock (this)
            {
                return this._items.Contains(item);
            }
        }

        /// <summary>
        /// 从特定的Array索引开始，将集合中的元素复制到一个 Array 中
        /// </summary>
        /// <param name="array">特定的Array</param>
        /// <param name="arrayIndex">特定的Array索引</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (this)
            {
                this._items.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// 获取集合中包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this)
                {
                    return this._items.Count;
                }
            }
        }

        /// <summary>
        /// 获取一个值，该值指示集合是否为只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从集合中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要移除的对象</param>
        /// <returns>移除成功返回true,否则返回false</returns>
        public bool Remove(T item)
        {
            lock (this)
            {
                if (this._items.Contains(item))
                {
                    return this._items.Remove(item);
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 搜索指定对象在集合中的索引
        /// </summary>
        /// <param name="item">指定对象</param>
        /// <returns>对象所在集合中的索引</returns>
        public int IndexOf(T item)
        {
            lock (this)
            {
                return this._items.IndexOf(item);
            }
        }

        /// <summary>
        /// 要移除0开始索引处的元素
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveAt(int index)
        {
            lock (this)
            {
                if (index < 0 || index >= this._items.Count)
                {
                    throw new ArgumentException("无效的索引");
                }

                this._items.RemoveAt(index);
            }
        }

        /// <summary>
        /// 获取或设置指定索引处的元素
        /// </summary>
        /// <param name="index">指定索引</param>
        /// <returns>指定索引处的元素</returns>
        public T this[int index]
        {
            get
            {
                lock (this)
                {
                    if (index < 0 || index >= this._items.Count)
                    {
                        throw new ArgumentException("无效的索引");
                    }

                    return this._items[index];
                }
            }
            set
            {
                lock (this)
                {
                    if (index < 0 || index >= this._items.Count)
                    {
                        throw new ArgumentException("无效的索引");
                    }

                    this._items[index] = value;
                }
            }
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>一个循环访问集合的枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this)
            {
                return this._items.GetEnumerator();
            }
        }

        /// <summary>
        /// 显示返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns>一个循环访问集合的枚举器</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
