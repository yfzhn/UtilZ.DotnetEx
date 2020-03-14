using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 雷达图值基类
    /// </summary>
    public abstract class ChartRadarValueAbs : ChartValueAbs, IChartRadarValue
    {
        /// <summary>
        /// Label
        /// </summary>
        public IChartLabelValue Label { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public double Value { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartRadarValueAbs()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartRadarValueAbs(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }


        /// <summary>
        /// 获取Label
        /// </summary>
        /// <returns>Label</returns>
        public IChartLabelValue GetLabel()
        {
            return this.Label;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns>值</returns>
        public double GetValue()
        {
            return this.Value;
        }
    }
}
