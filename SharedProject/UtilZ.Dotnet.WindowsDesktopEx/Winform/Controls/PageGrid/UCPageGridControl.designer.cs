namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    partial class UCPageGridControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelPageCount = new System.Windows.Forms.Label();
            this.btnLastPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnPrePage = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.numPageSize = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.btnFirstPage = new System.Windows.Forms.Button();
            this.numPageIndex = new System.Windows.Forms.NumericUpDown();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelPage = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.FlowLayoutPanelZ();
            this.cmsColVisibleSetting = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiHidenCol = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numPageSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPageIndex)).BeginInit();
            this.panelPage.SuspendLayout();
            this.cmsColVisibleSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelPageCount
            // 
            this.labelPageCount.AutoSize = true;
            this.labelPageCount.Location = new System.Drawing.Point(199, 6);
            this.labelPageCount.Margin = new System.Windows.Forms.Padding(0, 6, 3, 0);
            this.labelPageCount.Name = "labelPageCount";
            this.labelPageCount.Size = new System.Drawing.Size(83, 12);
            this.labelPageCount.TabIndex = 21;
            this.labelPageCount.Text = "0页|共0条记录";
            // 
            // btnLastPage
            // 
            this.btnLastPage.BackgroundImage = global::UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.Resource.lastPage;
            this.btnLastPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLastPage.FlatAppearance.BorderSize = 0;
            this.btnLastPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLastPage.Location = new System.Drawing.Point(184, 5);
            this.btnLastPage.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(15, 15);
            this.btnLastPage.TabIndex = 20;
            this.btnLastPage.UseVisualStyleBackColor = true;
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.BackgroundImage = global::UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.Resource.nextPage;
            this.btnNextPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNextPage.FlatAppearance.BorderSize = 0;
            this.btnNextPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNextPage.Location = new System.Drawing.Point(169, 5);
            this.btnNextPage.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(15, 15);
            this.btnNextPage.TabIndex = 19;
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPrePage
            // 
            this.btnPrePage.BackgroundImage = global::UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.Resource.prePage;
            this.btnPrePage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrePage.FlatAppearance.BorderSize = 0;
            this.btnPrePage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrePage.Location = new System.Drawing.Point(109, 5);
            this.btnPrePage.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnPrePage.Name = "btnPrePage";
            this.btnPrePage.Size = new System.Drawing.Size(15, 15);
            this.btnPrePage.TabIndex = 18;
            this.btnPrePage.UseVisualStyleBackColor = true;
            this.btnPrePage.Click += new System.EventHandler(this.btnPrePage_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(77, 6);
            this.label4.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "条";
            // 
            // numPageSize
            // 
            this.numPageSize.AutoSize = true;
            this.numPageSize.Location = new System.Drawing.Point(35, 2);
            this.numPageSize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.numPageSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPageSize.Name = "numPageSize";
            this.numPageSize.Size = new System.Drawing.Size(39, 21);
            this.numPageSize.TabIndex = 17;
            this.numPageSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numPageSize.ValueChanged += new System.EventHandler(this.numPageSize_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 6, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "每页";
            // 
            // btnFirstPage
            // 
            this.btnFirstPage.BackgroundImage = global::UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.Resource.firstPage;
            this.btnFirstPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirstPage.FlatAppearance.BorderSize = 0;
            this.btnFirstPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFirstPage.Location = new System.Drawing.Point(94, 5);
            this.btnFirstPage.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnFirstPage.Name = "btnFirstPage";
            this.btnFirstPage.Size = new System.Drawing.Size(15, 15);
            this.btnFirstPage.TabIndex = 9;
            this.btnFirstPage.UseVisualStyleBackColor = true;
            this.btnFirstPage.Click += new System.EventHandler(this.btnFirstPage_Click);
            // 
            // numPageIndex
            // 
            this.numPageIndex.AutoSize = true;
            this.numPageIndex.Location = new System.Drawing.Point(127, 3);
            this.numPageIndex.Name = "numPageIndex";
            this.numPageIndex.Size = new System.Drawing.Size(39, 21);
            this.numPageIndex.TabIndex = 11;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.SystemColors.Control;
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(365, 83);
            this.panelContent.TabIndex = 5;
            // 
            // panelPage
            // 
            this.panelPage.AutoSize = true;
            this.panelPage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelPage.Controls.Add(this.label2);
            this.panelPage.Controls.Add(this.numPageSize);
            this.panelPage.Controls.Add(this.label4);
            this.panelPage.Controls.Add(this.btnFirstPage);
            this.panelPage.Controls.Add(this.btnPrePage);
            this.panelPage.Controls.Add(this.numPageIndex);
            this.panelPage.Controls.Add(this.btnNextPage);
            this.panelPage.Controls.Add(this.btnLastPage);
            this.panelPage.Controls.Add(this.labelPageCount);
            this.panelPage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPage.Location = new System.Drawing.Point(0, 83);
            this.panelPage.Name = "panelPage";
            this.panelPage.Size = new System.Drawing.Size(365, 27);
            this.panelPage.TabIndex = 6;
            // 
            // cmsColVisibleSetting
            // 
            this.cmsColVisibleSetting.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiHidenCol});
            this.cmsColVisibleSetting.Name = "cmsColVisibleSetting";
            this.cmsColVisibleSetting.Size = new System.Drawing.Size(101, 26);
            // 
            // tsmiHidenCol
            // 
            this.tsmiHidenCol.Name = "tsmiHidenCol";
            this.tsmiHidenCol.Size = new System.Drawing.Size(100, 22);
            this.tsmiHidenCol.Text = "隐藏";
            this.tsmiHidenCol.Click += new System.EventHandler(this.tsmiHidenCol_Click);
            // 
            // UCPageGridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelPage);
            this.Name = "UCPageGridControl";
            this.Size = new System.Drawing.Size(365, 110);
            this.Load += new System.EventHandler(this.UCPageGridControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numPageSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPageIndex)).EndInit();
            this.panelPage.ResumeLayout(false);
            this.panelPage.PerformLayout();
            this.cmsColVisibleSetting.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnFirstPage;
        private System.Windows.Forms.NumericUpDown numPageIndex;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numPageSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLastPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnPrePage;
        private System.Windows.Forms.Label labelPageCount;
        private FlowLayoutPanelZ panelPage;
        private System.Windows.Forms.ContextMenuStrip cmsColVisibleSetting;
        private System.Windows.Forms.ToolStripMenuItem tsmiHidenCol;
    }
}
