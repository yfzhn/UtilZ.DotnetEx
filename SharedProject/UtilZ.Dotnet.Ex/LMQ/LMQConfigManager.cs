using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.LMQ
{
    /// <summary>
    /// 本地消息队列中心配置管理类
    /// </summary>
    public class LMQConfigManager
    {
        /// <summary>
        /// 本地消息队列中心配置集合
        /// </summary>
        private static readonly Hashtable _htLMQConfig = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 添加本地消息队列中心配置
        /// </summary>
        /// <param name="config">本地消息队列中心配置</param>
        public static void AddLMQConfig(LMQConfig config)
        {
            if (config == null)
            {
                return;
            }

            _htLMQConfig[config.Topic] = config;
        }

        /// <summary>
        /// 移除本地消息队列中心配置
        /// </summary>
        /// <param name="topic">主题</param>
        public static void RemoveLMQConfig(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                return;
            }

            if (_htLMQConfig.ContainsKey(topic))
            {
                _htLMQConfig.Remove(topic);
            }
        }

        /// <summary>
        /// 获取本地消息队列中心配置[无该主题的配置项返回null]
        /// </summary>
        /// <param name="topic">主题</param>
        /// <returns>本地消息队列中心配置</returns>
        public static LMQConfig GetLMQConfig(string topic)
        {
            return _htLMQConfig[topic] as LMQConfig;
        }
    }

    /// <summary>
    /// 本地消息队列中心配置类
    /// </summary>
    [Serializable]
    public class LMQConfig
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// 发布数据时是否同步发布[true:单线程同同步发布;false:多线程并行发布]
        /// </summary>
        public bool IsSyncPublish { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="isSyncPublish">发布数据时是否同步发布[true:单线程同同步发布;false:多线程并行发布]</param>
        public LMQConfig(string topic, bool isSyncPublish)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException("topic");
            }

            this.Topic = topic;
            this.IsSyncPublish = isSyncPublish;
        }
    }
}
