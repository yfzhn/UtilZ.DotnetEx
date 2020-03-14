using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// ChartValue子项接口
    /// </summary>
    public interface IChartChildValue : IChartItem
    {
        /// <summary>
        /// 获取子项值
        /// </summary>
        /// <returns>子项值</returns>
        object GetValue();
    }
}
