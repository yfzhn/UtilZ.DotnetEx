using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 线程扩展类
    /// </summary>
    public sealed class ThreadEx : IThreadEx
    {
        /// <summary>
        /// 线程要执行的委托,无参数
        /// </summary>
        private readonly Action<CancellationToken> _action = null;

        /// <summary>
        /// 线程要执行的委托,带参数
        /// </summary>
        private readonly Action<CancellationToken, object> _actionObj = null;

        /// <summary>
        /// true:无参数;false:带参数
        /// </summary>
        private readonly bool _flag;

        /// <summary>
        /// 线程名称
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// 是否后台运行[true:后台线程;false:前台线程]
        /// </summary>
        private readonly bool _isBackground;

        /// <summary>
        /// 外部调用线程锁
        /// </summary>
        private readonly object _threadLock = new object();
        /// <summary>
        /// 执行线程
        /// </summary>
        private Thread _thread = null;
        /// <summary>
        /// 获取内部线程
        /// </summary>
        public Thread OwnerThread
        {
            get { return _thread; }
        }

        /// <summary>
        /// 当前线程是否正在运行[true:正在运行;false:未运行]
        /// </summary>
        private bool _runing = false;

        /// <summary>
        ///线程参数
        /// </summary>
        private ThreadStartPara _threadStartPara = null;

        private bool _disposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="flag">true:无参数;false:带参数</param>
        /// <param name="name">线程名称</param>
        /// <param name="isBackground">是否后台运行[true:后台线程;false:前台线程]</param>
        private ThreadEx(bool flag, string name, bool isBackground)
        {
            this._flag = flag;
            this._name = name;
            this._isBackground = isBackground;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">线程要执行的委托</param>
        /// <param name="name">线程名称</param>
        /// <param name="isBackground">后台运行标识[true:后台线程;false:前台线程]</param>
        public ThreadEx(Action<CancellationToken> action, string name = null, bool isBackground = true)
            : this(true, name, isBackground)
        {
            this._action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="action">线程要执行的委托</param>
        /// <param name="name">线程名称</param>
        /// <param name="isBackground">后台运行标识[true:后台线程;false:前台线程]</param>
        public ThreadEx(Action<CancellationToken, object> action, string name = null, bool isBackground = true)
            : this(false, name, isBackground)
        {
            this._actionObj = action ?? throw new ArgumentNullException(nameof(action));
        }




        #region IThreadEx
        /// <summary>
        /// 线程执行完成事件
        /// </summary>
        public event EventHandler<ThreadExCompletedArgs> Completed;

        /// <summary>
        /// 触发线程执行完成事件
        /// </summary>
        /// <param name="type">线程执行完成类型</param>
        /// <param name="ex">当执行异常可取消时可能的异常信息</param>
        private void OnRaiseCompleted(ThreadExCompletedType type, Exception ex = null)
        {
            var handler = this.Completed;
            handler?.Invoke(this, new ThreadExCompletedArgs(type, ex));
        }

        /// <summary>
        /// 获取线程当前的状态
        /// </summary>
        public System.Threading.ThreadState ThreadState
        {
            get
            {
                lock (this._threadLock)
                {
                    if (this._thread == null)
                    {
                        return System.Threading.ThreadState.Unstarted;
                    }
                    else
                    {
                        return this._thread.ThreadState;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前托管线程的唯一标识符
        /// </summary>
        public int ManagedThreadId
        {
            get
            {
                lock (this._threadLock)
                {
                    if (this._thread == null)
                    {
                        throw new Exception("线程未启动");
                    }
                    else
                    {
                        return this._thread.ManagedThreadId;
                    }
                }
            }
        }

        /// <summary>
        /// 当前线程是否正在运行
        /// </summary>
        public bool IsRuning
        {
            get
            {
                lock (this._threadLock)
                {
                    return this._runing;
                }
            }
        }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <param name="obj">线程启动参数</param>
        /// <param name="apartmentState">指定的单元状态 System.Threading.Thread</param>
        public void Start(object obj = null, ApartmentState apartmentState = ApartmentState.Unknown)
        {
            lock (this._threadLock)
            {
                if (this._disposed)
                {
                    throw new ObjectDisposedException(string.Empty, "对象已释放");
                }

                if (this._runing)
                {
                    return;
                }

                this._threadStartPara = new ThreadStartPara(obj);
                this._thread = new Thread(new ParameterizedThreadStart(this.ThreadExcuteMethod));
                this._thread.SetApartmentState(apartmentState);
                if (string.IsNullOrWhiteSpace(this._name))
                {
                    var st = new System.Diagnostics.StackTrace(1, true);
                    var sf = st.GetFrame(0);
                    var method = sf.GetMethod();
                    this._thread.Name = string.Format("{0}.{1}.{2}.{3}", sf.GetFileName(), sf.GetFileLineNumber(), method.DeclaringType.FullName, method.Name);
                }
                else
                {
                    this._thread.Name = this._name;
                }

                this._thread.IsBackground = this._isBackground;
                this._runing = true;
                this._thread.Start(this._threadStartPara);
            }
        }

        /// <summary>
        /// 线程执行方法
        /// </summary>
        /// <param name="obj">线程参数</param>
        private void ThreadExcuteMethod(object obj)
        {
            var threadStartPara = (ThreadStartPara)obj;
            try
            {
                var token = threadStartPara.Cts.Token;
                if (this._flag)
                {
                    this._action(token);
                }
                else
                {
                    this._actionObj(token, threadStartPara.Obj);
                }

                if (token.IsCancellationRequested)
                {
                    this.OnRaiseCompleted(ThreadExCompletedType.Cancel);
                }
                else
                {
                    this.OnRaiseCompleted(ThreadExCompletedType.Completed);
                }
            }
            catch (System.Threading.ThreadAbortException taex)
            {
                this.OnRaiseCompleted(ThreadExCompletedType.Cancel, taex);
            }
            catch (Exception ex)
            {
                this.OnRaiseCompleted(ThreadExCompletedType.Exception, ex);
            }
            threadStartPara.Set();
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <param name="sycn">是否同步调用停止方法,同步调用会等线程结束后才退出本方法[true:同步;false:异步]</param>
        /// <param name="synMillisecondsTimeout">同步超时时间,-1表示无限期等待,单位/毫秒[isSycn为true时有效]</param>
        public void Stop(bool sycn = false, int synMillisecondsTimeout = -1)
        {
            lock (this._threadLock)
            {
                if (this._runing)
                {
                    this._threadStartPara.Cancell();

                    if (sycn)
                    {
                        if (!this._threadStartPara.WaitOne(synMillisecondsTimeout))
                        {
                            //超时终止
                            this.PrimitiveAbort();
                        }
                    }

                    this._threadStartPara.Dispose();
                    this._threadStartPara = null;
                    this._thread = null;
                    this._runing = false;
                }
            }
        }



        private void PrimitiveAbort(object stateInfo = null)
        {
            lock (this._threadLock)
            {
                if (this._thread.ThreadState == ThreadState.Stopped || this._thread.ThreadState == ThreadState.StopRequested)
                {
                    return;
                }

                if (stateInfo != null)
                {
                    this._thread.Abort(stateInfo);
                }
                else
                {
                    this._thread.Abort();
                }
            }
        }

        /// <summary>
        /// 终止线程
        /// </summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
        public void Abort(object stateInfo = null)
        {
            lock (this._threadLock)
            {
                if (this._runing)
                {
                    this._threadStartPara.Cancell();
                    this.PrimitiveAbort(stateInfo);
                    this._threadStartPara.Dispose();
                    this._threadStartPara = null;
                    this._thread = null;
                    this._runing = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            try
            {
                lock (this._threadLock)
                {
                    if (this._disposed)
                    {
                        return;
                    }
                    this._disposed = true;

                    if (this._runing)
                    {
                        this._threadStartPara.Cancell();
                        this.PrimitiveAbort(null);
                        this._threadStartPara.Dispose();
                        this._threadStartPara = null;
                        this._thread = null;
                        this._runing = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }





        #region 静态方法
        /// <summary>
        /// 创建线程对象
        /// </summary>
        /// <param name="action">线程要执行的委托</param>
        /// <param name="name">线程名称</param>
        /// <param name="isBackground">是否后台运行[true:后台线程;false:前台线程]</param>
        /// <returns>返回线程对象</returns>
        public static IThreadEx Start(Action<CancellationToken> action, string name = null, bool isBackground = true)
        {
            var ext = new ThreadEx(action, name, isBackground);
            ext.Start();
            return ext;
        }

        /// <summary>
        /// 创建线程对象
        /// </summary>
        /// <param name="action">线程要执行的委托</param>
        /// <param name="obj">线程启动参数</param>
        /// <param name="name">线程名称</param>
        /// <param name="isBackground">是否后台运行[true:后台线程;false:前台线程]</param>
        /// <returns>返回线程对象</returns>
        public static IThreadEx Start(Action<CancellationToken, object> action, object obj, string name = null, bool isBackground = true)
        {
            var ext = new ThreadEx(action, name, isBackground);
            ext.Start(obj);
            return ext;
        }

        ///// <summary>
        ///// win32方式指定当前线程运行在指定CPU核心上
        ///// </summary>
        ///// <param name="coreID">指定CPU核心ID</param>
        ///// <returns>设置结果</returns>
        //public static UIntPtr AssignCoreRun(uint coreID)
        //{
        //    //return NativeMethods.SetThreadAffinityMask(NativeMethods.GetCurrentThread(), new UIntPtr(SetCpuID(coreNum)));
        //    return NativeMethods.SetThreadAffinityMask(NativeMethods.GetCurrentThread(), new UIntPtr(coreID));
        //}

        /// <summary>
        /// .net方式指定当前线程运行在指定CPU核心上[多个核心间切换运行,不像win32方式是在一个核心上运行]
        /// </summary>
        /// <param name="threadID">线程ID</param>
        /// <param name="idealProcessor">首选处理器</param>
        /// <param name="coreID">目标处理器(Power(2,0-4]之间的单值或或位运算值)</param>
        public static void AssignCoreRun(int threadID, int idealProcessor, int coreID)
        {
            foreach (System.Diagnostics.ProcessThread proThreadItem in System.Diagnostics.Process.GetCurrentProcess().Threads)
            {
                if (threadID == proThreadItem.Id)
                {
                    proThreadItem.IdealProcessor = idealProcessor;
                    proThreadItem.ProcessorAffinity = (IntPtr)coreID;
                }
            }
        }

        //static ulong SetCpuID(int id)
        //{
        //    ulong cpuid = 0;
        //    if (id < 0 || id >= System.Environment.ProcessorCount)
        //    {
        //        id = 0;
        //    }

        //    cpuid |= 1UL << id;

        //    return cpuid;
        //}

        /// <summary>
        /// 设置线程是否为后台线程
        /// </summary>
        /// <param name="thread">要设置的线程</param>
        /// <param name="isBackground">true:后台线程;false:前台线程</param>
        public static void SetThreadIsBackground(Thread thread, bool isBackground)
        {
            if (thread == null)
            {
                return;
            }

            thread.IsBackground = isBackground;
        }
        #endregion
    }









    /// <summary>
    /// 线程启动参数
    /// </summary>
    public sealed class ThreadStartPara : IDisposable
    {
        /// <summary>
        /// 参数对象
        /// </summary>
        public object Obj { get; private set; }

        private readonly CancellationTokenSource _cts;
        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        public CancellationTokenSource Cts
        {
            get { return _cts; }
        }

        private readonly AutoResetEvent _syncStopEventHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="obj">参数对象</param>
        public ThreadStartPara(object obj)
        {
            this.Obj = obj;
            this._cts = new CancellationTokenSource();
            this._syncStopEventHandler = new AutoResetEvent(false);
        }


        private bool _cancel = false;
        private bool _disposed = false;

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (this._disposed)
                {
                    return;
                }
                this._disposed = true;

                if (!this._cancel)
                {
                    this._cts.Cancel();
                }

                this._cts.Dispose();
                this._syncStopEventHandler.Dispose();
            }
        }

        /// <summary>
        /// 取消执行
        /// </summary>
        public void Cancell()
        {
            lock (this)
            {
                if (this._disposed)
                {
                    return;
                }

                this._cts.Cancel();
                this._cancel = true;
            }
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="millisecondsTimeout">等待时长,毫秒</param>
        /// <returns></returns>
        public bool WaitOne(int millisecondsTimeout)
        {
            try
            {
                if (this._disposed)
                {
                    return false;
                }

                return this._syncStopEventHandler.WaitOne(millisecondsTimeout);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送信息通知
        /// </summary>
        public void Set()
        {
            try
            {
                if (this._disposed)
                {
                    return;
                }

                this._syncStopEventHandler.Set();
            }
            catch (ObjectDisposedException)
            {

            }
        }
    }



    /// <summary>
    /// 扩展线程接口
    /// </summary>
    public interface IThreadEx : IDisposable
    {
        /// <summary>
        /// 获取内部线程
        /// </summary>
        Thread OwnerThread { get; }

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <param name="obj">线程启动参数</param>
        /// <param name="apartmentState">指定的单元状态 System.Threading.Thread</param>
        void Start(object obj = null, ApartmentState apartmentState = ApartmentState.Unknown);

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <param name="isSycn">是否同步调用停止方法,同步调用会等线程结束后才退出本方法[true:同步;false:异步]</param>
        /// <param name="synMillisecondsTimeout">同步超时时间,-1表示无限期等待,单位/毫秒</param>
        void Stop(bool isSycn = false, int synMillisecondsTimeout = -1);

        /// <summary>
        /// 终止线程
        /// </summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
        void Abort(object stateInfo = null);

        /// <summary>
        /// 线程执行完成事件
        /// </summary>
        event EventHandler<ThreadExCompletedArgs> Completed;

        /// <summary>
        /// 获取线程当前的状态
        /// </summary>
        System.Threading.ThreadState ThreadState { get; }

        /// <summary>
        /// 获取当前托管线程的唯一标识符
        /// </summary>
        int ManagedThreadId { get; }

        /// <summary>
        /// 当前线程是否正在运行
        /// </summary>
        bool IsRuning { get; }
    }



    /// <summary>
    /// 线程执行完成事件参数
    /// </summary>
    public class ThreadExCompletedArgs : EventArgs
    {
        /// <summary>
        /// 线程执行完成类型
        /// </summary>
        public ThreadExCompletedType Type { get; private set; }

        /// <summary>
        /// 当执行异常可取消时可能的异常信息
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">线程执行完成类型</param>
        /// <param name="ex">当执行异常可取消时可能的异常信息</param>
        public ThreadExCompletedArgs(ThreadExCompletedType type, Exception ex)
        {
            this.Type = type;
            this.Exception = ex;
        }
    }



    /// <summary>
    /// 线程执行完成类型
    /// </summary>
    public enum ThreadExCompletedType
    {
        /// <summary>
        /// 完成
        /// </summary>
        Completed,

        /// <summary>
        /// 异常
        /// </summary>
        Exception,

        /// <summary>
        /// 取消
        /// </summary>
        Cancel
    }
}
