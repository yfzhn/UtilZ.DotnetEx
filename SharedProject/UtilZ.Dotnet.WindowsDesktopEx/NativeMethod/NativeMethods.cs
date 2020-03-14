using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.NativeMethod
{
    /// <summary>
    /// 系统win32方法
    /// </summary>
    internal class NativeMethods
    {
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
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

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
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// 刷新系统缓存图标
        /// Notifies the system of an event that an application has performed. An application should use this function if it performs an action that may affect the Shell. 
        /// </summary>
        /// <param name="wEventId">Describes the event that has occurred. Typically, only one event is specified at a time. If more than one event is specified, the values contained in the dwItem1 and dwItem2 parameters must be the same, respectively, for all specified events. This parameter can be one or more of the following values. </param>
        /// <param name="uFlags">Flags that indicate the meaning of the dwItem1 and dwItem2 parameters. The uFlags parameter must be one of the following values.</param>
        /// <param name="dwItem1">First event-dependent value. </param>
        /// <param name="dwItem2">Second event-dependent value.</param>
        [DllImport("shell32.dll", EntryPoint = "SHChangeNotify", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        internal static extern void SHChangeNotify(Int32 wEventId, UInt32 uFlags, IntPtr dwItem1, IntPtr dwItem2);

        /// <summary>
        /// 根据句柄查询该句柄对应的进程ID
        /// </summary>
        /// <param name="hwnd">应用程序句柄</param>
        /// <param name="processId">进程ID</param>
        /// <returns>创建窗口的线程ID</returns>
        [DllImport(@"user32.dll")]
        internal static extern int GetWindowThreadProcessId(IntPtr hwnd, out int processId);

        /// <summary>
        /// 最小化窗口在任务栏闪烁一次
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="bInvert">true:表示窗口从一个状态闪烁到另一个状态;false:表示窗口恢复到初始状态（可能是激活的也可能是非激活的）</param>
        /// <returns>表示调用FlashWindow函数之前窗口的活动状态，若指定窗口在调用函数之前是激活的，那么返回非零值，否则返回零值</returns>
        [DllImport("user32.dll")]
        internal static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        /// <summary>
        /// 进程无焦点时，任务栏最小化窗口闪烁
        /// </summary>
        /// <param name="pwfi">窗口闪烁信息</param>
        /// <returns>返回调用 FlashWindowEx 函数之前指定窗口状态。如果调用之前窗口标题是活动的，返回值为非零值</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        #region 最小化窗口在任务栏闪烁
        /// <summary>
        /// Stop flashing. The system restores the window to its original stae.
        /// </summary>
        internal const uint FLASHW_STOP = 0;

        /// <summary>
        /// Flash the window caption.
        /// </summary>
        internal const uint FLASHW_CAPTION = 1;

        /// <summary>
        /// Flash the taskbar button.
        /// </summary>
        internal const uint FLASHW_TRAY = 2;

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        /// </summary>
        internal const uint FLASHW_ALL = 3;

        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        internal const uint FLASHW_TIMER = 4;

        /// <summary>
        /// Flash continuously until the window comes to the foreground.
        /// </summary>
        internal const uint FLASHW_TIMERNOFG = 12;

        /// <summary>
        /// A boolean value indicating whether the application is running on Windows 2000 or later.
        /// </summary>
        internal static bool Win2000OrLater
        {
            get { return System.Environment.OSVersion.Version.Major >= 5; }
        }

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
        internal static bool Flash(IntPtr hwnd)
        {
            // Make sure we're running under Windows 2000 or later
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Flash the specified Window (form) for the specified number of times
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="count">闪烁次数</param>
        /// <returns></returns>
        internal static bool Flash(IntPtr hwnd, uint count)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, FLASHW_ALL, count, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Start Flashing the specified Window (form)
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns></returns>
        internal static bool Start(IntPtr hwnd)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, FLASHW_ALL, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
            }
            return false;
        }

        /// <summary>
        /// Stop Flashing the specified Window (form)
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <returns></returns>
        internal static bool Stop(IntPtr hwnd)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO fi = Create_FLASHWINFO(hwnd, FLASHW_STOP, uint.MaxValue, 0);
                return FlashWindowEx(ref fi);
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
        internal static bool WindowFadeInOut(IntPtr hwnd, int dwTime, int dwFlags)
        {
            return AnimateWindow(hwnd, dwTime, dwFlags);
        }

        /// <summary>
        /// 窗口淡入
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="dwTime">动画持续时间</param>
        /// <param name="dwFlags">动画类型(WindowAnimateType中的值按位枚举运算)</param>
        /// <returns>结果</returns>
        internal static bool WindowFadeIn(IntPtr hwnd, int dwTime = 300, int dwFlags = WindowAnimateType.AW_BLEND)
        {
            return AnimateWindow(hwnd, dwTime, dwFlags);
        }

        /// <summary>
        /// 窗口淡出
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="dwTime">动画持续时间</param>
        /// <param name="dwFlags">动画类型(WindowAnimateType中的值按位枚举运算)</param>
        /// <returns>结果</returns>
        internal static bool WindowFadeOut(IntPtr hwnd, int dwTime = 300, int dwFlags = WindowAnimateType.AW_SLIDE | WindowAnimateType.AW_HIDE | WindowAnimateType.AW_BLEND)
        {
            return AnimateWindow(hwnd, dwTime, dwFlags);
        }
        #endregion

        /// <summary>
        /// 设置应用程序的父窗口
        /// </summary>
        /// <param name="hWndChild">子窗口句柄</param>
        /// <param name="hWndNewParent">父窗口句柄</param>
        /// <returns>long</returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 移除应用启动的程序窗口边框
        /// </summary>
        /// <param name="hwnd">要移除边框的应用程序句柄</param>
        /// <param name="nIndex">索引</param>
        /// <param name="dwNewLong">边框值</param>
        /// <returns>long</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
        internal static extern long SetWindowLong(IntPtr hwnd, int nIndex, long dwNewLong);

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
        [DllImport("user32.dll", EntryPoint = "MoveWindow", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        /// <summary>
        /// 指定hThread运行在核心dwThreadAffinityMask
        /// </summary>
        /// <param name="hThread">线程句柄</param>
        /// <param name="dwThreadAffinityMask">CPU核心编号</param>
        /// <returns>设置结果</returns>
        [DllImport("kernel32.dll")]
        internal static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        /// <summary>
        /// 得到当前线程的句柄
        /// </summary>
        /// <returns>当前线程的句柄</returns>
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThread();



        /// <summary>
        /// 窗口淡入淡出
        /// </summary>
        /// <param name="hwnd">handle to window</param>
        /// <param name="dwTime">duration of animation</param>
        /// <param name="dwFlags">animation type</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
    }

    /// <summary>
    /// 窗口闪烁结构信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FLASHWINFO
    {
        /// <summary>
        /// The size of the structure in bytes.
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
        /// </summary>
        public IntPtr hwnd;

        /// <summary>
        /// The Flash Status.
        /// </summary>
        public uint dwFlags;

        /// <summary>
        /// The number of times to Flash the window.
        /// </summary>
        public uint uCount;

        /// <summary>
        /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
        /// </summary>
        public uint dwTimeout;
    }

    /// <summary>
    /// 动画类型定义类
    /// </summary>
    internal class WindowAnimateType
    {
        /// <summary>
        /// 从左到右打开窗口
        /// </summary>
        public const int AW_HOR_POSITIVE = 0x00000001;

        /// <summary>
        /// 从右到左打开窗口
        /// </summary>
        public const int AW_HOR_NEGATIVE = 0x00000002;

        /// <summary>
        /// 从上到下打开窗口
        /// </summary>
        public const int AW_VER_POSITIVE = 0x00000004;

        /// <summary>
        /// 从下到上打开窗口
        /// </summary>
        public const int AW_VER_NEGATIVE = 0x00000008;

        /// <summary>
        /// 若使用了AW_HIDE标志，则使窗口向内重叠；若未使用AW_HIDE标志，则使窗口向外扩展
        /// </summary>
        public const int AW_CENTER = 0x00000010;

        /// <summary>
        /// 隐藏窗口，缺省则显示窗口
        /// </summary>
        public const int AW_HIDE = 0x00010000;

        /// <summary>
        /// 激活窗口。在使用了AW_HIDE标志后不要使用这个标志
        /// </summary>
        public const int AW_ACTIVATE = 0x00020000;

        /// <summary>
        /// 使用滑动类型。缺省则为滚动动画类型。当使用AW_CENTER标志时，这个标志就被忽略
        /// </summary>
        public const int AW_SLIDE = 0x00040000;

        /// <summary>
        /// 使用淡出效果。只有当hWnd为顶层窗口的时候才可以使用此标志
        /// </summary>
        public const int AW_BLEND = 0x00080000;
    }
}
