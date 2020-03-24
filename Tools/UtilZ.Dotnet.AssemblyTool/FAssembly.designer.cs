namespace UtilZ.Dotnet.AssemblyTool
{
    partial class FAssembly
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtAssemblyPath = new System.Windows.Forms.TextBox();
            this.btnAssemblyChioce = new System.Windows.Forms.Button();
            this.groupBoxAssemblyInfo = new System.Windows.Forms.GroupBox();
            this.rtxtAssemblyInfo = new System.Windows.Forms.RichTextBox();
            this.cmsAssemblyInfo = new System.Windows.Forms.ContextMenuStrip();
            this.tsmiAssemblyInfoCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAssemblyInfoSelectedAll = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxAssemblyInfo.SuspendLayout();
            this.cmsAssemblyInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "程序集";
            // 
            // txtAssemblyPath
            // 
            this.txtAssemblyPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssemblyPath.Location = new System.Drawing.Point(54, 17);
            this.txtAssemblyPath.Name = "txtAssemblyPath";
            this.txtAssemblyPath.Size = new System.Drawing.Size(437, 21);
            this.txtAssemblyPath.TabIndex = 1;
            this.txtAssemblyPath.TextChanged += new System.EventHandler(this.txtAssemblyPath_TextChanged);
            // 
            // btnAssemblyChioce
            // 
            this.btnAssemblyChioce.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAssemblyChioce.Location = new System.Drawing.Point(497, 15);
            this.btnAssemblyChioce.Name = "btnAssemblyChioce";
            this.btnAssemblyChioce.Size = new System.Drawing.Size(35, 23);
            this.btnAssemblyChioce.TabIndex = 2;
            this.btnAssemblyChioce.Text = "...";
            this.btnAssemblyChioce.UseVisualStyleBackColor = true;
            this.btnAssemblyChioce.Click += new System.EventHandler(this.btnAssemblyChioce_Click);
            // 
            // groupBoxAssemblyInfo
            // 
            this.groupBoxAssemblyInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAssemblyInfo.Controls.Add(this.rtxtAssemblyInfo);
            this.groupBoxAssemblyInfo.Controls.Add(this.label1);
            this.groupBoxAssemblyInfo.Controls.Add(this.btnAssemblyChioce);
            this.groupBoxAssemblyInfo.Controls.Add(this.txtAssemblyPath);
            this.groupBoxAssemblyInfo.Location = new System.Drawing.Point(6, 12);
            this.groupBoxAssemblyInfo.Name = "groupBoxAssemblyInfo";
            this.groupBoxAssemblyInfo.Size = new System.Drawing.Size(538, 238);
            this.groupBoxAssemblyInfo.TabIndex = 3;
            this.groupBoxAssemblyInfo.TabStop = false;
            this.groupBoxAssemblyInfo.Text = "获取程序集信息";
            // 
            // rtxtAssemblyInfo
            // 
            this.rtxtAssemblyInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtAssemblyInfo.Location = new System.Drawing.Point(54, 45);
            this.rtxtAssemblyInfo.Name = "rtxtAssemblyInfo";
            this.rtxtAssemblyInfo.ReadOnly = true;
            this.rtxtAssemblyInfo.Size = new System.Drawing.Size(437, 187);
            this.rtxtAssemblyInfo.TabIndex = 3;
            this.rtxtAssemblyInfo.Text = "";
            // 
            // cmsAssemblyInfo
            // 
            this.cmsAssemblyInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAssemblyInfoCopy,
            this.tsmiAssemblyInfoSelectedAll});
            this.cmsAssemblyInfo.Name = "cmsAssemblyInfo";
            this.cmsAssemblyInfo.Size = new System.Drawing.Size(101, 48);
            // 
            // tsmiAssemblyInfoCopy
            // 
            this.tsmiAssemblyInfoCopy.Name = "tsmiAssemblyInfoCopy";
            this.tsmiAssemblyInfoCopy.Size = new System.Drawing.Size(100, 22);
            this.tsmiAssemblyInfoCopy.Text = "复制";
            this.tsmiAssemblyInfoCopy.Click += new System.EventHandler(this.tsmiAssemblyInfoCopy_Click);
            // 
            // tsmiAssemblyInfoSelectedAll
            // 
            this.tsmiAssemblyInfoSelectedAll.Name = "tsmiAssemblyInfoSelectedAll";
            this.tsmiAssemblyInfoSelectedAll.Size = new System.Drawing.Size(100, 22);
            this.tsmiAssemblyInfoSelectedAll.Text = "全选";
            this.tsmiAssemblyInfoSelectedAll.Click += new System.EventHandler(this.tsmiAssemblyInfoSelectedAll_Click);
            // 
            // FAssembly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 262);
            this.Controls.Add(this.groupBoxAssemblyInfo);
            this.Name = "FAssembly";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "程序集工具";
            this.groupBoxAssemblyInfo.ResumeLayout(false);
            this.groupBoxAssemblyInfo.PerformLayout();
            this.cmsAssemblyInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAssemblyPath;
        private System.Windows.Forms.Button btnAssemblyChioce;
        private System.Windows.Forms.GroupBox groupBoxAssemblyInfo;
        private System.Windows.Forms.RichTextBox rtxtAssemblyInfo;
        private System.Windows.Forms.ContextMenuStrip cmsAssemblyInfo;
        private System.Windows.Forms.ToolStripMenuItem tsmiAssemblyInfoCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiAssemblyInfoSelectedAll;
    }
}

