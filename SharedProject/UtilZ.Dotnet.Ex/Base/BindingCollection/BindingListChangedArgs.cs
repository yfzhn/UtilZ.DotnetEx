using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// UI绑定集合发生更改委托参数
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class BindingListChangedArgs<T> : EventArgs
    {
        /*
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">改变类型</param>
        /// <param name="oldItem">改变项</param>
        /// <param name="count">当前集合项组数</param>
        /// <param name="index">当前项在集合中的位置索引</param>
        public BindingListChangedArgs(BindingListChangedType type, T item, int count, int index)
            : this(type, item, default(T), null, count, index)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">改变类型</param>
        /// <param name="items">改变项集合</param>
        /// <param name="count">当前集合项组数</param>
        /// <param name="index">当前项在集合中的位置索引</param>
        public BindingListChangedArgs(BindingListChangedType type, List<T> items, int count, int index)
            : this(type, default(T), default(T), items, count, index)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">改变类型</param>
        /// <param name="item">改变项</param>
        /// <param name="count">当前集合项组数</param>
        /// <param name="index">当前项在集合中的位置索引</param>
        public BindingListChangedArgs(BindingListChangedType type, T oldItem, int count, int index)
            : this(type, default(T), oldItem, null, count, index)
        { }*/

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">改变类型</param>
        /// <param name="item">新项</param>
        /// <param name="oldItem">旧项</param>
        /// <param name="newItems">新项集合</param>
        /// <param name="oldtems">旧项集合</param>
        /// <param name="count">当前集合项组数</param>
        /// <param name="index">当前项在集合中的位置索引</param>
        public BindingListChangedArgs(BindingListChangedType type, T item, T oldItem, List<T> newItems, List<T> oldtems, int count, int index)
        {
            this.Type = type;
            this.NewItem = item;
            this.OldItem = oldItem;
            this.NewItems = newItems;
            this.Oldtems = oldtems;
            this.Count = count;
            this.Index = index;
        }

        /// <summary>
        /// UI绑定集合改变类型
        /// </summary>
        public BindingListChangedType Type { get; private set; }

        /// <summary>
        /// 新项
        /// </summary>
        public T NewItem { get; private set; }

        /// <summary>
        /// 旧项
        /// </summary>
        public T OldItem { get; private set; }

        /// <summary>
        /// 新项集合
        /// </summary>
        public List<T> NewItems { get; private set; }

        /// <summary>
        /// 旧项集合
        /// </summary>
        public List<T> Oldtems { get; private set; }

        /// <summary>
        /// 当前集合项组数
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 当前项在集合中的位置索引
        /// </summary>
        public int Index { get; private set; }
    }
}
