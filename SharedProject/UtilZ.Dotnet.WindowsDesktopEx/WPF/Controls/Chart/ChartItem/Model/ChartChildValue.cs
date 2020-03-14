using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// ChartValue子项实体类
    /// </summary>
    public class ChartChildValue : ChartChildValueAbs
    {
        /// <summary>
        /// 获取子项值
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartChildValue(object value)
            : base()
        {
            this.Value = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value">子项值</param>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartChildValue(object value, string tooltipText, object tag)
            : base(tooltipText, tag)
        {
            this.Value = value;
        }

        /// <summary>
        /// 获取子项值
        /// </summary>
        /// <returns>子项值</returns>
        public override object GetValue()
        {
            return this.Value;
        }
    }
}
