using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// ChartValue子项基类
    /// </summary>
    public abstract class ChartChildValueAbs : ChartItemAbs, IChartChildValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartChildValueAbs()
            : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartChildValueAbs(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }

        /// <summary>
        /// 获取子项值
        /// </summary>
        /// <returns>子项值</returns>
        public abstract object GetValue();
    }
}
