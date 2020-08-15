using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.LMQ
{
    /// <summary>
    /// 本地消息队列中心
    /// </summary>
    public class LMQCenter
    {
        /// <summary>
        /// 订阅组字典集合[key:Topic;value:SubscibeGroup]
        /// </summary>
        private static readonly ConcurrentDictionary<string, SubscibeGroup> _subscibeItemDic = new ConcurrentDictionary<string, SubscibeGroup>();
        private static readonly object _subscibeItemDicLock = new object();


        #region 订阅管理
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="item">订阅项</param>
        public static void Subscibe(SubscibeItem item)
        {
            SubscibeGroup subscibeGroup;
            if (!_subscibeItemDic.TryGetValue(item.Topic, out subscibeGroup))
            {
                lock (_subscibeItemDicLock)
                {
                    if (!_subscibeItemDic.TryGetValue(item.Topic, out subscibeGroup))
                    {
                        subscibeGroup = new SubscibeGroup(item.Topic);
                        _subscibeItemDic.TryAdd(item.Topic, subscibeGroup);
                    }
                }
            }

            subscibeGroup.Add(item);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="item">订阅项</param>
        public static void UnSubscibe(SubscibeItem item)
        {
            if (item == null)
            {
                return;
            }

            SubscibeGroup subscibeGroup;
            if (!_subscibeItemDic.TryGetValue(item.Topic, out subscibeGroup))
            {
                return;
            }

            subscibeGroup.Remove(item);

            //如果订阅组内的订阅项数为0，则移除该订阅组
            if (subscibeGroup.Count == 0)
            {
                lock (_subscibeItemDicLock)
                {
                    if (subscibeGroup.Count == 0)
                    {
                        subscibeGroup.Dispose();
                        _subscibeItemDic.TryRemove(item.Topic, out subscibeGroup);
                    }
                }
            }
        }

        /// <summary>
        /// 清空所有订阅
        /// </summary>
        public static void Clear()
        {
            lock (_subscibeItemDicLock)
            {
                foreach (var group in _subscibeItemDic.Values)
                {
                    group.Dispose();
                }

                _subscibeItemDic.Clear();
            }
        }

        /// <summary>
        /// 清空指定主题订阅
        /// </summary>
        /// <param name="topic">主题</param>
        public static void Clear(string topic)
        {
            SubscibeGroup subscibeGroup;
            if (_subscibeItemDic.TryRemove(topic, out subscibeGroup))
            {
                subscibeGroup.Clear();
                subscibeGroup.Dispose();
            }
        }
        #endregion


        /// <summary>
        /// 发布数据消息
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="message">消息</param>
        public static void Publish(string topic, object message)
        {
            SubscibeGroup subscibeGroup;
            if (_subscibeItemDic.TryGetValue(topic, out subscibeGroup))
            {
                subscibeGroup.Publish(message);
            }
        }
    }
}
