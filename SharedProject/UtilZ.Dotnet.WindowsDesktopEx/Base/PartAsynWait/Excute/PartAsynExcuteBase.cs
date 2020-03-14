using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute
{
    /// <summary>
    /// 执行异步等待基类
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    public abstract class PartAsynExcuteBase<T, TContainer, TResult> : IAsynExcute<T, TContainer, TResult> where TContainer : class
    {
        /// <summary>
        /// 异步执行线程
        /// </summary>
        protected Thread _asynExcuteThread = null;

        /// <summary>
        /// 异步执行线程取消对象
        /// </summary>
        protected CancellationTokenSource _asynExcuteThreadCts = null;

        /// <summary>
        /// 异步等待执行参数
        /// </summary>
        protected PartAsynWaitPara<T, TResult> _asynWaitPara = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynExcuteBase()
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
        protected IPartAsynWait CreateAsynWaitShadeControl(Type shadeType, PartAsynUIPara para)
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
            this.Dispose(this._isDisposed);
        }

        /// <summary>
        /// 释放资源方法
        /// </summary>
        /// <param name="isDispose">是否释放标识</param>
        protected virtual void Dispose(bool isDispose)
        {

        }
        #endregion
    }
}
