using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model
{
    /// <summary>
    /// 异步等待执行参数
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    [Serializable]
    public class PartAsynWaitPara<T, TResult> : PartAsynWaitParaAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynWaitPara()
            : base()
        {

        }

        /// <summary>
        /// 异步等待框背景色
        /// </summary>
        //private System.Drawing.Color _asynWaitBackground = System.Drawing.SystemColors.Control;
        //private System.Drawing.Color _asynWaitBackground = System.Drawing.Color.White;
        private object _asynWaitBackground = null;

        /// <summary>
        /// 获取或设置异步等待框背景色
        /// </summary>
        public object AsynWaitBackground
        {
            get { return this._asynWaitBackground; }
            set
            {
                if (this._asynWaitBackground == value)
                {
                    return;
                }

                this._asynWaitBackground = value;
            }
        }

        /// <summary>
        /// 异步委托执行参数
        /// </summary>
        private T _para = default(T);

        /// <summary>
        /// 获取或设置异步委托执行参数
        /// </summary>
        public T Para
        {
            get { return _para; }
            set
            {
                // 断言当前对象被锁住
                this.AssetLock();
                _para = value;
            }
        }

        /// <summary>
        /// 要执行的操作
        /// </summary>
        private Func<PartAsynFuncPara<T>, TResult> _function = null;

        /// <summary>
        /// 获取或设置要执行的操作
        /// </summary>
        public Func<PartAsynFuncPara<T>, TResult> Function
        {
            get { return _function; }
            set
            {
                // 断言当前对象被锁住
                this.AssetLock();

                _function = value;
            }
        }

        /// <summary>
        /// 执行完成后通知
        /// </summary>
        private Action<PartAsynExcuteResult<T, TResult>> _completed = null;

        /// <summary>
        /// 获取或设置执行完成后通知
        /// </summary>
        public Action<PartAsynExcuteResult<T, TResult>> Completed
        {
            get { return _completed; }
            set
            {
                // 断言当前对象被锁住
                this.AssetLock();
                _completed = value;
            }
        }
    }
}
