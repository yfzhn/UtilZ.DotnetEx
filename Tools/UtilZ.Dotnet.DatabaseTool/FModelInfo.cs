using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UtilZ.Dotnet.DatabaseTool
{
    public partial class FModelInfo : Form
    {
        public FModelInfo()
        {
            InitializeComponent();
        }

        private void FModelInfo_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtns.Text))
            {
                MessageBox.Show("命名空间不能为空");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(txtDir.Text))
            {
                MessageBox.Show("命名空间不能为空");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        public string Namespace
        {
            get { return txtns.Text.Trim(); }
        }

        public string BaseClassName
        {
            get
            {
                return txtBaseClassName.Text.Trim();
            }
        }

        public string Dir
        {
            get
            {
                return txtDir.Text.Trim();
            }
        }
    }
}
