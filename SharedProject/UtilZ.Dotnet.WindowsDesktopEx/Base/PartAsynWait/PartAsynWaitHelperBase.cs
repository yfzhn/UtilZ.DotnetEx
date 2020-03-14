using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait
{
    /// <summary>
    /// 异步等待器基类
    /// </summary>
    public abstract class PartAsynWaitHelperBase
    {
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="asynWaitPara"></param>
        /// <param name="containerControl"></param>
        protected static void ParaValidate<T, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, object containerControl)
        {
            if (asynWaitPara == null)
            {
                throw new ArgumentNullException("asynWaitPara");
            }

            if (containerControl == null)
            {
                throw new ArgumentNullException("containerControl");
            }
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TContainer">容器控件类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        ///<param name="asynWaitPara">异步等待执行参数</param>
        ///<param name="containerControl">容器控件</param>
        ///<param name="asynExcute">异步等待UI</param>
        public static void Wait<T, TContainer, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, TContainer containerControl, IAsynExcute<T, TContainer, TResult> asynExcute)
        {
            Wait<T, TContainer, TResult>(asynWaitPara, containerControl, null, asynExcute);
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TContainer">容器控件类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        ///<param name="asynWaitPara">异步等待执行参数</param>
        ///<param name="containerControl">容器控件</param>
        ///<param name="asynWait">异步等待UI</param>
        ///<param name="asynExcute">异步执行</param>
        public static void Wait<T, TContainer, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, TContainer containerControl, IPartAsynWait asynWait, IAsynExcute<T, TContainer, TResult> asynExcute)
        {
            ParaValidate(asynWaitPara, containerControl);
            if (asynExcute == null)
            {
                throw new ArgumentNullException("asynExcute");
            }

            asynWaitPara.AsynWait = asynWait;
            asynExcute.Excute(asynWaitPara, containerControl);
        }

        /// <summary>
        /// 取消一个异常等待
        /// </summary>
        /// <param name="asynWaitPara">异步等待参数</param>
        public static void Cancel<T, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara)
        {
            if (asynWaitPara == null)
            {
                return;
            }

            var asynWait = asynWaitPara.AsynWait;
            if (asynWait == null)
            {
                return;
            }

            asynWait.Cancel();
        }
    }
}
