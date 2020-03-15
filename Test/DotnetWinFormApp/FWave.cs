using System;
using System.Windows.Forms;
using UtilZ.Dotnet.Wav;
using UtilZ.Dotnet.Wav.ExBass;
using UtilZ.Dotnet.Wav.Model;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Base;

namespace DotnetWinFormApp
{
    public partial class FWave : Form
    {
        public FWave()
        {
            InitializeComponent();

            wavePlayer1.WavLog += wavePlayer1_WavLog;
            wavePlayer1.IsDrawWavMidLine = true;
            //wavePlayer1.PlayEnd += wavePlayer1_PlayEnd;
            wavePlayer1.DrawInterval = 100;
            //wavePlayer1.EnableTimeBackground = false;
            //wavePlayer1.EnableDbAreaBackground = false;
        }

        void wavePlayer1_PlayEnd(object sender, EventArgs e)
        {
            MessageBox.Show("播放完成");
        }

        void wavePlayer1_WavLog(object sender, UtilZ.Dotnet.Wav.Model.WavLogInfoArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        private void FWave22_Load(object sender, EventArgs e)
        {
            try
            {
                DropdownBoxHelper.BindingEnumToComboBox<BASSAttribute>(comboBoxBASSAttribute, BASSAttribute.BASS_ATTRIB_EAXMIX);

                this.trackBarQualityCoefficient.ValueChanged += new System.EventHandler(this.trackBarQualityCoefficient_ValueChanged);

                cbIsMergeChanel.Checked = wavePlayer1.IsMergeChanel;
                cbRingHear.Checked = wavePlayer1.IsRingHear;
                var ret = UtilZ.Dotnet.Wav.ExBass.Bass.BASS_GetVolume();
                if (ret <= 0)
                {
                    wavePlayer1.Volume = 80;
                }
                else
                {
                    trackBarVolume.Value = wavePlayer1.Volume;
                }

                trackBarQualityCoefficient.Value = wavePlayer1.QualityCoefficient;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = @"E:\wav";
                //ofd.Filter = @"*.wav|*.wav";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                wavePlayer1.FileName = ofd.FileName;
                trackBarTime.Maximum = (int)wavePlayer1.DurationTime;
                this.Text = ofd.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.Pause();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void trackBarQualityCoefficient_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.QualityCoefficient = trackBarQualityCoefficient.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void trackBarVolume_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.Volume = trackBarVolume.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void trackBarTime_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.Position = trackBarTime.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {

                //wavePlayer1.VoiceSlide(0.2f, 3000);
                //SaveFileDialog sfd = new SaveFileDialog();
                //if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                //{
                //    return;
                //}

                //wavePlayer1.SaveSelectionToFile(sfd.FileName);

                int ret = WavePlayer.BASS_GetConfig(BassConfigOption.BASS_CONFIG_UPDATETHREADS);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbIsMergeChanel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.IsMergeChanel = cbIsMergeChanel.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbRingHear_CheckedChanged(object sender, EventArgs e)
        {
            wavePlayer1.IsRingHear = cbRingHear.Checked;
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                wavePlayer1.PlaySpeed = ((float)trackBarSpeed.Value) / 10;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnJiaoZhun_Click(object sender, EventArgs e)
        {
            try
            {
                trackBarSpeed.Value = (int)(wavePlayer1.PlaySpeed * 10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGetValue_Click(object sender, EventArgs e)
        {
            try
            {
                BASSAttribute attri = DropdownBoxHelper.GetEnumFromComboBox<BASSAttribute>(comboBoxBASSAttribute);
                txtInfo.Text = wavePlayer1.GetChannelAttribute(attri).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSetValue_Click(object sender, EventArgs e)
        {
            try
            {
                BASSAttribute attri = DropdownBoxHelper.GetEnumFromComboBox<BASSAttribute>(comboBoxBASSAttribute);
                float value = (float)(numAttriValue.Value / 100);
                wavePlayer1.SetChannelAttribute(attri, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnFastSpeedPlay_Click(object sender, EventArgs e)
        {
            try
            {
                float value = (float)numFastSpeedReverseValue.Value;
                wavePlayer1.FastSpeedReversePlay(true, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnFastReversePlay_Click(object sender, EventArgs e)
        {
            try
            {
                float value = (float)numFastSpeedReverseValue.Value;
                wavePlayer1.FastSpeedReversePlay(false, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
