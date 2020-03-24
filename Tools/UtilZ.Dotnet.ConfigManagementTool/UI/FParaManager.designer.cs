namespace UtilZ.Dotnet.ConfigManagementTool.UI
{
    partial class FParaManager
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
            this.pgPara = new WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxParaGroup = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgPara
            // 
            this.pgPara.BackColor = System.Drawing.SystemColors.Control;
            this.pgPara.ColumnSettingVisible = false;
            this.pgPara.ContextMenuStrip = this.contextMenuStrip1;
            this.pgPara.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPara.Location = new System.Drawing.Point(0, 0);
            this.pgPara.MinimumSize = new System.Drawing.Size(30, 30);
            this.pgPara.GridControl.MultiSelect = false;
            this.pgPara.Name = "pgPara";
            this.pgPara.PagingVisible = false;
            this.pgPara.Size = new System.Drawing.Size(762, 372);
            this.pgPara.TabIndex = 0;
            this.pgPara.DataRowSelectionChanged += new System.EventHandler<WindowEx.Winform.Controls.PageGrid.Interface.DataRowSelectionChangedArgs>(this.pgPara_SelectionChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdd,
            this.tsmiDelete,
            this.tsmiSave});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 70);
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.Size = new System.Drawing.Size(100, 22);
            this.tsmiAdd.Text = "添加";
            this.tsmiAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(100, 22);
            this.tsmiDelete.Text = "删除";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(100, 22);
            this.tsmiSave.Text = "保存";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxParaGroup);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pgPara);
            this.splitContainer1.Size = new System.Drawing.Size(762, 401);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "选择组";
            // 
            // comboBoxParaGroup
            // 
            this.comboBoxParaGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParaGroup.FormattingEnabled = true;
            this.comboBoxParaGroup.Location = new System.Drawing.Point(50, 3);
            this.comboBoxParaGroup.Name = "comboBoxParaGroup";
            this.comboBoxParaGroup.Size = new System.Drawing.Size(335, 20);
            this.comboBoxParaGroup.TabIndex = 0;
            this.comboBoxParaGroup.SelectedIndexChanged += new System.EventHandler(this.comboBoxParaGroup_SelectedIndexChanged);
            // 
            // FParaManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 401);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FParaManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "配置参数管理";
            this.Load += new System.EventHandler(this.FParaManager_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl pgPara;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxParaGroup;
    }
}