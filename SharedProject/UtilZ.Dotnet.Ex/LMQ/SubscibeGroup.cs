using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.Ex.LMQ
{
    /// <summary>
    /// 订阅组
    /// </summary>
    internal class SubscibeGroup : LMQBase, IDisposable
    {
        /// <summary>
        /// 订阅项集合
        /// </summary>
        private readonly List<SubscibeItem> _items = new List<SubscibeItem>();

        /// <summary>
        /// 订阅项集合线程锁
        /// </summary>
        private readonly object _itemsMonitor = new object();

        /// <summary>
        /// 异步发布消息队列线程
        /// </summary>
        private readonly AsynQueue<object> _asynPublishParaQueueThread;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        public SubscibeGroup(string topic)
            : base(topic)
        {
            string name = string.Format("本地消息队列主题{0}数据消息发布线程", topic);
            this._asynPublishParaQueueThread = new AsynQueue<object>(this.PublishThreadMethod, name, true, true);
        }

        /// <summary>
        /// 发布消息线程方法
        /// </summary>
        /// <param name="dataMessage">数据消息</param>
        private void PublishThreadMethod(object dataMessage)
        {
            List<SubscibeItem> items;
            lock (this._itemsMonitor)
            {
                items = this._items.ToList();
            }

            if (items.Count == 0)
            {
                return;
            }

            var config = LMQConfigManager.GetLMQConfig(this.Topic);
            if (config != null && config.IsSyncPublish)
            {
                foreach (var item in items)
                {
                    item.Publish(dataMessage);
                }
            }
            else
            {
                //多线程并行发布消息
                Parallel.ForEach(items, (tmpItem) => { tmpItem.Publish(dataMessage); });
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="dataMessage">数据消息</param>
        public void Publish(object dataMessage)
        {
            this._asynPublishParaQueueThread.Enqueue(dataMessage);
        }

        /// <summary>
        /// 已订阅项数
        /// </summary>
        public int Count
        {
            get
            {
                lock (this._itemsMonitor)
                {
                    return this._items.Count;
                }
            }
        }

        /// <summary>
        /// 添加订阅项
        /// </summary>
        /// <param name="item">订阅项</param>
        public void Add(SubscibeItem item)
        {
            if (item == null)
            {
                return;
            }

            lock (this._itemsMonitor)
            {
                if (!this._items.Contains(item))
                {
                    this._items.Add(item);
                }
            }
        }

        /// <summary>
        /// 清空订阅项
        /// </summary>
        public void Clear()
        {
            lock (this._itemsMonitor)
            {
                this._items.Clear();
            }
        }

        /// <summary>
        /// 是否包含订阅项
        /// </summary>
        /// <param name="item">订阅项</param>
        /// <returns>包含返回true,否则返回false</returns>
        public bool Contains(SubscibeItem item)
        {
            if (item == null)
            {
                return false;
            }

            lock (this._itemsMonitor)
            {
                return this._items.Contains(item);
            }
        }

        /// <summary>
        /// 移除订阅项
        /// </summary>
        /// <param name="item">订阅项</param>
        /// <returns>移除结果</returns>
        public bool Remove(SubscibeItem item)
        {
            if (item == null)
            {
                return false;
            }

            lock (this._itemsMonitor)
            {
                return this._items.Remove(item);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {
            this._asynPublishParaQueueThread.Dispose();
        }
    }
}
