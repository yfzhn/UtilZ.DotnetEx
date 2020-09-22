using System;
using System.Collections.Generic;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait
{
    /// <summary>
    /// 执行异步等待基类
    /// </summary>
    /// <typeparam name="T">异步执行参数类型</typeparam>
    /// <typeparam name="TContainer">容器控件类型</typeparam>
    /// <typeparam name="TResult">异步执行返回值类型</typeparam>
    public abstract class WPFAsynExcuteAbs<T, TContainer, TResult> : AsynExcuteAbs<T, TContainer, TResult>, IAsynExcuteCancell where TContainer : class
    {
        /// <summary>
        /// 容器控件中是否含有Winform的子控件[true:有;false:没有]
        /// </summary>
        public bool HasWinformControl { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WPFAsynExcuteAbs()
            : base()
        {

        }
    }
}
