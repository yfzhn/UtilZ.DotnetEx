using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Interface;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Excute;
using System.Drawing;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait
{
    /// <summary>
    /// Winform异步执行对象创建工厂类
    /// </summary>
    public class PartAsynExcuteFactoryWinform : PartAsynExcuteFactoryAbs
    {
        /// <summary>
        /// 静态构造函数初始化默认异步等待类型
        /// </summary>
        static PartAsynExcuteFactoryWinform()
        {
            _partAsynExcuteType = WinformPartAsynExcuteTypeDefine.OpacityShadeDisableTab;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PartAsynExcuteFactoryWinform()
            : base()
        {

        }

        /// <summary>
        /// Winfrom异步执行对象创建类型
        /// </summary>
        private static int _partAsynExcuteType;

        /// <summary>
        /// 获取或设置Winfrom异步执行对象创建类型
        /// </summary>
        public static int PartAsynExcuteType
        {
            get { return _partAsynExcuteType; }
            set { _partAsynExcuteType = value; }
        }

        /// <summary>
        /// 转换遮罩层背景色
        /// </summary>
        /// <param name="shadeBackground">遮罩层背景色对象</param>
        /// <returns>遮罩层背景色</returns>
        public static Color ConvertShadeBackground(object shadeBackground)
        {
            if (shadeBackground == null)
            {
                return System.Drawing.Color.White;
            }
            else
            {
                return (Color)shadeBackground;
            }
        }

        /// <summary>
        /// 创建异步执行对象
        /// </summary>
        /// <typeparam name="T">异步执行参数类型</typeparam>
        /// <typeparam name="TContainer">容器控件类型</typeparam>
        /// <typeparam name="TResult">异步执行返回值类型</typeparam>
        /// <returns>异步执行对象</returns>
        public override IAsynExcute<T, TContainer, TResult> CreateExcute<T, TContainer, TResult>()
        {
            int partAsynExcuteType = _partAsynExcuteType;
            IAsynExcute<T, TContainer, TResult> asynExcute;
            if (partAsynExcuteType == WinformPartAsynExcuteTypeDefine.ShadeDisableControl)
            {
                asynExcute = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V1.WinformPartAsynExcuteV1<T, TContainer, TResult>();
            }
            else if (partAsynExcuteType == WinformPartAsynExcuteTypeDefine.OpacityDisableTab)
            {
                asynExcute = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V2.WinformPartAsynExcute2<T, TContainer, TResult>();
            }
            else if (partAsynExcuteType == WinformPartAsynExcuteTypeDefine.OpacityShadeDisableTab)
            {
                asynExcute = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V3.WinformPartAsynExcute3<T, TContainer, TResult>();
            }
            else if (partAsynExcuteType == WinformPartAsynExcuteTypeDefine.ScreenshotImgBackgrounDisableTab)
            {
                asynExcute = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V4.WinformPartAsynExcute4<T, TContainer, TResult>();
            }
            else if (partAsynExcuteType == WinformPartAsynExcuteTypeDefine.OpacityCustomerDisableTab)
            {
                asynExcute = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V5.WinformPartAsynExcute5<T, TContainer, TResult>();
            }
            else
            {
                throw new NotSupportedException(string.Format("不支持的异步执行对象创建类型{0}", partAsynExcuteType));
            }

            return asynExcute;
        }
    }
}
