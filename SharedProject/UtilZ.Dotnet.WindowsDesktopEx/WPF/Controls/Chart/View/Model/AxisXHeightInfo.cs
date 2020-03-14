using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal class AxisXHeightInfo
    {
        public double TopAxisTotalHeight { get; private set; }
        public double BottomAxisTotalHeight { get; private set; }

        public AxisXHeightInfo(double topAxisTotalHeight, double bottomAxisTotalHeight)
        {
            this.TopAxisTotalHeight = topAxisTotalHeight;
            this.BottomAxisTotalHeight = bottomAxisTotalHeight;
        }
    }
}
