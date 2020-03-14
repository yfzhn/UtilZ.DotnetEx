using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart绘制点信息
    /// </summary>
    public class PointInfo
    {
        /// <summary>
        /// Point
        /// </summary>
        public Point Point { get; private set; }

        /// <summary>
        /// 数据项
        /// </summary>
        public IChartValue Item { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="item">数据项</param>
        public PointInfo(Point point, IChartValue item)
        {
            this.Point = point;
            this.Item = item;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Point.X},{this.Point.Y}";
        }
    }
}
