namespace DotnetWinFormApp
{
    partial class FTestPartAsynWait
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
            this.btnTestAsynWait = new System.Windows.Forms.Button();
            this.btnSetGenericToComboBox = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnGetGenericFromComboBox = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.btnGetGenericFromToolStripComboBox = new System.Windows.Forms.Button();
            this.btnSetGenericToToolStripComboBox = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTestAsynWait
            // 
            this.btnTestAsynWait.Location = new System.Drawing.Point(10, 200);
            this.btnTestAsynWait.Name = "btnTestAsynWait";
            this.btnTestAsynWait.Size = new System.Drawing.Size(200, 23);
            this.btnTestAsynWait.TabIndex = 0;
            this.btnTestAsynWait.Text = "TestAsynWait";
            this.btnTestAsynWait.UseVisualStyleBackColor = true;
            this.btnTestAsynWait.Click += new System.EventHandler(this.btnTestAsynWait_Click);
            // 
            // btnSetGenericToComboBox
            // 
            this.btnSetGenericToComboBox.Location = new System.Drawing.Point(10, 126);
            this.btnSetGenericToComboBox.Name = "btnSetGenericToComboBox";
            this.btnSetGenericToComboBox.Size = new System.Drawing.Size(200, 23);
            this.btnSetGenericToComboBox.TabIndex = 1;
            this.btnSetGenericToComboBox.Text = "SetGenericToComboBox";
            this.btnSetGenericToComboBox.UseVisualStyleBackColor = true;
            this.btnSetGenericToComboBox.Click += new System.EventHandler(this.btnSetGenericToComboBox_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(10, 89);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 2;
            // 
            // btnGetGenericFromComboBox
            // 
            this.btnGetGenericFromComboBox.Location = new System.Drawing.Point(300, 126);
            this.btnGetGenericFromComboBox.Name = "btnGetGenericFromComboBox";
            this.btnGetGenericFromComboBox.Size = new System.Drawing.Size(200, 23);
            this.btnGetGenericFromComboBox.TabIndex = 3;
            this.btnGetGenericFromComboBox.Text = "GetGenericFromComboBox";
            this.btnGetGenericFromComboBox.UseVisualStyleBackColor = true;
            this.btnGetGenericFromComboBox.Click += new System.EventHandler(this.btnGetGenericFromComboBox_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 25);
            // 
            // btnGetGenericFromToolStripComboBox
            // 
            this.btnGetGenericFromToolStripComboBox.Location = new System.Drawing.Point(300, 44);
            this.btnGetGenericFromToolStripComboBox.Name = "btnGetGenericFromToolStripComboBox";
            this.btnGetGenericFromToolStripComboBox.Size = new System.Drawing.Size(200, 23);
            this.btnGetGenericFromToolStripComboBox.TabIndex = 6;
            this.btnGetGenericFromToolStripComboBox.Text = "GetGenericFromToolStripComboBox";
            this.btnGetGenericFromToolStripComboBox.UseVisualStyleBackColor = true;
            this.btnGetGenericFromToolStripComboBox.Click += new System.EventHandler(this.btnGetGenericFromToolStripComboBox_Click);
            // 
            // btnSetGenericToToolStripComboBox
            // 
            this.btnSetGenericToToolStripComboBox.Location = new System.Drawing.Point(10, 44);
            this.btnSetGenericToToolStripComboBox.Name = "btnSetGenericToToolStripComboBox";
            this.btnSetGenericToToolStripComboBox.Size = new System.Drawing.Size(200, 23);
            this.btnSetGenericToToolStripComboBox.TabIndex = 5;
            this.btnSetGenericToToolStripComboBox.Text = "SetGenericToToolStripComboBox";
            this.btnSetGenericToToolStripComboBox.UseVisualStyleBackColor = true;
            this.btnSetGenericToToolStripComboBox.Click += new System.EventHandler(this.btnSetGenericToToolStripComboBox_Click);
            // 
            // FTestPartAsynWait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnGetGenericFromToolStripComboBox);
            this.Controls.Add(this.btnSetGenericToToolStripComboBox);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnGetGenericFromComboBox);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnSetGenericToComboBox);
            this.Controls.Add(this.btnTestAsynWait);
            this.Name = "FTestPartAsynWait";
            this.Text = "FTestPartAsynWait";
            this.Load += new System.EventHandler(this.FTestPartAsynWait_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTestAsynWait;
        private System.Windows.Forms.Button btnSetGenericToComboBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnGetGenericFromComboBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.Button btnGetGenericFromToolStripComboBox;
        private System.Windows.Forms.Button btnSetGenericToToolStripComboBox;
    }
}