using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.Ex.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait
{
    /// <summary>
    /// WPF异步等待辅助类
    /// </summary>
    public class WPFPartAsynWaitHelper : PartAsynWaitHelperBase
    {
        /// <summary>
        /// 静态构造函数创建异步执行对象创建工厂对象
        /// </summary>
        static WPFPartAsynWaitHelper()
        {
            _partAsynExcuteFactory = new PartAsynExcuteFactoryWPF();
        }

        /// <summary>
        /// 异步执行对象创建工厂对象
        /// </summary>
        private static PartAsynExcuteFactoryAbs _partAsynExcuteFactory;

        /// <summary>
        /// 获取或设置异步执行对象创建工厂对象
        /// </summary>
        public static PartAsynExcuteFactoryAbs PartAsynExcuteFactory
        {
            get { return _partAsynExcuteFactory; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _partAsynExcuteFactory = value;
            }
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        ///<param name="asynWaitPara">异步等待执行参数</param>
        ///<param name="containerControl">容器控件</param>
        ///<param name="hasWinformControl">容器控件中是否含有Winform的子控件[true:有;false:没有]</param>
        public static void Wait<T, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, UIElement containerControl, bool hasWinformControl = false)
        {
            Wait<T, TResult>(asynWaitPara, containerControl, null, hasWinformControl);
        }

        /// <summary>
        /// 异步等待
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        ///<param name="asynWaitPara">异步等待执行参数</param>
        ///<param name="containerControl">容器控件</param>
        ///<param name="asynWait">异步等待UI</param>
        ///<param name="hasWinformControl">容器控件中是否含有Winform的子控件[true:有;false:没有]</param>
        public static void Wait<T, TResult>(PartAsynWaitPara<T, TResult> asynWaitPara, UIElement containerControl, IPartAsynWait asynWait, bool hasWinformControl = false)
        {
            ParaValidate(asynWaitPara, containerControl);
            var asynExcute = (WPFAsynExcuteAbs<T, UIElement, TResult>)_partAsynExcuteFactory.CreateExcute<T, UIElement, TResult>();
            asynExcute.HasWinformControl = hasWinformControl;
            PartAsynUIParaProxy.SetAsynWait(asynWaitPara, asynWait);
            asynExcute.Excute(asynWaitPara, containerControl);
        }
    }
}
