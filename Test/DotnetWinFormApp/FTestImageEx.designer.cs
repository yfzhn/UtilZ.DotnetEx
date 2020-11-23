
namespace DotnetWinFormApp
{
    partial class FTestImageEx
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
            this.btnOpenSrcImg = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSrcImgFilePath = new System.Windows.Forms.TextBox();
            this.txtDstImgFilePath = new System.Windows.Forms.TextBox();
            this.btnOpenDstImg = new System.Windows.Forms.Button();
            this.btnConvert = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenSrcImg
            // 
            this.btnOpenSrcImg.Location = new System.Drawing.Point(532, 21);
            this.btnOpenSrcImg.Name = "btnOpenSrcImg";
            this.btnOpenSrcImg.Size = new System.Drawing.Size(52, 23);
            this.btnOpenSrcImg.TabIndex = 0;
            this.btnOpenSrcImg.Text = "...";
            this.btnOpenSrcImg.UseVisualStyleBackColor = true;
            this.btnOpenSrcImg.Click += new System.EventHandler(this.btnOpenSrcImg_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "原始图片文件路径";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "目标图片文件路径";
            // 
            // txtSrcImgFilePath
            // 
            this.txtSrcImgFilePath.Location = new System.Drawing.Point(120, 23);
            this.txtSrcImgFilePath.Name = "txtSrcImgFilePath";
            this.txtSrcImgFilePath.Size = new System.Drawing.Size(406, 21);
            this.txtSrcImgFilePath.TabIndex = 3;
            // 
            // txtDstImgFilePath
            // 
            this.txtDstImgFilePath.Location = new System.Drawing.Point(120, 55);
            this.txtDstImgFilePath.Name = "txtDstImgFilePath";
            this.txtDstImgFilePath.Size = new System.Drawing.Size(406, 21);
            this.txtDstImgFilePath.TabIndex = 5;
            // 
            // btnOpenDstImg
            // 
            this.btnOpenDstImg.Location = new System.Drawing.Point(532, 53);
            this.btnOpenDstImg.Name = "btnOpenDstImg";
            this.btnOpenDstImg.Size = new System.Drawing.Size(52, 23);
            this.btnOpenDstImg.TabIndex = 4;
            this.btnOpenDstImg.Text = "...";
            this.btnOpenDstImg.UseVisualStyleBackColor = true;
            this.btnOpenDstImg.Click += new System.EventHandler(this.btnOpenDstImg_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(451, 82);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 6;
            this.btnConvert.Text = "转换";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "label3";
            // 
            // FTestImageEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.txtDstImgFilePath);
            this.Controls.Add(this.btnOpenDstImg);
            this.Controls.Add(this.txtSrcImgFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOpenSrcImg);
            this.Name = "FTestImageEx";
            this.Text = "FTestImageEx";
            this.Load += new System.EventHandler(this.FTestImageEx_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenSrcImg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSrcImgFilePath;
        private System.Windows.Forms.TextBox txtDstImgFilePath;
        private System.Windows.Forms.Button btnOpenDstImg;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Label label3;
    }
}