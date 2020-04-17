using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 异步优先级队列
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class PriorityQueueThread<T> : IEnumerable<KeyValuePair<int, Queue<T>>>, IDisposable
    {
        private readonly IThreadEx _ext = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PriorityQueueThread()
        {
            _ext = new ThreadEx(this.ExcuteThreadMethod);
        }

        /// <summary>
        /// 线程等待通知对象
        /// </summary>
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        /// <summary>
        /// 优先级队列
        /// </summary>
        private readonly PriorityQueue<T> _priorityQueue = new PriorityQueue<T>();

        /// <summary>
        /// 数据处理委托
        /// </summary>
        public Action<T> DataProcessAction;

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="item">待处理数据项</param>
        private void OnRaiseDataProcess(T item)
        {
            if (this.DataProcessAction != null)
            {
                this.DataProcessAction(item);
            }
        }

        /// <summary>
        /// 重写执行线程方法
        /// </summary>
        /// <param name="para">线程参数</param>
        private void ExcuteThreadMethod(ThreadExPara para)
        {
            //执行数据处理方法
            long count = 0;

            while (true)
            {
                try
                {
                    //获取等待处理的数据项数
                    count = this._priorityQueue.Count;

                    //如果线程取消
                    if (para.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    //如果等待处理的数据项数数为0.则等待信号的输入
                    if (count == 0)
                    {
                        this._autoResetEvent.WaitOne();
                        continue;
                    }

                    //数据处理                   
                    this.OnRaiseDataProcess(this._priorityQueue.Dequeue());

                    //如果线程取消
                    if (para.Token.IsCancellationRequested)
                    {
                        break;
                    }
                }
                catch (ThreadAbortException taex)
                {
                    Loger.Info(taex.Message);
                }
                catch (Exception ex)
                {
                    Loger.Error(ex);
                }
            }
        }

        /// <summary>
        /// 启动输出
        /// </summary>
        public void Start()
        {
            this._ext.Start();
        }

        /// <summary>
        /// 停止输出
        /// </summary>
        public void Stop()
        {
            this._ext.Stop();
            this._autoResetEvent.Set();
        }

        /// <summary>
        /// 将对象添加到结尾处
        /// </summary>
        /// <param name="item">对象</param>
        /// <param name="priorityLevel">优先级,值越小优先级越高,越大越低</param>
        public void Enqueue(T item, int priorityLevel)
        {
            this._priorityQueue.Enqueue(priorityLevel, item);
            this._autoResetEvent.Set();
        }

        /// <summary>
        /// 移除队列中的前N项
        /// </summary>
        /// <param name="count">项数</param>
        public void RemoveRange(long count)
        {
            this._priorityQueue.RemoveRange(count);
        }

        /// <summary>
        /// 移除队列中的前N项
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        /// <param name="count">项数</param>
        public void RemoveRange(int priorityLevel, long count)
        {
            this._priorityQueue.RemoveRange(priorityLevel, count);
        }

        /// <summary>
        /// 清空全部队列
        /// </summary>
        public void Clear()
        {
            this._priorityQueue.Clear();
        }

        /// <summary>
        /// 清空指定优先级的队列
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        public void Clear(int priorityLevel)
        {
            this._priorityQueue.Clear(priorityLevel);
        }

        /// <summary>
        /// 队列项数
        /// </summary>
        public long Count
        {
            get { return this._priorityQueue.Count; }
        }

        /// <summary>
        /// 获取队列中指定优先级的元素数
        /// </summary>
        /// <param name="priorityLevel">优先级</param>
        /// <returns>元素数</returns>
        public int GetCountByPriority(int priorityLevel)
        {
            return this._priorityQueue.GetCountByPriority(priorityLevel);
        }

        /// <summary>
        /// 获取当前存储的优先级类型数
        /// </summary>
        public int PriorityCount
        {
            get { return this._priorityQueue.PriorityCount; }
        }

        #region IDisposable
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
            this._autoResetEvent.Dispose();
        }
        #endregion

        /// <summary>
        /// 返回循环的遍历枚举
        /// </summary>
        /// <returns>循环的遍历枚举</returns>
        public IEnumerator<KeyValuePair<int, Queue<T>>> GetEnumerator()
        {
            return this._priorityQueue.GetEnumerator();
        }

        /// <summary>
        /// 返回循环的遍历枚举
        /// </summary>
        /// <returns>循环的遍历枚举</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
