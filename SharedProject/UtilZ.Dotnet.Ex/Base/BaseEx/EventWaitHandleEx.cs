using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base.MemoryCache;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// EventWaitHandle扩展类,通知正在等待的线程已发生事件
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class EventWaitHandleEx : IDisposable
    {
        /// <summary>
        /// 通知正在等待的线程已发生事件对象
        /// </summary>
        private readonly EventWaitHandle _eventWaitHandle;

        /// <summary>
        /// 对象是否已释放[true:已释放;false:未释放]
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="mode"></param>
        public EventWaitHandleEx(bool initialState, EventResetMode mode)
        {
            if (mode == EventResetMode.AutoReset)
            {
                this._eventWaitHandle = new AutoResetEvent(initialState);
            }
            else if (mode == EventResetMode.ManualReset)
            {
                this._eventWaitHandle = new ManualResetEvent(initialState);
            }
            else
            {
                throw new NotImplementedException(mode.ToString());
            }
        }

        /// <summary>
        /// 将事件状态设置为非终止，从而导致线程受阻[如果该操作成功，则为 true；否则为 false]
        /// </summary>
        /// <returns>如果该操作成功，则为 true；否则为 false</returns>
        public bool Reset()
        {
            if (this._isDisposed)
            {
                return false;
                //throw new ObjectDisposedException("_eventWaitHandle");
            }

            try
            {
                return this._eventWaitHandle.Reset();
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 将事件状态设置为有信号，从而允许一个或多个等待线程继续执行[如果该操作成功，则为 true；否则为 false]
        /// </summary>
        /// <returns>如果该操作成功，则为 true；否则为 false</returns>
        public bool Set()
        {
            if (this._isDisposed)
            {
                return false;
                //throw new ObjectDisposedException("_eventWaitHandle");
            }

            try
            {
                return this._eventWaitHandle.Set();
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 阻止当前线程，直到当前 System.Threading.WaitHandle 收到信号，同时使用 32 位带符号整数指定时间间隔（以毫秒为单位）
        /// </summary>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 System.Threading.Timeout.Infinite (-1)，表示无限期等待</param>
        /// <returns></returns>
        public bool WaitOne(int millisecondsTimeout = System.Threading.Timeout.Infinite)
        {
            if (this._isDisposed)
            {
                return false;
                //throw new ObjectDisposedException("_eventWaitHandle");
            }

            try
            {
                return this._eventWaitHandle.WaitOne(millisecondsTimeout);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {
            if (this._isDisposed)
            {
                return;
            }

            this._eventWaitHandle.Dispose();
            this._isDisposed = true;
        }
    }

    /// <summary>
    /// EventWaitHandle自动管理类
    /// </summary>
    public class AutoEventWaitHandleManager
    {
        /// <summary>
        /// key:EventKey(object);value:(EventWaitHandleInfo)
        /// </summary>
        private static readonly Hashtable _htEventWaitHandle = Hashtable.Synchronized(new Hashtable());

        private static void ExpirationCacheEntryRemovedCallback(CacheEntryRemovedArguments arguments)
        {
            try
            {
                if (arguments.RemovedReason != CacheEntryRemovedReason.Removed)
                {
                    var id = arguments.CacheItem.Value;
                    var eventWaitHandleInfo = _htEventWaitHandle[id] as EventWaitHandleInfo;
                    _htEventWaitHandle.Remove(id);
                    if (eventWaitHandleInfo != null)
                    {
                        try
                        {
                            eventWaitHandleInfo.EventWaitHandle.Dispose();
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
            lock (_htEventWaitHandle.SyncRoot)
            {
                if (_htEventWaitHandle.ContainsKey(id))
                {
                    throw new Exception($"key{id}已存在");
                }

                var eventWaitHandle = new AutoResetEvent(false);
                var eventWaitHandleInfo = new EventWaitHandleInfo(id, eventWaitHandle, tag);
                _htEventWaitHandle.Add(id, eventWaitHandleInfo);
                AddIdToCache(id, expirationMilliseconds);
                return eventWaitHandleInfo;
            }
        }

        /// <summary>
        /// 添加外部创建的EventWaitHandle
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <param name="eventWaitHandle">EventWaitHandle</param>
        /// <param name="expiration">缓存项有效时间,小于等于0永不过期,单位/毫秒</param>
        /// <param name="tag">Tag</param>
        public static void AddEventWaitHandle(object id, EventWaitHandle eventWaitHandle, int expiration = -1, object tag = null)
        {
            lock (_htEventWaitHandle.SyncRoot)
            {
                if (_htEventWaitHandle.ContainsKey(id))
                {
                    throw new Exception($"key{id}已存在");
                }

                _htEventWaitHandle.Add(id, new EventWaitHandleInfo(id, eventWaitHandle, tag));
                AddIdToCache(id, expiration);
            }
        }

        /// <summary>
        /// 设置外部创建的EventWaitHandle,如果id存在则替换,否则则添加
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <param name="eventWaitHandle">EventWaitHandle</param>
        /// <param name="expiration">缓存项有效时间,小于等于0永不过期,单位/毫秒</param>
        /// <param name="tag">Tag</param>
        public static EventWaitHandleInfo SetEventWaitHandle(object id, EventWaitHandle eventWaitHandle, int expiration = -1, object tag = null)
        {
            lock (_htEventWaitHandle.SyncRoot)
            {
                var oldEventWaitHandleInfo = _htEventWaitHandle[id] as EventWaitHandleInfo;
                _htEventWaitHandle[id] = new EventWaitHandleInfo(id, eventWaitHandle, tag);
                AddIdToCache(id, expiration);
                return oldEventWaitHandleInfo;
            }
        }

        /// <summary>
        /// 获取EventWaitHandle
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <returns>EventWaitHandle</returns>
        public static EventWaitHandle GetEventWaitHandle(object id)
        {
            lock (_htEventWaitHandle.SyncRoot)
            {
                if (_htEventWaitHandle.ContainsKey(id))
                {
                    return ((EventWaitHandleInfo)_htEventWaitHandle[id]).EventWaitHandle;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取EventWaitHandleInfo
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <returns>EventWaitHandleInfo</returns>
        public static EventWaitHandleInfo GetEventWaitHandleInfo(object id)
        {
            lock (_htEventWaitHandle.SyncRoot)
            {
                return _htEventWaitHandle[id] as EventWaitHandleInfo;
            }
        }

        /// <summary>
        /// 移除并返回EventWaitHandle
        /// </summary>
        /// <param name="id">EventWaitHandle的唯一标识</param>
        /// <returns>EventWaitHandle</returns>
        public static EventWaitHandleInfo RemoveEventWaitHandle(object id)
        {
            lock (_htEventWaitHandle.SyncRoot)
            {
                if (_htEventWaitHandle.ContainsKey(id))
                {
                    var eventWaitHandleInfo = _htEventWaitHandle[id] as EventWaitHandleInfo;
                    _htEventWaitHandle.Remove(id);
                    FromCacheRemoveId(id);
                    return eventWaitHandleInfo;
                }
                else
                {
                    return null;
                }
            }
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
    }
}
