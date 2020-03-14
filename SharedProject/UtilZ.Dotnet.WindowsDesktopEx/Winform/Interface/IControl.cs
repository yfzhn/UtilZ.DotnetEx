using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Interface
{
    /// <summary>
    /// 控件接口
    /// </summary>
    public interface IControl
    {
        #region 事件
        /// <summary>
        /// 在第一次显示控件前发生
        /// </summary>
        event EventHandler Load;

        /// <summary>
        /// 当控件失去焦点时发生
        /// </summary>
        event EventHandler LostFocus;
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置控件的宽度（以像素为单位）
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// 获取或设置控件的高度（以像素为单位）
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// 获取或设置窗体的大小
        /// </summary>
        System.Drawing.Size Size { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示该控件及其所有子控件
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// 获取或设置以屏幕坐标表示的代表 System.Windows.Forms.Form 左上角的 System.Drawing.Point。
        /// </summary>
        System.Drawing.Point Location { get; set; }

        /// <summary>
        /// 获取UI是否处于设计器模式
        /// </summary>
        bool UIDesignMode { get; }

        /// <summary>
        /// 获取一个值，该值指示调用方在对控件进行方法调用时是否必须调用 Invoke 方法，因为调用方位于创建控件所在的线程以外的线程中
        /// 如果控件的 System.Windows.Forms.Control.Handle 是在与调用线程不同的线程上创建的（说明您必须通过 Invoke方法对控件进行调用），则为 true；否则为 false。
        /// </summary>
        bool InvokeRequired { get; }

        /// <summary>
        /// 获取包含在控件内的控件的集合
        /// </summary>
        System.Windows.Forms.Control.ControlCollection Controls { get; }

        /// <summary>
        /// 获取或设置哪些控件边框停靠到其父控件并确定控件如何随其父级一起调整大小
        /// </summary>
        DockStyle Dock { get; set; }

        /// <summary>
        /// 获取一个值，该值指示控件是否已经被释放[如果控件已经被释放，则为 true；否则为 false]
        /// </summary>
        bool IsDisposed { get; }

        ///// <summary>
        ///// 获取控件的同步上下文
        ///// </summary>
        //System.Threading.SynchronizationContext SynContext { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 在拥有此控件的基础窗口句柄的线程上执行指定的委托
        /// </summary>
        /// <param name="method">包含要在控件的线程上下文中调用的方法的委托</param>
        /// <returns>正在被调用的委托的返回值，或者如果委托没有返回值，则为 null</returns>
        object Invoke(Delegate method);

        /// <summary>
        /// 在拥有控件的基础窗口句柄的线程上，用指定的参数列表执行指定委托
        /// </summary>
        /// <param name="method">一个方法委托，它采用的参数的数量和类型与 args 参数中所包含的相同</param>
        /// <param name="args">作为指定方法的参数传递的对象数组。如果此方法没有参数，该参数可以是 null</param>
        /// <returns>System.Object，它包含正被调用的委托返回值；如果该委托没有返回值，则为 null</returns>
        object Invoke(Delegate method, params object[] args);

        /// <summary>
        /// 检索控件所在的窗体
        /// </summary>
        /// <returns></returns>
        Form FindForm();

        /// <summary>
        /// 为控件设置输入焦点
        /// </summary>
        /// <returns></returns>
        bool Focus();

        /// <summary>
        /// 隐藏控件
        /// </summary>
        void Hide();

        /// <summary>
        /// 强制控件使其工作区无效并立即重绘自己和任何子控件
        /// </summary>
        void Refresh();

        /// <summary>
        /// 检索一个值，该值指示指定控件是否为一个控件的子控件
        /// </summary>
        /// <param name="ctl">要计算的 System.Windows.Forms.Control</param>
        /// <returns>如果指定控件是控件的子控件，则为 true；否则为 false</returns>
        bool Contains(Control ctl);
        #endregion
    }
}
