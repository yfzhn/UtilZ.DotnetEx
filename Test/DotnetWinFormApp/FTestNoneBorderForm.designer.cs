namespace DotnetWinFormApp
{
    partial class FTestNoneBorderForm
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
            this.btnMax = new System.Windows.Forms.Button();
            this.btnMin = new System.Windows.Forms.Button();
            this.btnNM = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cbAllowClose = new System.Windows.Forms.CheckBox();
            this.cbFullScreen = new System.Windows.Forms.CheckBox();
            this.btnTestLog = new System.Windows.Forms.Button();
            this.btnWriteLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMax
            // 
            this.btnMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMax.Location = new System.Drawing.Point(385, 12);
            this.btnMax.Name = "btnMax";
            this.btnMax.Size = new System.Drawing.Size(75, 23);
            this.btnMax.TabIndex = 0;
            this.btnMax.Text = "Max";
            this.btnMax.UseVisualStyleBackColor = true;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // btnMin
            // 
            this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMin.Location = new System.Drawing.Point(304, 11);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(75, 24);
            this.btnMin.TabIndex = 1;
            this.btnMin.Text = "Min";
            this.btnMin.UseVisualStyleBackColor = true;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            // 
            // btnNM
            // 
            this.btnNM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNM.Location = new System.Drawing.Point(304, 61);
            this.btnNM.Name = "btnNM";
            this.btnNM.Size = new System.Drawing.Size(75, 23);
            this.btnNM.TabIndex = 2;
            this.btnNM.Text = "NM";
            this.btnNM.UseVisualStyleBackColor = true;
            this.btnNM.Click += new System.EventHandler(this.btnNM_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(385, 61);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cbAllowClose
            // 
            this.cbAllowClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAllowClose.AutoSize = true;
            this.cbAllowClose.Location = new System.Drawing.Point(383, 107);
            this.cbAllowClose.Name = "cbAllowClose";
            this.cbAllowClose.Size = new System.Drawing.Size(84, 16);
            this.cbAllowClose.TabIndex = 4;
            this.cbAllowClose.Text = "AllowClose";
            this.cbAllowClose.UseVisualStyleBackColor = true;
            // 
            // cbFullScreen
            // 
            this.cbFullScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFullScreen.AutoSize = true;
            this.cbFullScreen.Location = new System.Drawing.Point(293, 107);
            this.cbFullScreen.Name = "cbFullScreen";
            this.cbFullScreen.Size = new System.Drawing.Size(84, 16);
            this.cbFullScreen.TabIndex = 5;
            this.cbFullScreen.Text = "FullScreen";
            this.cbFullScreen.UseVisualStyleBackColor = true;
            // 
            // btnTestLog
            // 
            this.btnTestLog.Location = new System.Drawing.Point(61, 99);
            this.btnTestLog.Name = "btnTestLog";
            this.btnTestLog.Size = new System.Drawing.Size(75, 23);
            this.btnTestLog.TabIndex = 6;
            this.btnTestLog.Text = "TestLog";
            this.btnTestLog.UseVisualStyleBackColor = true;
            this.btnTestLog.Click += new System.EventHandler(this.btnTestLog_Click);
            // 
            // btnWriteLog
            // 
            this.btnWriteLog.Location = new System.Drawing.Point(61, 146);
            this.btnWriteLog.Name = "btnWriteLog";
            this.btnWriteLog.Size = new System.Drawing.Size(75, 23);
            this.btnWriteLog.TabIndex = 7;
            this.btnWriteLog.Text = "WriteLog";
            this.btnWriteLog.UseVisualStyleBackColor = true;
            this.btnWriteLog.Click += new System.EventHandler(this.btnWriteLog_Click);
            // 
            // FTestNoneBorderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 327);
            this.Controls.Add(this.btnWriteLog);
            this.Controls.Add(this.btnTestLog);
            this.Controls.Add(this.cbFullScreen);
            this.Controls.Add(this.cbAllowClose);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnNM);
            this.Controls.Add(this.btnMin);
            this.Controls.Add(this.btnMax);
            this.Name = "FTestNoneBorderForm";
            this.ShowInTaskbar = false;
            this.Text = "FTestNoneBorderForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FTestNoneBorderForm_FormClosing);
            this.Load += new System.EventHandler(this.FTestNoneBorderForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnMin;
        private System.Windows.Forms.Button btnNM;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox cbAllowClose;
        private System.Windows.Forms.CheckBox cbFullScreen;
        private System.Windows.Forms.Button btnTestLog;
        private System.Windows.Forms.Button btnWriteLog;
    }
}