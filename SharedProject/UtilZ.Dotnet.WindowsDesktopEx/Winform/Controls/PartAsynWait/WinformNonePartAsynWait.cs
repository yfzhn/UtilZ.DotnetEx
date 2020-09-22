using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait
{
    /// <summary>
    /// Winform空IPartAsynWait
    /// </summary>
    public class WinformNonePartAsynWait : Control, IPartAsynWait
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WinformNonePartAsynWait()
        {

        }

        private bool _canceled = false;
        /// <summary>
        /// 获取是否已经取消
        /// </summary>
        public bool Canceled
        {
            get { return _canceled; }
        }

        /// <summary>
        /// 获取或设置提示标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取或设置提示内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否显示取消按钮
        /// </summary>
        public bool ShowCancel { get; set; }

        /// <summary>
        /// 获取或设置遮罩层背景色
        /// </summary>
        public object ShadeBackground { get; set; }

        /// <summary>
        /// 获取 或设置是否需要调用Invoke
        /// </summary>
        bool IPartAsynWait.InvokeRequired
        {
            get
            {
                return base.InvokeRequired;
            }
        }

        /// <summary>
        /// 取消通知事件
        /// </summary>
        public event EventHandler CanceledNotify;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _monitor = new object();

        /// <summary>
        /// 取消操作
        /// </summary>
        public void Cancel()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Cancel));
                return;
            }

            lock (this._monitor)
            {
                if (this._canceled)
                {
                    return;
                }

                this._canceled = true;
            }

            var handler = this.CanceledNotify;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// 在拥有此控件的基础窗口句柄的线程上执行指定的委托
        /// </summary>
        /// <param name="method">包含要在控件的线程上下文中调用的方法的委托</param>
        /// <returns>正在被调用的委托的返回值，或者如果委托没有返回值，则为 null</returns>
        object IPartAsynWait.Invoke(Delegate method)
        {
            return this.Invoke(method);
        }

        /// <summary>
        /// 在拥有控件的基础窗口句柄的线程上，用指定的自变量列表执行指定委托
        /// </summary>
        /// <param name="method">一个方法委托，它采用的参数的数量和类型与 args 参数中所包含的相同</param>
        /// <param name="args">作为指定方法的自变量传递的对象数组。 如果此方法没有参数，该参数可以是 null</param>
        /// <returns>System.Object，它包含正被调用的委托返回值；如果该委托没有返回值，则为 null</returns>
        object IPartAsynWait.Invoke(Delegate method, params object[] args)
        {
            return this.Invoke(method, args);
        }

        /// <summary>
        /// 重置异步等待框
        /// </summary>
        public void Reset()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(this.Reset));
            }
            else
            {
                this._canceled = false;
            }
        }

        /// <summary>
        /// 设置信息(保留接口)
        /// </summary>
        /// <param name="para">参数</param>
        public void SetInfo(object para)
        {

        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void StartAnimation()
        {

        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void StopAnimation()
        {

        }
    }
}
