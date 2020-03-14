using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 张位存实体
    /// </summary>
    public class CacheItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">CacheItem 项的唯一标识符</param>
        /// <param name="value">CacheItem 项的数据</param>
        public CacheItem(object key, object value)
        {
            this.Key = key;
            this.Value = value;
        }



        /// <summary>
        /// 获取CacheItem 实例的唯一标识符
        /// </summary>
        public object Key { get; private set; }

        /// <summary>
        /// 获取或设置 System.Runtime.Caching.CacheItem 实例的数据
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 绝对过期时间，为null则条件无效
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        ///// <summary>
        ///// 相对当前时间的绝对过期时间（使用TimeSpan），为null条件无效
        ///// </summary>
        //public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        /// <summary>
        /// 滑动过期时间，为null条件无效
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// 提供用来自定义缓存过期，为null条件无效
        /// </summary>
        public Func<CacheItem, bool> CustomerExpiration { get; set; }

        /// <summary>
        /// 缓存移除回调，为null条件无效
        /// </summary>
        public CacheEntryRemovedCallback RemovedCallback { get; set; }




        /// <summary>
        /// 指示移除回调通知是否是ObjectCache内赋值的
        /// </summary>
        internal bool InnerRemovedCallback { get; set; } = false;

        /// <summary>
        /// 上次Get时间
        /// </summary>
        internal DateTimeOffset? LastGetTime { get; set; }

        /// <summary>
        /// 缓存项添加时间
        /// </summary>
        public DateTimeOffset AddTime { get; set; }



        /// <summary>
        /// 获取当前缓存项是否过期
        /// </summary>
        /// <returns></returns>
        internal bool Expiration()
        {
            lock (this)
            {
                var currentTime = DateTimeOffset.Now;
                if (this.AbsoluteExpiration.HasValue && currentTime >= this.AbsoluteExpiration.Value)
                {
                    return true;
                }

                if (this.SlidingExpiration.HasValue)
                {
                    if (this.LastGetTime.HasValue)
                    {
                        if (currentTime - this.LastGetTime.Value > this.SlidingExpiration.Value)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (currentTime - this.AddTime > this.SlidingExpiration.Value)
                        {
                            return true;
                        }
                    }
                }

                var handler = this.CustomerExpiration;
                if (handler != null)
                {
                    if (handler(this))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 触发缓存项移除回调
        /// </summary>
        /// <param name="objectCache"></param>
        /// <param name="cacheEntryRemovedReason"></param>
        internal void CallRemovedCallback(ObjectCache objectCache, CacheEntryRemovedReason cacheEntryRemovedReason)
        {
            lock (this)
            {
                var handler = this.RemovedCallback;
                if (handler != null)
                {
                    handler(new CacheEntryRemovedArguments(objectCache, cacheEntryRemovedReason, this));
                    if (this.InnerRemovedCallback)
                    {
                        this.RemovedCallback = null;
                    }
                }
            }
        }
    }
}
