using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilZ.Dotnet.Ex.Base;
using UtilZ.Dotnet.Wav.VoicePlayer.Ex;
using UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls;

namespace DotnetWinFormApp
{
    public partial class FTestWaveControl : Form
    {
        public FTestWaveControl()
        {
            InitializeComponent();
        }

        private void FTestWaveControl_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            //waveControl.BackColor = System.Drawing.Color.FromArgb(23, 23, 23);

            ////y坐标图元信息
            //waveControl.YAxis = new YAxisPlotElementInfo(30f,
            //     new SolidBrush(System.Drawing.Color.Blue),
            //     new SolidBrush(System.Drawing.Color.FromArgb(127, 113, 90)),
            //     PlotConstant.DEFAULLINE_WIDTH, new Font("MS UI Gothic", 8), DockStyle.Left);
            ////waveControl.YAxis = null;


            //int order = 1;

            //全局视图图元信息
            //waveControl.GlobalView = null;
            //waveControl.GlobalView = new GlobalViewPlotElementInfo(25f, new SolidBrush(System.Drawing.Color.Red),
            //new SolidBrush(System.Drawing.Color.FromArgb(84, 217, 150)), PlotConstant.DEFAULLINE_WIDTH, order++);

            //waveControl.GlobalView = new GlobalViewPlotElementInfo(25f, new SolidBrush(System.Drawing.Color.Red),
            //new SolidBrush(System.Drawing.Color.FromArgb(84, 217, 150)), PlotConstant.DEFAULLINE_WIDTH, 1);



            ////内容图元信息
            //waveControl.Content = new ContentPlotElementInfo(new SolidBrush(System.Drawing.Color.Yellow),
            //    new SolidBrush(System.Drawing.Color.FromArgb(84, 217, 150)), PlotConstant.DEFAULLINE_WIDTH, order++);


            ////x轴(时间)坐标图元信息
            //waveControl.XAxis = new XAxisPlotElementInfo(25f,
            //     new SolidBrush(System.Drawing.Color.Green),
            //     new SolidBrush(System.Drawing.Color.FromArgb(127, 113, 90)),
            //     PlotConstant.DEFAULLINE_WIDTH, new Font("MS UI Gothic", 8), order++);


            waveControl.DrawDensity = 3;
            waveControl.PlayLinePostionSetting += WaveControl_PlayLinePostionSetting; ;
        }


        private double? _offsetMilliseconds = null;
        private void WaveControl_PlayLinePostionSetting(object sender, PlayLinePostionSettingArgs e)
        {
            this._offsetMilliseconds = e.OffsetMilliseconds;
        }

        private PcmDataInfo _pcmDataInfo = null;
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = @"F:\FA\混音_h\Wavs\踏歌行_fs_48_chans_1_bit_16.wav";
                //filePath = @"F:\FA\混音_h\Wavs\踏歌行_fs_48_chans_1_bit_16_2222.wav";

                //filePath = @"F:\FA\混音_h\6chan\6chan.wma";
                filePath = @"F:\Music\踏歌行.mp3";

                filePath = @"G:\Tmp\Ring01.wav";
                filePath = @"G:\Tmp\踏歌行_01.wav";

                PcmDataInfo pcmDataInfo = FileSoundPlayer.GetPcmDataShort(filePath);
                this._pcmDataInfo = pcmDataInfo;
                short[] pcmData = pcmDataInfo.Data2;
                int chanelDataLength = pcmData.Length / pcmDataInfo.ChanelCount;
                List<short[]> targetFFTDataList = new List<short[]>(pcmDataInfo.ChanelCount);
                for (int i = 0; i < pcmDataInfo.ChanelCount; i++)
                {
                    targetFFTDataList.Add(new short[chanelDataLength]);
                }


                int index = 0;
                for (int i = 0; i < chanelDataLength; i++)
                {
                    for (int j = 0; j < pcmDataInfo.ChanelCount; j++)
                    {
                        targetFFTDataList[j][i] = pcmData[index];
                        index++;
                    }
                }

                List<ChannelPlotData> channelFFTDataList = new List<ChannelPlotData>(pcmDataInfo.ChanelCount);
                for (int i = 0; i < pcmDataInfo.ChanelCount; i++)
                {
                    channelFFTDataList.Add(new ChannelPlotData(targetFFTDataList[i], $"F-{i + 1}"));
                }

                //channelFFTDataList.Add(new ChannelPlotData(pcmData, $"F-123"));

                DateTime baseTime = DateTime.Parse("2020-01-01 00:00:00");
                var plotPara = new WavePlotPara(baseTime, pcmDataInfo.DurationSeconds * 1000);
                waveControl.UpdateData(channelFFTDataList, plotPara);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private ThreadEx _playLineThread = null;
        private void btnPlayLine_Click(object sender, EventArgs e)
        {
            if (this._pcmDataInfo == null)
            {
                return;
            }

            if (this._playLineThread == null)
            {
                this._playLineThread = new ThreadEx(this.PlayLineThreadMethod, "播放位置线线程");
            }

            if (this._playLineThread.ThreadState == ThreadState.Running)
            {
                this._playLineThread.Stop();
            }
            else
            {
                this._playLineThread.Start(this._pcmDataInfo.DurationSeconds);
            }
        }

        private void PlayLineThreadMethod(ThreadExPara threadPara)
        {
            double durationSeconds = (double)threadPara.Para * 1000;
            double offsetTimeMilliseconds = 0d;
            int interval = 100;

            while (!threadPara.Token.IsCancellationRequested && offsetTimeMilliseconds < durationSeconds)
            {
                threadPara.WaitOne(interval);

                if (this._offsetMilliseconds.HasValue)
                {
                    offsetTimeMilliseconds = this._offsetMilliseconds.Value;
                    this._offsetMilliseconds = null;
                }

                IAsyncResult asyncResult = this.BeginInvoke(new Action<double>((d) =>
                {
                    waveControl.UpdatePostionLine(d);
                }), new object[] { offsetTimeMilliseconds });
                asyncResult.AsyncWaitHandle.WaitOne();
                offsetTimeMilliseconds += interval;
            }
        }
    }
}
