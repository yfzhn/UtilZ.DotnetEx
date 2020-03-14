using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls;

namespace CoreWpfApp.Model
{
    public class ChartDateTimeItem : ChartAxisValueAbs
    {
        private DateTime _x;
        public DateTime X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y { get; set; }

        public ChartDateTimeItem(DateTime x, double value, string tooltipText)
            : base(tooltipText, null)
        {
            this._x = x;
            this.Y = value;
        }

        public override string ToString()
        {
            return $"{_x} {Y}";
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


 
}
