using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Series接口
    /// </summary>
    public interface ISeries : INotifyPropertyChanged
    {
        /// <summary>
        /// 获取或设置X坐标轴
        /// </summary>
        AxisAbs AxisX { get; set; }

        /// <summary>
        /// 获取或设置Y坐标轴
        /// </summary>
        AxisAbs AxisY { get; set; }

        /// <summary>
        /// 获取或设置创建坐标点对应的附加控件回调
        /// </summary>
        Func<PointInfo, FrameworkElement> CreatePointFunc { get; set; }

        /// <summary>
        /// Values集合改变事件
        /// </summary>
        event NotifyCollectionChangedEventHandler ValuesCollectionChanged;

        /// <summary>
        /// 获取或设置值集合
        /// </summary>
        ValueCollection Values { get; set; }

        /// <summary>
        /// 获取或设置Series样式
        /// </summary>
        Style Style { get; set; }

        /// <summary>
        /// 获取或设置是否启用Tooltip[true:启用Tooltip;false:禁用Tooltip]
        /// </summary>
        bool EnableTooltip { get; set; }

        /// <summary>
        /// 获取或设置Tooltip有效区域,鼠标点周围范围内有点则触发Tooltip,小于0使用默认值
        /// </summary>
        double TooltipArea { get; set; }

        /// <summary>
        /// 获取或设置Series标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 将Series绘制到已设置设定大小的画布中
        /// </summary>
        /// <param name="canvas">目标画布</param>
        void Draw(Canvas canvas);

        /// <summary>
        /// 将Series从画布中移除[返回值:true:需要全部重绘;false:不需要重绘]
        /// </summary>
        /// <returns>返回值:true:需要全部重绘;false:不需要重绘</returns>
        bool Clear();

        /// <summary>
        /// 更新Series
        /// </summary>
        void Update();

        /// <summary>
        /// 将Series中的Legend项追加到列表中
        /// </summary>
        /// <param name="legendBrushList">Legend列表</param>
        void AppendLegendItemToList(List<SeriesLegendItem> legendBrushList);

        /// <summary>
        /// 获取或设置SeriesVisibility
        /// </summary>
        Visibility Visibility { get; set; }

        /// <summary>
        /// 获取或设置Tag
        /// </summary>
        object Tag { get; set; }
    }
}
