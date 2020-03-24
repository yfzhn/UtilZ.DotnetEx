namespace UtilZ.Dotnet.ConfigManagementTool.UI.UCViews
{
    partial class UCParaView
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comGroup = new System.Windows.Forms.ComboBox();
            this.pgConfigParaKeyValue = new WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pgValidDomain = new WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer.Size = new System.Drawing.Size(800, 600);
            this.splitContainer.SplitterDistance = 557;
            this.splitContainer.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comGroup);
            this.groupBox1.Controls.Add(this.pgConfigParaKeyValue);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(557, 600);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "参数列表";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "参数组";
            // 
            // comGroup
            // 
            this.comGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comGroup.FormattingEnabled = true;
            this.comGroup.Location = new System.Drawing.Point(53, 23);
            this.comGroup.Name = "comGroup";
            this.comGroup.Size = new System.Drawing.Size(228, 20);
            this.comGroup.TabIndex = 1;
            this.comGroup.SelectedIndexChanged += new System.EventHandler(this.comGroup_SelectedIndexChanged);
            // 
            // pgConfigParaKeyValue
            // 
            this.pgConfigParaKeyValue.AdvanceSettingVisible = false;
            this.pgConfigParaKeyValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgConfigParaKeyValue.BackColor = System.Drawing.SystemColors.Control;
            this.pgConfigParaKeyValue.ColumnSettingStatus = WindowEx.Winform.Controls.PageGrid.PageGridColumnSettingStatus.Hiden;
            this.pgConfigParaKeyValue.ColumnSettingVisible = false;
            this.pgConfigParaKeyValue.ColumnSettingWidth = 20;
            this.pgConfigParaKeyValue.FocusedRowIndex = -1;
            this.pgConfigParaKeyValue.IsLastColumnAutoSizeModeFill = false;
            this.pgConfigParaKeyValue.Location = new System.Drawing.Point(3, 51);
            this.pgConfigParaKeyValue.MinimumSize = new System.Drawing.Size(30, 30);
            this.pgConfigParaKeyValue.Name = "pgConfigParaKeyValue";
            this.pgConfigParaKeyValue.PagingVisible = false;
            this.pgConfigParaKeyValue.RowNumVisible = true;
            this.pgConfigParaKeyValue.Size = new System.Drawing.Size(551, 546);
            this.pgConfigParaKeyValue.TabIndex = 0;
            this.pgConfigParaKeyValue.DataRowDoubleClick += new System.EventHandler<WindowEx.Winform.Controls.PageGrid.Interface.DataRowClickArgs>(this.pgConfigParaKeyValue_DataRowDoubleClick);
            this.pgConfigParaKeyValue.DataRowSelectionChanged += new System.EventHandler<WindowEx.Winform.Controls.PageGrid.Interface.DataRowSelectionChangedArgs>(this.pgConfigParaKeyValue_SelectionChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pgValidDomain);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(239, 600);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "作用域";
            // 
            // pgValidDomain
            // 
            this.pgValidDomain.AdvanceSettingVisible = true;
            this.pgValidDomain.BackColor = System.Drawing.SystemColors.Control;
            this.pgValidDomain.ColumnSettingStatus = WindowEx.Winform.Controls.PageGrid.PageGridColumnSettingStatus.Hiden;
            this.pgValidDomain.ColumnSettingVisible = false;
            this.pgValidDomain.ColumnSettingWidth = 20;
            this.pgValidDomain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgValidDomain.FocusedRowIndex = -1;
            this.pgValidDomain.IsLastColumnAutoSizeModeFill = false;
            this.pgValidDomain.Location = new System.Drawing.Point(3, 17);
            this.pgValidDomain.MinimumSize = new System.Drawing.Size(30, 30);
            this.pgValidDomain.Name = "pgValidDomain";
            this.pgValidDomain.PagingVisible = false;
            this.pgValidDomain.RowNumVisible = true;
            this.pgValidDomain.Size = new System.Drawing.Size(233, 580);
            this.pgValidDomain.TabIndex = 1;
            // 
            // UCParaView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "UCParaView";
            this.Load += new System.EventHandler(this.UCParaView_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBox1;
        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl pgConfigParaKeyValue;
        private System.Windows.Forms.GroupBox groupBox2;
        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl pgValidDomain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comGroup;
    }
}
