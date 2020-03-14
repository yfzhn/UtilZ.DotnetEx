using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Base
{
    /// <summary>
    /// TextBlock扩展类
    /// </summary>
    public class TextBlockEx
    {
        /// <summary>
        /// 当文本长度超出TextBlock范围时，设置ToolTip
        /// </summary>
        /// <param name="textBlock">TextBlock</param>
        /// <param name="tooltip">tooltip</param>
        /// <param name="pre">误差值</param>
        public static void SetTextBlockToolTip(TextBlock textBlock, string tooltip, double pre = 5d)
        {
            if (double.IsInfinity(textBlock.MaxWidth))
            {
                throw new ArgumentException("TextBlock未设置MaxWidth值");
            }

            double width = UITextHelper.MeasureTextSize(textBlock).Width;
            if (textBlock.MaxWidth - width < pre)
            {
                textBlock.ToolTip = tooltip;
            }
            else
            {
                textBlock.ToolTip = null;
            }
        }
    }
}
