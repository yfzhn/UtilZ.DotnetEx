using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Log
{
    /// <summary>
    /// 重定向输出中心
    /// </summary>
    public sealed class RedirectOuputCenter
    {
        static RedirectOuputCenter()
        {
            _logOutputThread = new Thread(LogOutputThreadMethod);
            _logOutputThread.IsBackground = true;
            _logOutputThread.Name = "日志输出中心.日志输出线程";
            _logOutputThread.Start();
        }

        #region 日志输出线程
        /// <summary>
        /// 日志输出线程
        /// </summary>
        private static readonly Thread _logOutputThread;

        /// <summary>
        /// 线程取消通知对象
        /// </summary>
        private static readonly CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// 日志输出线程同步对象
        /// </summary>
        private static readonly AutoResetEvent _logOutputAutoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 日志输出队列
        /// </summary>
        private static readonly ConcurrentQueue<RedirectOuputItem> _logOutputQueue = new ConcurrentQueue<RedirectOuputItem>();

        /// <summary>
        /// 日志输出线程方法
        /// </summary>
        /// <param name="obj">参数</param>
        private static void LogOutputThreadMethod(object obj)
        {
            var token = _cts.Token;
            RedirectOuputItem item;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_logOutputQueue.Count == 0)
                    {
                        _logOutputAutoResetEvent.WaitOne();
                    }

                    if (_logOutputQueue.Count == 0)
                    {
                        continue;
                    }

                    if (_logOutputQueue.TryDequeue(out item))
                    {
                        LogOutput(item);
                    }
                }
                catch (Exception ex)
                {
                    LogSysInnerLog.OnRaiseLog(null, ex);
                }
            }
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="logItem"></param>
        private static void LogOutput(RedirectOuputItem logItem)
        {
            RedirectOutputChannel[] redirectOutputChannelArr;
            lock (_logOutputSubscribeItemListMonitor)
            {
                redirectOutputChannelArr = _logOutputSubscribeItemList.ToArray();
            }

            foreach (var redirectOutputChannel in redirectOutputChannelArr)
            {
                redirectOutputChannel.OnRaiseLogOutput(logItem);
            }
        }
        #endregion

        #region 日志输出订阅
        /// <summary>
        /// 日志输出订阅项集合
        /// </summary>
        private static readonly List<RedirectOutputChannel> _logOutputSubscribeItemList = new List<RedirectOutputChannel>();

        /// <summary>
        /// 日志输出订阅项集合线程锁
        /// </summary>
        private static readonly object _logOutputSubscribeItemListMonitor = new object();

        /// <summary>
        /// 获取日志输出订阅项数组集合
        /// </summary>
        public static RedirectOutputChannel[] RedirectOutputChannelArray
        {
            get
            {
                lock (_logOutputSubscribeItemListMonitor)
                {
                    return _logOutputSubscribeItemList.ToArray();
                }
            }
        }

        /// <summary>
        /// 添加日志输出订阅项
        /// </summary>
        /// <param name="item">日志输出订阅项</param>
        public static void Add(RedirectOutputChannel item)
        {
            if (item == null)
            {
                return;
            }

            lock (_logOutputSubscribeItemListMonitor)
            {
                if (!_logOutputSubscribeItemList.Contains(item))
                {
                    _logOutputSubscribeItemList.Add(item);
                }
            }
        }

        /// <summary>
        /// 移除日志输出订阅项
        /// </summary>
        /// <param name="item">日志输出订阅项</param>
        public static void Remove(RedirectOutputChannel item)
        {
            if (item == null)
            {
                return;
            }

            lock (_logOutputSubscribeItemListMonitor)
            {
                if (_logOutputSubscribeItemList.Contains(item))
                {
                    _logOutputSubscribeItemList.Remove(item);
                }
            }
        }

        /// <summary>
        /// 清空日志输出订阅项
        /// </summary>
        public static void Clear()
        {
            lock (_logOutputSubscribeItemListMonitor)
            {
                _logOutputSubscribeItemList.Clear();
            }
        }
        #endregion

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="appenderName">日志追加器名称</param>
        /// <param name="logItem">日志项</param>
        internal static void Output(string appenderName, LogItem logItem)
        {
            _logOutputQueue.Enqueue(new RedirectOuputItem(appenderName, logItem));
            _logOutputAutoResetEvent.Set();
        }
    }
}
