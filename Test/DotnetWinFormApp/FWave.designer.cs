namespace DotnetWinFormApp
{
    partial class FWave
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
            this.trackBarQualityCoefficient = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.trackBarTime = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.cbIsMergeChanel = new System.Windows.Forms.CheckBox();
            this.cbRingHear = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBarSpeed = new System.Windows.Forms.TrackBar();
            this.comboBoxBASSAttribute = new System.Windows.Forms.ComboBox();
            this.btnJiaoZhun = new System.Windows.Forms.Button();
            this.btnGetValue = new System.Windows.Forms.Button();
            this.btnSetValue = new System.Windows.Forms.Button();
            this.numAttriValue = new System.Windows.Forms.NumericUpDown();
            this.wavePlayer1 = new UtilZ.Dotnet.Wav.WavePlayer();
            this.btnFastSpeedPlay = new System.Windows.Forms.Button();
            this.btnFastReversePlay = new System.Windows.Forms.Button();
            this.numFastSpeedReverseValue = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarQualityCoefficient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttriValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFastSpeedReverseValue)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarQualityCoefficient
            // 
            this.trackBarQualityCoefficient.Location = new System.Drawing.Point(647, 11);
            this.trackBarQualityCoefficient.Maximum = 5;
            this.trackBarQualityCoefficient.Minimum = 1;
            this.trackBarQualityCoefficient.Name = "trackBarQualityCoefficient";
            this.trackBarQualityCoefficient.Size = new System.Drawing.Size(365, 45);
            this.trackBarQualityCoefficient.TabIndex = 30;
            this.trackBarQualityCoefficient.Value = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(528, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 29;
            this.label3.Text = "QualityCoefficient";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(23, 12);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 26;
            this.btnOpenFile.Text = "OpenFile";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(266, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 25;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(185, 12);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 24;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(104, 12);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 23;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // trackBarTime
            // 
            this.trackBarTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarTime.Location = new System.Drawing.Point(104, 75);
            this.trackBarTime.Maximum = 100;
            this.trackBarTime.Name = "trackBarTime";
            this.trackBarTime.Size = new System.Drawing.Size(1157, 45);
            this.trackBarTime.TabIndex = 36;
            this.trackBarTime.ValueChanged += new System.EventHandler(this.trackBarTime_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 35;
            this.label2.Text = "Time";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 34;
            this.label1.Text = "volume";
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(92, 181);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(237, 21);
            this.txtInfo.TabIndex = 33;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(11, 181);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 32;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarVolume.Location = new System.Drawing.Point(104, 42);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(1157, 45);
            this.trackBarVolume.TabIndex = 31;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            // 
            // cbIsMergeChanel
            // 
            this.cbIsMergeChanel.AutoSize = true;
            this.cbIsMergeChanel.Location = new System.Drawing.Point(23, 114);
            this.cbIsMergeChanel.Name = "cbIsMergeChanel";
            this.cbIsMergeChanel.Size = new System.Drawing.Size(156, 16);
            this.cbIsMergeChanel.TabIndex = 38;
            this.cbIsMergeChanel.Text = "是否合并左右声道波形图";
            this.cbIsMergeChanel.UseVisualStyleBackColor = true;
            this.cbIsMergeChanel.CheckedChanged += new System.EventHandler(this.cbIsMergeChanel_CheckedChanged);
            // 
            // cbRingHear
            // 
            this.cbRingHear.AutoSize = true;
            this.cbRingHear.Location = new System.Drawing.Point(23, 137);
            this.cbRingHear.Name = "cbRingHear";
            this.cbRingHear.Size = new System.Drawing.Size(48, 16);
            this.cbRingHear.TabIndex = 39;
            this.cbRingHear.Text = "环听";
            this.cbRingHear.UseVisualStyleBackColor = true;
            this.cbRingHear.CheckedChanged += new System.EventHandler(this.cbRingHear_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 40;
            this.label4.Text = "播放速度";
            // 
            // trackBarSpeed
            // 
            this.trackBarSpeed.Location = new System.Drawing.Point(149, 130);
            this.trackBarSpeed.Maximum = 20;
            this.trackBarSpeed.Minimum = 1;
            this.trackBarSpeed.Name = "trackBarSpeed";
            this.trackBarSpeed.Size = new System.Drawing.Size(533, 45);
            this.trackBarSpeed.TabIndex = 41;
            this.trackBarSpeed.Value = 1;
            this.trackBarSpeed.ValueChanged += new System.EventHandler(this.trackBarSpeed_ValueChanged);
            // 
            // comboBoxBASSAttribute
            // 
            this.comboBoxBASSAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBASSAttribute.FormattingEnabled = true;
            this.comboBoxBASSAttribute.Location = new System.Drawing.Point(344, 181);
            this.comboBoxBASSAttribute.Name = "comboBoxBASSAttribute";
            this.comboBoxBASSAttribute.Size = new System.Drawing.Size(338, 20);
            this.comboBoxBASSAttribute.TabIndex = 42;
            // 
            // btnJiaoZhun
            // 
            this.btnJiaoZhun.Location = new System.Drawing.Point(689, 125);
            this.btnJiaoZhun.Name = "btnJiaoZhun";
            this.btnJiaoZhun.Size = new System.Drawing.Size(75, 23);
            this.btnJiaoZhun.TabIndex = 43;
            this.btnJiaoZhun.Text = "校准";
            this.btnJiaoZhun.UseVisualStyleBackColor = true;
            this.btnJiaoZhun.Click += new System.EventHandler(this.btnJiaoZhun_Click);
            // 
            // btnGetValue
            // 
            this.btnGetValue.Location = new System.Drawing.Point(689, 179);
            this.btnGetValue.Name = "btnGetValue";
            this.btnGetValue.Size = new System.Drawing.Size(75, 23);
            this.btnGetValue.TabIndex = 44;
            this.btnGetValue.Text = "获取值";
            this.btnGetValue.UseVisualStyleBackColor = true;
            this.btnGetValue.Click += new System.EventHandler(this.btnGetValue_Click);
            // 
            // btnSetValue
            // 
            this.btnSetValue.Location = new System.Drawing.Point(689, 209);
            this.btnSetValue.Name = "btnSetValue";
            this.btnSetValue.Size = new System.Drawing.Size(75, 23);
            this.btnSetValue.TabIndex = 45;
            this.btnSetValue.Text = "设置值";
            this.btnSetValue.UseVisualStyleBackColor = true;
            this.btnSetValue.Click += new System.EventHandler(this.btnSetValue_Click);
            // 
            // numAttriValue
            // 
            this.numAttriValue.Location = new System.Drawing.Point(344, 208);
            this.numAttriValue.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numAttriValue.Name = "numAttriValue";
            this.numAttriValue.Size = new System.Drawing.Size(339, 21);
            this.numAttriValue.TabIndex = 46;
            // 
            // wavePlayer1
            // 
            this.wavePlayer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wavePlayer1.BackColor = System.Drawing.Color.SlateBlue;
            this.wavePlayer1.BackgroudColor = System.Drawing.Color.Black;
            this.wavePlayer1.DbWidth = 50;
            this.wavePlayer1.DrawInterval = 100;
            this.wavePlayer1.EnableDbAreaBackground = true;
            this.wavePlayer1.EnableLogoAreaBackground = false;
            this.wavePlayer1.EnableTimeBackground = true;
            this.wavePlayer1.EnableWavAreaBackground = false;
            this.wavePlayer1.EnableZoomWavBackground = false;
            this.wavePlayer1.Freq = 44100;
            this.wavePlayer1.IsDrawChannelDivideLine = true;
            this.wavePlayer1.IsDrawPlayLine = true;
            this.wavePlayer1.IsDrawWavMidLine = true;
            this.wavePlayer1.IsMergeChanel = true;
            this.wavePlayer1.IsVersionValidate = true;
            this.wavePlayer1.Location = new System.Drawing.Point(11, 265);
            this.wavePlayer1.MinimumSize = new System.Drawing.Size(200, 100);
            this.wavePlayer1.Name = "wavePlayer1";
            this.wavePlayer1.PlayLocationLineRefreshInterval = 100;
            this.wavePlayer1.QualityCoefficient = 1;
            this.wavePlayer1.Size = new System.Drawing.Size(1240, 242);
            this.wavePlayer1.TabIndex = 37;
            this.wavePlayer1.WavSelecteMouseStyle = System.Windows.Forms.Cursors.IBeam;
            this.wavePlayer1.ZoomHeight = 60;
            this.wavePlayer1.ZoomMuilt = 2;
            this.wavePlayer1.ZoomWavDisplayAreaMmoveMouseStyle = System.Windows.Forms.Cursors.SizeAll;
            // 
            // btnFastSpeedPlay
            // 
            this.btnFastSpeedPlay.Location = new System.Drawing.Point(805, 124);
            this.btnFastSpeedPlay.Name = "btnFastSpeedPlay";
            this.btnFastSpeedPlay.Size = new System.Drawing.Size(75, 23);
            this.btnFastSpeedPlay.TabIndex = 47;
            this.btnFastSpeedPlay.Text = "快进";
            this.btnFastSpeedPlay.UseVisualStyleBackColor = true;
            this.btnFastSpeedPlay.Click += new System.EventHandler(this.btnFastSpeedPlay_Click);
            // 
            // btnFastReversePlay
            // 
            this.btnFastReversePlay.Location = new System.Drawing.Point(805, 151);
            this.btnFastReversePlay.Name = "btnFastReversePlay";
            this.btnFastReversePlay.Size = new System.Drawing.Size(75, 23);
            this.btnFastReversePlay.TabIndex = 48;
            this.btnFastReversePlay.Text = "快退";
            this.btnFastReversePlay.UseVisualStyleBackColor = true;
            this.btnFastReversePlay.Click += new System.EventHandler(this.btnFastReversePlay_Click);
            // 
            // numFastSpeedReverseValue
            // 
            this.numFastSpeedReverseValue.Location = new System.Drawing.Point(902, 127);
            this.numFastSpeedReverseValue.Name = "numFastSpeedReverseValue";
            this.numFastSpeedReverseValue.Size = new System.Drawing.Size(120, 21);
            this.numFastSpeedReverseValue.TabIndex = 49;
            // 
            // FWave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1264, 519);
            this.Controls.Add(this.numFastSpeedReverseValue);
            this.Controls.Add(this.btnFastReversePlay);
            this.Controls.Add(this.btnFastSpeedPlay);
            this.Controls.Add(this.numAttriValue);
            this.Controls.Add(this.btnSetValue);
            this.Controls.Add(this.btnGetValue);
            this.Controls.Add(this.btnJiaoZhun);
            this.Controls.Add(this.comboBoxBASSAttribute);
            this.Controls.Add(this.trackBarSpeed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbRingHear);
            this.Controls.Add(this.cbIsMergeChanel);
            this.Controls.Add(this.wavePlayer1);
            this.Controls.Add(this.trackBarTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.trackBarVolume);
            this.Controls.Add(this.trackBarQualityCoefficient);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnPlay);
            this.Name = "FWave";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FWave22";
            this.Load += new System.EventHandler(this.FWave22_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarQualityCoefficient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAttriValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFastSpeedReverseValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarQualityCoefficient;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.TrackBar trackBarTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private UtilZ.Dotnet.Wav.WavePlayer wavePlayer1;
        private System.Windows.Forms.CheckBox cbIsMergeChanel;
        private System.Windows.Forms.CheckBox cbRingHear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBarSpeed;
        private System.Windows.Forms.ComboBox comboBoxBASSAttribute;
        private System.Windows.Forms.Button btnJiaoZhun;
        private System.Windows.Forms.Button btnGetValue;
        private System.Windows.Forms.Button btnSetValue;
        private System.Windows.Forms.NumericUpDown numAttriValue;
        private System.Windows.Forms.Button btnFastSpeedPlay;
        private System.Windows.Forms.Button btnFastReversePlay;
        private System.Windows.Forms.NumericUpDown numFastSpeedReverseValue;
    }
}