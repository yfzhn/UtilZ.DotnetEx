using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UtilZ.Dotnet.Ex.Log;

namespace CoreWpfApp
{
    /// <summary>
    /// TestLogControl.xaml 的交互逻辑
    /// </summary>
    public partial class TestLogControl : Window
    {
        public TestLogControl()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private int _index = 1;
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog($"Trace_{this._index++}", LogLevel.Trace);
            logControl.AddLog($"Debug_{this._index++}", LogLevel.Debug);
            logControl.AddLog($"Info_{this._index++}", LogLevel.Info);
            logControl.AddLog($"Warn_{this._index++}", LogLevel.Warn);
            logControl.AddLog($"Error_{this._index++}", LogLevel.Error);
            logControl.AddLog($"Fatal_{this._index++}", LogLevel.Fatal);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            logControl.Clear();
        }
    }
}
