using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Chart值基类
    /// </summary>
    public abstract class ChartValueAbs : ChartItemAbs, IChartValue
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartValueAbs()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tooltipText">tooltipText</param>
        /// <param name="tag">tag</param>
        public ChartValueAbs(string tooltipText, object tag)
            : base(tooltipText, tag)
        {

        }
    }
}
