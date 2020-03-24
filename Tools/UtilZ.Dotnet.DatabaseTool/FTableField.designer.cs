namespace UtilZ.Dotnet.DatabaseTool
{
    partial class FTableField
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
            this.label3 = new System.Windows.Forms.Label();
            this.txtTableFieldFilter = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvTableFields = new UtilZ.Dotnet.WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvIndex = new UtilZ.Dotnet.WindowEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rtxtIndexDetail = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "过滤";
            // 
            // txtTableFieldFilter
            // 
            this.txtTableFieldFilter.Location = new System.Drawing.Point(53, 12);
            this.txtTableFieldFilter.Name = "txtTableFieldFilter";
            this.txtTableFieldFilter.Size = new System.Drawing.Size(256, 21);
            this.txtTableFieldFilter.TabIndex = 13;
            this.txtTableFieldFilter.TextChanged += new System.EventHandler(this.txtTableFieldFilter_TextChanged);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(4, 39);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(759, 671);
            this.splitContainer2.SplitterDistance = 505;
            this.splitContainer2.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvTableFields);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(759, 505);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "字段";
            // 
            // dgvTableFields
            // 
            this.dgvTableFields.AlignDirection = true;
            this.dgvTableFields.ColumnSettingStatus = UtilZ.Dotnet.WindowEx.Winform.Controls.PageGrid.PageGridColumnSettingStatus.Hiden;
            this.dgvTableFields.ColumnSettingWidth = 20;
            this.dgvTableFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTableFields.EnableColumnHeaderContextMenuStripHiden = true;
            this.dgvTableFields.EnablePagingBar = false;
            this.dgvTableFields.EnableRowNum = true;
            this.dgvTableFields.EnableUserAdjustPageSize = true;
            this.dgvTableFields.FocusedRowIndex = -1;
            this.dgvTableFields.IsLastColumnAutoSizeModeFill = true;
            this.dgvTableFields.Location = new System.Drawing.Point(3, 17);
            this.dgvTableFields.Name = "dgvTableFields";
            this.dgvTableFields.PageSizeMaximum = 100;
            this.dgvTableFields.PageSizeMinimum = 1;
            this.dgvTableFields.Size = new System.Drawing.Size(753, 485);
            this.dgvTableFields.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.splitContainer3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(759, 162);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "索引";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 17);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer3.Size = new System.Drawing.Size(753, 142);
            this.splitContainer3.SplitterDistance = 464;
            this.splitContainer3.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgvIndex);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(464, 142);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = " 索引列表";
            // 
            // dgvIndex
            // 
            this.dgvIndex.AlignDirection = true;
            this.dgvIndex.ColumnSettingStatus = UtilZ.Dotnet.WindowEx.Winform.Controls.PageGrid.PageGridColumnSettingStatus.Disable;
            this.dgvIndex.ColumnSettingWidth = 20;
            this.dgvIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvIndex.EnableColumnHeaderContextMenuStripHiden = true;
            this.dgvIndex.EnablePagingBar = false;
            this.dgvIndex.EnableRowNum = true;
            this.dgvIndex.EnableUserAdjustPageSize = true;
            this.dgvIndex.FocusedRowIndex = -1;
            this.dgvIndex.IsLastColumnAutoSizeModeFill = true;
            this.dgvIndex.Location = new System.Drawing.Point(3, 17);
            this.dgvIndex.Name = "dgvIndex";
            this.dgvIndex.PageSizeMaximum = 100;
            this.dgvIndex.PageSizeMinimum = 1;
            this.dgvIndex.Size = new System.Drawing.Size(458, 122);
            this.dgvIndex.TabIndex = 2;
            this.dgvIndex.DataRowSelectionChanged += new System.EventHandler<UtilZ.Dotnet.WindowEx.Winform.Controls.PageGrid.DataRowSelectionChangedArgs>(this.dgvIndex_DataRowSelectionChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rtxtIndexDetail);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(285, 142);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "详情";
            // 
            // rtxtIndexDetail
            // 
            this.rtxtIndexDetail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtIndexDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtIndexDetail.Location = new System.Drawing.Point(3, 17);
            this.rtxtIndexDetail.Name = "rtxtIndexDetail";
            this.rtxtIndexDetail.ReadOnly = true;
            this.rtxtIndexDetail.Size = new System.Drawing.Size(279, 122);
            this.rtxtIndexDetail.TabIndex = 0;
            this.rtxtIndexDetail.Text = "";
            // 
            // FTableField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 712);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTableFieldFilter);
            this.Name = "FTableField";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FTableField";
            this.Load += new System.EventHandler(this.FTableField_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTableFieldFilter;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl dgvTableFields;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox groupBox4;
        private WindowEx.Winform.Controls.PageGrid.UCPageGridControl dgvIndex;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RichTextBox rtxtIndexDetail;
    }
}