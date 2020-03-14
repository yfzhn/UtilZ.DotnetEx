using System;
using System.Collections.Generic;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.NativeMethod;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 窗口帮助类
    /// </summary>
    public class WindowHelper
    {
        //private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    //使本程序与文件类型关联
        //    ShortcutHelper.AssociateWithFile("ATCSTestCaseFile", ".tc", @"C:\Program Files\ATCS\ATCS.exe");
        //    // 刷新系统缓存图标
        //    RefreshSystemdCatchIcon();

        //    //分离本程序与文件类型的关联
        //    ShortcutHelper.DisassocateWithFile("ATCSTestCaseFile", ".tc");
        //    // 刷新系统缓存图标
        //    RefreshSystemdCatchIcon();
        //}


        //https://msdn.microsoft.com/en-us/library/windows/desktop/ms633499(v=vs.85).aspx
        /// <summary>
        /// Retrieves a handle to the top-level window whose class name and window name match the specified strings. This function does not search child windows. This function does not perform a case-sensitive search.
        /// To search child windows, beginning with a specified child window, use the FindWindowEx function.
        /// If the lpWindowName parameter is not NULL, FindWindow calls the GetWindowText function to retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks for GetWindowText.
        /// </summary>
        /// <param name="lpClassName">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero.
        /// If lpClassName points to a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
        /// If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.</param>
        /// <param name="lpWindowName">The window name (the window's title). If this parameter is NULL, all window names match</param>
        /// <returns>Type:
        /// Type: HWND
        /// If the function succeeds, the return value is a handle to the window that has the specified class name and window name.
        /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        public static IntPtr FindWindow(string lpClassName, string lpWindowName)
        {
            return NativeMethods.FindWindow(lpClassName, lpWindowName);
        }

        /// <summary>
        /// Retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows, beginning with the one following the specified child window. This function does not perform a case-sensitive search.
        /// If the lpszWindow parameter is not NULL, FindWindowEx calls the GetWindowText function to retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks section of GetWindowText.
        /// An application can call this function in the following way.
        /// FindWindowEx(NULL, NULL, MAKEINTATOM(0x8000), NULL );
        /// Note that 0x8000 is the atom for a menu class. When an application calls this function, the function checks whether a context menu is being displayed that the application created.
        /// </summary>
        /// <param name="hwndParent">A handle to the parent window whose child windows are to be searched.
        /// If hwndParent is NULL, the function uses the desktop window as the parent window.The function searches among windows that are child windows of the desktop.
        /// If hwndParent is HWND_MESSAGE, the function searches all message-only windows.</param>
        /// <param name="hwndChildAfter">A handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window.
        /// If hwndChildAfter is NULL, the search begins with the first child window of hwndParent.
        /// Note that if both hwndParent and hwndChildAfter are NULL, the function searches all top-level and message-only windows</param>
        /// <param name="lpszClass">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.
        /// If lpszClass is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names, or it can be MAKEINTATOM(0x8000). In this latter case, 0x8000 is the atom for a menu class. For more information, see the Remarks section of this topic</param>
        /// <param name="lpszWindow">The window name (the window's title). If this parameter is NULL, all window names match.</param>
        /// <returns>ype:
        /// Type: HWND
        ///  If the function succeeds, the return value is a handle to the window that has the specified class and window names.
        ///  If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        public static IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow)
        {
            return NativeMethods.FindWindowEx(hwndParent, hwndChildAfter, lpszClass, lpszWindow);
        }

        /// <summary>
        /// 刷新系统缓存图标
        /// Notifies the system of an event that an application has performed. An application should use this function if it performs an action that may affect the Shell. 
        /// </summary>
        /// <param name="wEventId">Describes the event that has occurred. Typically, only one event is specified at a time. If more than one event is specified, the values contained in the dwItem1 and dwItem2 parameters must be the same, respectively, for all specified events. This parameter can be one or more of the following values. </param>
        /// <param name="uFlags">Flags that indicate the meaning of the dwItem1 and dwItem2 parameters. The uFlags parameter must be one of the following values.</param>
        /// <param name="dwItem1">First event-dependent value. </param>
        /// <param name="dwItem2">Second event-dependent value.</param>
        public static void SHChangeNotify(Int32 wEventId, UInt32 uFlags, IntPtr dwItem1, IntPtr dwItem2)
        {
            NativeMethods.SHChangeNotify(wEventId, uFlags, dwItem1, dwItem2);
        }

        /// <summary>
        /// 根据句柄查询该句柄对应的进程ID
        /// </summary>
        /// <param name="hwnd">应用程序句柄</param>
        /// <param name="processId">进程ID</param>
        /// <returns>创建窗口的线程ID</returns>
        public static int GetWindowThreadProcessId(IntPtr hwnd, out int processId)
        {
            return NativeMethods.GetWindowThreadProcessId(hwnd, out processId);
        }

        /// <summary>
        /// 最小化窗口在任务栏闪烁一次
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="bInvert">true:表示窗口从一个状态闪烁到另一个状态;false:表示窗口恢复到初始状态（可能是激活的也可能是非激活的）</param>
        /// <returns>表示调用FlashWindow函数之前窗口的活动状态，若指定窗口在调用函数之前是激活的，那么返回非零值，否则返回零值</returns>
        public static bool FlashWindow(IntPtr hwnd, bool bInvert)
        {
            return NativeMethods.FlashWindow(hwnd, bInvert);
        }


        #region 最小化窗口在任务栏闪烁
        /// <summary>
        /// 创建窗口闪烁对象信息
        /// </summary>
        /// <param name="handle">窗口够本</param>
        /// <param name="flags">The Flash Status.</param>
        /// <param name="count">次数</param>
        /// <param name="timeout">The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.</param>
        /// <returns></returns>
        private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(System.Runtime.InteropServices.Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }

        /// <summary>
        /// 任务栏窗口闪烁直到该窗口接收到焦点为止
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns></returns>
        public static bool Flash(IntPtr hwnd)
        {
            // Make sure we're running under Windows 2000 or later
            if (NativeMethods.Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, NativeMethods.FLASHW_ALL | NativeMethods.FLASHW_TIMERNOFG, uint.MaxValue, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Flash the specified Window (form) for the specified number of times
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="count">闪烁次数</param>
        /// <returns></returns>
        public static bool Flash(IntPtr hwnd, uint count)
        {
            if (NativeMethods.Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, NativeMethods.FLASHW_ALL, count, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Start Flashing the specified Window (form)
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns></returns>
        public static bool Start(IntPtr hwnd)
        {
            if (NativeMethods.Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, NativeMethods.FLASHW_ALL, uint.MaxValue, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Stop Flashing the specified Window (form)
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns></returns>
        public static bool Stop(IntPtr hwnd)
        {
            if (NativeMethods.Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, NativeMethods.FLASHW_STOP, uint.MaxValue, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }
            return false;
        }
        #endregion

        #region 窗口淡入淡出
        /// <summary>
        /// 窗口淡入淡出
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="dwTime">动画持续时间</param>
        /// <param name="dwFlags">动画类型(WindowAnimateType中的值按位枚举运算)</param>
        /// <returns>结果</returns>
        public static bool WindowFadeInOut(IntPtr hwnd, int dwTime, int dwFlags)
        {
            return NativeMethods.AnimateWindow(hwnd, dwTime, dwFlags);
        }

        /// <summary>
        /// 窗口淡入
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="dwTime">动画持续时间</param>
        /// <param name="dwFlags">动画类型(WindowAnimateType中的值按位枚举运算)</param>
        /// <returns>结果</returns>
        public static bool WindowFadeIn(IntPtr hwnd, int dwTime = 300, int dwFlags = WindowAnimateType.AW_BLEND)
        {
            return NativeMethods.AnimateWindow(hwnd, dwTime, dwFlags);
        }

        /// <summary>
        /// 窗口淡出
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="dwTime">动画持续时间</param>
        /// <param name="dwFlags">动画类型(WindowAnimateType中的值按位枚举运算)</param>
        /// <returns>结果</returns>
        public static bool WindowFadeOut(IntPtr hwnd, int dwTime = 300, int dwFlags = WindowAnimateType.AW_SLIDE | WindowAnimateType.AW_HIDE | WindowAnimateType.AW_BLEND)
        {
            return NativeMethods.AnimateWindow(hwnd, dwTime, dwFlags);
        }
        #endregion

        /// <summary>
        /// 设置应用程序的父窗口
        /// </summary>
        /// <param name="hWndChild">子窗口句柄</param>
        /// <param name="hWndNewParent">父窗口句柄</param>
        /// <returns>long</returns>
        public static long SetParent(IntPtr hWndChild, IntPtr hWndNewParent)
        {
            return NativeMethods.SetParent(hWndChild, hWndNewParent);
        }

        /// <summary>
        /// 移除应用启动的程序窗口边框
        /// </summary>
        /// <param name="hwnd">要移除边框的应用程序句柄</param>
        /// <param name="nIndex">索引</param>
        /// <param name="dwNewLong">边框值</param>
        /// <returns>long</returns>
        public static long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong)
        {
            return NativeMethods.SetWindowLong(hwnd, nIndex, dwNewLong);
        }

        /// <summary>
        /// 移动应用程序窗口位置
        /// </summary>
        /// <param name="hwnd">程序窗口句柄</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="cx">宽度</param>
        /// <param name="cy">高度</param>
        /// <param name="repaint">是否修正</param>
        /// <returns>bool</returns>
        public static bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint)
        {
            return NativeMethods.MoveWindow(hwnd, x, y, cx, cy, repaint);
        }

        /// <summary>
        /// 指定hThread运行在核心dwThreadAffinityMask
        /// </summary>
        /// <param name="hThread">线程句柄</param>
        /// <param name="dwThreadAffinityMask">CPU核心编号</param>
        /// <returns>设置结果</returns>
        public static UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask)
        {
            return NativeMethods.SetThreadAffinityMask(hThread, dwThreadAffinityMask);
        }

        /// <summary>
        /// 得到当前线程的句柄
        /// </summary>
        /// <returns>当前线程的句柄</returns>
        public static IntPtr GetCurrentThread()
        {
            return NativeMethods.GetCurrentThread();
        }
    }
}
