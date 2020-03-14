using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal class LegendAddResult
    {
        public bool HasLegend { get; private set; }

        public double Bottom { get; private set; }

        public double Left { get; private set; }

        public double Right { get; private set; }

        public double Top { get; private set; }

        public LegendAddResult(bool hasLegend, double left, double top, double right, double bottom)
        {
            this.HasLegend = hasLegend;
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }
}
