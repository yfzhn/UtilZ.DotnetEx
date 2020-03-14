using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 包含坐标双值项接口
    /// </summary>
    public interface IChartAxisDoubleValue : IChartValue
    {
        /// <summary>
        /// 获取X轴坐标值1
        /// </summary>
        /// <returns>X轴坐标值</returns>
        IChartChildValue GetXValue1();

        /// <summary>
        /// 获取X轴坐标值2
        /// </summary>
        /// <returns>X轴坐标值</returns>
        IChartChildValue GetXValue2();



        /// <summary>
        /// 获取Y轴坐标值1
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        IChartChildValue GetYValue1();

        /// <summary>
        /// 获取Y轴坐标值2
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        IChartChildValue GetYValue2();
    }
}
