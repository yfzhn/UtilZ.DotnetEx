namespace DotnetWinFormApp
{
    partial class FTestFTP
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDirExists = new System.Windows.Forms.Button();
            this.btnCreateDir = new System.Windows.Forms.Button();
            this.btnUploadFile = new System.Windows.Forms.Button();
            this.btnDownloadFile = new System.Windows.Forms.Button();
            this.btnDeleteFile = new System.Windows.Forms.Button();
            this.btnDirList = new System.Windows.Forms.Button();
            this.btnFileList = new System.Windows.Forms.Button();
            this.btnDeleteDir = new System.Windows.Forms.Button();
            this.btnDownloadDir = new System.Windows.Forms.Button();
            this.btnUploadDir = new System.Windows.Forms.Button();
            this.rtxtMsg = new System.Windows.Forms.RichTextBox();
            this.btnFileExist = new System.Windows.Forms.Button();
            this.btnFileLength = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDirExists
            // 
            this.btnDirExists.Location = new System.Drawing.Point(25, 14);
            this.btnDirExists.Name = "btnDirExists";
            this.btnDirExists.Size = new System.Drawing.Size(75, 23);
            this.btnDirExists.TabIndex = 0;
            this.btnDirExists.Text = " 目录存在";
            this.btnDirExists.UseVisualStyleBackColor = true;
            this.btnDirExists.Click += new System.EventHandler(this.btnDirExists_Click);
            // 
            // btnCreateDir
            // 
            this.btnCreateDir.Location = new System.Drawing.Point(125, 14);
            this.btnCreateDir.Name = "btnCreateDir";
            this.btnCreateDir.Size = new System.Drawing.Size(75, 23);
            this.btnCreateDir.TabIndex = 1;
            this.btnCreateDir.Text = "创建目录";
            this.btnCreateDir.UseVisualStyleBackColor = true;
            this.btnCreateDir.Click += new System.EventHandler(this.btnCreateDir_Click);
            // 
            // btnUploadFile
            // 
            this.btnUploadFile.Location = new System.Drawing.Point(406, 12);
            this.btnUploadFile.Name = "btnUploadFile";
            this.btnUploadFile.Size = new System.Drawing.Size(75, 23);
            this.btnUploadFile.TabIndex = 2;
            this.btnUploadFile.Text = "上传文件";
            this.btnUploadFile.UseVisualStyleBackColor = true;
            this.btnUploadFile.Click += new System.EventHandler(this.btnUploadFile_Click);
            // 
            // btnDownloadFile
            // 
            this.btnDownloadFile.Location = new System.Drawing.Point(498, 11);
            this.btnDownloadFile.Name = "btnDownloadFile";
            this.btnDownloadFile.Size = new System.Drawing.Size(75, 23);
            this.btnDownloadFile.TabIndex = 3;
            this.btnDownloadFile.Text = "下载文件";
            this.btnDownloadFile.UseVisualStyleBackColor = true;
            this.btnDownloadFile.Click += new System.EventHandler(this.btnDownloadFile_Click);
            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Location = new System.Drawing.Point(496, 50);
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteFile.TabIndex = 4;
            this.btnDeleteFile.Text = "删除文件";
            this.btnDeleteFile.UseVisualStyleBackColor = true;
            this.btnDeleteFile.Click += new System.EventHandler(this.btnDeleteFile_Click);
            // 
            // btnDirList
            // 
            this.btnDirList.Location = new System.Drawing.Point(25, 51);
            this.btnDirList.Name = "btnDirList";
            this.btnDirList.Size = new System.Drawing.Size(75, 23);
            this.btnDirList.TabIndex = 5;
            this.btnDirList.Text = "目录列表";
            this.btnDirList.UseVisualStyleBackColor = true;
            this.btnDirList.Click += new System.EventHandler(this.btnDirList_Click);
            // 
            // btnFileList
            // 
            this.btnFileList.Location = new System.Drawing.Point(125, 50);
            this.btnFileList.Name = "btnFileList";
            this.btnFileList.Size = new System.Drawing.Size(75, 23);
            this.btnFileList.TabIndex = 6;
            this.btnFileList.Text = "文件列表";
            this.btnFileList.UseVisualStyleBackColor = true;
            this.btnFileList.Click += new System.EventHandler(this.btnFileList_Click);
            // 
            // btnDeleteDir
            // 
            this.btnDeleteDir.Location = new System.Drawing.Point(406, 51);
            this.btnDeleteDir.Name = "btnDeleteDir";
            this.btnDeleteDir.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteDir.TabIndex = 7;
            this.btnDeleteDir.Text = "删除目录";
            this.btnDeleteDir.UseVisualStyleBackColor = true;
            this.btnDeleteDir.Click += new System.EventHandler(this.btnDeleteDir_Click);
            // 
            // btnDownloadDir
            // 
            this.btnDownloadDir.Location = new System.Drawing.Point(312, 50);
            this.btnDownloadDir.Name = "btnDownloadDir";
            this.btnDownloadDir.Size = new System.Drawing.Size(75, 23);
            this.btnDownloadDir.TabIndex = 8;
            this.btnDownloadDir.Text = "下载目录";
            this.btnDownloadDir.UseVisualStyleBackColor = true;
            this.btnDownloadDir.Click += new System.EventHandler(this.btnDownloadDir_Click);
            // 
            // btnUploadDir
            // 
            this.btnUploadDir.Location = new System.Drawing.Point(220, 50);
            this.btnUploadDir.Name = "btnUploadDir";
            this.btnUploadDir.Size = new System.Drawing.Size(75, 23);
            this.btnUploadDir.TabIndex = 9;
            this.btnUploadDir.Text = "上传目录";
            this.btnUploadDir.UseVisualStyleBackColor = true;
            this.btnUploadDir.Click += new System.EventHandler(this.btnUploadDir_Click);
            // 
            // rtxtMsg
            // 
            this.rtxtMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtMsg.Location = new System.Drawing.Point(22, 80);
            this.rtxtMsg.Name = "rtxtMsg";
            this.rtxtMsg.ReadOnly = true;
            this.rtxtMsg.Size = new System.Drawing.Size(635, 279);
            this.rtxtMsg.TabIndex = 10;
            this.rtxtMsg.Text = "";
            // 
            // btnFileExist
            // 
            this.btnFileExist.Location = new System.Drawing.Point(220, 14);
            this.btnFileExist.Name = "btnFileExist";
            this.btnFileExist.Size = new System.Drawing.Size(75, 23);
            this.btnFileExist.TabIndex = 11;
            this.btnFileExist.Text = "文件是否存在";
            this.btnFileExist.UseVisualStyleBackColor = true;
            this.btnFileExist.Click += new System.EventHandler(this.btnFileExist_Click);
            // 
            // btnFileLength
            // 
            this.btnFileLength.Location = new System.Drawing.Point(312, 14);
            this.btnFileLength.Name = "btnFileLength";
            this.btnFileLength.Size = new System.Drawing.Size(75, 23);
            this.btnFileLength.TabIndex = 12;
            this.btnFileLength.Text = "文件大小";
            this.btnFileLength.UseVisualStyleBackColor = true;
            this.btnFileLength.Click += new System.EventHandler(this.btnFileLength_Click);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(585, 11);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 13;
            this.btnRename.Text = "重命名";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // FTestFTP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 371);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnFileLength);
            this.Controls.Add(this.btnFileExist);
            this.Controls.Add(this.rtxtMsg);
            this.Controls.Add(this.btnUploadDir);
            this.Controls.Add(this.btnDownloadDir);
            this.Controls.Add(this.btnDeleteDir);
            this.Controls.Add(this.btnFileList);
            this.Controls.Add(this.btnDirList);
            this.Controls.Add(this.btnDeleteFile);
            this.Controls.Add(this.btnDownloadFile);
            this.Controls.Add(this.btnUploadFile);
            this.Controls.Add(this.btnCreateDir);
            this.Controls.Add(this.btnDirExists);
            this.Name = "FTestFTP";
            this.Text = "FTestFTP";
            this.Load += new System.EventHandler(this.FTestFTP_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDirExists;
        private System.Windows.Forms.Button btnCreateDir;
        private System.Windows.Forms.Button btnUploadFile;
        private System.Windows.Forms.Button btnDownloadFile;
        private System.Windows.Forms.Button btnDeleteFile;
        private System.Windows.Forms.Button btnDirList;
        private System.Windows.Forms.Button btnFileList;
        private System.Windows.Forms.Button btnDeleteDir;
        private System.Windows.Forms.Button btnDownloadDir;
        private System.Windows.Forms.Button btnUploadDir;
        private System.Windows.Forms.RichTextBox rtxtMsg;
        private System.Windows.Forms.Button btnFileExist;
        private System.Windows.Forms.Button btnFileLength;
        private System.Windows.Forms.Button btnRename;
    }
}