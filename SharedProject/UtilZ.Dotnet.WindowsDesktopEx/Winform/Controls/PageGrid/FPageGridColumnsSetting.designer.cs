namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid
{
    partial class FPageGridColumnsSetting
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
            this.pictureBoxMenu = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAutoHiden = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFloat = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.dgvColumnSetting = new System.Windows.Forms.DataGridView();
            this.labelTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMenu)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumnSetting)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxMenu
            // 
            this.pictureBoxMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxMenu.Image = global::UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.Resource.Dropdown;
            this.pictureBoxMenu.Location = new System.Drawing.Point(184, 2);
            this.pictureBoxMenu.Name = "pictureBoxMenu";
            this.pictureBoxMenu.Size = new System.Drawing.Size(15, 15);
            this.pictureBoxMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMenu.TabIndex = 2;
            this.pictureBoxMenu.TabStop = false;
            this.pictureBoxMenu.Click += new System.EventHandler(this.pictureBoxMenu_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAutoHiden,
            this.tsmiFloat,
            this.tsmiDock,
            this.tsmiSave});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(153, 114);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // tsmiAutoHiden
            // 
            this.tsmiAutoHiden.Name = "tsmiAutoHiden";
            this.tsmiAutoHiden.Size = new System.Drawing.Size(152, 22);
            this.tsmiAutoHiden.Text = "隐藏";
            this.tsmiAutoHiden.Click += new System.EventHandler(this.tsmiAutoHiden_Click);
            // 
            // tsmiFloat
            // 
            this.tsmiFloat.Name = "tsmiFloat";
            this.tsmiFloat.Size = new System.Drawing.Size(152, 22);
            this.tsmiFloat.Text = "浮动";
            this.tsmiFloat.Click += new System.EventHandler(this.tsmiFloat_Click);
            // 
            // tsmiDock
            // 
            this.tsmiDock.Name = "tsmiDock";
            this.tsmiDock.Size = new System.Drawing.Size(152, 22);
            this.tsmiDock.Text = "停靠";
            this.tsmiDock.Click += new System.EventHandler(this.tsmiDock_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.Name = "tsmiSave";
            this.tsmiSave.Size = new System.Drawing.Size(152, 22);
            this.tsmiSave.Text = "保存";
            this.tsmiSave.Click += new System.EventHandler(this.tsmiSave_Click);
            // 
            // dgvColumnSetting
            // 
            this.dgvColumnSetting.AllowUserToAddRows = false;
            this.dgvColumnSetting.AllowUserToDeleteRows = false;
            this.dgvColumnSetting.AllowUserToResizeRows = false;
            this.dgvColumnSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvColumnSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumnSetting.Location = new System.Drawing.Point(2, 20);
            this.dgvColumnSetting.MultiSelect = false;
            this.dgvColumnSetting.Name = "dgvColumnSetting";
            this.dgvColumnSetting.RowHeadersVisible = false;
            this.dgvColumnSetting.RowTemplate.Height = 23;
            this.dgvColumnSetting.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvColumnSetting.Size = new System.Drawing.Size(196, 178);
            this.dgvColumnSetting.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(1, 3);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(53, 12);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "显示设置";
            this.labelTitle.Click += new System.EventHandler(this.labelTitle_Click);
            //this.labelTitle.MouseEnter += new System.EventHandler(this.labelTitle_MouseEnter);
            // 
            // FPageGridColumnsSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(200, 200);
            this.Controls.Add(this.dgvColumnSetting);
            this.Controls.Add(this.pictureBoxMenu);
            this.Controls.Add(this.labelTitle);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(20, 0);
            this.Name = "FPageGridColumnsSetting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "显示列设置";
            this.Load += new System.EventHandler(this.FPageGridColumnsSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMenu)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumnSetting)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBoxMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoHiden;
        private System.Windows.Forms.ToolStripMenuItem tsmiFloat;
        private System.Windows.Forms.ToolStripMenuItem tsmiDock;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.DataGridView dgvColumnSetting;
        private System.Windows.Forms.Label labelTitle;
    }
}