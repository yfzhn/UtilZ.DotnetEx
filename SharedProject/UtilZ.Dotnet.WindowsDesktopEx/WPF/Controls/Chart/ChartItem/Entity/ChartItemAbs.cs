using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart项基类
    /// </summary>
    public abstract class ChartItemAbs : IChartItem
    {
        /// <summary>
        /// Series上点显示的TooltipText
        /// </summary>
        public string TooltipText { get; set; }

        /// <summary>
        /// Tag
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartItemAbs()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartItemAbs(string tooltipText, object tag)
        {
            this.TooltipText = tooltipText;
            this.Tag = tag;
        }
    }
}
