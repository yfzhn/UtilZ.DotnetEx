using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UtilZ.Dotnet.WindowsDesktopEx.NativeMethod;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Base
{
    /// <summary>
    /// Process扩展方法类
    /// </summary>
    public static class WindowNestProcess
    {
        /// <summary>
        /// 在窗口控件中启动应用程序
        /// </summary>
        /// <param name="process">进程实例</param>
        /// <param name="containerControl">容器控件</param>
        public static NestProcess Start(this Process process, System.Windows.Forms.Control containerControl)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            NestProcess nprocess = new NestProcess(process);
            nprocess.StartNormalApp(containerControl);
            return nprocess;
        }
    }

    /// <summary>
    /// 应用程序嵌套启动类
    /// </summary>
    public class NestProcess
    {
        /// <summary>
        /// 私有构造函数不允许实例化
        /// </summary>
        internal NestProcess(Process process)
        {
            this._process = process;
        }

        /// <summary>
        /// 移除窗口边框消息索引
        /// </summary>
        private const int GWL_STYLE = (-16);

        /// <summary>
        /// 移除窗口边框消息编号
        /// </summary>
        private const int WS_VISIBLE = 0x10000000;

        /// <summary>
        /// 窗口控件中的应用程序进程
        /// </summary>
        private readonly Process _process = null;

        /// <summary>
        /// 获取窗口控件中的应用程序进程
        /// </summary>
        public Process Process
        {
            get { return _process; }
        }

        /// <summary>
        /// 在窗口控件中启动应用程序
        /// </summary>
        /// <param name="containerControl">容器控件</param>
        internal void StartNormalApp(System.Windows.Forms.Control containerControl)
        {
            //启动进程
            this._process.Start();

            // Wait for process to be created and enter idle condition 
            this._process.WaitForInputIdle();

            //启用的应用程序要等待这么久,否则不会被嵌套到指定的容器控件中,具体原因为毛以后再研究
            System.Threading.Thread.Sleep(190);

            // Remove border and whatnot
            NativeMethods.SetWindowLong(this._process.MainWindowHandle, NestProcess.GWL_STYLE, NestProcess.WS_VISIBLE);

            // Put it into this form
            NativeMethods.SetParent(this._process.MainWindowHandle, containerControl.Handle);

            // Move the window to overlay it on this window
            NativeMethods.MoveWindow(this._process.MainWindowHandle, 0, 0, containerControl.Width, containerControl.Height, true);

            //注册窗口控件大小改变及窗口控件所在窗口关闭事件
            containerControl.Resize += containerControl_Resize;
            containerControl.FindForm().FormClosing += form_FormClosing;
        }

        /// <summary>
        /// 窗口控件所在窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void form_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this._process.CloseMainWindow();
            this._process.WaitForExit();

            // Post a colse message
            //Qianru.PostMessage(this._process.MainWindowHandle, 0x10, 0, 0);
            //this._process.WaitForExit();

            //process.Kill();
        }

        /// <summary>
        /// 窗口控件大小改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void containerControl_Resize(object sender, EventArgs e)
        {
            System.Windows.Forms.Control containerControl = sender as System.Windows.Forms.Control;
            NativeMethods.MoveWindow(this._process.MainWindowHandle, 0, 0, containerControl.Width, containerControl.Height, true);
        }
    }
}
