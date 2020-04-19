using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.WindowsDesktopEx.AsynWait;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute
{
    /// <summary>
    /// 执行异步等待基类
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    public abstract class AsynExcuteAbs<T, TContainer, TResult> : IAsynExcute<T, TContainer, TResult>, IAsynExcuteCancell where TContainer : class
    {
        /// <summary>
        /// 异步执行线程
        /// </summary>
        private Thread _asynExcuteThread = null;

        /// <summary>
        /// 异步执行线程取消对象
        /// </summary>
        private CancellationTokenSource _asynExcuteThreadCts = null;

        /// <summary>
        /// 异步等待执行参数
        /// </summary>
        protected PartAsynWaitPara<T, TResult> _asynWaitPara = null;

        /// <summary>
        /// 是否已执行完成
        /// </summary>
        private bool _excuteCompleted = false;
        private readonly object _excuteCompletedLock = new object();

        /// <summary>
        /// 获取包含有关控件的数据的对象
        /// </summary>
        public object Tag
        {
            get
            {
                var asynWaitPara = _asynWaitPara;
                return asynWaitPara == null ? null : asynWaitPara.Tag;
            }
        }



        /// <summary>
        /// 构造函数
        /// </summary>
        protected AsynExcuteAbs()
        {

        }

        /// <summary>
        /// 执行异步委托
        /// </summary>
        /// <param name="asynWaitPara">异步等待执行参数</param>
        /// <param name="containerControl">容器控件</param>
        public abstract void Excute(Model.PartAsynWaitPara<T, TResult> asynWaitPara, TContainer containerControl);

        /// <summary>
        /// 断言对象类型是IAsynWait和UserControl的子类对象类型
        /// </summary>
        /// <param name="value">要断言的对象类型</param>
        /// <param name="asynControlType">异步等待控件基类型</param>
        protected static void AssertIAsynWait(Type value, Type asynControlType)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type iShadeType = typeof(IPartAsynWait);
            if (value.GetInterface(iShadeType.Name) == null)
            {
                throw new Exception(string.Format("类型:{0}没有实现接口:{1}", value.Name, iShadeType.Name));
            }

            if (!value.IsSubclassOf(asynControlType))
            {
                throw new Exception(string.Format("类型:{0}不是{1}的子类型", value.Name, asynControlType.FullName));
            }
        }

        /// <summary>
        /// 根据异步等待遮罩层类型创建遮罩层
        /// </summary>
        /// <param name="shadeType">异步等待遮罩层类</param>
        /// <param name="para">异步等待UI参数</param>
        /// <returns>异步等待遮罩层类型创建遮罩层</returns>
        protected IPartAsynWait CreateAsynWaitShadeControl(Type shadeType, PartAsynWaitParaAbs para)
        {
            if (shadeType == null)
            {
                throw new Exception("没有指定自定义异步等待遮罩层类型");
            }

            IPartAsynWait ishade = (IPartAsynWait)Activator.CreateInstance(shadeType);
            ishade.Caption = para.Caption;
            ishade.Hint = para.Hint;
            ishade.IsShowCancel = para.IsShowCancel;
            return ishade;
        }

        /// <summary>
        /// 启动执行线程
        /// </summary>
        protected void StartAsynExcuteThread()
        {
            lock (this._excuteCompletedLock)
            {
                AsynExcuteCancellHelper.AddAsynExcuteCancell(this);
                this._excuteCompleted = false;

                //取消执行委托
                this._asynWaitPara.AsynWait.Canceled += CancellExcute;

                //启动滚动条动画
                this._asynWaitPara.AsynWait.StartAnimation();

                this._asynExcuteThreadCts = new CancellationTokenSource();
                this._asynExcuteThread = new Thread(this.AsynExcuteThreadMethod);
                this._asynExcuteThread.IsBackground = true;
                this._asynExcuteThread.Name = "UI异步执行线程";
                this._asynExcuteThread.Start();
            }
        }

        /// <summary>
        /// UI异步执行线程方法
        /// </summary>
        private void AsynExcuteThreadMethod()
        {
            TResult result = default(TResult);
            PartAsynExcuteStatus excuteStatus;
            Exception excuteEx = null;
            try
            {
                var token = this._asynExcuteThreadCts.Token;
                var function = this._asynWaitPara.Function;
                if (function != null)
                {
                    result = function(new PartAsynFuncPara<T>(this._asynWaitPara.Para, token, this._asynWaitPara.AsynWait));
                }
                
                if (token.IsCancellationRequested)
                {
                    excuteStatus = PartAsynExcuteStatus.Cancel;
                }
                else
                {
                    excuteStatus = PartAsynExcuteStatus.Completed;
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                excuteStatus = PartAsynExcuteStatus.Exception;
                excuteEx = ex;
            }

            lock (this._excuteCompletedLock)
            {
                if (this._excuteCompleted)
                {
                    return;
                }
                this._excuteCompleted = true;

                this.ExcuteCompleted(result, excuteStatus, excuteEx);
            }
        }

        private void ExcuteCompleted(TResult result, PartAsynExcuteStatus excuteStatus, Exception excuteEx)
        {
            this.ReleaseResource();
            var asynExcuteResult = new PartAsynExcuteResult<T, TResult>(this._asynWaitPara.Para, excuteStatus, result, excuteEx);
            this.OnRaiseCompleted(asynExcuteResult);
        }

        private void OnRaiseCompleted(PartAsynExcuteResult<T, TResult> asynExcuteResult)
        {
            if (this._asynWaitPara.AsynWait.InvokeRequired)
            {
                this._asynWaitPara.AsynWait.Invoke(new Action(() =>
                {
                    this.OnRaiseCompleted(asynExcuteResult);
                }));
            }
            else
            {
                var endAction = this._asynWaitPara.Completed;
                if (endAction != null)
                {
                    endAction(asynExcuteResult);
                }
            }
        }




        /// <summary>
        /// 取消执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancellExcute(object sender, EventArgs e)
        {
            this.PrimitiveCancell(this._asynWaitPara.CancelAbort);
        }

        /// <summary>
        /// 取消执行
        /// </summary>
        public void Cancell()
        {
            this.PrimitiveCancell(true);
        }

        /// <summary>
        /// 取消执行
        /// </summary>
        /// <param name="abortThread"></param>
        private void PrimitiveCancell(bool abortThread)
        {
            lock (this._excuteCompletedLock)
            {
                if (abortThread)
                {
                    if (this._excuteCompleted)
                    {
                        return;
                    }
                    this._excuteCompleted = true;
                }

                try
                {
                    var cts = this._asynExcuteThreadCts;
                    if (cts != null && cts.Token.IsCancellationRequested)
                    {
                        return;
                    }
                    cts.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                var asynExcuteThread = this._asynExcuteThread;
                if (abortThread && asynExcuteThread != null)
                {
                    //asynExcuteThread.Abort();//.netcore不支持此方法,直接报平台不支持异常
                    this._asynExcuteThread = null;
                    this.ExcuteCompleted(default(TResult), PartAsynExcuteStatus.Cancel, null);
                }
            }
        }



        /// <summary>
        /// 释放异步委托资源
        /// </summary>
        private void ReleaseResource()
        {
            try
            {
                PartAsynUIParaProxy.UnLock(this._asynWaitPara);
                this._asynWaitPara.AsynWait.Canceled -= CancellExcute;
                this._asynWaitPara.AsynWait.StopAnimation();
                AsynExcuteCancellHelper.RemoveAsynExcuteCancell(this);
                this._asynExcuteThreadCts.Dispose();
                this._asynExcuteThreadCts = null;
                this.PrimitiveReleseResource();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        /// <summary>
        /// 释放异步委托资源
        /// </summary>
        protected virtual void PrimitiveReleseResource()
        {

        }




        #region IDisposable
        /// <summary>
        /// 是否释放标识
        /// </summary>
        private bool _isDisposed = false;

        /// <summary>
        /// 资源释放 
        /// </summary>
        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            this._isDisposed = true;
            this.Dispose(true);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDisposing">是否释放标识</param>
        protected virtual void Dispose(bool isDisposing)
        {

        }
        #endregion
    }
}
