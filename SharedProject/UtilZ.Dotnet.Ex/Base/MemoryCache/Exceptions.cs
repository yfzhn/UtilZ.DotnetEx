using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base.MemoryCache
{
    /// <summary>
    /// 缓存key已存在异常
    /// </summary>
    public class CacheKeyExistException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">Key</param>
        public CacheKeyExistException(object key)
            : base($"缓存Key{key}已存在")
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CacheKeyExistException()
            : base()
        {

        }
    }


    /// <summary>
    /// 缓存key已存在异常
    /// </summary>
    public class CacheKeyNullException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CacheKeyNullException()
            : base($"缓存Key为null")
        {

        }
    }


    /// <summary>
    /// 缓存添加失败异常
    /// </summary>
    public class CacheAddException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CacheAddException(object key)
            : base($"缓存Key{key}添加失败,原因未知")
        {

        }
    }
}
