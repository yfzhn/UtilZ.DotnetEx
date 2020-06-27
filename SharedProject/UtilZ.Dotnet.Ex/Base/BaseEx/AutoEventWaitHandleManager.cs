using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base.MemoryCache;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// EventWaitHandle自动管理类
    /// </summary>
    public class AutoEventWaitHandleManager
    {
        /// <summary>
        /// key:EventKey(object);value:(EventWaitHandleInfo)
        /// </summary>
        private static readonly ConcurrentDictionary<object, EventWaitHandleInfo> _eventWaitHandleDic = new ConcurrentDictionary<object, EventWaitHandleInfo>();

        private static void ExpirationCacheEntryRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            try
            {
                if (arguments.RemovedReason != CacheEntryRemovedReason.Removed)
                {
                    var id = arguments.CacheItem.Value;
                    EventWaitHandleInfo eventWaitHandleInfo;
                    if (_eventWaitHandleDic.TryRemove(id, out eventWaitHandleInfo))
                    {
                        try
                        {
                            if (eventWaitHandleInfo != null)
                            {
                                eventWaitHandleInfo.EventWaitHandle.Dispose();
                            }
                        }
                        catch (ObjectDisposedException)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    string err = $"方法{typeof(AutoEventWaitHandleManager).Name}.{nameof(ExpirationCacheEntryRemovedCallback)}发生异常;RemovedReason:{arguments.RemovedReason},Key:{arguments.CacheItem.Key},Value:{arguments.CacheItem.Value}";
                    Loger.Error(ex, err);
                }
                catch (Exception ex2)
                {
                    Loger.Error(ex, ex2.Message);
                }
            }
        }

        private static string GetCacheKeyFromObjectId(object id)
        {
            if (id is string)
            {
                return (string)id;
            }

            //原理是同一个对象无论调用多少次GetHashCode得到的值都是一样的
            return id.GetHashCode().ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expirationMilliseconds">缓存项有效时间,小于等于0永不过期,单位/毫秒</param>
        private static void AddIdToCache(object id, int expirationMilliseconds)
        {
            if (expirationMilliseconds <= 0)
            {
                return;
            }

            string key = GetCacheKeyFromObjectId(id);
            MemoryCacheEx.Set(key, id, expirationMilliseconds, new CacheEntryRemovedCallback(ExpirationCacheEntryRemovedCallback));
        }

        private static void FromCacheRemoveId(object id)
        {
            string key = GetCacheKeyFromObjectId(id);
            MemoryCacheEx.Remove(key);
        }

        /// <summary>
        /// 创建并添加EventWaitHandle
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <param name="expirationMilliseconds">缓存项有效时间,小于等于0永不过期,单位/毫秒</param>
        /// <param name="tag">Tag</param>
        /// <returns>创建的EventWaitHandle</returns>
        public static EventWaitHandleInfo CreateEventWaitHandle(object id, int expirationMilliseconds = -1, object tag = null)
        {
            var eventWaitHandleInfo = new EventWaitHandleInfo(id, new AutoResetEvent(false), tag);
            if (_eventWaitHandleDic.TryAdd(id, eventWaitHandleInfo))
            {
                AddIdToCache(id, expirationMilliseconds);
                return eventWaitHandleInfo;
            }

            eventWaitHandleInfo.EventWaitHandle.Dispose();
            throw new ArgumentException($"key{id}已存在");
        }

        /// <summary>
        /// 添加外部创建的EventWaitHandle
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <param name="eventWaitHandle">EventWaitHandle</param>
        /// <param name="expirationMilliseconds">缓存项有效时间,小于等于0永不过期,单位/毫秒</param>
        /// <param name="tag">Tag</param>
        public static void AddEventWaitHandle(object id, EventWaitHandle eventWaitHandle, int expirationMilliseconds = -1, object tag = null)
        {
            if (_eventWaitHandleDic.TryAdd(id, new EventWaitHandleInfo(id, eventWaitHandle, tag)))
            {
                AddIdToCache(id, expirationMilliseconds);
            }
            else
            {
                throw new ArgumentException($"key{id}已存在");
            }
        }

        /// <summary>
        /// 添加或更新外部创建的EventWaitHandle,如果id存在则替换并返回旧的项,否则则添加返回null
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <param name="eventWaitHandle">EventWaitHandle</param>
        /// <param name="expiration">缓存项有效时间,小于等于0永不过期,单位/毫秒</param>
        /// <param name="tag">Tag</param>
        /// <returns>旧的EventWaitHandle</returns>
        public static EventWaitHandle AddOrUpdateEventWaitHandle(object id, EventWaitHandle eventWaitHandle, int expiration = -1, object tag = null)
        {
            EventWaitHandleInfo oldEventWaitHandleInfo = null;
            var newValue = new EventWaitHandleInfo(id, eventWaitHandle, tag);
            _eventWaitHandleDic.AddOrUpdate(id, newValue, (key, oldValue) =>
            {
                oldEventWaitHandleInfo = oldValue;
                return newValue;
            });
            AddIdToCache(id, expiration);

            if (oldEventWaitHandleInfo != null)
            {
                return oldEventWaitHandleInfo.EventWaitHandle;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取EventWaitHandle,获取成功返回获取EventWaitHandle;失败返回null
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <returns>EventWaitHandle</returns>
        public static EventWaitHandle GetEventWaitHandle(object id)
        {
            EventWaitHandleInfo eventWaitHandleInfo = GetEventWaitHandleInfo(id);
            if (eventWaitHandleInfo != null)
            {
                return eventWaitHandleInfo.EventWaitHandle;
            }

            return null;
        }

        /// <summary>
        /// 获取EventWaitHandleInfo
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <returns>EventWaitHandleInfo</returns>
        public static EventWaitHandleInfo GetEventWaitHandleInfo(object id)
        {
            EventWaitHandleInfo eventWaitHandleInfo;
            _eventWaitHandleDic.TryGetValue(id, out eventWaitHandleInfo);
            return eventWaitHandleInfo;
        }

        /// <summary>
        /// 移除并返回EventWaitHandle
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <returns>EventWaitHandle</returns>
        public static EventWaitHandleInfo RemoveEventWaitHandle(object id)
        {
            EventWaitHandleInfo eventWaitHandleInfo;
            if (_eventWaitHandleDic.TryRemove(id, out eventWaitHandleInfo))
            {
                FromCacheRemoveId(id);
            }

            return eventWaitHandleInfo;
        }
    }

    /// <summary>
    /// EventWaitHandleInfo
    /// </summary>
    public class EventWaitHandleInfo
    {
        /// <summary>
        /// EventWaitHandle的唯一标识
        /// </summary>
        public object Id { get; private set; }

        /// <summary>
        /// EventWaitHandle
        /// </summary>
        public EventWaitHandle EventWaitHandle { get; private set; }

        /// <summary>
        /// Tag
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventWaitHandle"></param>
        /// <param name="tag"></param>
        public EventWaitHandleInfo(object id, EventWaitHandle eventWaitHandle, object tag = null)
        {
            this.Id = id;
            this.EventWaitHandle = eventWaitHandle;
            this.Tag = tag;
        }

        /// <summary>
        /// 发出通知,对象释放将抛出异常
        /// </summary>
        /// <param name="tag">tag</param>
        public void Set(object tag = null)
        {
            this.Tag = tag;
            this.EventWaitHandle.Set();
        }

        /// <summary>
        /// 发出通知,对象释放将抛出异常
        /// </summary>
        public void Set()
        {
            this.EventWaitHandle.Set();
        }

        /// <summary>
        /// 尝试发出通知,成功返回true,否则返回false
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool TrySet(object tag = null)
        {
            try
            {
                this.Set(tag);
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试发出通知,成功返回true,否则返回false
        /// </summary>
        /// <returns>成功返回true,否则返回false</returns>
        public bool TrySet()
        {
            try
            {
                this.Set();
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }
}
