using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.TypeConverters;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo
{
    /// <summary>
    /// 地址
    /// </summary>
    public class NAddress
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 重写ToString
        /// </summary>
        public override string ToString()
        {
            return Text;
        }
    }
}
