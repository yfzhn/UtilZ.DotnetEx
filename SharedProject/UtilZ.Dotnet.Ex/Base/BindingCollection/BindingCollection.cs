using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 绑定集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BindingCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : new()
    {
        private readonly ICollectionOwner _owner;
        /// <summary>
        /// 触发一个Action
        /// </summary>
        /// <param name="action">Action</param>
        protected void OnRaiseInvokeAction(Action action)
        {
            var owner = this._owner;
            if (owner != null && owner.InvokeRequired)
            {
                owner.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private readonly Collection<T> _items;

        /// <summary>
        /// 获取数据源集合
        /// </summary>
        public Collection<T> DataSource
        {
            get
            {
                return this._items;
            }
        }

        /// <summary>
        /// 集合改变锁
        /// </summary>
        public readonly object SyncRoot = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">集合所属ui对象</param>
        /// <param name="items">绑定集合[Winform:BindingListEx;WPF:ObservableCollection或是其子类]</param>
        public BindingCollection(ICollectionOwner owner, Collection<T> items)
           : base()
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this._owner = owner;
            this._items = items;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="owner">集合所属ui对象</param>
        public BindingCollection(ICollectionOwner owner)
           : this(owner, CreateCollectionByOwner(owner))
        {

        }

        private static Collection<T> CreateCollectionByOwner(object owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            Type type = owner.GetType();
            //System.Windows.Controls.Control  //DispatcherObject
            const string wpfSupperBaseTypeFullName = "System.Windows.Threading.DispatcherObject";
            const string supperBaseTypeFullName = "System.Object";
            const string winformControlFullName = "System.Windows.Forms.Control";

            while (!type.FullName.Equals(supperBaseTypeFullName))
            {
                if (type.FullName.Equals(winformControlFullName))
                {
                    return new BindingListEx<T>();
                }

                if (type.FullName.Equals(wpfSupperBaseTypeFullName))
                {
                    return new ObservableCollection<T>();
                }

                type = type.BaseType;
            }

            throw new ArgumentException(string.Format("类型{0}必须继承{1}或{2}类型", type.FullName, winformControlFullName, wpfSupperBaseTypeFullName));
        }

        /// <summary>
        /// 将对象添加到集合的结尾处
        /// </summary>
        /// <param name="item">要添加的新项。对于引用类型，该值可以为 null</param>
        public void Add(T item)
        {
            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    this._items.Add(item);
                }
            });
        }

        /// <summary>
        /// 隐藏重写AddNew,将新项添加集合开始
        /// </summary>
        public void AddNew()
        {
            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    var item = new T();
                    this._items.Insert(0, item);
                }
            });
        }

        /// <summary>
        /// 隐藏重写AddNewCore,将新项添加到末尾
        /// </summary>
        public void AddNewCore()
        {
            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    var item = new T();
                    this._items.Add(item);
                }
            });
        }

        /// <summary>
        /// 添加集合到末尾
        /// </summary>
        /// <param name="items">集合</param>
        public void AddRange(IEnumerable<T> items)
        {
            int count = items.Count();
            if (items == null || count == 0)
            {
                return;
            }

            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    foreach (var item in items)
                    {
                        this._items.Add(item);
                    }
                }
            });
        }

        /// <summary>
        /// 将元素插入集合的指定索引处
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item</param>
        /// <param name="item">要插入的对象。对于引用类型，该值可以为 null</param>
        public void Insert(int index, T item)
        {
            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    this._items.Insert(index, item);
                }
            });
        }

        /// <summary>
        /// 从集合中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要从集合中移除的对象。对于引用类型，该值可以为 null</param>
        /// <returns>如果成功移除 item，则为 true；否则为 false。如果在原始集合中未找到 item，此方法也会返回 false</returns>
        public bool Remove(T item)
        {
            lock (this.SyncRoot)
            {
                if (this._items.Contains(item))
                {
                    var owner = this._owner;
                    if (owner != null && owner.InvokeRequired)
                    {
                        return (bool)owner.Invoke(new Func<T, bool>((t) =>
                        {
                            return this._items.Remove(item);
                        }), item);
                    }
                    else
                    {
                        return this._items.Remove(item);
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 移除集合的指定索引处的元素
        /// 异常:
        /// System.ArgumentOutOfRangeException:index 小于零。- 或 -index 等于或大于集合.Count
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引</param>
        public void RemoveAt(int index)
        {
            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    this._items.RemoveAt(index);
                }
            });
        }

        /// <summary>
        /// 从指定位置移除指定个数的项
        /// </summary>
        /// <param name="index">移除起始位置</param>
        /// <param name="count">移除项个数</param>
        public void RemoveArea(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    int endIndex = index + count;
                    if (this._items.Count < endIndex)
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                    //每次都移除此索引处的项,因为当前一项移除后,后一项的索引又变成这么多了
                    for (int i = index; i < endIndex; i++)
                    {
                        this._items.RemoveAt(index);
                    }
                }
            });
        }

        /// <summary>
        /// 从指定位置移除指定个数的项
        /// </summary>
        /// <param name="items">要移除的集合</param>
        public void RemoveArrange(IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0)
            {
                return;
            }

            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    //移除项
                    foreach (var item in items)
                    {
                        if (this._items.Contains(item))
                        {
                            this._items.Remove(item);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 从集合中移除所有元素
        /// </summary>
        public void Clear()
        {
            this.OnRaiseInvokeAction(() =>
            {
                lock (this.SyncRoot)
                {
                    this._items.Clear();
                }
            });
        }

        /// <summary>
        /// 获取或设置指定索引处的元素
        /// 异常:System.ArgumentOutOfRangeException:index 小于零。- 或 -index 等于或大于 System.Collections.ObjectModel.Collection`1.Count。
        /// </summary>
        /// <param name="index">要获得或设置的元素从零开始的索引</param>
        /// <returns>指定索引处的元素</returns>
        public T this[int index]
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._items[index];
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    this._items[index] = value;
                }
            }
        }

        /// <summary>
        /// 获取集合中实际包含的元素数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
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
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 确定某元素是否在集合中[集合中找到 item，则为 true；否则为 false]
        /// </summary>
        /// <param name="item">要在 System.Collections.ObjectModel.Collection`1 中定位的对象。对于引用类型，该值可以为 null</param>
        /// <returns>集合中找到 item，则为 true；否则为 false</returns>
        public bool Contains(T item)
        {
            lock (this.SyncRoot)
            {
                return this._items.Contains(item);
            }
        }

        /// <summary>
        /// 从目标数组的指定索引处开始将整个集合复制到兼容的一维 System.Array
        /// 异常:
        /// System.ArgumentNullException:array 为 null
        /// System.ArgumentOutOfRangeException: index 小于零
        /// System.ArgumentException:源集合中的元素数目大于从 index 到目标 array 末尾之间的可用空间
        /// </summary>
        /// <param name="array">作为从 System.Collections.ObjectModel.Collection`1 复制的元素的目标位置的一维 System.Array。System.Array必须具有从零开始的索引</param>
        /// <param name="index">array 中从零开始的索引，将在此处开始复制</param>
        public void CopyTo(T[] array, int index)
        {
            lock (this.SyncRoot)
            {
                this._items.CopyTo(array, index);
            }
        }

        /// <summary>
        /// 搜索指定的对象，并返回整个集合中第一个匹配项的从零开始的索引
        /// </summary>
        /// <param name="item">要在集合中定位的对象。对于引用类型，该值可以为 null</param>
        /// <returns>如果在整个集合中找到 item 的第一个匹配项，则为该项的从零开始的索引；否则为-1</returns>
        public int IndexOf(T item)
        {
            lock (this.SyncRoot)
            {
                return this._items.IndexOf(item);
            }
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>用于集合的IEnumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
