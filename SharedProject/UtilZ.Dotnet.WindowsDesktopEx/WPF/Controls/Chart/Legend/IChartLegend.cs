using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Legend接口
    /// </summary>
    public interface IChartLegend
    {
        /// <summary>
        /// 获取或设置Legend控件停靠方向
        /// </summary>
        ChartDockOrientation DockOrientation { get; set; }

        /// <summary>
        /// 获取或设置允许选中项
        /// </summary>
        bool AllowChecked { get; set; }

        /// <summary>
        /// 获取或设置水平方向高度;垂直方向宽高度
        /// </summary>
        double Size { get; set; }

        /// <summary>
        /// 获取或设置Legend控件水平对齐方式
        /// </summary>
        HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// 获取或设置Legend控件垂直对齐方式
        /// </summary>
        VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// 获取或设置Legend控件Margin
        /// </summary>
        Thickness Margin { get; set; }

        /// <summary>
        /// 获取或设置获取Legend控件
        /// </summary>
        FrameworkElement LegendControl { get; }

        /// <summary>
        /// 更新Legend控件显示项
        /// </summary>
        /// <param name="legendBrushList">Legend控件显示项列表</param>
        void UpdateLegend(List<SeriesLegendItem> legendBrushList);
    }
}
