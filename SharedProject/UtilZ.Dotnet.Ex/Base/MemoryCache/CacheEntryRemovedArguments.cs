using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 提供有关已从缓存中移除的缓存项的信息
    /// </summary>
    public class CacheEntryRemovedArguments
    {
        /// <summary>
        /// 获取已从缓存中移除的缓存项的Key
        /// </summary>
        public object Key { get; private set; }

        /// <summary>
        /// 获取已从缓存中移除的缓存项的实例
        /// </summary>
        public CacheItem CacheItem { get; private set; }

        /// <summary>
        /// 获取一个值，该值指示移除某个缓存项的原因
        /// </summary>
        public CacheEntryRemovedReason RemovedReason { get; private set; }

        /// <summary>
        /// 获取对源 System.Runtime.Caching.ObjectCache 实例的引用，该实例最初包含已移除的缓存项
        /// </summary>
        public ObjectCache Source { get; private set; }

        /// <summary>
        /// 初始化 System.Runtime.Caching.CacheEntryRemovedArguments 类的新实例
        /// </summary>
        /// <param name="source">已从中移除 cacheItem 的 System.Runtime.Caching.ObjectCache 实例</param>
        /// <param name="reason">用于指示移除 cacheItem 的原因的枚举值之一</param>
        /// <param name="cacheItem">已移除的缓存项的实例</param>
        public CacheEntryRemovedArguments(ObjectCache source, CacheEntryRemovedReason reason, CacheItem cacheItem)
        {
            this.Key = cacheItem.Key;
            this.Source = source;
            this.RemovedReason = reason;
            this.CacheItem = cacheItem;
        }
    }
}
