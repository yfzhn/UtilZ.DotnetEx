using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 指定已移除或将要移除某个缓存项的原因
    /// </summary>
    public enum CacheEntryRemovedReason
    {
        /// <summary>
        /// 通过使用Remove或Set方法移除了某个缓存项
        /// </summary>
        Removed = 0,

        /// <summary>
        /// 某个缓存项由于已过期而被移除。过期可基于绝对过期时间或可调过期时间
        /// </summary>
        Expired = 1,

        /// <summary>
        /// 某个缓存项由于被新项替换而被移除。
        /// </summary>
        Replace = 2,

        ///// <summary>
        ///// 某个缓存项由于释放缓存中的内存的原因而被移除。当某个缓存实例将超出特定于缓存的内存限制或某个进程或缓存实例将超出整个计算机范围的内存限制时，会发生这种情况
        ///// </summary>
        //Evicted = 2,

        ///// <summary>
        ///// 某个缓存项由于相关依赖项（如一个文件或其他缓存项）触发了其逐出操作而被移除
        ///// </summary>
        //ChangeMonitorChanged = 3,

        ///// <summary>
        ///// 某个缓存项由于特定缓存实现定义的原因而被逐出
        ///// </summary>
        //CacheSpecificEviction = 4
    }
}


//绝对过期支持
//滑动过期支持（指定一个时间，TimeSpan，指定时间内有被Get缓存时间则顺延，否则过期）
//过期回调
//自定义过期



//    Key 缓存key

//Value 缓存值

//AbsoluteExpiration 绝对过期时间，为null则条件无效

//AbsoluteExpirationRelativeToNow 相对当前时间的绝对过期时间（使用TimeSpan），为null条件无效

//SlidingExpiration 滑动过期时间

//ExpirationTokens 提供用来自定义缓存过期

//PostEvictionCallbacks 缓存失效回调

//Priority 缓存项优先级（在缓存满载的时候绝对清除的顺序）