using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    ///  DateTime类型扩展方法类
    /// </summary>
    public static class DateTimeEx
    {
        /// <summary>
        /// 比较日期的年月日是否相等
        /// </summary>
        /// <param name="dt">日期</param>
        /// <param name="dstTime">目标日期</param>
        /// <returns>相等返回true,不等返回false</returns>
        public static bool CompareDate(this DateTime dt, DateTime dstTime)
        {
            return dt.Year == dstTime.Year && dt.DayOfYear == dstTime.DayOfYear;
        }

        /// <summary>
        /// 日期时间戳转为日期时间
        /// </summary>
        /// <param name="timestampValue">日期时间戳值</param>
        /// <returns>日期时间</returns>
        public static DateTime TimestampToDateTime(double timestampValue)
        {
            DateTime timestampStandardDateTime = new DateTime(1970, 1, 1, 0, 0, 0, System.Globalization.Calendar.CurrentEra);//时间戳基准时间
            var ts = TimeSpan.FromMilliseconds(timestampValue);
            DateTime dateTime = timestampStandardDateTime.Add(ts);//加上时间戳值
            dateTime = dateTime.Add(TimeZoneInfo.Local.BaseUtcOffset);//加上时区时间偏移
            return dateTime;
        }
    }
}
