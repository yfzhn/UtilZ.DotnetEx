using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Base.PartAsynWait.Model;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait;

namespace DotnetWinFormApp
{
    public partial class FTestPartAsynWait : Form
    {
        public FTestPartAsynWait()
        {
            InitializeComponent();
        }
        
        private readonly List<CobITemInfo> items = new List<CobITemInfo>();
        private void FTestPartAsynWait_Load(object sender, EventArgs e)
        {
            items.Add(new CobITemInfo("yf", 23));
            items.Add(new CobITemInfo("zhn", 31));
            items.Add(new CobITemInfo("tq", 18));
            DropdownBoxHelper.BindingIEnumerableGenericToComboBox<CobITemInfo>(comboBox1, items);
            //DropdownBoxHelper.BindingIEnumerableGenericToComboBox<CobITemInfo>(comboBox1, items, string.Empty);
            //DropdownBoxHelper.BindingIEnumerableGenericToComboBox<CobITemInfo>(comboBox1, t => { return t.Name; }, items);

            //DropdownBoxHelper.BindingIEnumerableGenericToToolStripComboBox<CobITemInfo>(toolStripComboBox1, items, nameof(CobITemInfo.Name));
            //DropdownBoxHelper.BindingIEnumerableGenericToToolStripComboBox<CobITemInfo>(toolStripComboBox1, items);
            DropdownBoxHelper.BindingIEnumerableGenericToToolStripComboBox<CobITemInfo>(toolStripComboBox1, t => { return t.Name; }, items);
        }

        private void btnTestAsynWait_Click(object sender, EventArgs e)
        {
            var para = new PartAsynWaitPara<int, string>();
            para.Para = 10;
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

                    if (i > 5)
                    {
                        throw new NotSupportedException("XXX");
                    }
                }

                return "OK";
            };
            para.CancelButtonVisible = true;
            para.AsynWaitBackground = Color.Red;
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

                MessageBox.Show(this, str);
            };

            WinformPartAsynWaitHelper.Wait(para, this);
        }

        private void btnSetGenericToComboBox_Click(object sender, EventArgs e)
        {
            DropdownBoxHelper.SetGenericToComboBox(comboBox1, items[1]);
        }

        private void btnGetGenericFromComboBox_Click(object sender, EventArgs e)
        {
            btnGetGenericFromComboBox.Text = DropdownBoxHelper.GetGenericFromComboBox<CobITemInfo>(comboBox1).Name;
        }

        private void btnSetGenericToToolStripComboBox_Click(object sender, EventArgs e)
        {
            DropdownBoxHelper.SetGenericToToolStripComboBox(toolStripComboBox1, items[1]);
        }

        private void btnGetGenericFromToolStripComboBox_Click(object sender, EventArgs e)
        {
            btnGetGenericFromToolStripComboBox.Text = DropdownBoxHelper.GetGenericFromToolStripComboBox<CobITemInfo>(toolStripComboBox1).Name;
        }
    }

    public class CobITemInfo
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public CobITemInfo(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return string.Format("Name:{0},Age:{1}", Name, Age);
        }
    }
}
