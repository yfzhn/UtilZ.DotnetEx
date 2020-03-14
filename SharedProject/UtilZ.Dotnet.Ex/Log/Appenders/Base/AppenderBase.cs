using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 日志追加器基类
    /// </summary>
    public abstract class AppenderBase : IDisposable
    {
        private string _name = null;
        /// <summary>
        /// 日志追加器名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }

        /// <summary>
        /// 配置对象
        /// </summary>
        protected readonly BaseConfig _config;

        /// <summary>
        /// 获取配置对象
        /// </summary>
        public BaseConfig Config
        {
            get
            {
                return _config;
            }
        }

        /// <summary>
        /// 日志写线程队列
        /// </summary>
        private LogAsynQueue<LogItem> _logWriteQueue = null;

        #region 构造函数-初始化
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ele">配置元素</param>
        public AppenderBase(XElement ele)
        {
            this._config = this.CreateConfig(ele);
            this.Init();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置对象</param>
        public AppenderBase(BaseConfig config)
        {
            if (config == null)
            {
                config = this.CreateConfig(null);
            }

            this._config = config;
            this.Init();
        }

        private void Init()
        {
            if (this._config != null && this._config.EnableOutputCache)
            {
                this._logWriteQueue = new LogAsynQueue<LogItem>(this.PrimitiveWriteLog, string.Format("{0}日志输出线程", this._config.AppenderName));
            }
        }


        #endregion

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="item">日志项</param>
        protected abstract void PrimitiveWriteLog(LogItem item);

        /// <summary>
        /// 创建配置对象实例
        /// </summary>
        /// <param name="ele">配置元素</param>
        /// <returns>配置对象实例</returns>
        protected abstract BaseConfig CreateConfig(XElement ele);

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="item">日志项</param>
        public void WriteLog(LogItem item)
        {
            if (this._config.EnableOutputCache)
            {
                this._logWriteQueue.Enqueue(item);
            }
            else
            {
                this.PrimitiveWriteLog(item);
            }
        }

        /// <summary>
        /// 验证日志是否允许输出[返回值true:允许输出;false:丢弃]
        /// </summary>
        /// <param name="config">配置</param>
        /// <param name="item">要验证输出的日志项</param>
        /// <returns>true:允许输出;false:丢弃</returns>
        protected virtual bool Validate(BaseConfig config, LogItem item)
        {
            if (item == null)
            {
                return false;
            }

            if (!config.Enable)
            {
                return false;
            }

            if (config.Levels != null && !config.Levels.Contains(item.Level))
            {
                return false;
            }

            if (config.EventIdMin != LogConstant.DEFAULT_EVENT_ID && item.EventID < config.EventIdMin)
            {
                return false;
            }

            if (config.EventIdMax != LogConstant.DEFAULT_EVENT_ID && item.EventID > config.EventIdMax)
            {
                return false;
            }

            var matchString = config.MatchString;
            if (!string.IsNullOrEmpty(matchString) &&
                !string.IsNullOrEmpty(item.Content) &&
                !Regex.IsMatch(item.Content, matchString))
            {
                return false;
            }

            var matchExceptionType = config.MatchExceptionType;
            if (matchExceptionType != null && item.Exception != null)
            {
                Type exType = item.Exception.GetType();
                if (exType != matchExceptionType && !exType.IsSubclassOf(matchExceptionType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 布局一条日志文本记录
        /// </summary>
        /// <param name="item">日志信息对象</param>
        /// <returns>日志文本记录</returns>
        protected string LayoutLog(LogItem item)
        {
            string logMsg = string.Empty;
            try
            {
                string layoutFormat = this._config.Layout;
                List<object> args = new List<object>();
                int index = 0;
                string tmp;

                //时间
                if (layoutFormat.Contains(LogConstant.TIME))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.TIME, string.Format("{{{0}}}", index++));
                    tmp = item.Time.ToString(this._config.DateFormat);
                    args.Add(tmp);
                }

                //日志级别
                if (layoutFormat.Contains(LogConstant.LEVEL))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.LEVEL, string.Format("{{{0}}}", index++));
                    args.Add(this._config.GetLogLevelName(item.Level));
                }

                //事件ID
                if (layoutFormat.Contains(LogConstant.EVENT))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.EVENT, string.Format("{{{0}}}", index++));
                    args.Add(item.EventID);
                }

                if (layoutFormat.Contains(LogConstant.TAG))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.TAG, string.Format("{{{0}}}", index++));
                    args.Add(item.Tag);
                }

                //线程
                if (layoutFormat.Contains(LogConstant.THREAD))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.THREAD, string.Format("{{{0}}}", index++));
                    if (string.IsNullOrWhiteSpace(item.ThreadName))
                    {
                        tmp = item.ThreadID.ToString();
                    }
                    else
                    {
                        tmp = item.ThreadName;
                    }

                    args.Add(tmp);
                }

                //内容
                if (layoutFormat.Contains(LogConstant.CONTENT))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.CONTENT, string.Format("{{{0}}}", index++));
                    args.Add(item.Content);
                }

                //堆栈位置信息
                if (layoutFormat.Contains(LogConstant.STACKTRACE))
                {
                    layoutFormat = layoutFormat.Replace(LogConstant.STACKTRACE, string.Format("{{{0}}}", index++));
                    args.Add(item.StackTraceInfo);
                }

                //生成日志
                if (args.Count > 0)
                {
                    logMsg = string.Format(layoutFormat, args.ToArray());
                }
                else
                {
                    logMsg = item.Message;
                }
            }
            catch (Exception ex)
            {
                LogSysInnerLog.OnRaiseLog("LayoutManager", ex);
                logMsg = item.ToString();
            }

            return logMsg;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._name == null ? base.ToString() : this._name;
        }

        /// <summary>
        /// 构造函数释放非托管资源
        /// </summary>
        ~AppenderBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放资源标识</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this._logWriteQueue != null)
            {
                this._logWriteQueue.Dispose();
            }
        }
    }
}
