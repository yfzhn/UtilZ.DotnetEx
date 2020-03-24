namespace UtilZ.Dotnet.DatabaseTool
{
    partial class FModelInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtns = new System.Windows.Forms.TextBox();
            this.txtBaseClassName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "命名空间";
            // 
            // txtns
            // 
            this.txtns.Location = new System.Drawing.Point(72, 13);
            this.txtns.Name = "txtns";
            this.txtns.Size = new System.Drawing.Size(336, 21);
            this.txtns.TabIndex = 1;
            this.txtns.Text = "App.Service.ExecTask.Tables";
            // 
            // txtBaseClassName
            // 
            this.txtBaseClassName.Location = new System.Drawing.Point(72, 50);
            this.txtBaseClassName.Name = "txtBaseClassName";
            this.txtBaseClassName.Size = new System.Drawing.Size(336, 21);
            this.txtBaseClassName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "基类名称";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(333, 105);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "生成";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtDir
            // 
            this.txtDir.Location = new System.Drawing.Point(72, 78);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(336, 21);
            this.txtDir.TabIndex = 6;
            this.txtDir.Text = "E:\\Tmp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "生成目录";
            // 
            // FModelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 137);
            this.Controls.Add(this.txtDir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtBaseClassName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtns);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FModelInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据模型信息";
            this.Load += new System.EventHandler(this.FModelInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtns;
        private System.Windows.Forms.TextBox txtBaseClassName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Label label3;
    }
}