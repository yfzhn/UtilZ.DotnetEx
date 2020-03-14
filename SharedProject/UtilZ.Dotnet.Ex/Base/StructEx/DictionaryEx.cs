using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// Dictionary扩展类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DictionaryEx()
            : base()
        {
            
        }

        /// <summary>
        /// 获取可用于同步对 System.Collections.Hashtable 的访问的对象。
        /// </summary>
        public readonly object SyncRoot = new object();
    }
}
