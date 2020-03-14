using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 包含坐标值项基类
    /// </summary>
    public abstract class ChartAxisValueAbs : ChartValueAbs, IChartAxisValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartAxisValueAbs()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartAxisValueAbs(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }

        /// <summary>
        /// 获取X轴坐标值
        /// </summary>
        /// <returns>X轴坐标值</returns>
        public abstract object GetXValue();

        /// <summary>
        /// 获取Y轴坐标值
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        public abstract object GetYValue();
    }
}
