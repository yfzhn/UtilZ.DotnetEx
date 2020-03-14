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
        /// 订阅项集合
        /// </summary>
        private static readonly Hashtable _htSubscibeItems = Hashtable.Synchronized(new Hashtable());

        #region 订阅管理
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="item">订阅项</param>
        public static void Subscibe(SubscibeItem item)
        {
            lock (_htSubscibeItems.SyncRoot)
            {
                SubscibeGroup subscibeGroup;
                string topic = item.Topic;
                if (_htSubscibeItems.ContainsKey(topic))
                {
                    subscibeGroup = _htSubscibeItems[topic] as SubscibeGroup;
                    if (subscibeGroup.Contains(item))
                    {
                        return;
                    }
                }
                else
                {
                    subscibeGroup = new SubscibeGroup(topic);
                    _htSubscibeItems.Add(topic, subscibeGroup);
                }

                subscibeGroup.Add(item);
            }
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

            lock (_htSubscibeItems.SyncRoot)
            {
                if (_htSubscibeItems.ContainsKey(item.Topic))
                {
                    var subscibeGroup = _htSubscibeItems[item.Topic] as SubscibeGroup;
                    subscibeGroup.Remove(item);

                    //如果订阅组内的订阅项数为0，则移除该订阅组
                    if (subscibeGroup.Count == 0)
                    {
                        subscibeGroup.Dispose();
                        _htSubscibeItems.Remove(item.Topic);
                    }
                }
            }
        }

        /// <summary>
        /// 清空订阅
        /// </summary>
        /// <param name="topic">订阅ID下要清空订阅主题,为null清空所有订阅组</param>
        public static void Clear(string topic)
        {
            lock (_htSubscibeItems.SyncRoot)
            {
                if (topic == null)
                {
                    _htSubscibeItems.Clear();
                    return;
                }

                if (_htSubscibeItems.ContainsKey(topic))
                {
                    var subscibeGroup = _htSubscibeItems[topic] as SubscibeGroup;
                    subscibeGroup.Clear();

                    //如果订阅组内的订阅项数为0，则移除该订阅组
                    _htSubscibeItems.Remove(topic);
                }
            }
        }
        #endregion

        /// <summary>
        /// 发布数据消息
        /// </summary>
        /// <param name="topic">主题名称</param>
        /// <param name="obj">数据对象</param>
        public static void Publish(string topic, object obj)
        {
            var subscibeGroup = _htSubscibeItems[topic] as SubscibeGroup;
            if (subscibeGroup != null)
            {
                subscibeGroup.Publish(obj);
            }
        }
    }
}
