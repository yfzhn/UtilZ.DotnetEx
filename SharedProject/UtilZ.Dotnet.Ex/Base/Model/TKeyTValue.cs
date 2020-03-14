using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 泛型键值类
    /// </summary>
    [Serializable]
    public class TKeyTValue<TKey, TValue>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TKeyTValue()
        {
            
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public TKeyTValue(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// 获取或设置键
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public TValue Value { get; set; }
    }
}
