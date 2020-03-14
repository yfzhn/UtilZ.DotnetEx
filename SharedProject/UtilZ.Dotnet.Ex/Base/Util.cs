using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 通用扩展类
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 字符串参数验证
        /// </summary>
        /// <param name="para">参数</param>
        /// <param name="name">参数名</param>
        /// <param name="allowWhiteSpace">是否允许空白字符[true:允许;false:不允许]</param>
        /// <param name="allowEmpty">是否允许为空字符串[true:允许;false:不允许]</param>
        public static void ParaValidateNull(this string para, string name, bool allowWhiteSpace = false, bool allowEmpty = false)
        {
            if (allowWhiteSpace)
            {
                if (allowEmpty)
                {
                    if (para == null)
                    {
                        throw new ArgumentNullException(name);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(para))
                    {
                        throw new ArgumentNullException(name);
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(para))
                {
                    throw new ArgumentNullException(name);
                }
            }
        }

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="para">参数</param>
        /// <param name="name">参数名</param>
        public static void ParaValidateNull<T>(this T para, string name) where T : class
        {
            if (para == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// ConcurrentDictionary字典添加项[返回添加结果]
        /// </summary>
        /// <typeparam name="T">key类型</typeparam>
        /// <typeparam name="W">value类型</typeparam>
        /// <param name="dic">ConcurrentDictionary字典</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="repeatCount">添加失败重试次数</param>
        /// <returns>返回添加结果</returns>
        public static bool Add<T, W>(this ConcurrentDictionary<T, W> dic, T key, W value, int repeatCount = 0)
        {
            int currentRepeatCount = 0;
            bool ret = dic.TryAdd(key, value);
            while (!ret && currentRepeatCount++ < repeatCount)
            {
                System.Threading.Thread.Sleep(10);
                ret = dic.TryAdd(key, value);
            }

            return ret;
        }

        /// <summary>
        /// 获取永久时间片
        /// </summary>
        /// <returns>永久时间片</returns>
        public static TimeSpan GetForeverTimeSpan()
        {
            //将1000年当作为永久时间
            return new TimeSpan(365 * 1000, 0, 0, 0, 0);
        }
    }
}
