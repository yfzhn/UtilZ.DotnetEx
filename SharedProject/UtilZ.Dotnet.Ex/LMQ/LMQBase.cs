using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.LMQ
{
    /// <summary>
    /// 本地消息队列数据模型基类
    /// </summary>
    public abstract class LMQBase
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        protected LMQBase(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("主题不能为空或null", "topic");
            }

            this.Topic = topic;
        }
    }
}
