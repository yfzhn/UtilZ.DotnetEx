using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 雷达图值接口
    /// </summary>
    public interface IChartRadarValue : IChartItem
    {
        /// <summary>
        /// 获取Label
        /// </summary>
        /// <returns>Label</returns>
        IChartLabelValue GetLabel();

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns>值</returns>
        double GetValue();
    }
}
