using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface
{
    /// <summary>
    /// 执行异步等待接口
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    public interface IAsynExcute<T, TContainer, TResult> : IDisposable
    {
        /// <summary>
        /// 执行异步委托
        /// </summary>
        /// <param name="asynWaitPara">异步等待执行参数</param>
        /// <param name="containerControl">容器控件</param>
        void Excute(PartAsynWaitPara<T, TResult> asynWaitPara, TContainer containerControl);
    }
}
