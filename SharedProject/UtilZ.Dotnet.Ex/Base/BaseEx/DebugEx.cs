using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 调试辅助类
    /// </summary>
    public class DebugEx
    {
        private static readonly Dictionary<string, object> _objDic = new Dictionary<string, object>();
        /// <summary>
        /// 获取对象字典集合
        /// </summary>
        public static Dictionary<string, object> ObjDic
        {
            get { return _objDic; }
        }

        /// <summary>
        /// 添加一个调试对象
        /// </summary>
        /// <param name="key">对象key</param>
        /// <param name="obj">对象</param>
        public static void Add(string key, object obj)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_objDic.ContainsKey(key))
            {
                throw new ArgumentNullException($"重复的Key:{key}");
            }

            _objDic.Add(key, obj);
        }

        /// <summary>
        /// 移除一个调试对象
        /// </summary>
        /// <param name="key">对象key</param>
        /// <returns>移除结果</returns>
        public static bool Remove(string key)
        {
            if (_objDic.ContainsKey(key))
            {
                return _objDic.Remove(key);
            }
            else
            {
                return false;
            }
        }
    }
}
