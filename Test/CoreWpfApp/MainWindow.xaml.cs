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
using System.Xml;
using System.Xml.Linq;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Ex.Log;

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





        private void btnXmlExXmlNode_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("logConfig.xml");
            XmlNode loggerNode = xmlDoc.SelectSingleNode(@"logconfig/loger");
            string name = XmlEx.GetXmlNodeAttributeValue(loggerNode, "level");
            string name2 = XmlEx.GetXmlNodeAttributeValue(loggerNode, "level2", false);
            string name3 = XmlEx.GetXmlNodeAttributeValue(loggerNode, "level3", true);

            bool enable = XmlEx.GetXmlNodeAttributeValue<bool>(loggerNode, "enable");
            bool enable2 = XmlEx.GetXmlNodeAttributeValue<bool>(loggerNode, "enable2");



            xmlDoc.Load("UtilZ.Dotnet.Ex.xml");
            XmlNode nameNode = xmlDoc.SelectSingleNode(@"doc/assembly/name");
            string ret1 = XmlEx.GetXmlNodeValue(nameNode);
            string ret2 = XmlEx.GetXmlNodeValue(null, true);
            string ret3 = XmlEx.GetXmlNodeValue(null, false);

            XmlNode ageNode = XmlEx.CreateXmlNode(xmlDoc, "Age", "nongli", "2.15", "23");
            int ret4 = XmlEx.GetXmlNodeValue<int>(ageNode);
            int ret5 = XmlEx.GetXmlNodeValue<int>(null);

            XmlEx.SetXmlNodeAttribute(ageNode, "bir", "2012", true);
            XmlEx.SetXmlNodeAttribute(ageNode, "bir2", null, true);
            XmlEx.SetXmlNodeAttribute(ageNode, "bir3", null, false);


            XmlNode noteNode = XmlEx.CreateXmlCDataSection(xmlDoc, "AgeNote", "comment...");
            string ret6 = XmlEx.GetXmlNodeValue(noteNode, false);
            string ret7 = XmlEx.GetXmlNodeValue(noteNode, true);
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
                Loger.Error(ex);
            }
        }
    }
}
