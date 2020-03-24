namespace UtilZ.Dotnet.ConfigManagementTool.UI
{
    partial class FParaGroupManager
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
            this.pgGroup = new WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgGroup
            // 
            this.pgGroup.BackColor = System.Drawing.SystemColors.Control;
            this.pgGroup.ColumnSettingVisible = false;
            this.pgGroup.ContextMenuStrip = this.contextMenuStrip1;
            this.pgGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgGroup.Location = new System.Drawing.Point(0, 0);
            this.pgGroup.MinimumSize = new System.Drawing.Size(30, 30);
            this.pgGroup.GridControl.MultiSelect = true;
            this.pgGroup.Name = "pgGroup";
            this.pgGroup.PagingVisible = false;
            this.pgGroup.Size = new System.Drawing.Size(561, 355);
            this.pgGroup.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdd,
            this.tsmiDelete,
            this.tsmiSave});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.Size = new System.Drawing.Size(152, 22);
            this.tsmiAdd.Text = "添加";
            this.tsmiAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(152, 22);
            this.tsmiDelete.Text = "删除";
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(152, 22);
            this.tsmiSave.Text = "保存";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // FParaGroupManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 355);
            this.Controls.Add(this.pgGroup);
            this.Name = "FParaGroupManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "参数组管理";
            this.Load += new System.EventHandler(this.FParaGroupManager_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl pgGroup;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
    }
}