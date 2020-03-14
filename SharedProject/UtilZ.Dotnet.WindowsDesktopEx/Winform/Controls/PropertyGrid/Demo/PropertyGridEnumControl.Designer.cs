namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PropertyGrid.Demo
{
    partial class PropertyGridEnumControl
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxEnum = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBoxEnum
            // 
            this.comboBoxEnum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxEnum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEnum.FormattingEnabled = true;
            this.comboBoxEnum.Location = new System.Drawing.Point(0, 0);
            this.comboBoxEnum.Name = "comboBoxEnum";
            this.comboBoxEnum.Size = new System.Drawing.Size(150, 20);
            this.comboBoxEnum.TabIndex = 0;
            // 
            // PropertyGridEnumControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBoxEnum);
            this.Name = "PropertyGridEnumControl";
            this.Size = new System.Drawing.Size(150, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxEnum;
    }
}
