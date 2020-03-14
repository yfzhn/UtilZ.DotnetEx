using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 超时基类
    /// </summary>
    public abstract class TimeoutBase
    {
        /// <summary>
        /// 超时时长,单位毫秒
        /// </summary>
        private readonly int _millisecondsTimeout;

        /// <summary>
        /// 获取超时时长,单位毫秒
        /// </summary>
        public int MillisecondsTimeout
        {
            get { return _millisecondsTimeout; }
        }

        /// <summary>
        /// 最后一次访问时间戳
        /// </summary>
        private long _lastAccessTimestamp;

        /// <summary>
        /// 获取最后一次访问时间戳
        /// </summary>
        public long LastAccessTimestamp
        {
            get { return _lastAccessTimestamp; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="millisecondsTimeout">超时时长,单位毫秒</param>
        public TimeoutBase(int millisecondsTimeout)
        {
            this._millisecondsTimeout = millisecondsTimeout;
            this.UpdateLastAccessTimestamp();
        }

        /// <summary>
        /// 更新访问时间戳
        /// </summary>
        public void UpdateLastAccessTimestamp()
        {
            this._lastAccessTimestamp = TimeEx.GetTimestamp();
        }

        /// <summary>
        /// 是否超时
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTimeout()
        {
            var currentTimestamp = TimeEx.GetTimestamp();
            if (currentTimestamp - this._lastAccessTimestamp >= this._millisecondsTimeout)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
