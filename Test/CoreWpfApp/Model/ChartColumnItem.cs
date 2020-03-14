using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls;

namespace CoreWpfApp.Model
{
    public class ChartColumnItemHorizontal : ChartAxisValueAbs
    {
        public DateTime Y { get; set; }
        public double X { get; set; }


        public ChartColumnItemHorizontal(DateTime y, double x, string tooltip)
            : base(tooltip, null)
        {
            X = x;
            Y = y;
        }

        public override object GetXValue()
        {
            return this.X;
        }

        public override object GetYValue()
        {
            return this.Y;
        }
    }

    public class ChartColumnItemVertical : ChartAxisValueAbs
    {
        public double Y { get; set; }
        public DateTime X { get; set; }



        public ChartColumnItemVertical(DateTime x, double y, string tooltip)
             : base(tooltip, null)
        {
            X = x;
            Y = y;
        }

        public override object GetXValue()
        {
            return this.X;
        }

        public override object GetYValue()
        {
            return this.Y;
        }
    }









    public class ChartStackColumnItemVertical : ChartAxisValueAbs
    {
        public List<IChartChildValue> Y { get; set; }
        public DateTime X { get; set; }



        public ChartStackColumnItemVertical(DateTime x, List<IChartChildValue> y)
             : base()
        {
            X = x;
            Y = y;
        }

        public override object GetXValue()
        {
            return this.X;
        }

        public override object GetYValue()
        {
            return this.Y;
        }
    }

    public class ChartStackColumnItemHorizontal : ChartAxisValueAbs
    {
        public DateTime Y { get; set; }
        public List<IChartChildValue> X { get; set; }


        public ChartStackColumnItemHorizontal(DateTime y, List<IChartChildValue> x)
            : base()
        {
            X = x;
            Y = y;
        }

        public override object GetXValue()
        {
            return this.X;
        }

        public override object GetYValue()
        {
            return this.Y;
        }
    }

    public class ChartStackColumnChildItem : ChartChildValueAbs
    {
        private double _value;
        public ChartStackColumnChildItem(double value, string tooltipText, object tag)
            : base(tooltipText, tag)
        {
            this._value = value;
        }

        public override object GetValue()
        {
            return this._value;
        }
    }
}
