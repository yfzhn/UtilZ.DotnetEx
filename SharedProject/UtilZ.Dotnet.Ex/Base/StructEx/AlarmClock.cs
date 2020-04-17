using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 闹钟
    /// </summary>
    public class AlarmClock : IDisposable
    {
        /// <summary>
        /// 闹钟响铃时刻
        /// </summary>
        private class AlarmTime
        {
            /// <summary>
            /// 获取或设置时刻
            /// </summary>
            public TimeSpan Time { get; set; }
        }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public AlarmClock()
        {
            this._timingThread = new ThreadEx(this.TimingThreadMethod, "闹钟线程", true);
        }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="times">响铃的时刻集合</param>
        public AlarmClock(IEnumerable<TimeSpan> times)
            : this()
        {
            foreach (var ts in times)
            {
                this.AddTime(ts);
            }
        }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="times">响铃的时刻集合</param>
        /// <param name="excutCount">执行次数</param>
        public AlarmClock(IEnumerable<TimeSpan> times, int excutCount)
            : this(times)
        {
            this._count = excutCount;
        }

        /// <summary>
        /// 响铃时执行的Action操作
        /// </summary>
        public event EventHandler<RingArgs> Ring;

        /// <summary>
        /// 当响铃时调用
        /// </summary>
        /// <param name="ts">响铃的时刻</param>
        private void OnRing(TimeSpan ts)
        {
            if (this.Ring != null)
            {
                try
                {
                    this.Ring(this, new RingArgs { Time = ts });
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// 时间点集合
        /// </summary>
        private SortedDictionary<TimeSpan, AlarmTime> _sdTimes = new SortedDictionary<TimeSpan, AlarmTime>();

        /// <summary>
        /// 获取或设置定时的时间字符串,不带日期的,格式:12:12:12
        /// </summary>
        public List<TimeSpan> Times
        {
            get { return this._sdTimes.Keys.ToList(); }
        }

        /// <summary>
        /// 添加一个时间点
        /// </summary>
        /// <param name="ts">时间点</param>
        public void AddTime(TimeSpan ts)
        {
            lock (this)
            {
                if (this._timingThread.IsRuning)
                {
                    throw new Exception("闹钟处于工作状态时不能添加时间点");
                }
            }

            if (ts.TotalDays >= 1.0d)
            {
                throw new Exception(string.Format("值:{0},不是有效的时间点,时间点要在24小时内", ts));
            }

            if (this._sdTimes.ContainsKey(ts))
            {
                throw new Exception(string.Format("已存在时间点:{0}", ts));
            }

            this._sdTimes.Add(ts, new AlarmTime() { Time = ts });
        }

        /// <summary>
        /// 添加多个时间点
        /// </summary>
        /// <param name="tss">时刻点集合</param>
        public void AddTime(IEnumerable<TimeSpan> tss)
        {
            foreach (var timeStr in tss)
            {
                this.AddTime(timeStr);
            }
        }

        /// <summary>
        /// 执行的次数,-1为无限次
        /// </summary>
        private int _count = -1;
        /// <summary>
        /// 获取或设置执行的次数,-1为无限次
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                if (value < -1 || value == 0)
                {
                    throw new Exception(string.Format("{0}不是有效的次数", value));
                }

                _count = value;
            }
        }

        /// <summary>
        /// 闹钟执行线程
        /// </summary>
        private ThreadEx _timingThread = null;

        /// <summary>
        /// 创建时刻点环形链表
        /// </summary>
        /// <returns>时刻点环形链表</returns>
        private LoopLinked<AlarmTime> CreateAlarmLoopLinked()
        {
            LoopLinked<AlarmTime> alarmLoopLinked = new LoopLinked<AlarmTime>();
            foreach (var item in this._sdTimes.Values)
            {
                alarmLoopLinked.AddLast(item);
            }

            return alarmLoopLinked;
        }

        /// <summary>
        /// 定时操作
        /// </summary>
        /// <param name="para">线程参数</param>
        private void TimingThreadMethod(ThreadExPara para)
        {
            LoopLinked<AlarmTime> alarmLoopLinked = this.CreateAlarmLoopLinked();
            if (alarmLoopLinked.Count == 0)
            {
                throw new Exception("没有添加时间点");
            }

            // 当前执行次数
            int excuteCount = 0;
            LoopLinkedNode<AlarmTime> currentNode = alarmLoopLinked.FirstNode;
            TimeSpan tsWait;
            while (true)
            {
                tsWait = this.CaculateWaitTime(currentNode.Value.Time);
                //如果停止门铃执行
                if (para.Token.IsCancellationRequested)
                {
                    break;
                }

                Thread.Sleep(tsWait);

                //如果停止门铃执行
                if (para.Token.IsCancellationRequested)
                {
                    break;
                }

                //响铃
                this.OnRing(currentNode.Value.Time);

                //执行次数验证,如果不为无限次,那么当执行的次数超过要执行的总次数时,就停止
                if (this._count != -1)
                {
                    excuteCount++;
                    if (excuteCount >= this._count)
                    {
                        break;
                    }
                }

                currentNode = currentNode.Next;
            }
        }

        /// <summary>
        /// 计算到下次响铃的等待时间
        /// </summary>
        /// <param name="tsc">下一个要响铃的时刻</param>
        /// <returns>到下次响铃的等待时间</returns>
        private TimeSpan CaculateWaitTime(TimeSpan tsc)
        {
            DateTime dtc = DateTime.Now;
            DateTime dtn = new DateTime(dtc.Year, dtc.Month, dtc.Day, tsc.Hours, tsc.Minutes, tsc.Seconds, tsc.Milliseconds);
            if (dtn < dtc)//10:21:34<11:45:36
            {
                dtn = dtn.AddDays(1);
            }

            return dtn - dtc;
        }

        /// <summary>
        /// 启动闹钟
        /// </summary>
        public void Start()
        {
            this._timingThread.Start();
        }

        /// <summary>
        /// 停止闹钟
        /// </summary>
        public void Stop()
        {
            this._timingThread.Stop();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {
            this._timingThread.Dispose();
        }
    }

    /// <summary>
    /// 响铃事件参数
    /// </summary>
    public class RingArgs : EventArgs
    {
        /// <summary>
        /// 响铃时的时刻
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}
