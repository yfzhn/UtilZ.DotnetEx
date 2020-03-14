using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.LMQ
{
    /// <summary>
    /// 订阅项
    /// </summary>
    [Serializable]
    public class SubscibeItem : LMQBase
    {
        /// <summary>
        /// 消息通知委托
        /// </summary>
        public Action<SubscibeItem, object> MessageNotify;

        /// <summary>
        /// 标签
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        public SubscibeItem(string topic)
            : base(topic)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="messageNotify">消息通知委托</param>
        public SubscibeItem(string topic, Action<SubscibeItem, object> messageNotify)
            : base(topic)
        {
            this.MessageNotify = messageNotify;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message">数据消息</param>
        internal void Publish(object message)
        {
            try
            {
                var handler = this.MessageNotify;
                if (handler != null)
                {
                    handler(this, message);
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
