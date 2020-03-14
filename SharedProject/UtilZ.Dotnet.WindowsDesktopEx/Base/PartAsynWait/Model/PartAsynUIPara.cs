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
    [Serializable]
    public abstract class PartAsynUIPara
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
        private string _caption = string.Empty;

        /// <summary>
        /// 获取或设置标题
        /// </summary>
        public string Caption
        {
            get { return _caption; }
            set
            {
                this.AssetLock();
                _caption = value;
            }
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        private string _hint = string.Empty;

        /// <summary>
        /// 获取或设置提示信息
        /// </summary>
        public string Hint
        {
            get { return _hint; }
            set
            {
                this.AssetLock();
                _hint = value;
            }
        }

        private bool _isShowCancel = true;

        /// <summary>
        /// 获取或设置是否显示取消按钮
        /// </summary>
        public bool IsShowCancel
        {
            get { return _isShowCancel; }
            set
            {
                this.AssetLock();
                _isShowCancel = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynUIPara()
        {

        }
    }
}
