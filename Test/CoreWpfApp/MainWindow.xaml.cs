using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;
using UtilZ.Dotnet.Ex.Base;

namespace CoreWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

       
        private void btnXmlEx_Click(object sender, RoutedEventArgs e)
        {
            XElement ele = new XElement("Person");
            XmlEx.SetXElementAttribute(ele, "name", "yf", false);
            XmlEx.SetXElementAttribute(ele, "age", null, false);
            XmlEx.SetXElementAttribute(ele, "addr", "chengdu", true);
            XmlEx.SetXElementAttribute(ele, "sex", null, true);

            XmlEx.SetXElementAttribute(ele, "name", null, false);
            XmlEx.SetXElementAttribute(ele, "addr", null, true);
        }

      



        private ThreadEx _thread = null;
        private void btnThreadEx_Click(object sender, RoutedEventArgs e)
        {

            if (_thread != null)
            {
                _thread.Stop();
                _thread.Dispose();
                _thread = null;
                return;
            }

            _thread = new ThreadEx(ThreadMethod, "xx", true);
            _thread.Start();
        }

        private void ThreadMethod(ThreadExPara para)
        {
            try
            {
                while (!para.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (para.WaitOne(10000))
                        {
                            continue;
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
