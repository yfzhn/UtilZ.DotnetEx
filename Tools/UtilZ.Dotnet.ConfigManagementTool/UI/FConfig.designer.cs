namespace UtilZ.Dotnet.ConfigManagementTool.UI
{
    partial class FConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FConfig));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbServiceMapManager = new System.Windows.Forms.ToolStripButton();
            this.tsbParaGroupManager = new System.Windows.Forms.ToolStripButton();
            this.tsbConfigParaManager = new System.Windows.Forms.ToolStripButton();
            this.tsbViewSwitch = new System.Windows.Forms.ToolStripButton();
            this.panelView = new System.Windows.Forms.Panel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbServiceMapManager,
            this.tsbParaGroupManager,
            this.tsbConfigParaManager,
            this.tsbViewSwitch});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbServiceMapManager
            // 
            this.tsbServiceMapManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbServiceMapManager.Image = global::UtilZ.Dotnet.ConfigManagementTool.Properties.Resources.Map;
            this.tsbServiceMapManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbServiceMapManager.Name = "tsbServiceMapManager";
            this.tsbServiceMapManager.Size = new System.Drawing.Size(23, 22);
            this.tsbServiceMapManager.Text = "服务映射管理";
            this.tsbServiceMapManager.Click += new System.EventHandler(this.tsbServiceMapManager_Click);
            // 
            // tsbParaGroupManager
            // 
            this.tsbParaGroupManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbParaGroupManager.Image = global::UtilZ.Dotnet.ConfigManagementTool.Properties.Resources.group;
            this.tsbParaGroupManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbParaGroupManager.Name = "tsbParaGroupManager";
            this.tsbParaGroupManager.Size = new System.Drawing.Size(23, 22);
            this.tsbParaGroupManager.Text = "配置参数组管理";
            this.tsbParaGroupManager.Click += new System.EventHandler(this.tsbParaGroupManager_Click);
            // 
            // tsbConfigParaManager
            // 
            this.tsbConfigParaManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConfigParaManager.Image = global::UtilZ.Dotnet.ConfigManagementTool.Properties.Resources.ParaManager;
            this.tsbConfigParaManager.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConfigParaManager.Name = "tsbConfigParaManager";
            this.tsbConfigParaManager.Size = new System.Drawing.Size(23, 22);
            this.tsbConfigParaManager.Text = "配置参数管理";
            this.tsbConfigParaManager.Click += new System.EventHandler(this.tsbConfigParaManager_Click);
            // 
            // tsbViewSwitch
            // 
            this.tsbViewSwitch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbViewSwitch.Image = global::UtilZ.Dotnet.ConfigManagementTool.Properties.Resources.View;
            this.tsbViewSwitch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbViewSwitch.Name = "tsbViewSwitch";
            this.tsbViewSwitch.Size = new System.Drawing.Size(23, 22);
            this.tsbViewSwitch.Text = "视图切换";
            this.tsbViewSwitch.Click += new System.EventHandler(this.tsbViewSwitch_Click);
            // 
            // panelView
            // 
            this.panelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelView.Location = new System.Drawing.Point(0, 25);
            this.panelView.Name = "panelView";
            this.panelView.Size = new System.Drawing.Size(784, 536);
            this.panelView.TabIndex = 2;
            // 
            // FConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panelView);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配置参数管理";
            this.Load += new System.EventHandler(this.FConfig_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbServiceMapManager;
        private System.Windows.Forms.ToolStripButton tsbParaGroupManager;
        private System.Windows.Forms.ToolStripButton tsbConfigParaManager;
        private System.Windows.Forms.ToolStripButton tsbViewSwitch;
        private System.Windows.Forms.Panel panelView;
    }
}

