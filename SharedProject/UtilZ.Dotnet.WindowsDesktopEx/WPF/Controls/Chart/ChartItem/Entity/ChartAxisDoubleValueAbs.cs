using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 包含坐标双值项基类
    /// </summary>
    public abstract class ChartAxisDoubleValueAbs : ChartValueAbs, IChartAxisDoubleValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartAxisDoubleValueAbs()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartAxisDoubleValueAbs(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }

        /// <summary>
        /// 获取X轴坐标值1
        /// </summary>
        /// <returns>X轴坐标值</returns>
        public abstract IChartChildValue GetXValue1();

        /// <summary>
        /// 获取X轴坐标值2
        /// </summary>
        /// <returns>X轴坐标值</returns>
        public abstract IChartChildValue GetXValue2();



        /// <summary>
        /// 获取Y轴坐标值1
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        public abstract IChartChildValue GetYValue1();

        /// <summary>
        /// 获取Y轴坐标值2
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        public abstract IChartChildValue GetYValue2();
    }
}
