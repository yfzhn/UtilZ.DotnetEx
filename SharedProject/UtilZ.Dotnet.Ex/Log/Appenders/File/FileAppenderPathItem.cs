using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.Ex.Log
{
    internal class FileAppenderPathItem
    {
        /// <summary>
        /// 当前路径项是否是需要实时创建的路径[true:需要实时创建;false:固定路径]
        /// </summary>
        private readonly bool _isRealCreatePath;
        private readonly int _pathLength;
        private readonly string _datePattern;
        private readonly string _targetPath;
        private readonly int _datePatternIndex;
        private readonly int _datePatternLength;

        public FileAppenderPathItem(string path)
        {
            int begin = path.IndexOf(LogConstant.PATTERN_FALG_CHAR);
            if (begin > -1)
            {
                int end = path.LastIndexOf(LogConstant.PATTERN_FALG_CHAR);
                if (end < 0)
                {
                    throw new ArgumentException("日期匹配字符串无效");
                }

                int length = end - begin;
                string leftStr = path.Substring(0, begin);
                string rightStr = path.Substring(end + 1);
                this._datePatternIndex = begin;
                this._datePatternLength = end - begin - 1;
                string datePattern = path.Substring(begin + 1, this._datePatternLength);
                string str = DateTime.Now.ToString(datePattern);
                this._datePattern = datePattern;
                this._pathLength = path.Length - 2;
                this._targetPath = leftStr + "{0}" + rightStr;
                this._isRealCreatePath = true;
            }
            else
            {
                this._targetPath = path;
                this._pathLength = path.Length;
                this._isRealCreatePath = false;
            }
        }

        public string CreatePath()
        {
            string path;
            if (this._isRealCreatePath)
            {
                path = string.Format(this._targetPath, DateTime.Now.ToString(this._datePattern));
            }
            else
            {
                path = this._targetPath;
            }

            return path;
        }

        /// <summary>
        /// 检查日志文件路径是否是有效路径[有效返回true;无效返回false]
        /// </summary>
        /// <param name="path"></param>
        /// <param name="createTime"></param>
        /// <returns></returns>
        internal bool CheckPath(string path, out DateTime createTime)
        {
            createTime = default(DateTime);
            if (this._isRealCreatePath)
            {
                if (path.Length != this._pathLength)
                {
                    return false;
                }

                string timeStr = path.Substring(this._datePatternIndex, this._datePatternLength);
                return DateTime.TryParseExact(timeStr, this._datePattern, null, System.Globalization.DateTimeStyles.None, out createTime);
            }
            else
            {
                if (string.Equals(this._targetPath, path))
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
}
