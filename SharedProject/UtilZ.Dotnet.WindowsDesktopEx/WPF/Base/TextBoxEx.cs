using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Base
{
    /// <summary>
    /// TextBox扩展类
    /// </summary>
    public class TextBoxEx
    {
        /// <summary>
        /// 当文本长度超出TextBox范围时，设置ToolTip
        /// </summary>
        /// <param name="textBox">TextBox</param>
        /// <param name="tooltip">tooltip</param>
        /// <param name="pre">误差值</param>
        public static void SetTextBoxToolTip(TextBox textBox, string tooltip, double pre = 5d)
        {
            if (double.IsInfinity(textBox.MaxWidth))
            {
                throw new ArgumentException("TextBox未设置MaxWidth值");
            }

            double width = UITextHelper.MeasureTextSize(textBox).Width;
            if (textBox.MaxWidth - width < pre)
            {
                textBox.ToolTip = tooltip;
            }
            else
            {
                textBox.ToolTip = null;
            }
        }
    }
}
