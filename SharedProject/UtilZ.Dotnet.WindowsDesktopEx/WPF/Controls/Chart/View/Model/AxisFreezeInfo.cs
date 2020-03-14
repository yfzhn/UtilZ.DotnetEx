using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal class AxisFreezeInfo
    {
        public double Width { get; private set; }

        public double Height { get; private set; }

        public AxisFreezeType AxisFreezeType { get; private set; }

        public AxisFreezeInfo(double width, double height, AxisFreezeType axisFreezeType)
        {
            this.Width = width;
            this.Height = height;
            this.AxisFreezeType = axisFreezeType;
        }
    }
}
