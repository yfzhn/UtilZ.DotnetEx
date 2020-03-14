using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 无坐标值接口
    /// </summary>
    public interface IChartNoAxisValue : IChartValue
    {
        /// <summary>
        /// Label
        /// </summary>
        string Label { get; }

        /// <summary>
        /// 标题
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 样式
        /// </summary>
        Style Style { get; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        object GetValue();
    }
}
