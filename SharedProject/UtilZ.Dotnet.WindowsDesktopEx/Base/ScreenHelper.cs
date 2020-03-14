using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.WindowsDesktopEx.Base
{
    /// <summary>
    /// 屏幕辅助类
    /// </summary>
    public class ScreenHelper
    {
        /// <summary>
        /// 获取任务栏区域
        /// </summary>
        /// <returns>任务栏区域</returns>
        public static ScreenArea GetTaskbarArea()
        {
            int width = 0, height = 0;
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            ScreenAreaOrientation orientation;
            int x, y;

            if (bounds.Width == workingArea.Width)
            {
                if (workingArea.Y == 0)
                {
                    //bottom
                    width = workingArea.Width;
                    height = bounds.Height - workingArea.Height;
                    orientation = ScreenAreaOrientation.Bottom;
                    x = 0;
                    y = workingArea.Height;
                }
                else
                {
                    //top
                    width = workingArea.Width;
                    height = bounds.Height - workingArea.Height;
                    orientation = ScreenAreaOrientation.Top;
                    x = 0;
                    y = 0;
                }
            }
            else if (bounds.Height == workingArea.Height)
            {
                if (workingArea.X == 0)
                {
                    //right
                    width = bounds.Width - workingArea.Width;
                    height = workingArea.Height;
                    orientation = ScreenAreaOrientation.Right;
                    x = workingArea.Width;
                    y = 0;
                }
                else
                {
                    //left
                    width = bounds.Width - workingArea.Width;
                    height = workingArea.Height;
                    orientation = ScreenAreaOrientation.Left;
                    x = 0;
                    y = 0;
                }
            }
            else
            {
                throw new ArgumentException($"内部错误");
            }

            return new ScreenArea(width, height, orientation, x, y);
        }

        /// <summary>
        /// 获取工作区域
        /// </summary>
        /// <param name="offset">宽度和调度偏移量,因为按照系统API值计算出的结果比实际屏幕显示区域要大,原因不明,以后有空研究通了再去年此参数,现在使用默认值即可,不合理可调整</param>
        /// <returns>工作区域</returns>
        public static ScreenArea GetWorkingArea(int offset = -8)
        {
            //大小 
            //var workingArea = System.Windows.Forms.SystemInformation.PrimaryMonitorMaximizedWindowSize;
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int width = workingArea.Width - offset;
            int height = workingArea.Height - offset;

            //方向
            int x, y;
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            ScreenAreaOrientation orientation;

            if (bounds.Width == workingArea.Width)
            {
                if (workingArea.Y == 0)
                {
                    //taskbar bottom
                    orientation = ScreenAreaOrientation.Top;
                    x = 0;
                    y = 0;
                }
                else
                {
                    //taskbar top
                    orientation = ScreenAreaOrientation.Bottom;
                    x = 0;
                    y = bounds.Height - workingArea.Height;
                }
            }
            else if (bounds.Height == workingArea.Height)
            {
                if (workingArea.X == 0)
                {
                    //taskbar right
                    orientation = ScreenAreaOrientation.Left;
                    x = bounds.Width - workingArea.Width;
                    y = 0;
                }
                else
                {
                    //taskbar left
                    orientation = ScreenAreaOrientation.Right;
                    x = 0;
                    y = 0;
                }
            }
            else
            {
                throw new ArgumentException($"内部错误");
            }

            return new ScreenArea(width, height, orientation, x, y);
        }
    }

    /// <summary>
    /// 区域区域信息
    /// </summary>
    public class ScreenArea
    {
        /// <summary>
        /// 区域宽度
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 区域高度
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 区域所在屏幕方向
        /// </summary>
        public ScreenAreaOrientation Orientation { get; private set; }

        /// <summary>
        /// 在屏幕中的X坐标
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 在屏幕中的Y坐标
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="width">区域宽度</param>
        /// <param name="height">区域高度</param>
        /// <param name="orientation">区域所在屏幕方向</param>
        /// <param name="x">在屏幕中的X坐标</param>
        /// <param name="y">在屏幕中的Y坐标</param>
        public ScreenArea(int width, int height, ScreenAreaOrientation orientation, int x, int y)
        {
            this.Width = width;
            this.Height = height;
            this.Orientation = orientation;
            this.X = x;
            this.Y = y;
        }
    }

    /// <summary>
    /// 任务栏所在方向
    /// </summary>
    public enum ScreenAreaOrientation
    {
        /// <summary>
        /// 顶端
        /// </summary>
        Top = 1,

        /// <summary>
        /// 底部
        /// </summary>
        Bottom = 2,

        /// <summary>
        /// 左部
        /// </summary>
        Left = 3,

        /// <summary>
        /// 右部
        /// </summary>
        Right = 4
    }
}
