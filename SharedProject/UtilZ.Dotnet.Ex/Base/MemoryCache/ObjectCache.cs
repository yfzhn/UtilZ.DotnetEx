using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 原始缓存类
    /// </summary>
    public sealed class ObjectCache : IDisposable
    {
        /// <summary>
        /// 缓存项存放字典[key:key;value:缓存项]
        /// </summary>
        private readonly ConcurrentDictionary<object, CacheItem> _cacheDic = new ConcurrentDictionary<object, CacheItem>();

        /// <summary>
        /// 过期检查间隔
        /// </summary>
        private readonly int _checkIntervalMillisecondsTimeout;

        /// <summary>
        /// 缓存项过期检查线程
        /// </summary>
        private readonly ThreadEx _expirationChaeckThread;

        /// <summary>
        /// Disposable标识[true:已释放;false:未释放]
        /// </summary>
        private bool _disposed = false;



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="checkIntervalMillisecondsTimeout">过期检查间隔,单位:毫秒,默认值为100毫秒</param>
        public ObjectCache(int checkIntervalMillisecondsTimeout = 100)
        {
            if (checkIntervalMillisecondsTimeout <= 0)
            {
                throw new ArgumentOutOfRangeException("过期检查间隔值不能小于等于0");
            }

            this._checkIntervalMillisecondsTimeout = checkIntervalMillisecondsTimeout;
            this._expirationChaeckThread = new ThreadEx(this.ExpirationChaeckThreadMethod, "缓存项过期检查线程", true);
        }




        /// <summary>
        /// 启动缓存过期检查线程
        /// </summary>
        private void StartExpirationChaeckThread()
        {
            this._expirationChaeckThread.Start();
        }

        /// <summary>
        /// 如果缓存项数为0,则停止缓存过期检查线程
        /// </summary>
        private void StopExpirationChaeckThread()
        {
            if (this._cacheDic.Count == 0)
            {
                this._expirationChaeckThread.Stop();
            }
        }

        private void ExpirationChaeckThreadMethod(CancellationToken token)
        {
            try
            {
                CacheItem[] cacheItemArr;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        token.WaitHandle.WaitOne(this._checkIntervalMillisecondsTimeout, true);
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                    catch (AbandonedMutexException)
                    {
                        break;
                    }

                    try
                    {
                        cacheItemArr = this._cacheDic.Values.ToArray();
                        foreach (var cacheItem in cacheItemArr)
                        {
                            if (cacheItem.Expiration())
                            {
                                this.ExpirationRemove(cacheItem.Key);
                            }
                        }

                        this.StopExpirationChaeckThread();
                    }
                    catch (Exception exi)
                    {
                        Loger.Error(exi);
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }


        /// <summary>
        /// 返回移除结果[true:移除成功]
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool ExpirationRemove(object key)
        {
            lock (this._cacheDic)
            {
                CacheItem value;
                if (this._cacheDic.TryGetValue(key, out value))
                {
                    if (value.Expiration())
                    {
                        this._cacheDic.TryRemove(key, out value);
                        value.CallRemovedCallback(this, CacheEntryRemovedReason.Expired);
                        return true;
                    }
                }

                return false;
            }
        }








        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>缓存项,获取成功返回缓存项,值过期或key不存在返回null</returns>
        public object Get(object key)
        {
            if (key == null)
            {
                return null;
            }

            CacheItem value;
            while (this._cacheDic.TryGetValue(key, out value))
            {
                if (value.Expiration())
                {
                    if (this.ExpirationRemove(key))
                    {
                        return null;
                    }
                    else
                    {
                        continue;
                    }
                }

                value.LastGetTime = DateTimeOffset.Now;
                return value.Value;
            }

            return null;
        }






        /// <summary>
        /// 是否存在key值的数据[存在返回true;不存在返回false]
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>存在返回true;不存在返回false</returns>
        public bool Exist(object key)
        {
            if (key == null)
            {
                return false;
            }

            return this._cacheDic.ContainsKey(key);
        }




        /// <summary>
        /// 移除一个缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>被移除的缓存项</returns>
        public object Remove(object key)
        {
            if (key == null)
            {
                return null;
            }

            CacheItem value;
            if (this._cacheDic.TryRemove(key, out value))
            {
                value.CallRemovedCallback(this, CacheEntryRemovedReason.Removed);
                return value.Value;
            }

            return null;
        }








        #region Add
        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        public void Add(object key, object value)
        {
            this.PrimitiveAdd(key, value, null, null, null, null);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="expirationMilliseconds">缓存项绝对有效时间,单位/毫秒</param>
        /// <param name="removedCallback">移除回调</param>
        public void Add(object key, object value, int expirationMilliseconds, CacheEntryRemovedCallback removedCallback = null)
        {
            this.PrimitiveAdd(key, value, DateTimeOffset.Now.AddMilliseconds(expirationMilliseconds), null, null, removedCallback);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="slidingExpiration">缓存项滑动有效时间</param>
        /// <param name="removedCallback">移除回调</param>
        public void Add(object key, object value, TimeSpan slidingExpiration, CacheEntryRemovedCallback removedCallback = null)
        {
            this.PrimitiveAdd(key, value, null, slidingExpiration, null, removedCallback);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="customerExpiration">缓存项自定义过期验证回调</param>
        /// <param name="removedCallback">移除回调</param>
        public void Add(object key, object value, Func<CacheItem, bool> customerExpiration, CacheEntryRemovedCallback removedCallback = null)
        {
            this.PrimitiveAdd(key, value, null, null, customerExpiration, removedCallback);
        }

        /// <summary>
        /// 添加缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public void Add(CacheItem cacheItem)
        {
            if (cacheItem == null)
            {
                return;
            }

            if (cacheItem.Key == null)
            {
                throw new CacheKeyNullException();
            }

            lock (this._cacheDic)
            {
                if (this._cacheDic.ContainsKey(cacheItem.Key))
                {
                    throw new CacheKeyExistException(cacheItem.Key);
                }

                this.PrimitiveAdd(cacheItem);
            }
        }

        private void PrimitiveAdd(object key, object value, DateTimeOffset? absoluteExpiration, TimeSpan? slidingExpiration, Func<CacheItem, bool> customerExpiration, CacheEntryRemovedCallback removedCallback)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            lock (this._cacheDic)
            {
                if (this._cacheDic.ContainsKey(key))
                {
                    throw new CacheKeyExistException(key);
                }

                CacheItem cacheItem = new CacheItem(key, value);
                cacheItem.AbsoluteExpiration = absoluteExpiration;
                cacheItem.SlidingExpiration = slidingExpiration;
                cacheItem.CustomerExpiration = customerExpiration;
                cacheItem.RemovedCallback = removedCallback;
                cacheItem.InnerRemovedCallback = true;
                this.PrimitiveAdd(cacheItem);
            }
        }

        private void PrimitiveAdd(CacheItem cacheItem)
        {
            cacheItem.AddTime = DateTimeOffset.Now;
            if (this._cacheDic.TryAdd(cacheItem.Key, cacheItem))
            {
                this.StartExpirationChaeckThread();
            }
            else
            {
                throw new CacheAddException(cacheItem.Key);
            }
        }
        #endregion





        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        public void Set(object key, object value)
        {
            this.PrimitiveSet(key, value, null, null, null, null);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="expirationMilliseconds">缓存项有效时间,单位/毫秒</param>
        /// <param name="removedCallback">移除回调</param>
        public void Set(object key, object value, int expirationMilliseconds, CacheEntryRemovedCallback removedCallback = null)
        {
            this.PrimitiveSet(key, value, DateTimeOffset.Now.AddMilliseconds(expirationMilliseconds), null, null, removedCallback);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="slidingExpiration">缓存项滑动有效时间</param>
        /// <param name="removedCallback">移除回调</param>
        public void Set(object key, object value, TimeSpan slidingExpiration, CacheEntryRemovedCallback removedCallback)
        {
            this.PrimitiveSet(key, value, null, slidingExpiration, null, removedCallback);
        }


        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">缓存项</param>
        /// <param name="customerExpiration">缓存项自定义过期验证回调</param>
        /// <param name="removedCallback">移除回调</param>
        public void Set(object key, object value, Func<CacheItem, bool> customerExpiration, CacheEntryRemovedCallback removedCallback = null)
        {
            this.PrimitiveSet(key, value, null, null, customerExpiration, removedCallback);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="cacheItem">缓存项</param>
        public void Set(CacheItem cacheItem)
        {
            if (cacheItem == null)
            {
                return;
            }

            if (cacheItem.Key == null)
            {
                throw new CacheKeyNullException();
            }

            this.PrimitiveSet(cacheItem);
        }

        private void PrimitiveSet(object key, object value, DateTimeOffset? absoluteExpiration, TimeSpan? slidingExpiration, Func<CacheItem, bool> customerExpiration, CacheEntryRemovedCallback removedCallback)
        {
            if (key == null)
            {
                throw new CacheKeyNullException();
            }

            CacheItem cacheItem = new CacheItem(key, value);
            cacheItem.AbsoluteExpiration = absoluteExpiration;
            cacheItem.SlidingExpiration = slidingExpiration;
            cacheItem.CustomerExpiration = customerExpiration;
            cacheItem.RemovedCallback = removedCallback;
            cacheItem.InnerRemovedCallback = true;
            this.PrimitiveSet(cacheItem);
        }


        private void PrimitiveSet(CacheItem cacheItem)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            lock (this._cacheDic)
            {
                cacheItem.AddTime = DateTimeOffset.Now;
                this._cacheDic.AddOrUpdate(cacheItem.Key, cacheItem, (k, old) =>
                {
                    if (old != null)
                    {
                        //触发移除回调
                        old.CallRemovedCallback(this, CacheEntryRemovedReason.Replace);
                    }

                    return cacheItem;
                });
                this.StartExpirationChaeckThread();
            }
        }


        /// <summary>
        /// 清空缓存
        /// </summary>
        public void Clear()
        {
            var values = this._cacheDic.Values.ToArray();
            this._cacheDic.Clear();
            foreach (var value in values)
            {
                value.CallRemovedCallback(this, CacheEntryRemovedReason.Removed);
            }
            this.StopExpirationChaeckThread();
        }






        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this._disposed)
                {
                    return;
                }
                this._disposed = true;

                this.Clear();
                this._expirationChaeckThread.Dispose();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }
}
