using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// Series值集合
    /// </summary>
    public class ValueCollection : ChartCollection<IChartValue>
    {
        /// <summary>
        /// 参数值名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValueCollection()
            : base()
        {

        }
    }
}
