using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart Label值基类
    /// </summary>
    public abstract class ChartLabelValueAbs : ChartValueAbs, IChartLabelValue
    {
        /// <summary>
        /// Label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Label线样式
        /// </summary>
        public Style LabelLineStyle { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public Style LabelStyle { get; set; }

        /// <summary>
        /// 最小值,double.NaN自动计算
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// 最大值,double.NaN自动计算
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartLabelValueAbs()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartLabelValueAbs(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }


        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return Label;
        }
    }
}
