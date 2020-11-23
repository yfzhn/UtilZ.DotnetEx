using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.WindowsDesktopEx.Base;

namespace DotnetWinFormApp
{
    public partial class FTestImageEx : Form
    {
        public FTestImageEx()
        {
            InitializeComponent();
        }

        private void FTestImageEx_Load(object sender, EventArgs e)
        {

        }

        private void btnOpenSrcImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "*.*|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtSrcImgFilePath.Text = ofd.FileName;
        }

        private void btnOpenDstImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.*|*.*";
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtDstImgFilePath.Text = sfd.FileName;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtSrcImgFilePath.Text))
            {
                return;
            }

            if (string.IsNullOrEmpty(txtDstImgFilePath.Text))
            {
                return;
            }

            try
            {
                ImageEx.ConvertToIcon(txtSrcImgFilePath.Text, txtDstImgFilePath.Text);
                label3.Text = "转换成功";
            }
            catch(Exception ex)
            {
                label3.Text = "转换失败";
                MessageBox.Show(ex.Message);
            }
        }
    }
}
