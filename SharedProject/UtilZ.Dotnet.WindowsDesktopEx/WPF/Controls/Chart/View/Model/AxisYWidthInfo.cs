using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal class AxisYWidthInfo
    {
        public double LeftAxisTotalWidth { get; private set; }
        public double RightAxisTotalWidth { get; private set; }

        public Dictionary<AxisAbs, List<double>> AxisYLabelDic { get; private set; }

        public AxisYWidthInfo(double leftAxisTotalWidth, double rightAxisTotalWidth, Dictionary<AxisAbs, List<double>> axisYLabelDic)
        {
            this.LeftAxisTotalWidth = leftAxisTotalWidth;
            this.RightAxisTotalWidth = rightAxisTotalWidth;
            this.AxisYLabelDic = axisYLabelDic;
        }
    }
}
