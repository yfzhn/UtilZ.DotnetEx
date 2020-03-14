using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// 无坐标值实体类
    /// </summary>
    public class ChartNoAxisValue : ChartNoAxisValueAbs
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ChartNoAxisValue()
              : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        /// <param name="title"></param>
        /// <param name="style"></param>
        /// <param name="tooltip"></param>
        public ChartNoAxisValue(object value, string label, string title, Style style, string tooltip)
       : base(value, label, title, style, tooltip)
        {

        }
    }
}
