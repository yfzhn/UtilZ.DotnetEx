using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 缓存项移除回调委托
    /// </summary>
    /// <param name="arguments">回调参数</param>
    public delegate void CacheEntryRemovedCallback(CacheEntryRemovedArguments arguments);
}
