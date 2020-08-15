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
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Base;

namespace CoreWpfApp
{
    /// <summary>
    /// TestWPFHelper.xaml 的交互逻辑
    /// </summary>
    public partial class TestWPFHelper : Window
    {
        public TestWPFHelper()
        {
            InitializeComponent();
        }
        private void btnGetTemplateControlList_Click(object sender, RoutedEventArgs e)
        {
            List<Visual> visualList = WPFHelper.GetTemplateControlList(slider);
            StringBuilder sb = new StringBuilder();
            FrameworkElement fe;
            int count = 0;

            foreach (var visual in visualList)
            {
                fe = visual as FrameworkElement;
                if (fe != null)
                {
                    sb.AppendLine($"Name:{fe.Name}      Type:{visual.GetType().FullName}");
                }
                else
                {
                    count++;
                }
            }

            if (count > 0)
            {
                sb.AppendLine($"no FrameworkElement :{count}");
            }

            string str = sb.ToString();
        }

        private void btnFindTemplateControl_Click(object sender, RoutedEventArgs e)
        {
            Visual result = WPFHelper.FindTemplateControl(slider, (v) =>
            {
                var fe = v as FrameworkElement;
                if (fe == null)
                {
                    return false;
                }

                return string.Equals("PART_Track", fe.Name);
            });
        }

        private void btnFindTemplateControlByName_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement result = WPFHelper.FindTemplateControlByName(slider, "PART_Track");
        }
    }
}
