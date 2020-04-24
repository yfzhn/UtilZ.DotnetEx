using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 异步等待UI接口
    /// </summary>
    public interface IPartAsynWait
    {
        /// <summary>
        /// 获取是否已经取消
        /// </summary>
        bool Canceled { get; }

        /// <summary>
        /// 获取或设置提示标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 获取或设置提示内容
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 是否显示取消按钮
        /// </summary>
        bool ShowCancel { get; set; }

        /// <summary>
        /// 获取或设置遮罩层背景色
        /// </summary>
        object ShadeBackground { get; set; }

        /// <summary>
        /// 取消通知事件
        /// </summary>
        event EventHandler CanceledNotify;

        /// <summary>
        /// 取消操作
        /// </summary>
        void Cancel();

        /// <summary>
        /// 开始等待动画
        /// </summary>
        void StartAnimation();

        /// <summary>
        /// 停止等待动画
        /// </summary>
        void StopAnimation();

        /// <summary>
        /// 设置信息(保留接口),比如用来设置其它的什么进度条之类的
        /// </summary>
        /// <param name="para">参数</param>
        void SetInfo(object para);

        /// <summary>
        /// 重置异步等待框
        /// </summary>
        void Reset();

        /// <summary>
        ///获取一个值，该值指示调用方在对控件进行方法调用时是否必须调用 Invoke 方法，因为调用方位于创建控件所在的线程以外的线程中[如果控件的 true 是在与调用线程不同的线程上创建的（说明您必须通过 Invoke 方法对控件进行调用），则为 System.Windows.Forms.Control.Handle；否则为false]
        /// </summary>
        bool InvokeRequired { get; }

        /// <summary>
        /// 在拥有此控件的基础窗口句柄的线程上执行指定的委托
        /// </summary>
        /// <param name="method">包含要在控件的线程上下文中调用的方法的委托</param>
        /// <returns>正在被调用的委托的返回值，或者如果委托没有返回值，则为 null</returns>
        object Invoke(Delegate method);

        /// <summary>
        /// 在拥有控件的基础窗口句柄的线程上，用指定的自变量列表执行指定委托
        /// </summary>
        /// <param name="method">一个方法委托，它采用的参数的数量和类型与 args 参数中所包含的相同</param>
        /// <param name="args">作为指定方法的自变量传递的对象数组。 如果此方法没有参数，该参数可以是 null</param>
        /// <returns>System.Object，它包含正被调用的委托返回值；如果该委托没有返回值，则为 null</returns>
        object Invoke(Delegate method, params object[] args);
    }
}
