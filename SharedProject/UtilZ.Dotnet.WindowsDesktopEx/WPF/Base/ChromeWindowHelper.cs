using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Base
{
    /// <summary>
    /// ChromeWindow辅助类
    /// </summary>
    public static class ChromeWindowHelper
    {
        /// <summary>
        /// 调整窗口全屏大小和位置
        /// </summary>
        /// <param name="window">目标窗口</param>
        /// <param name="pre">用于比较的误差精度值</param>
        public static void AdjustWindowFullScreenSizeLocation(this Window window, double pre = 1e-6)
        {
            var workingArea = ScreenHelper.GetWorkingArea();
            if (double.IsInfinity(window.MaxHeight) ||
               Math.Abs(window.MaxHeight - workingArea.Height) > pre)
            {
                window.MaxHeight = workingArea.Height;
            }

            if (double.IsInfinity(window.MaxWidth) ||
               Math.Abs(window.MaxWidth - workingArea.Width) > pre)
            {
                window.MaxWidth = workingArea.Width;
            }

            //if (Math.Abs(window.Top - workingArea.Y) > pre)
            //{
            //    window.Top = workingArea.Y;
            //}

            //if (Math.Abs(window.Left - workingArea.X) > pre)
            //{
            //    window.Left = workingArea.X;
            //}
        }
    }
}
