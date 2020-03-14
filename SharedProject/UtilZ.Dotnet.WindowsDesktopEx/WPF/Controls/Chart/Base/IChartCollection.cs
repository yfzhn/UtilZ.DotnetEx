using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart集合接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChartCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// Chart集合改变事件
        /// </summary>
        event EventHandler<ChartCollectionChangedEventArgs<T>> ChartCollectionChanged;

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="collection">The collection.</param>
        void InsertRange(int index, IEnumerable<T> collection);
    }

    /// <summary>
    /// Chart集合改变事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChartCollectionChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 改变行为
        /// </summary>
        public ChartCollectionChangedAction Action { get; private set; }

        /// <summary>
        /// 新项集合
        /// </summary>
        public List<T> NewItems { get; private set; }

        /// <summary>
        /// 旧项集合
        /// </summary>
        public List<T> OldItems { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">改变行为</param>
        /// <param name="newItems">新项集合</param>
        /// <param name="oldItems">旧项集合</param>
        public ChartCollectionChangedEventArgs(ChartCollectionChangedAction action, List<T> newItems, List<T> oldItems)
        {
            this.Action = action;
            this.NewItems = newItems;
            this.OldItems = oldItems;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">改变行为</param>
        /// <param name="newItem">新项</param>
        /// <param name="oldItem">旧项</param>
        public ChartCollectionChangedEventArgs(ChartCollectionChangedAction action, T newItem, T oldItem)
        {
            this.Action = action;
            if (newItem != null)
            {
                this.NewItems = new List<T>() { newItem };
            }

            if (oldItem != null)
            {
                this.OldItems = new List<T>() { oldItem };
            }
        }
    }

    /// <summary>
    /// Chart集合改变改变行为
    /// </summary>
    public enum ChartCollectionChangedAction
    {
        /// <summary>
        /// 添加
        /// </summary>
        Add = 0,

        /// <summary>
        /// 移除
        /// </summary>
        Remove = 1,

        /// <summary>
        /// 替换
        /// </summary>
        Replace = 2,

        /// <summary>
        /// 移动
        /// </summary>
        Move = 3,
    }
}
