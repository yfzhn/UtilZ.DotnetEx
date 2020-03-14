using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls.PartAsynWait;

namespace CoreWpfApp
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            TestAsynWait();

            //metroRotaionIndicator.StartAnimal();
        }

        private void TestAsynWait()
        {
            var para = new PartAsynWaitPara<int, string>();
            para.Para = 100;
            para.Caption = "测试这个控件";
            para.Function = (inp) =>
            {
                for (int i = 0; i < inp.Para; i++)
                {
                    inp.AsynWait.Hint = string.Format("正在处理:{0}项..", i);
                    Thread.Sleep(500);
                    if (inp.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    //if (i > 5)
                    //{
                    //    throw new NotSupportedException("XXX");
                    //}
                }

                return "OK";
            };
            para.IsShowCancel = true;
            para.AsynWaitBackground = Brushes.Red;
            para.Completed = (p) =>
            {
                string str;
                switch (p.Status)
                {
                    case PartAsynExcuteStatus.Completed:
                        str = p.Result;
                        break;
                    case PartAsynExcuteStatus.Exception:
                        str = p.Exception.Message;
                        break;
                    case PartAsynExcuteStatus.Cancel:
                        str = "Cancel";
                        break;
                    default:
                        str = "none";
                        break;
                }

                MessageBox.Show(str);
            };

            WPFPartAsynWaitHelper.Wait(para, this);
        }
    }
}
