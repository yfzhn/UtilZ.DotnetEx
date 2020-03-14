using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 包含坐标值项接口
    /// </summary>
    public interface IChartAxisValue : IChartValue
    {
        /// <summary>
        /// 获取X轴坐标值
        /// </summary>
        /// <returns>X轴坐标值</returns>
        object GetXValue();

        /// <summary>
        /// 获取Y轴坐标值
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        object GetYValue();
    }
}
