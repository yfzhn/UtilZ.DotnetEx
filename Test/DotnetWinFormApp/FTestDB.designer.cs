namespace DotnetWinFormApp
{
    partial class FTestDB
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnDataBaseName = new System.Windows.Forms.Button();
            this.btnUserName = new System.Windows.Forms.Button();
            this.btnDatabasePropertyInfo = new System.Windows.Forms.Button();
            this.btnEF = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExistTable = new System.Windows.Forms.Button();
            this.ckGetFieldInfo = new System.Windows.Forms.CheckBox();
            this.btnDBQueryExpression = new System.Windows.Forms.Button();
            this.checkBoxIgnoreCase = new System.Windows.Forms.CheckBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dgv = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid.UCPageGridControl();
            this.btnPagingQuery = new System.Windows.Forms.Button();
            this.txtFieldName = new System.Windows.Forms.TextBox();
            this.txtTblName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExistField = new System.Windows.Forms.Button();
            this.btnTableInfo = new System.Windows.Forms.Button();
            this.btnDatabaseVer = new System.Windows.Forms.Button();
            this.btnTestEF = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.comboBoxDB = new System.Windows.Forms.ComboBox();
            this.btnDataBaseSysTime = new System.Windows.Forms.Button();
            this.logControl = new UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.LogControlF();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnQuery);
            this.splitContainer1.Panel1.Controls.Add(this.btnDataBaseName);
            this.splitContainer1.Panel1.Controls.Add(this.btnUserName);
            this.splitContainer1.Panel1.Controls.Add(this.btnDatabasePropertyInfo);
            this.splitContainer1.Panel1.Controls.Add(this.btnEF);
            this.splitContainer1.Panel1.Controls.Add(this.btnClear);
            this.splitContainer1.Panel1.Controls.Add(this.btnExistTable);
            this.splitContainer1.Panel1.Controls.Add(this.ckGetFieldInfo);
            this.splitContainer1.Panel1.Controls.Add(this.btnDBQueryExpression);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxIgnoreCase);
            this.splitContainer1.Panel1.Controls.Add(this.btnUpdate);
            this.splitContainer1.Panel1.Controls.Add(this.dgv);
            this.splitContainer1.Panel1.Controls.Add(this.btnPagingQuery);
            this.splitContainer1.Panel1.Controls.Add(this.txtFieldName);
            this.splitContainer1.Panel1.Controls.Add(this.txtTblName);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnExistField);
            this.splitContainer1.Panel1.Controls.Add(this.btnTableInfo);
            this.splitContainer1.Panel1.Controls.Add(this.btnDatabaseVer);
            this.splitContainer1.Panel1.Controls.Add(this.btnTestEF);
            this.splitContainer1.Panel1.Controls.Add(this.btnTest);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxDB);
            this.splitContainer1.Panel1.Controls.Add(this.btnDataBaseSysTime);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logControl);
            this.splitContainer1.Size = new System.Drawing.Size(1265, 788);
            this.splitContainer1.SplitterDistance = 202;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnQuery
            // 
            this.btnQuery.Location = new System.Drawing.Point(470, 102);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(75, 23);
            this.btnQuery.TabIndex = 23;
            this.btnQuery.Text = "Query";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // btnDataBaseName
            // 
            this.btnDataBaseName.Location = new System.Drawing.Point(95, 131);
            this.btnDataBaseName.Name = "btnDataBaseName";
            this.btnDataBaseName.Size = new System.Drawing.Size(93, 23);
            this.btnDataBaseName.TabIndex = 22;
            this.btnDataBaseName.Text = "DataBaseName";
            this.btnDataBaseName.UseVisualStyleBackColor = true;
            this.btnDataBaseName.Click += new System.EventHandler(this.btnDataBaseName_Click);
            // 
            // btnUserName
            // 
            this.btnUserName.Location = new System.Drawing.Point(13, 131);
            this.btnUserName.Name = "btnUserName";
            this.btnUserName.Size = new System.Drawing.Size(75, 23);
            this.btnUserName.TabIndex = 21;
            this.btnUserName.Text = "UserName";
            this.btnUserName.UseVisualStyleBackColor = true;
            this.btnUserName.Click += new System.EventHandler(this.btnUserName_Click);
            // 
            // btnDatabasePropertyInfo
            // 
            this.btnDatabasePropertyInfo.Location = new System.Drawing.Point(95, 102);
            this.btnDatabasePropertyInfo.Name = "btnDatabasePropertyInfo";
            this.btnDatabasePropertyInfo.Size = new System.Drawing.Size(155, 23);
            this.btnDatabasePropertyInfo.TabIndex = 20;
            this.btnDatabasePropertyInfo.Text = "DatabasePropertyInfo";
            this.btnDatabasePropertyInfo.UseVisualStyleBackColor = true;
            this.btnDatabasePropertyInfo.Click += new System.EventHandler(this.btnDatabasePropertyInfo_Click);
            // 
            // btnEF
            // 
            this.btnEF.Location = new System.Drawing.Point(13, 102);
            this.btnEF.Name = "btnEF";
            this.btnEF.Size = new System.Drawing.Size(75, 23);
            this.btnEF.TabIndex = 19;
            this.btnEF.Text = "EF";
            this.btnEF.UseVisualStyleBackColor = true;
            this.btnEF.Click += new System.EventHandler(this.btnEF_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(3, 176);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnExistTable
            // 
            this.btnExistTable.Location = new System.Drawing.Point(306, 40);
            this.btnExistTable.Name = "btnExistTable";
            this.btnExistTable.Size = new System.Drawing.Size(75, 23);
            this.btnExistTable.TabIndex = 6;
            this.btnExistTable.Text = "ExistTable";
            this.btnExistTable.UseVisualStyleBackColor = true;
            this.btnExistTable.Click += new System.EventHandler(this.btnExistTable_Click);
            // 
            // ckGetFieldInfo
            // 
            this.ckGetFieldInfo.AutoSize = true;
            this.ckGetFieldInfo.Checked = true;
            this.ckGetFieldInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckGetFieldInfo.Location = new System.Drawing.Point(247, 45);
            this.ckGetFieldInfo.Name = "ckGetFieldInfo";
            this.ckGetFieldInfo.Size = new System.Drawing.Size(72, 16);
            this.ckGetFieldInfo.TabIndex = 17;
            this.ckGetFieldInfo.Text = "GetField";
            this.ckGetFieldInfo.UseVisualStyleBackColor = true;
            // 
            // btnDBQueryExpression
            // 
            this.btnDBQueryExpression.Location = new System.Drawing.Point(95, 70);
            this.btnDBQueryExpression.Name = "btnDBQueryExpression";
            this.btnDBQueryExpression.Size = new System.Drawing.Size(155, 23);
            this.btnDBQueryExpression.TabIndex = 16;
            this.btnDBQueryExpression.Text = "DBQueryExpression";
            this.btnDBQueryExpression.UseVisualStyleBackColor = true;
            this.btnDBQueryExpression.Click += new System.EventHandler(this.btnDBQueryExpression_Click);
            // 
            // checkBoxIgnoreCase
            // 
            this.checkBoxIgnoreCase.AutoSize = true;
            this.checkBoxIgnoreCase.Checked = true;
            this.checkBoxIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreCase.Location = new System.Drawing.Point(388, 72);
            this.checkBoxIgnoreCase.Name = "checkBoxIgnoreCase";
            this.checkBoxIgnoreCase.Size = new System.Drawing.Size(84, 16);
            this.checkBoxIgnoreCase.TabIndex = 15;
            this.checkBoxIgnoreCase.Text = "忽略大小写";
            this.checkBoxIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(470, 68);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 14;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dgv
            // 
            this.dgv.AlignDirection = true;
            this.dgv.ColumnSettingStatus = UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid.PageGridColumnSettingStatus.Hiden;
            this.dgv.ColumnSettingWidth = 20;
            this.dgv.EnableColumnHeaderContextMenuStripHiden = true;
            this.dgv.EnablePagingBar = true;
            this.dgv.EnableRowNum = true;
            this.dgv.EnableUserAdjustPageSize = true;
            this.dgv.FocusedRowIndex = -1;
            this.dgv.IsLastColumnAutoSizeModeFill = true;
            this.dgv.Location = new System.Drawing.Point(551, 3);
            this.dgv.Name = "dgv";
            this.dgv.PageSizeMaximum = 100;
            this.dgv.PageSizeMinimum = 1;
            this.dgv.Size = new System.Drawing.Size(711, 196);
            this.dgv.TabIndex = 13;
            this.dgv.QueryData += new System.EventHandler<UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid.QueryDataArgs>(this.dgv_QueryData);
            this.dgv.PageSizeChanged += new System.EventHandler<UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid.PageSizeChangedArgs>(this.dgv_PageSizeChanged);
            // 
            // btnPagingQuery
            // 
            this.btnPagingQuery.Location = new System.Drawing.Point(470, 37);
            this.btnPagingQuery.Name = "btnPagingQuery";
            this.btnPagingQuery.Size = new System.Drawing.Size(75, 23);
            this.btnPagingQuery.TabIndex = 12;
            this.btnPagingQuery.Text = "PagingQuery";
            this.btnPagingQuery.UseVisualStyleBackColor = true;
            this.btnPagingQuery.Click += new System.EventHandler(this.btnPagingQuery_Click);
            // 
            // txtFieldName
            // 
            this.txtFieldName.Location = new System.Drawing.Point(445, 10);
            this.txtFieldName.Name = "txtFieldName";
            this.txtFieldName.Size = new System.Drawing.Size(100, 21);
            this.txtFieldName.TabIndex = 11;
            this.txtFieldName.Text = "Name";
            // 
            // txtTblName
            // 
            this.txtTblName.Location = new System.Drawing.Point(292, 10);
            this.txtTblName.Name = "txtTblName";
            this.txtTblName.Size = new System.Drawing.Size(100, 21);
            this.txtTblName.TabIndex = 10;
            this.txtTblName.Text = "Stu";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(398, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "字段名";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(257, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "表名";
            // 
            // btnExistField
            // 
            this.btnExistField.Location = new System.Drawing.Point(388, 38);
            this.btnExistField.Name = "btnExistField";
            this.btnExistField.Size = new System.Drawing.Size(75, 23);
            this.btnExistField.TabIndex = 7;
            this.btnExistField.Text = "ExistField";
            this.btnExistField.UseVisualStyleBackColor = true;
            this.btnExistField.Click += new System.EventHandler(this.btnExistField_Click);
            // 
            // btnTableInfo
            // 
            this.btnTableInfo.Location = new System.Drawing.Point(175, 42);
            this.btnTableInfo.Name = "btnTableInfo";
            this.btnTableInfo.Size = new System.Drawing.Size(75, 23);
            this.btnTableInfo.TabIndex = 5;
            this.btnTableInfo.Text = "TableInfo";
            this.btnTableInfo.UseVisualStyleBackColor = true;
            this.btnTableInfo.Click += new System.EventHandler(this.btnTableInfo_Click);
            // 
            // btnDatabaseVer
            // 
            this.btnDatabaseVer.Location = new System.Drawing.Point(93, 43);
            this.btnDatabaseVer.Name = "btnDatabaseVer";
            this.btnDatabaseVer.Size = new System.Drawing.Size(75, 23);
            this.btnDatabaseVer.TabIndex = 4;
            this.btnDatabaseVer.Text = "DatabaseVer";
            this.btnDatabaseVer.UseVisualStyleBackColor = true;
            this.btnDatabaseVer.Click += new System.EventHandler(this.btnDatabaseVer_Click);
            // 
            // btnTestEF
            // 
            this.btnTestEF.Location = new System.Drawing.Point(13, 72);
            this.btnTestEF.Name = "btnTestEF";
            this.btnTestEF.Size = new System.Drawing.Size(75, 23);
            this.btnTestEF.TabIndex = 3;
            this.btnTestEF.Text = "TestEF";
            this.btnTestEF.UseVisualStyleBackColor = true;
            this.btnTestEF.Click += new System.EventHandler(this.btnTestEF_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(140, 10);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 2;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // comboBoxDB
            // 
            this.comboBoxDB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDB.FormattingEnabled = true;
            this.comboBoxDB.Location = new System.Drawing.Point(13, 13);
            this.comboBoxDB.Name = "comboBoxDB";
            this.comboBoxDB.Size = new System.Drawing.Size(121, 20);
            this.comboBoxDB.TabIndex = 1;
            // 
            // btnDataBaseSysTime
            // 
            this.btnDataBaseSysTime.Location = new System.Drawing.Point(12, 43);
            this.btnDataBaseSysTime.Name = "btnDataBaseSysTime";
            this.btnDataBaseSysTime.Size = new System.Drawing.Size(75, 23);
            this.btnDataBaseSysTime.TabIndex = 0;
            this.btnDataBaseSysTime.Text = "DataBaseSysTime";
            this.btnDataBaseSysTime.UseVisualStyleBackColor = true;
            this.btnDataBaseSysTime.Click += new System.EventHandler(this.btnDataBaseSysTime_Click);
            // 
            // logControl
            // 
            this.logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl.IsLock = false;
            this.logControl.Location = new System.Drawing.Point(0, 0);
            this.logControl.MaxItemCount = 100;
            this.logControl.Name = "logControl";
            this.logControl.Size = new System.Drawing.Size(1265, 582);
            this.logControl.TabIndex = 0;
            // 
            // FTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 788);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FTest";
            this.Text = "Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FTest_FormClosing);
            this.Load += new System.EventHandler(this.FTest_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.LogControlF logControl;
        private System.Windows.Forms.Button btnDataBaseSysTime;
        private System.Windows.Forms.ComboBox comboBoxDB;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnTestEF;
        private System.Windows.Forms.Button btnDatabaseVer;
        private System.Windows.Forms.Button btnTableInfo;
        private System.Windows.Forms.Button btnExistTable;
        private System.Windows.Forms.TextBox txtFieldName;
        private System.Windows.Forms.TextBox txtTblName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExistField;
        private System.Windows.Forms.Button btnPagingQuery;
        private UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PageGrid.UCPageGridControl dgv;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.CheckBox checkBoxIgnoreCase;
        private System.Windows.Forms.Button btnDBQueryExpression;
        private System.Windows.Forms.CheckBox ckGetFieldInfo;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnEF;
        private System.Windows.Forms.Button btnDatabasePropertyInfo;
        private System.Windows.Forms.Button btnDataBaseName;
        private System.Windows.Forms.Button btnUserName;
        private System.Windows.Forms.Button btnQuery;
    }
}

