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

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            logControl.AddLog("Trace", LogLevel.Trace);
            logControl.AddLog("Debug", LogLevel.Debug);
            logControl.AddLog("Info", LogLevel.Info);
            logControl.AddLog("Warn", LogLevel.Warn);
            logControl.AddLog("Error", LogLevel.Error);
            logControl.AddLog("Fatal", LogLevel.Fatal);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            logControl.Clear();
        }
    }
}
