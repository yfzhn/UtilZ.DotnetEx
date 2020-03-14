using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart Label值类
    /// </summary>
    public class ChartLabelValue : ChartLabelValueAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartLabelValue()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartLabelValue(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }
    }
}
