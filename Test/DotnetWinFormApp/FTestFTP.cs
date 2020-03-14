using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.FileTransfer;

namespace DotnetWinFormApp
{
    public partial class FTestFTP : Form
    {
        private readonly IFileTransfer _ftp;
        public FTestFTP()
        {
            InitializeComponent();

            string ftpUrl = @"ftp://192.168.0.102/";
            string ftpUserName = string.Empty;
            string ftpPassword = string.Empty;
            this._ftp = new FtpFileTransfer(ftpUrl, ftpUserName, ftpPassword);
        }

        private void FTestFTP_Load(object sender, EventArgs e)
        {

        }

        private void btnDirExists_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = @"q1";
                bool ret = this._ftp.ExistDirectory(dir);

                dir = @"/q1";
                bool ret2 = this._ftp.ExistDirectory(dir);

                dir = @"q1/";
                bool ret3 = this._ftp.ExistDirectory(dir);

                dir = @"\q1";
                bool ret4 = this._ftp.ExistDirectory(dir);

                dir = @"\q1\";
                bool ret5 = this._ftp.ExistDirectory(dir);

                dir = @"\q1/";
                bool ret6 = this._ftp.ExistDirectory(dir);

                dir = @"/q1\";
                bool ret7 = this._ftp.ExistDirectory(dir);

                dir = @"\q1/a";
                bool ret8 = this._ftp.ExistDirectory(dir);

                dir = @"\q1/a/";
                bool ret9 = this._ftp.ExistDirectory(dir);

                dir = @"\q1/a\";
                bool ret10 = this._ftp.ExistDirectory(dir);

                dir = @"q1/a/";
                bool ret11 = this._ftp.ExistDirectory(dir);

                dir = @"q1/a\";
                bool ret12 = this._ftp.ExistDirectory(dir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnCreateDir_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = @"b1";
                this._ftp.CreateDirectory(dir);

                dir = @"\b2";
                this._ftp.CreateDirectory(dir);

                dir = @"b3\";
                this._ftp.CreateDirectory(dir);

                dir = @"\b4\";
                this._ftp.CreateDirectory(dir);

                dir = @"/b5";
                this._ftp.CreateDirectory(dir);

                dir = @"b6/";
                this._ftp.CreateDirectory(dir);

                dir = @"/b7";
                this._ftp.CreateDirectory(dir);



                dir = @"b8\c";
                this._ftp.CreateDirectory(dir);

                dir = @"b9/c";
                this._ftp.CreateDirectory(dir);


                dir = @"\b10\c";
                this._ftp.CreateDirectory(dir);

                dir = @"/b11\c";
                this._ftp.CreateDirectory(dir);



                dir = @"\b12\c/";
                this._ftp.CreateDirectory(dir);

                dir = @"/b13\c\";
                this._ftp.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnFileExist_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteFilePath = @"PowerMode.vsix";
                bool ret = this._ftp.ExistFile(remoteFilePath);

                remoteFilePath = @"abc.msi";
                bool ret2 = this._ftp.ExistFile(remoteFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnFileLength_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteFilePath = @"PowerMode.vsix";
                long length = this._ftp.GetFileLength(remoteFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string localFilePath, remoteFilePath;

                /*localFilePath = @"F:\Soft\KKSetup_2000.exe";
                remoteFilePath = @"KKSetup_2000.exe";
                this._extendFTP.UploadFile(localFilePath, remoteFilePath, 2048, true);

                localFilePath = @"F:\Soft\dotNetFx40_Full_x86_x64.exe";
                remoteFilePath = @"Soft\dotNetFx40_Full_x86_x64.exe";
                this._extendFTP.UploadFile(localFilePath, remoteFilePath, 2048, true);



                localFilePath = @"F:\Soft\feiq.rar";
                remoteFilePath = @"feiq.rar";
                this._extendFTP.UploadFile(localFilePath, remoteFilePath, 2048, true);*/


                remoteFilePath = @"12/刀剑心.mp3";
                localFilePath = @"F:\刀剑心.mp3";
                using (var fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    this._ftp.Upload(remoteFilePath, fs);
                }
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnDownloadFile_Click(object sender, EventArgs e)
        {
            try
            {
                string localFilePath = @"G:\Tmp\test\dotNetFx40_Full_x86_x64.exe";
                string remoteFilePath = @"Soft\dotNetFx40_Full_x86_x64.exe";
                this._ftp.Download(remoteFilePath, localFilePath);

                localFilePath = @"G:\Tmp\test\feiq.rar";
                remoteFilePath = @"feiq.rar";
                this._ftp.Download(remoteFilePath, localFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            string oldFileName = "流程图.vsd";
            string newFileName = "FlowImg.vsd";
            try
            {
                this._ftp.Rename(oldFileName, newFileName);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteFilePath = @"feiq.rar";
                this._ftp.DeleteFile(remoteFilePath);

                remoteFilePath = @"Soft/dotNetFx40_Full_x86_x64.exe";
                this._ftp.DeleteFile(remoteFilePath);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnDirList_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteDir = null;
                var dirList = this._ftp.GetDirectories(remoteDir);

                remoteDir = @"a";
                var dirList2 = this._ftp.GetDirectories(remoteDir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnFileList_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteDir = null;
                var dirList = this._ftp.GetFiles(remoteDir);

                remoteDir = @"a";
                var dirList2 = this._ftp.GetFiles(remoteDir);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }

        private void btnUploadDir_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                string localDir = @"G:\Tmp_Study\2014-11-4";
                try
                {
                    string remoteDir = @"2014-11-4";
                    this._ftp.UploadDirectory(localDir, remoteDir);
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        rtxtMsg.AppendText(ex.Message);
                        rtxtMsg.AppendText(Environment.NewLine);
                    }));
                }
            });
        }

        private void btnDownloadDir_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                string localDir = @"G:\Tmp\test\2014-11-4";
                try
                {
                    string remoteDir = @"a\2014-11-4";
                    this._ftp.DownloadDirectory(localDir, remoteDir);
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        rtxtMsg.AppendText(ex.Message);
                        rtxtMsg.AppendText(Environment.NewLine);
                    }));
                }
            });
        }

        private void btnDeleteDir_Click(object sender, EventArgs e)
        {
            try
            {
                string remoteDir = @"Soft";
                remoteDir = @"新寻22";
                //this._ftp.DeleteDirectory(remoteDir, true);

                remoteDir = @"bb";
                this._ftp.DeleteDirectory(remoteDir, true);
            }
            catch (Exception ex)
            {
                rtxtMsg.AppendText(ex.Message);
                rtxtMsg.AppendText(Environment.NewLine);
            }
        }
    }
}
