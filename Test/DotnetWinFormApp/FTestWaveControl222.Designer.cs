namespace DotnetWinFormApp
{
    partial class FTestWaveControl222
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
            this.btnPlayLine = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.waveControl = new UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl.WaveControl();
            this.SuspendLayout();
            // 
            // btnPlayLine
            // 
            this.btnPlayLine.Location = new System.Drawing.Point(109, 13);
            this.btnPlayLine.Name = "btnPlayLine";
            this.btnPlayLine.Size = new System.Drawing.Size(75, 23);
            this.btnPlayLine.TabIndex = 5;
            this.btnPlayLine.Text = "PlayLine";
            this.btnPlayLine.UseVisualStyleBackColor = true;
            this.btnPlayLine.Click += new System.EventHandler(this.btnPlayLine_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(12, 13);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // waveControl
            // 
            this.waveControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.waveControl.Location = new System.Drawing.Point(12, 53);
            this.waveControl.MinimumSize = new System.Drawing.Size(200, 130);
            this.waveControl.Name = "waveControl";
            this.waveControl.PlayPositionLine = true;
            this.waveControl.Size = new System.Drawing.Size(776, 385);
            this.waveControl.TabIndex = 3;
            this.waveControl.Text = "waveControl1";
            // 
            // FTestWaveControl222
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPlayLine);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.waveControl);
            this.Name = "FTestWaveControl222";
            this.Text = "FTestWaveControl222";
            this.Load += new System.EventHandler(this.FTestWaveControl222_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPlayLine;
        private System.Windows.Forms.Button btnTest;
        private UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl.WaveControl waveControl;
    }
}