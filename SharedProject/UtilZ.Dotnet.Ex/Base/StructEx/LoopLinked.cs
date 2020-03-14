using UtilZ.Dotnet.Ex.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 环形链表
    /// </summary>
    /// <typeparam name="T">数据泛型</typeparam>
    public class LoopLinked<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LoopLinked()
        {

        }

        /// <summary>
        /// 所有数据项泛型List集合
        /// </summary>
        private List<T> _items = new List<T>();

        /// <summary>
        /// 第一个节点
        /// </summary>
        private LoopLinkedNode<T> _firstNode = null;

        /// <summary>
        /// 获取第一个节点
        /// </summary>
        public LoopLinkedNode<T> FirstNode
        {
            get { return this._firstNode; }
        }

        /// <summary>
        /// 获取节点数
        /// </summary>
        public int Count
        {
            get { return this._items.Count; }
        }

        /// <summary>
        /// 转换为泛型List集合
        /// </summary>
        /// <returns>泛型List集合</returns>
        public List<T> ToList()
        {
            return new List<T>(this._items);
        }

        /// <summary>
        /// 在纯属处添加新节点
        /// </summary>
        /// <param name="item">新项</param>
        public void AddLast(T item)
        {
            if (this._firstNode == null)
            {
                this._firstNode = new LoopLinkedNode<T>(item);
                this._firstNode.Next = this._firstNode;
                this._firstNode.Previous = this._firstNode;
            }
            else
            {
                var newNode = new LoopLinkedNode<T>(item);

                newNode.Previous = this._firstNode.Previous;
                this._firstNode.Previous.Next = newNode;

                newNode.Next = this._firstNode;
                this._firstNode.Previous = newNode;
            }

            this._items.Add(item);
        }

        /// <summary>
        /// 在指定节点之前添加新节点
        /// </summary>
        /// <param name="node">指定的节点</param>
        /// <param name="item">新项</param>
        public void AddBefore(LoopLinkedNode<T> node, T item)
        {
            var newNode = new LoopLinkedNode<T>(item);

            node.Previous.Next = newNode;
            newNode.Previous = node.Previous;

            newNode.Next = node;
            node.Previous = newNode;

            this._items.Add(item);
        }

        /// <summary>
        /// 在指定节点之后添加新节点
        /// </summary>
        /// <param name="node">指定的节点</param>
        /// <param name="item">新项</param>
        public void AddAfter(LoopLinkedNode<T> node, T item)
        {
            var newNode = new LoopLinkedNode<T>(item);

            newNode.Next = node.Next;
            node.Next.Previous = newNode;

            newNode.Previous = node;
            node.Next = newNode;

            this._items.Add(item);
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        /// <param name="item">要移除的项</param>
        public void Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var delNode = this.Find(item);
            if (delNode == null)
            {
                return;
            }

            this.DeleteNode(delNode);
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        /// <param name="node">要移除的项</param>
        public void Remove(LoopLinkedNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (this.ExistNode(node))
            {
                this.DeleteNode(node);
            }
            else
            {
                throw new ArgumentException("结点不属于当前环形链表");
            }
        }

        /// <summary>
        /// 判断一个结点是否属于当前的环形链表[属于返回true,不属于则返回false]
        /// </summary>
        /// <param name="node">结点</param>
        /// <returns>属于返回true,不属于则返回false</returns>
        private bool ExistNode(LoopLinkedNode<T> node)
        {
            //如果没有节点就直接返回
            if (this._firstNode == null)
            {
                return false;
            }

            //如果只有一个节点,就直接判断
            if (this._items.Count == 1)
            {
                return this._firstNode == node;
            }

            bool result = false;
            //当有两个以上的节点果则二分查找
            var pp = this._firstNode.Previous;//上一个
            var pn = this._firstNode;//下一个

            while (true)
            {
                if (pp == pn)
                {
                    if (pp == node)
                    {
                        result = true;
                    }

                    break;
                }

                if (pp == node)
                {
                    result = true;
                    break;
                }

                if (pn == node)
                {
                    result = true;
                    break;
                }

                if (pn.Next == pp)
                {
                    if (pp == node)
                    {
                        result = true;
                    }

                    break;
                }

                pp = pp.Previous;
                pn = pn.Next;
            }

            return result;
        }

        /// <summary>
        /// 删除一个指定的节点
        /// </summary>
        /// <param name="node">要删除的节点</param>
        private void DeleteNode(LoopLinkedNode<T> node)
        {
            //如果删除的是第一个节点,就把先把第二节点设置为第一个节点
            if (this._firstNode == node)
            {
                this._firstNode = this._firstNode.Next;
            }

            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;
            this._items.Remove(node.Value);
        }

        /// <summary>
        /// 查找数据项所在节点
        /// </summary>
        /// <param name="item">数据项</param>
        /// <returns>找到返回该项所在节点,没找到返回null</returns>
        public LoopLinkedNode<T> Find(T item)
        {
            //如果没有节点就直接返回
            if (this._firstNode == null)
            {
                return null;
            }

            //如果只有一个节点,就直接判断
            if (this._items.Count == 1)
            {
                if (this._firstNode.Value.Equals(item))
                {
                    return this._firstNode;
                }
                else
                {
                    return null;
                }
            }

            LoopLinkedNode<T> result = null;
            //当有两个以上的节点果则二分查找
            var pp = this._firstNode.Previous;//上一个
            var pn = this._firstNode;//下一个

            while (true)
            {
                if (pp == pn)
                {
                    if (pp.Value.Equals(item))
                    {
                        result = pp;
                    }

                    break;
                }

                if (pp.Value.Equals(item))
                {
                    result = pp;
                    break;
                }

                if (pn.Value.Equals(item))
                {
                    result = pn;
                    break;
                }

                if (pn.Next == pp)
                {
                    if (pp.Value.Equals(item))
                    {
                        result = pp;
                    }

                    break;
                }

                pp = pp.Previous;
                pn = pn.Next;
            }

            return result;
        }

        /// <summary>
        /// IEnumerable接口
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        /// <summary>
        /// IEnumerable接口
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// 环形链表节点
    /// </summary>
    /// <typeparam name="T">数据泛型</typeparam>
    public class LoopLinkedNode<T>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">数据项</param>
        internal LoopLinkedNode(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">数据项</param>
        /// <param name="pre">上一个节点</param>
        /// <param name="next">下一个节点</param>
        internal LoopLinkedNode(T value, LoopLinkedNode<T> pre, LoopLinkedNode<T> next)
            : this(value)
        {
            this.Previous = pre;
            this.Next = next;
        }

        /// <summary>
        /// 上一个节点
        /// </summary>
        public LoopLinkedNode<T> Previous { get; internal set; }

        /// <summary>
        /// 下一个节点
        /// </summary>
        public LoopLinkedNode<T> Next { get; internal set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Value { get; set; }
    }
}
