using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 雷达图值类
    /// </summary>
    public class ChartRadarValue : ChartRadarValueAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartRadarValue()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartRadarValue(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }
    }
}
