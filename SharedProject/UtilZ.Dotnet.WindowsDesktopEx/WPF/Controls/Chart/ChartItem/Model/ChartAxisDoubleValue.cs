using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 包含坐标双值项类
    /// </summary>
    public class ChartAxisDoubleValue : ChartAxisDoubleValueAbs
    {
        /// <summary>
        /// X轴坐标值1
        /// </summary>
        public IChartChildValue XValue1 { get; set; }

        /// <summary>
        /// X轴坐标值2
        /// </summary>
        public IChartChildValue XValue2 { get; set; }

        /// <summary>
        /// Y轴坐标值1
        /// </summary>
        public IChartChildValue YValue1 { get; set; }

        /// <summary>
        /// Y轴坐标值2
        /// </summary>
        public IChartChildValue YValue2 { get; set; }



        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartAxisDoubleValue()
              : base()
        {

        }



        /// <summary>
        /// 获取X轴坐标值1
        /// </summary>
        /// <returns>X轴坐标值</returns>
        public override IChartChildValue GetXValue1()
        {
            return this.XValue1;
        }

        /// <summary>
        /// 获取X轴坐标值2
        /// </summary>
        /// <returns>X轴坐标值</returns>
        public override IChartChildValue GetXValue2()
        {
            return this.XValue2;
        }



        /// <summary>
        /// 获取Y轴坐标值1
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        public override IChartChildValue GetYValue1()
        {
            return this.YValue1;
        }

        /// <summary>
        /// 获取Y轴坐标值2
        /// </summary>
        /// <returns>Y轴坐标值</returns>
        public override IChartChildValue GetYValue2()
        {
            return this.YValue2;
        }
    }
}
