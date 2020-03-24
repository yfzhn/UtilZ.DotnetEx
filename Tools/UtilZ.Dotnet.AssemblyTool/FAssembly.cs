using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace UtilZ.Dotnet.AssemblyTool
{
    public partial class FAssembly : Form
    {
        public FAssembly()
        {
            InitializeComponent();
        }

        private void btnAssemblyChioce_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = @"*.dll|*.dll";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtAssemblyPath.Text = ofd.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtAssemblyPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string filePath = txtAssemblyPath.Text.Trim();
                if (!File.Exists(filePath))
                {
                    rtxtAssemblyInfo.Clear();
                    return;
                }

                AssemblyName assemblyName = AssemblyName.GetAssemblyName(filePath);
                //Assembly assembly = Assembly.LoadFile(filePath);
                StringBuilder sb = new StringBuilder();

                sb.Append("Name:");
                sb.AppendLine(assemblyName.Name);

                sb.Append("FullName:");
                sb.AppendLine(assemblyName.FullName);

                rtxtAssemblyInfo.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tsmiAssemblyInfoCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(rtxtAssemblyInfo.SelectedText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tsmiAssemblyInfoSelectedAll_Click(object sender, EventArgs e)
        {
            try
            {
                rtxtAssemblyInfo.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
