using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 包含坐标值项实体类
    /// </summary>
    public class ChartAxisValue : ChartAxisValueAbs
    {
        /// <summary>
        /// 获取X轴值
        /// </summary>
        public object XValue { get; private set; }

        /// <summary>
        /// 获取Y轴值
        /// </summary>
        public object YValue { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xValue">X轴值</param>
        /// <param name="yValue">Y轴值</param>
        public ChartAxisValue(object xValue, object yValue)
          : base()
        {
            this.XValue = xValue;
            this.YValue = yValue;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xValue">X轴值</param>
        /// <param name="yValue">Y轴值</param>
        /// <param name="tooltipText">tooltipText</param>
        public ChartAxisValue(object xValue, object yValue, string tooltipText)
          : base(tooltipText, null)
        {
            this.XValue = xValue;
            this.YValue = yValue;
        }

        /// <summary>
        /// 获取X轴坐标值
        /// </summary>
        /// <returns>X轴坐标值</returns>
        public override object GetXValue()
        {
            return this.XValue;
        }

        /// <summary>
        /// 获取Y轴坐标值
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        public override object GetYValue()
        {
            return this.YValue;
        }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"XValue:{XValue},YValue:{YValue}";
        }
    }
}
