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
    /// TestLoger.xaml 的交互逻辑
    /// </summary>
    public partial class TestLoger : Window
    {
        public TestLoger()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var redirectAppenderToUI = (RedirectAppender)Loger.GetAppenderByName(null, "RedirectToUI");
            if (redirectAppenderToUI != null)
            {
                redirectAppenderToUI.RedirectOuput += RedirectLogOutput;
            }
        }

        private void RedirectLogOutput(object sender, RedirectOuputArgs e)
        {
            logControl.AddLog(e.Item.Content, e.Item.Level);

            //string str;
            //try
            //{
            //    str = string.Format("{0} {1}", DateTime.Now, e.Item.Content);
            //}
            //catch (Exception ex)
            //{
            //    str = ex.Message;
            //}

            //logControl.AddLog(str, e.Item.Level);
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Loger.Trace("Trace");
            Loger.Debug("Debug");
            Loger.Info("Info");
            Loger.Warn("Warn");
            Loger.Error("Error");
            Loger.Fatal("Fatal");
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExpire_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
