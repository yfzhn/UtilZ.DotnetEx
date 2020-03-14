using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 时间扩展类
    /// </summary>
    public class TimeEx
    {
        private readonly static DateTimeOffset _refTime;
        static TimeEx()
        {
            _refTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromTicks(0L));
            //_refTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        }

        /// <summary>
        /// 获取当前时间的时间戳
        /// </summary>
        /// <param name="utc">是否使用utc时间[true:utc时间;false:本机时间]</param>
        /// <returns>当前时间的时间戳</returns>
        public static long GetTimestamp(bool utc = true)
        {
            TimeSpan ts;
            if (utc)
            {
                ts = DateTimeOffset.UtcNow - _refTime;
            }
            else
            {
                ts = DateTimeOffset.Now - _refTime;
            }

            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        /// <summary>
        /// 日期时间转换为时间戳
        /// </summary>
        /// <param name="datetime">指定日期时间</param>
        /// <returns>当前时间的时间戳</returns>
        public static long DateTimeToTimestamp(DateTimeOffset datetime)
        {
            var ts = datetime - _refTime;
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        /// <summary>
        /// 时间戳转换为日期时间
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns>时间</returns>
        public static DateTimeOffset TimestampToDateTime(long timestamp)
        {
            return _refTime.AddMilliseconds(timestamp);
        }
    }
}
