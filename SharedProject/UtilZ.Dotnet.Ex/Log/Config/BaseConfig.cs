using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 基础配置
    /// </summary>
    public abstract class BaseConfig
    {
        #region 基础配置
        /// <summary>
        /// 日志追加器名称
        /// </summary>
        public string AppenderName { get; set; } = null;

        private string _layout = null;
        /// <summary>
        /// 日志布局[%d %l %e %c 堆栈:%s]
        /// </summary>
        public string Layout
        {
            get { return _layout; }
            set
            {
                _layout = value;
                this.UpdateLogLayout();
            }
        }

        private string _dateFormat = LogConstant.DateTimeFormat;
        /// <summary>
        /// 时间格式[yyyy-MM-dd HH:mm:ss]
        /// </summary>
        public string DateFormat
        {
            get { return _dateFormat; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _dateFormat = LogConstant.DateTimeFormat;
                }
                else
                {
                    DateTime.Now.ToString(value);
                    _dateFormat = value;
                }
            }
        }

        private string _levelMap = null;
        /// <summary>
        /// 日志级别名称映射[Info:信息;Warn:warning;...]
        /// </summary>
        public string LevelMap
        {
            get { return _levelMap; }
            set
            {
                if (string.Equals(_levelMap, value))
                {
                    return;
                }

                _levelMap = value;
                this.UpdateLogLevelMapDic();
            }
        }

        private int _separatorCount = 140;
        /// <summary>
        /// 分隔线长度
        /// </summary>
        public int SeparatorCount
        {
            get { return _separatorCount; }
            set
            {
                if (value < 0 || value > 1000)
                {
                    return;
                }

                _separatorCount = value;
                this.UpdateLogLayout();
            }
        }

        /// <summary>
        /// 是否启用日志输出缓存[true:启用;false:禁用]
        /// </summary>
        public bool EnableOutputCache { get; set; } = false;
        #endregion

        #region 过滤
        /// <summary>
        /// 是否启用日志追加器
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 过滤日志级别
        /// </summary>
        public LogLevel[] Levels { get; set; } = null;

        /// <summary>
        /// 事件ID最小值(包含该值,默认值为不限)
        /// </summary>
        public int EventIdMin { get; set; } = LogConstant.DEFAULT_EVENT_ID;

        /// <summary>
        /// 事件ID最大值(包含该值,默认值为不限)
        /// </summary>
        public int EventIdMax { get; set; } = LogConstant.DEFAULT_EVENT_ID;

        /// <summary>
        /// 消息匹配指定的字符串才被记录,为空或null不匹配(默认为null)
        /// </summary>
        public string MatchString { get; set; } = null;

        /// <summary>
        /// 要记录的异常的类型为指定类型或其子类才被记录,为null不匹配(默认为null)
        /// </summary>
        public Type MatchExceptionType { get; set; } = null;
        #endregion



        /// <summary>
        /// 日志级别名称映射字典集合
        /// </summary>
        private Dictionary<LogLevel, string> _levelMapDic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public BaseConfig(XElement ele)
        {
            if (ele == null)
            {
                this.UpdateLogLayout();
                return;
            }

            this.AppenderName = LogUtil.GetAttributeValue(ele, "name");

            bool enable;
            if (bool.TryParse(LogUtil.GetAttributeValue(ele, nameof(this.Enable).ToLower()), out enable))
            {
                this.Enable = enable;
            }

            this._layout = LogUtil.GetChildXElementValue(ele, nameof(this.Layout));
            try
            {
                this.DateFormat = LogUtil.GetChildXElementValue(ele, nameof(this.DateFormat));
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog(this, ex);
            }

            this.LevelMap = LogUtil.GetChildXElementValue(ele, nameof(this.LevelMap));

            int separatorCount;
            if (int.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.SeparatorCount)), out separatorCount))
            {
                this._separatorCount = separatorCount;
            }

            bool enableOutputCache;
            if (bool.TryParse(LogUtil.GetAttributeValue(ele, nameof(this.EnableOutputCache)), out enableOutputCache))
            {
                this.EnableOutputCache = enableOutputCache;
            }

            string levels = LogUtil.GetChildXElementValue(ele, nameof(this.Levels)).Trim();
            if (!string.IsNullOrWhiteSpace(levels))
            {
                string[] levelStrs = levels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var logLevels = new List<LogLevel>();
                LogLevel level;
                foreach (var levelStr in levelStrs)
                {
                    if (Enum.TryParse<LogLevel>(levelStr, true, out level))
                    {
                        logLevels.Add(level);
                    }
                }

                this.Levels = logLevels.ToArray();
            }

            int eventId;
            if (int.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.EventIdMin)), out eventId))
            {
                if (eventId < LogConstant.DEFAULT_EVENT_ID)
                {
                    eventId = LogConstant.DEFAULT_EVENT_ID;
                }

                this.EventIdMin = eventId;
            }

            if (int.TryParse(LogUtil.GetChildXElementValue(ele, nameof(this.EventIdMax)), out eventId))
            {
                if (eventId < LogConstant.DEFAULT_EVENT_ID)
                {
                    eventId = LogConstant.DEFAULT_EVENT_ID;
                }

                this.EventIdMax = eventId;
            }

            this.MatchString = LogUtil.GetChildXElementValue(ele, nameof(this.MatchString));
            string matchExceptionTypeName = LogUtil.GetChildXElementValue(ele, nameof(this.MatchExceptionType)).Trim();
            if (!string.IsNullOrWhiteSpace(matchExceptionTypeName))
            {
                try
                {
                    this.MatchExceptionType = LogUtil.GetType(matchExceptionTypeName);
                }
                catch (Exception ex)
                {
                    LogSysInnerLog.OnRaiseLog(this, ex);
                }
            }

            this.UpdateLogLayout();
        }

        private void UpdateLogLevelMapDic()
        {
            string levelMap = this._levelMap;
            //levelMap=>Info:信息;Warn:warning;...
            if (string.IsNullOrWhiteSpace(levelMap))
            {
                return;
            }

            string[] levelMapStrArr = levelMap.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var splitChs = new char[] { ':' };
            string[] keyValue;
            LogLevel logLevel;
            Type logLevelType = typeof(LogLevel);
            var levelMapDic = new Dictionary<LogLevel, string>();

            foreach (var levelMapStr in levelMapStrArr)
            {
                keyValue = levelMapStr.Split(splitChs, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length != 2)
                {
                    continue;
                }

                if (Enum.TryParse<LogLevel>(keyValue[0], true, out logLevel))
                {
                    levelMapDic[logLevel] = keyValue[1];
                }
            }

            this._levelMapDic = levelMapDic;
        }

        private void UpdateLogLayout()
        {
            string layoutFormat = this._layout;
            if (string.IsNullOrWhiteSpace(layoutFormat))
            {
                //如果日志布局格式为空则采用默认日志布局
                //layoutFormat = string.Format("时间:{0}\r\n级别:{1}\r\n线程:{2}\r\n事件ID:{3}\r\n日志:{4}\r\n堆栈:{5}", LogConstant.TIME, LogConstant.LEVEL, LogConstant.THREAD, LogConstant.EVENT, LogConstant.CONTENT, LogConstant.STACKTRACE);
                layoutFormat = string.Format("{0} {1} {2} 堆栈:{3}", LogConstant.TIME, LogConstant.LEVEL, LogConstant.CONTENT, LogConstant.STACKTRACE);
                //layoutFormat = string.Format("{0} {1} {2}", LogConstant.TIME, LogConstant.LEVEL, LogConstant.CONTENT);
            }

            //是否显示分隔线
            int separatorCount = this._separatorCount;
            if (separatorCount > 1)
            {
                layoutFormat = string.Format("{0}\r\n{1}", new string('-', separatorCount), layoutFormat);
            }

            this._layout = layoutFormat;
        }

        internal string GetLogLevelName(LogLevel level)
        {
            var levelMapDic = this._levelMapDic;
            string levelName;
            if (levelMapDic != null && levelMapDic.ContainsKey(level))
            {
                levelName = levelMapDic[level];
            }
            else
            {
                levelName = LogConstant.GetLogLevelName(level);
            }

            return levelName;
        }
    }
}
