using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        /// 订阅项列表
        /// </summary>
        private readonly List<SubscibeItem> _subscibeItemList = new List<SubscibeItem>();

        /// <summary>
        /// 订阅项列表线程锁
        /// </summary>
        private readonly object _subscibeItemListMonitor = new object();

        /// <summary>
        /// 异步发布消息队列线程
        /// </summary>
        private AsynQueue<PublishItem> _asynPublishParaQueueThread = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        public SubscibeGroup(string topic)
            : base(topic)
        {

        }

        /// <summary>
        /// 发布消息线程方法
        /// </summary>
        /// <param name="publishItem">数据消息</param>
        private void PublishThreadMethod(PublishItem publishItem)
        {
            if (this._subscibeItemList.Count == 0)
            {
                return;
            }

            Monitor.Enter(this._subscibeItemListMonitor);
            try
            {
                if (publishItem.Config != null && publishItem.Config.ParallelPublish)
                {
                    //多线程并行发布消息
                    Parallel.ForEach(this._subscibeItemList, (tmpItem) => { tmpItem.Publish(publishItem.Message); });
                }
                else
                {
                    //单线程发布
                    foreach (var item in this._subscibeItemList)
                    {
                        item.Publish(publishItem.Message);
                    }
                }
            }
            finally
            {
                Monitor.Exit(this._subscibeItemListMonitor);
            }
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">消息</param>
        public void Publish(object message)
        {
            LMQConfig config = LMQConfigManager.GetLMQConfig(this.Topic);
            var publishItem = new PublishItem(config, message);

            if (config != null && config.SyncPublish)
            {
                this.PublishThreadMethod(publishItem);
            }
            else
            {
                if (this._asynPublishParaQueueThread == null)
                {
                    string name = string.Format("本地消息队列主题{0}数据消息发布线程", this.Topic);
                    this._asynPublishParaQueueThread = new AsynQueue<PublishItem>(this.PublishThreadMethod, name, true, true);
                }

                this._asynPublishParaQueueThread.Enqueue(publishItem);
            }
        }

        /// <summary>
        /// 已订阅项数
        /// </summary>
        public int Count
        {
            get
            {
                Monitor.Enter(this._subscibeItemListMonitor);
                try
                {
                    return this._subscibeItemList.Count;
                }
                finally
                {
                    Monitor.Exit(this._subscibeItemListMonitor);
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

            Monitor.Enter(this._subscibeItemListMonitor);
            try
            {
                if (!this._subscibeItemList.Contains(item))
                {
                    this._subscibeItemList.Add(item);
                }
            }
            finally
            {
                Monitor.Exit(this._subscibeItemListMonitor);
            }
        }

        /// <summary>
        /// 清空订阅项
        /// </summary>
        public void Clear()
        {
            Monitor.Enter(this._subscibeItemListMonitor);
            try
            {
                this._subscibeItemList.Clear();
            }
            finally
            {
                Monitor.Exit(this._subscibeItemListMonitor);
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

            Monitor.Enter(this._subscibeItemListMonitor);
            try
            {
                return this._subscibeItemList.Contains(item);
            }
            finally
            {
                Monitor.Exit(this._subscibeItemListMonitor);
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

            Monitor.Enter(this._subscibeItemListMonitor);
            try
            {
                return this._subscibeItemList.Remove(item);
            }
            finally
            {
                Monitor.Exit(this._subscibeItemListMonitor);
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
            if (this._asynPublishParaQueueThread != null)
            {
                this._asynPublishParaQueueThread.Dispose();
                this._asynPublishParaQueueThread = null;
            }
        }
    }


    internal sealed class PublishItem
    {
        public LMQConfig Config { get; private set; }

        public object Message { get; private set; }

        public PublishItem(LMQConfig config, object message)
        {
            this.Config = config;
            this.Message = message;
        }
    }
}
