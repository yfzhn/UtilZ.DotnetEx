namespace UtilZ.Dotnet.ConfigManagementTool.UI.UCViews
{
    partial class FServiceConfigParaKeyValueEdit
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pgConfigParaKeyValue = new WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCheckAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDirectionCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiUnCheck = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtServiceDes = new System.Windows.Forms.RichTextBox();
            this.txtServiceName = new System.Windows.Forms.TextBox();
            this.txtServiceId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pgConfigParaKeyValue);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(688, 299);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "可选参数列表";
            // 
            // pgConfigParaKeyValue
            // 
            this.pgConfigParaKeyValue.BackColor = System.Drawing.SystemColors.Control;
            this.pgConfigParaKeyValue.ColumnSettingVisible = false;
            this.pgConfigParaKeyValue.ContextMenuStrip = this.contextMenuStrip1;
            this.pgConfigParaKeyValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgConfigParaKeyValue.Location = new System.Drawing.Point(3, 17);
            this.pgConfigParaKeyValue.MinimumSize = new System.Drawing.Size(30, 30);
            this.pgConfigParaKeyValue.GridControl.MultiSelect = false;
            this.pgConfigParaKeyValue.Name = "pgConfigParaKeyValue";
            this.pgConfigParaKeyValue.PagingVisible = false;
            this.pgConfigParaKeyValue.Size = new System.Drawing.Size(682, 279);
            this.pgConfigParaKeyValue.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCheckAll,
            this.tsmiDirectionCheck,
            this.tsmiUnCheck,
            this.tsmiSave});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 92);
            // 
            // tsmiCheckAll
            // 
            this.tsmiCheckAll.Name = "tsmiCheckAll";
            this.tsmiCheckAll.Size = new System.Drawing.Size(100, 22);
            this.tsmiCheckAll.Text = "全选";
            this.tsmiCheckAll.Click += new System.EventHandler(this.tsmiCheckAll_Click);
            // 
            // tsmiDirectionCheck
            // 
            this.tsmiDirectionCheck.Name = "tsmiDirectionCheck";
            this.tsmiDirectionCheck.Size = new System.Drawing.Size(100, 22);
            this.tsmiDirectionCheck.Text = "反选";
            this.tsmiDirectionCheck.Click += new System.EventHandler(this.tsmiDirectionCheck_Click);
            // 
            // tsmiUnCheck
            // 
            this.tsmiUnCheck.Name = "tsmiUnCheck";
            this.tsmiUnCheck.Size = new System.Drawing.Size(100, 22);
            this.tsmiUnCheck.Text = "取消";
            this.tsmiUnCheck.Click += new System.EventHandler(this.tsmiUnCheck_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(100, 22);
            this.tsmiSave.Text = "保存";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer.Size = new System.Drawing.Size(688, 427);
            this.splitContainer.SplitterDistance = 124;
            this.splitContainer.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtServiceDes);
            this.groupBox2.Controls.Add(this.txtServiceName);
            this.groupBox2.Controls.Add(this.txtServiceId);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(688, 124);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "服务信息";
            // 
            // txtServiceDes
            // 
            this.txtServiceDes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceDes.Location = new System.Drawing.Point(85, 65);
            this.txtServiceDes.Name = "txtServiceDes";
            this.txtServiceDes.ReadOnly = true;
            this.txtServiceDes.Size = new System.Drawing.Size(591, 53);
            this.txtServiceDes.TabIndex = 5;
            this.txtServiceDes.Text = "";
            // 
            // txtServiceName
            // 
            this.txtServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceName.Location = new System.Drawing.Point(85, 38);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.ReadOnly = true;
            this.txtServiceName.Size = new System.Drawing.Size(591, 21);
            this.txtServiceName.TabIndex = 4;
            // 
            // txtServiceId
            // 
            this.txtServiceId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceId.Location = new System.Drawing.Point(85, 11);
            this.txtServiceId.Name = "txtServiceId";
            this.txtServiceId.ReadOnly = true;
            this.txtServiceId.Size = new System.Drawing.Size(591, 21);
            this.txtServiceId.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "描述";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "服务名称";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务映射ID";
            // 
            // FServiceConfigParaKeyValueEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 427);
            this.Controls.Add(this.splitContainer);
            this.Name = "FServiceConfigParaKeyValueEdit";
            this.Text = "服务配置参数编辑";
            this.Load += new System.EventHandler(this.FServiceConfigParaKeyValueEdit_Load);
            this.groupBox1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl pgConfigParaKeyValue;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiCheckAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDirectionCheck;
        private System.Windows.Forms.ToolStripMenuItem tsmiUnCheck;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtServiceDes;
        private System.Windows.Forms.TextBox txtServiceName;
        private System.Windows.Forms.TextBox txtServiceId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}