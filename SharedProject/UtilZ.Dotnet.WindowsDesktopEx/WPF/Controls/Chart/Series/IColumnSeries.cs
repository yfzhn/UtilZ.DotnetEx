using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    internal interface IColumnSeries : ISeries
    {
        SeriesOrientation Orientation { get; set; }

        double Size { get; set; }

        Style GetStyle();
    }
}
