using System;
using System.Collections.Generic;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 内存缓存类
    /// </summary>
    public class MemoryCacheEx
    {
        private readonly static ObjectCache _default;

        static MemoryCacheEx()
        {
            _default = new ObjectCache();
        }



        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>缓存项,获取成功返回缓存项,值过期或key不存在返回null</returns>
        public static object Get(object key)
        {
            return _default.Get(key);
        }

        /// <summary>
        /// 是否存在key值的数据[存在返回true;不存在返回false]
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true;不存在返回false</returns>
        public static bool Exist(object key)
        {
            return _default.Exist(key);
        }

        /// <summary>
        /// 移除一个缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>移除的缓存项</returns>
        public static object Remove(object key)
        {
            return _default.Remove(key);
        }






        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        public static void Add(object key, object value)
        {
            _default.Add(key, value);
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="expirationMilliseconds">缓存项有效时间,单位/毫秒</param>
        /// <param name="removedCallback">移除回调</param>
        public static void Add(object key, object value, int expirationMilliseconds, CacheEntryRemovedCallback removedCallback = null)
        {
            _default.Add(key, value, expirationMilliseconds, removedCallback);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="slidingExpiration">缓存项滑动有效时间</param>
        /// <param name="removedCallback">移除回调</param>
        public static void Add(object key, object value, TimeSpan slidingExpiration, CacheEntryRemovedCallback removedCallback = null)
        {
            _default.Add(key, value, slidingExpiration, removedCallback);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="customerExpiration">缓存项自定义过期验证回调</param>
        /// <param name="removedCallback">移除回调</param>
        public static void Add(object key, object value, Func<CacheItem, bool> customerExpiration, CacheEntryRemovedCallback removedCallback = null)
        {
            _default.Add(key, value, customerExpiration, removedCallback);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public static void Add(CacheItem cacheItem)
        {
            _default.Add(cacheItem);
        }







        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        public static void Set(object key, object value)
        {
            _default.Set(key, value);
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="expirationMilliseconds">缓存项有效时间,单位/毫秒</param>
        /// <param name="removedCallback">移除回调</param>
        public static void Set(object key, object value, int expirationMilliseconds, CacheEntryRemovedCallback removedCallback = null)
        {
            _default.Set(key, value, expirationMilliseconds, removedCallback);
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="slidingExpiration">缓存项滑动有效时间</param>
        /// <param name="removedCallback">移除回调</param>
        public static void Set(object key, object value, TimeSpan slidingExpiration, CacheEntryRemovedCallback removedCallback)
        {
            _default.Set(key, value, slidingExpiration, removedCallback);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="customerExpiration">缓存项自定义过期验证回调</param>
        /// <param name="removedCallback">移除回调</param>
        public static void Set(object key, object value, Func<CacheItem, bool> customerExpiration, CacheEntryRemovedCallback removedCallback = null)
        {
            _default.Set(key, value, customerExpiration, removedCallback);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public static void Set(CacheItem cacheItem)
        {
            _default.Set(cacheItem);
        }




        /// <summary>
        /// 清空缓存
        /// </summary>
        public static void Clear()
        {
            _default.Clear();
        }
    }
}