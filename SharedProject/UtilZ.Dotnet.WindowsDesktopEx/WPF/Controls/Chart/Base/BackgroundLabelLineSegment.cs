using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 图表区域坐标线段
    /// </summary>
    public class BackgroundLabelLineSegment
    {
        /// <summary>
        /// 点1
        /// </summary>
        public Point Point1 { get; private set; }

        /// <summary>
        /// 点2
        /// </summary>
        public Point Point2 { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="point1">点1</param>
        /// <param name="point2">点2</param>
        public BackgroundLabelLineSegment(Point point1, Point point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }
    }
}
