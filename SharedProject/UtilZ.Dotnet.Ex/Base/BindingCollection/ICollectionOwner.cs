using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 集合所属接口
    /// </summary>
    public interface ICollectionOwner
    {
        /// <summary>
        /// 在拥有控件的基础窗口句柄的线程上，用指定的参数列表执行指定委托
        /// </summary>
        /// <param name="method">一个方法委托，它采用的参数的数量和类型与 args 参数中所包含的相同</param>
        /// <param name="args">作为指定方法的参数传递的对象数组。如果此方法没有参数，该参数可以是 null</param>
        /// <returns>System.Object，它包含正被调用的委托返回值；如果该委托没有返回值，则为 null</returns>
        object Invoke(Delegate method, params object[] args);

        /// <summary>
        /// 获取一个值，该值指示调用方在对控件进行方法调用时是否必须调用 Invoke 方法，因为调用方位于创建控件所在的线程以外的线程中.
        /// 如果控件的 System.Windows.Forms.Control.Handle 是在与调用线程不同的线程上创建的（说明您必须通过 Invoke 方法对控件进行调用），则为true；否则为 false。
        /// </summary>
        bool InvokeRequired { get; }

        /// <summary>
        /// 获取一个值，该值指示控件是否已经被释放[如果控件已经被释放，则为 true；否则为 false]
        /// </summary>
        bool IsDisposed { get; }
    }
}
