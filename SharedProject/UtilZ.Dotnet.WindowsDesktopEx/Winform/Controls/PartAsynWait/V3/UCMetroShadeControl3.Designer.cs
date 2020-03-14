namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.PartAsynWait.Excute.Winform.V3
{
    partial class UCMetroShadeControl3
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
            this.splitContainerExtern = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.labelControlCaption = new System.Windows.Forms.Label();
            this.labelControlTitle = new System.Windows.Forms.Label();
            this.pictureBoxImg = new System.Windows.Forms.PictureBox();
            this.btnCancell = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerExtern)).BeginInit();
            this.splitContainerExtern.Panel1.SuspendLayout();
            this.splitContainerExtern.Panel2.SuspendLayout();
            this.splitContainerExtern.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImg)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerExtern
            // 
            this.splitContainerExtern.IsSplitterFixed = true;
            this.splitContainerExtern.Location = new System.Drawing.Point(8, 8);
            this.splitContainerExtern.Name = "splitContainerExtern";
            this.splitContainerExtern.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerExtern.Panel1
            // 
            this.splitContainerExtern.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainerExtern.Panel2
            // 
            this.splitContainerExtern.Panel2.Controls.Add(this.btnCancell);
            this.splitContainerExtern.Size = new System.Drawing.Size(300, 140);
            this.splitContainerExtern.SplitterDistance = 111;
            this.splitContainerExtern.TabIndex = 1;
            this.splitContainerExtern.TabStop = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBoxImg);
            this.splitContainer2.Size = new System.Drawing.Size(300, 111);
            this.splitContainer2.SplitterDistance = 49;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.TabStop = false;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.labelControlCaption);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.labelControlTitle);
            this.splitContainer3.Size = new System.Drawing.Size(300, 49);
            this.splitContainer3.SplitterDistance = 25;
            this.splitContainer3.TabIndex = 0;
            this.splitContainer3.TabStop = false;
            // 
            // labelControlCaption
            // 
            this.labelControlCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControlCaption.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.labelControlCaption.Location = new System.Drawing.Point(0, 0);
            this.labelControlCaption.Name = "labelControlCaption";
            this.labelControlCaption.Size = new System.Drawing.Size(300, 25);
            this.labelControlCaption.TabIndex = 0;
            this.labelControlCaption.Text = "温馨提示";
            this.labelControlCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelControlTitle
            // 
            this.labelControlTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControlTitle.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.labelControlTitle.Location = new System.Drawing.Point(0, 0);
            this.labelControlTitle.Name = "labelControlTitle";
            this.labelControlTitle.Size = new System.Drawing.Size(300, 25);
            this.labelControlTitle.TabIndex = 0;
            this.labelControlTitle.Text = "正在保存，请稍候...";
            this.labelControlTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxImg
            // 
            this.pictureBoxImg.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBoxImg.Image = global::UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls.Resource.LoadWait;
            this.pictureBoxImg.Location = new System.Drawing.Point(122, 2);
            this.pictureBoxImg.Name = "pictureBoxImg";
            this.pictureBoxImg.Size = new System.Drawing.Size(54, 54);
            this.pictureBoxImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxImg.TabIndex = 0;
            this.pictureBoxImg.TabStop = false;
            // 
            // btnCancell
            // 
            this.btnCancell.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancell.AutoSize = true;
            this.btnCancell.Location = new System.Drawing.Point(111, 0);
            this.btnCancell.Name = "btnCancell";
            this.btnCancell.Size = new System.Drawing.Size(74, 23);
            this.btnCancell.TabIndex = 0;
            this.btnCancell.Text = "取消";
            this.btnCancell.UseVisualStyleBackColor = true;
            this.btnCancell.Click += new System.EventHandler(this.btnCancell_Click);
            // 
            // UCMetroShadeControl3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerExtern);
            this.Name = "UCMetroShadeControl3";
            this.Size = new System.Drawing.Size(300, 151);
            this.Load += new System.EventHandler(this.UCMetroShadeControl2_Load);
            this.splitContainerExtern.Panel1.ResumeLayout(false);
            this.splitContainerExtern.Panel2.ResumeLayout(false);
            this.splitContainerExtern.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerExtern)).EndInit();
            this.splitContainerExtern.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerExtern;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label labelControlCaption;
        private System.Windows.Forms.Label labelControlTitle;
        private System.Windows.Forms.Button btnCancell;
        private System.Windows.Forms.PictureBox pictureBoxImg;
    }
}
