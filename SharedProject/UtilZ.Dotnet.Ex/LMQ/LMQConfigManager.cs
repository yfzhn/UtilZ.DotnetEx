using System;
using System.Collections;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, LMQConfig> _lmqConfigDic = null;

        static LMQConfigManager()
        {
            _lmqConfigDic = new ConcurrentDictionary<string, LMQConfig>();
        }

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

            _lmqConfigDic.AddOrUpdate(config.Topic, config, (t, c) => { return config; });
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

            LMQConfig config;
            _lmqConfigDic.TryRemove(topic, out config);
        }

        /// <summary>
        /// 获取本地消息队列中心配置[无该主题的配置项返回null]
        /// </summary>
        /// <param name="topic">主题</param>
        /// <returns>本地消息队列中心配置</returns>
        public static LMQConfig GetLMQConfig(string topic)
        {
            LMQConfig config;
            _lmqConfigDic.TryGetValue(topic, out config);
            return config;
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
        public bool SyncPublish { get; set; } = true;

        /// <summary>
        /// 多线程并行发布[true:多线程并行发布;false:单线程循环发布]
        /// </summary>
        public bool ParallelPublish { get; set; } = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        public LMQConfig(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            this.Topic = topic;
        }
    }
}
