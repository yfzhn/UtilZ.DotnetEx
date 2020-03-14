using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 无坐标值基类
    /// </summary>
    public abstract class ChartNoAxisValueAbs : ChartValueAbs, IChartNoAxisValue
    {
        /// <summary>
        /// Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public Style Style { get; set; }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return this.Value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartNoAxisValueAbs()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="label">Label</param>
        /// <param name="title">标题</param>
        /// <param name="style">样式</param>
        /// <param name="tooltip">tooltip</param>
        public ChartNoAxisValueAbs(object value, string label, string title, Style style, string tooltip)
            : base(tooltip, null)
        {
            this.Value = value;
            this.Label = label;
            this.Title = title;
            this.Style = style;
        }
    }
}
