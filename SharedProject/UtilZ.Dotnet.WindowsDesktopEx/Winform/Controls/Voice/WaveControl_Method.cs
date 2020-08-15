using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    //WavePlayer-Method
    public partial class WaveControl
    {
        private WavePlotPara _plotPara = null;
        private IEnumerable<ChannelPlotData> _plotDatas = null;
        private SelectedInfo _selectedInfo = null;
        private readonly ZoomInfo _zoomInfo = new ZoomInfo();


        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            this._plotDatas = null;
            this._plotPara = null;

            //全部绘制
            Graphics graphics = this._grafx.Graphics;

            //清空所有已绘制的图形
            graphics.Clear(this.BackColor);

            this.RefreshInvalidateArea();//刷新
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plotPara">绘图参数</param>
        /// <param name="pcmData">pcm数据</param>
        public void UpdateData(WavePlotPara plotPara, short[] pcmData)
        {
            this.UpdateData(new ChannelPlotData[] { new ChannelPlotData(pcmData, null) }, plotPara);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fftDatas">
        /// 声道数据集合
        /// 单声道:L
        /// 双声道:L-R
        /// 四声道:L-R-LS-RS
        /// 5.1声道FFT数据顺序: L-R-C-LFE-LS-RS
        /// 7.1声道FFT数据顺序: L-R-C-LFE-LS-RS-不晓得了
        /// </param>
        /// <param name="plotPara">绘图参数</param>
        public void UpdateData(IEnumerable<ChannelPlotData> fftDatas, WavePlotPara plotPara)
        {
            if (plotPara == null)
            {
                throw new ArgumentNullException(nameof(plotPara));
            }

            this._plotPara = plotPara;
            int srcPcmMaxDataLength = 0, drawPcmMaxDataLength = 0;

            if (fftDatas != null)
            {
                int targetCount = this.GetResamplePointCount();
                foreach (ChannelPlotData channelFFTData in fftDatas)
                {
                    if (channelFFTData == null || channelFFTData.PCMData == null)
                    {
                        continue;
                    }

                    if (srcPcmMaxDataLength == 0)
                    {
                        srcPcmMaxDataLength = channelFFTData.PCMData.Length;
                    }
                    else
                    {
                        if (srcPcmMaxDataLength != channelFFTData.PCMData.Length)
                        {
                            throw new ArgumentException("FFT数据长度不一致");
                        }
                    }


                    channelFFTData.Resample(targetCount, 0, channelFFTData.PCMData.Length);
                    channelFFTData.GlobalViewData = channelFFTData.DrawData;
                    if (drawPcmMaxDataLength < channelFFTData.DrawData.Length)
                    {
                        drawPcmMaxDataLength = channelFFTData.DrawData.Length;
                    }
                }
            }
            this._plotDatas = fftDatas;
            plotPara.SourcePcmDataLength = srcPcmMaxDataLength;
            plotPara.DrawPcmDataLength = drawPcmMaxDataLength;
            plotPara.GlobalViewPcmDataLength = drawPcmMaxDataLength;
            this.AllDraw();
        }

        private int GetResamplePointCount()
        {
            return (int)(this._content.Area.Width * this._drawDensity);
        }

        private void SizeChangedResample()
        {
            WavePlotPara plotPara = this._plotPara;
            if (plotPara == null)
            {
                return;
            }

            int globalViewFFTDataLength = 0;
            var fftDatas = this._plotDatas;
            if (fftDatas != null)
            {
                int targetCount = this.GetResamplePointCount();
                foreach (ChannelPlotData channelFFTData in fftDatas)
                {
                    if (channelFFTData.PCMData == null)
                    {
                        continue;
                    }

                    channelFFTData.GlobalViewData = channelFFTData.PrimitiveResample(targetCount, 0, channelFFTData.PCMData.Length);
                    if (globalViewFFTDataLength == 0 || globalViewFFTDataLength < channelFFTData.GlobalViewData.Length)
                    {
                        globalViewFFTDataLength = channelFFTData.GlobalViewData.Length;
                    }
                }
            }

            plotPara.GlobalViewPcmDataLength = globalViewFFTDataLength;
        }

        private void Resample()
        {
            WavePlotPara plotPara = this._plotPara;
            if (plotPara == null)
            {
                return;
            }

            //计算重采样数据起始-结束索引
            int wavBegeinIndex, wavEndIndex;
            this.CalWavSpecturumArea(plotPara, out wavBegeinIndex, out wavEndIndex);

            //int voiceBegeinIndex, voiceEndIndex;
            //this.CalVoiceSpecturumArea(plotPara, out voiceBegeinIndex, out voiceEndIndex);


            //重采样
            int srcFFTDataLength = 0, drawFFTDataLength = 0;
            var fftDatas = this._plotDatas;
            if (fftDatas != null)
            {
                int resampleTargetCount = this.GetResamplePointCount();
                foreach (ChannelPlotData channelFFTData in fftDatas)
                {
                    if (channelFFTData.PCMData == null)
                    {
                        continue;
                    }

                    if (srcFFTDataLength == 0)
                    {
                        srcFFTDataLength = channelFFTData.PCMData.Length;
                    }
                    else
                    {
                        if (srcFFTDataLength != channelFFTData.PCMData.Length)
                        {
                            throw new ArgumentException("FFT数据长度不一致");
                        }
                    }

                    channelFFTData.Resample(resampleTargetCount, wavBegeinIndex, wavEndIndex);
                    //channelFFTData.GlobalViewData = channelFFTData.DrawData;
                    if (drawFFTDataLength < channelFFTData.DrawData.Length)
                    {
                        drawFFTDataLength = channelFFTData.DrawData.Length;
                    }
                }
            }

            plotPara.DrawPcmDataLength = drawFFTDataLength;
        }

        //private void CalVoiceSpecturumArea(WavePlotPara plotPara, out int begeinIndex, out int endIndex)
        //{
        //    int frameCount = plotPara.FrameCount;
        //    begeinIndex = (int)(frameCount * plotPara.SBTOMillisecond / plotPara.DurationMillisecond);
        //    endIndex = (int)(frameCount * plotPara.GetSETOMillisecond() / plotPara.DurationMillisecond);
        //    if (begeinIndex < 0)
        //    {
        //        int offset = Math.Abs(begeinIndex);
        //        begeinIndex = 0;
        //        endIndex += offset;
        //        if (endIndex > frameCount)
        //        {
        //            endIndex = frameCount;
        //        }
        //    }
        //    else
        //    {
        //        if (endIndex > frameCount)
        //        {
        //            begeinIndex = begeinIndex - (endIndex - frameCount);
        //            endIndex = frameCount;
        //            if (begeinIndex < 0)
        //            {
        //                begeinIndex = 0;
        //            }
        //        }
        //    }
        //}
        private void CalWavSpecturumArea(WavePlotPara plotPara, out int begeinIndex, out int endIndex)
        {
            int sourceFFTDataLength = plotPara.SourcePcmDataLength;
            begeinIndex = (int)(sourceFFTDataLength * plotPara.SBTOMillisecond / plotPara.DurationMillisecond);
            endIndex = (int)(sourceFFTDataLength * plotPara.GetSETOMillisecond() / plotPara.DurationMillisecond);
            if (begeinIndex < 0)
            {
                int offset = Math.Abs(begeinIndex);
                begeinIndex = 0;
                endIndex += offset;
                if (endIndex > sourceFFTDataLength)
                {
                    endIndex = sourceFFTDataLength;
                }
            }
            else
            {
                if (endIndex > sourceFFTDataLength)
                {
                    begeinIndex = begeinIndex - (endIndex - sourceFFTDataLength);
                    endIndex = sourceFFTDataLength;
                    if (begeinIndex < 0)
                    {
                        begeinIndex = 0;
                    }
                }
            }
        }





        /// <summary>
        /// 放大时间
        /// </summary>
        public void ZoomIn()
        {
            if (this._plotPara == null)
            {
                return;
            }

            this._zoomInfo.ZoomIn(this._plotPara);
            this.Resample();
            this.PartDraw_ZoomMove();
        }

        /// <summary>
        /// 缩小时间
        /// </summary>
        public void ZoomOut()
        {
            if (this._plotPara == null)
            {
                return;
            }

            this._zoomInfo.ZoomOut(this._plotPara);
            this.Resample();
            this.PartDraw_ZoomMove();
        }

        /// <summary>
        /// 更新播放位置指示线
        /// </summary>
        /// <param name="time">播放时间</param>
        public void UpdatePostionLine(DateTime time)
        {
            var plotPara = this._plotPara;
            if (plotPara == null)
            {
                //参数为空
                return;
            }

            double offsetTimeMilliseconds = (time - plotPara.BaseTime).TotalMilliseconds;
            this.PrimitiveUpdatePostionLine(plotPara, offsetTimeMilliseconds);
        }

        /// <summary>
        /// 更新播放位置指示线
        /// </summary>
        /// <param name="offsetTimeMilliseconds">播放时间</param>
        public void UpdatePostionLine(double offsetTimeMilliseconds)
        {
            var plotPara = this._plotPara;
            if (plotPara == null)
            {
                //参数为空
                return;
            }

            this.PrimitiveUpdatePostionLine(plotPara, offsetTimeMilliseconds);
        }

        /// <summary>
        /// 保存选中区域到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveSelectionToFile(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 快进
        /// </summary>
        /// <param name="time">快进时跳过的时间长度,单位/秒</param>
        public void Forward(double time)
        {
            //if (double.IsInfinity(time) || double.IsNaN(time) || time < WavePlayerConstant.PRECISION)
            //{
            //    throw new ArgumentException($"时间值\"{time}\"无效");
            //}

            //double currentPos = this._soundPlayer.Position;
            //double newPos = currentPos + time;
            //double duration = this._soundPlayer.Duration;
            //if (newPos > this._soundPlayer.Duration)
            //{
            //    newPos = duration;
            //}
            //this._soundPlayer.Position = newPos;
        }

        /// <summary>
        /// 快退
        /// </summary>
        /// <param name="time">快退时跳过的时间长度,单位/秒</param>
        public void Back(double time)
        {
            //if (double.IsInfinity(time) || double.IsNaN(time) || time < WavePlayerConstant.PRECISION)
            //{
            //    throw new ArgumentException($"时间值\"{time}\"无效");
            //}

            //double currentPos = this._soundPlayer.Position;
            //double newPos = currentPos - time;
            //double duration = this._soundPlayer.Duration;
            //if (newPos < WavePlayerConstant.PRECISION)
            //{
            //    newPos = 0d;
            //}
            //this._soundPlayer.Position = newPos;
        }

    }
}
