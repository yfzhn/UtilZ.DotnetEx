using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model
{
    /// <summary>
    /// 异步等待UI参数
    /// </summary>
    public abstract class PartAsynWaitParaAbs
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// 当前对象是否锁住
        /// </summary>
        private bool _isLock = false;

        /// <summary>
        /// 获取当前对象是否锁住
        /// </summary>
        public bool Islock
        {
            get
            {
                lock (this._monitor)
                {
                    return _isLock;
                }
            }
        }

        /// <summary>
        /// 锁住参数对象[true:锁成功;false:锁失败]
        /// </summary>
        /// <returns>锁结果</returns>
        internal bool Lock()
        {
            if (_isLock)
            {
                return false;
            }

            lock (this._monitor)
            {
                if (_isLock)
                {
                    return false;
                }

                _isLock = true;
                return true;
            }
        }

        /// <summary>
        /// 解锁参数对象[true:解锁成功;false:解锁失败]
        /// </summary>
        /// <returns>解锁结果</returns>
        internal bool UnLock()
        {

            lock (this._monitor)
            {
                if (_isLock)
                {
                    _isLock = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 断言当前对象被锁住
        /// </summary>
        protected void AssetLock()
        {
            if (this._isLock)
            {
                throw new Exception("对象已锁住,在本次执行异步等待操作完成前不允许修改");
            }
        }

        /// <summary>
        /// 线程同步对象
        /// </summary>
        public readonly object SyncRoot = new object();

        /// <summary>
        /// 一个异步等待
        /// </summary>
        private IPartAsynWait _asynWait = null;

        /// <summary>
        /// 获取或设置一个异步等待
        /// </summary>
        public IPartAsynWait AsynWait
        {
            get
            {
                return _asynWait;
            }
            internal set
            {
                this._asynWait = value;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        private string _title = string.Empty;

        /// <summary>
        /// 获取或设置标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                this.AssetLock();
                _title = value;
            }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        private string _message = string.Empty;

        /// <summary>
        /// 获取或设置提示信息
        /// </summary>
        public string Message
        {
            get { return _message; }
            set
            {
                this.AssetLock();
                _message = value;
            }
        }

        private bool _showCancel = true;

        /// <summary>
        /// 取消按钮可见性[true:显示;false:隐藏]
        /// </summary>
        public bool ShowCancel
        {
            get { return _showCancel; }
            set
            {
                this.AssetLock();
                _showCancel = value;
            }
        }

        private bool _immediatelyCompleted = false;
        /// <summary> 
        /// 点击取消按钮后,立即调用完成方法
        /// [true:当调用取消后,直接调用执行完成方法,执行操作的线程方法等待操作完成后再根据Token判断取消操作,跳出执行,该选项无执行结果;
        /// false:当调用取消后,不调用执行完成方法,直到后台线程等待操作完成后返回,再根据Token判断取消操作,最后才执行完成方法,该选项有执行结果]
        /// </summary>
        public bool ImmediatelyCompleted
        {
            get { return _immediatelyCompleted; }
            set
            {
                this.AssetLock();
                _immediatelyCompleted = value;
            }
        }

        /// <summary>
        /// 获取或设置包含有关控件的数据的对象
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynWaitParaAbs()
        {

        }
    }
}
