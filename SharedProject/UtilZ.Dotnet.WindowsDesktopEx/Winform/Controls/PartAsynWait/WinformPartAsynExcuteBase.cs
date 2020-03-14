using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform
{
    /// <summary>
    /// Winform异步执行基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class WinformPartAsynExcuteBase<T, TContainer, TResult> : PartAsynExcuteAbs<T, TContainer, TResult> where TContainer : class
    {
        /// <summary>
        /// 异步等待控件类型
        /// </summary>
        protected readonly static Type _asynControlType;

        /// <summary>
        /// Tab禁用的控件集合
        /// </summary>
        protected readonly List<Control> _asynModifyControls = new List<Control>();

        /// <summary>
        /// 静态构造函数初始化
        /// </summary>
        static WinformPartAsynExcuteBase()
        {
            _asynControlType = typeof(System.Windows.Forms.Control);
        }

        /// <summary>
        /// 断言对象类型是IAsynWait和UserControl的子类对象类型
        /// </summary>
        /// <param name="value">要断言的对象类型</param>
        protected static void AssertIAsynWait(Type value)
        {
            AssertIAsynWait(value, _asynControlType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WinformPartAsynExcuteBase()
            : base()
        {

        }
    }
}
