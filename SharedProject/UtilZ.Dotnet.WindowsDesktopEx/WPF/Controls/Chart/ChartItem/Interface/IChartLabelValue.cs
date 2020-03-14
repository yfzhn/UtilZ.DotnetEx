using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart Label值接口
    /// </summary>
    public interface IChartLabelValue : IChartValue
    {
        /// <summary>
        /// Label
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Label线样式
        /// </summary>
        Style LabelLineStyle { get; }

        /// <summary>
        /// Label文本样式
        /// </summary>
        Style LabelStyle { get; }

        /// <summary>
        /// 最小值,double.NaN自动计算
        /// </summary>
        double MinValue { get; set; }

        /// <summary>
        /// 最大值,double.NaN自动计算
        /// </summary>
        double MaxValue { get; set; }
    }
}
