using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// UCCabinet4U.xaml 的交互逻辑
    /// </summary>
    public partial class UCCabinetEmptyUControl : UserControl
    {
        public UCCabinetEmptyUControl()
        {
            InitializeComponent();
        }

        public void UpdateCabinetDevice(CabinetDeviceUnit deviceUnit)
        {
            this.Height = deviceUnit.Height * CabinetConstant.SINGLE_U_HEIGHT;

            grid.RowDefinitions.Clear();
            double rowHeight = this.Height / deviceUnit.Height;
            int index = deviceUnit.BeginLocation + deviceUnit.Height - 1;

            for (int i = 0; i < deviceUnit.Height; i++)
            {
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(rowHeight, GridUnitType.Pixel);
                grid.RowDefinitions.Add(rowDefinition);

                var cabinetUnitControl = new UCCabinetEmptyUCalibrationControl();
                cabinetUnitControl.DataContext = new CabinetUnit(index);
                grid.Children.Add(cabinetUnitControl);
                Grid.SetRow(cabinetUnitControl, i);
                index--;
            }
        }
    }
}
