using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Wav.ExBass;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav
{
    // WAV播放控件-方法
    public partial class WavePlayer
    {
        /// <summary>
        /// 设置选项值
        /// </summary>
        /// <param name="option">选项</param>
        /// <param name="value">选项值</param>
        public static bool BASS_SetConfig(BassConfigOption option, int value)
        {
            return Bass.BASS_SetConfig(option, value);
        }

        /// <summary>
        /// 获取选项值
        /// </summary>
        /// <param name="option">选项</param>
        /// <returns>选项值</returns>
        public static int BASS_GetConfig(BassConfigOption option)
        {
            return Bass.BASS_GetConfig(option);
        }

        /// <summary>
        /// 播放播放
        /// </summary>
        public void Play()
        {
            var playStatus = this.GrtPlayState();
            if (playStatus == PlayStatus.PLAYING || this._handle == -1)
            {
                return;
            }

            if (Bass.BASS_ChannelPlay(this._handle, playStatus == PlayStatus.STOPPED))
            {
                this._playLocationLineTimer.Start();
            }
            else
            {
                this.OnRaiseLog("Play Fail:" + BassErrorCode.GetErrorInfo());
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            if (this.GrtPlayState() == PlayStatus.STOPPED || this._handle == -1)
            {
                return;
            }

            if (!Bass.BASS_ChannelStop(this._handle))
            {
                this.OnRaiseLog("Stop Fail:" + BassErrorCode.GetErrorInfo());
            }

            this._playLocationLineTimer.Stop();
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            if (this.GrtPlayState() == PlayStatus.PAUSED || this._handle == -1)
            {
                return;
            }

            if (!Bass.BASS_ChannelPause(this._handle))
            {
                this.OnRaiseLog("Pause Fail:" + BassErrorCode.GetErrorInfo());
            }

            this._playLocationLineTimer.Stop();
        }

        /// <summary>
        /// 缩放波形
        /// </summary>
        /// <param name="zoomFlag">缩放标识[true:放大,false:缩小]</param>
        public void Zoom(bool zoomFlag)
        {
            if (this._srcLeftChannelDataLength == 0)
            {
                this._ws = 0;
                this._we = 0;
                return;
            }

            int width = this._wavArea.Width;
            long area = this._we - this._ws;
            if (zoomFlag)
            {
                if (width / area > 0)
                {
                    return;
                }

                area = area / this._zoomMuilt;
                long offset = area / 2;
                this._ws = this._ws + offset;
                this._we = this._we - offset;
            }
            else
            {
                long offset = area / 2;
                area = area * this._zoomMuilt;
                if (area > this._srcLeftChannelDataLength)
                {
                    if (this._ws > 0)
                    {
                        this._ws = 0;
                        this._we = this._srcLeftChannelDataLength;
                    }
                    else
                    {
                        this._ws = 0;
                        this._we = this._we + area;
                    }
                }
                else
                {
                    this._ws = this._ws - offset;
                    this._we = this._we + offset;

                    if (this._ws < 0)
                    {
                        this._we = this._we + Math.Abs(this._ws);
                        this._ws = 0;
                    }
                }
            }

            if (this._we > this._srcLeftChannelDataLength)
            {
                this._we = this._srcLeftChannelDataLength;
            }

            //缩放时,需要局部刷新缩略波形中主波形显示段区域+主波形区域+时间区域
            this.AddPartRefreshArea(this._zoomDisplayArea);//缩略波形中旧的主波形显示段区域

            //更新波形选中区域
            this.UpdateSelectedArea();

            //添加局部刷新区域
            this.AddPartRefreshArea(this._zoomDisplayArea);//缩略波形中新的主波形显示段区域
            this.AddPartRefreshArea(this._wavArea);//主波形区域
            this.AddPartRefreshArea(this._timeArea);//时间区域

            //计算bass处于非播放时主波形播放位置指示线X坐标
            this.CalculateWavPlayLocationLine();

            //更新主波形播放位置指示线上次指示线所在的位置是否在主波形显示段内
            this.UpdateWavPlayLocationLingLastInArea();

            //刷新波形图
            this.RefreshWave(true, true, true, false, false, true);

            //设置鼠标样式
            this.SetZoomWavMoveMouseStyle(this.GetUIArea(this._currentMouseArgs.Location));
        }

        /// <summary>
        /// 保存选中区域到文件[未完成]
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveSelectionToFile(string filePath)
        {
            //如果文件存在则先删除
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (FileStream fsr = File.OpenRead(this._fileName))
            {
                int headSize = 44;
                long dataTotalLength = fsr.Length - headSize;
                long s = this._ss * dataTotalLength / this._srcLeftChannelDataLength;
                long e = this._se * dataTotalLength / this._srcLeftChannelDataLength;
                int dataSize = (int)(e - s);

                //long s = this._ss * this._bassDataTotalLength / this._srcLeftChannelDataLength;
                //long e = this._se * this._bassDataTotalLength / this._srcLeftChannelDataLength;
                //int dataSize = (int)(e - s) * 2;

                using (FileStream fs = File.OpenWrite(filePath))
                {
                    byte[] buffer;
                    string valueStr = string.Empty;

                    //RIFF，资源交换文件标志,4byte
                    buffer = Encoding.UTF8.GetBytes("RIFF");
                    fs.Write(buffer, 0, 4);
                    valueStr = Encoding.Default.GetString(buffer);

                    //从RIFF下一个地址开始到文件尾的总字节数,4byte
                    int length = dataSize + 44 - 8;
                    buffer = BitConverter.GetBytes(length);
                    fs.Write(buffer, 0, 4);
                    valueStr = BitConverter.ToInt32(buffer, 0).ToString();

                    //WAVE，代表wav文件格式,4byte
                    buffer = Encoding.UTF8.GetBytes("WAVE");
                    fs.Write(buffer, 0, 4);
                    valueStr = Encoding.Default.GetString(buffer);

                    //fmt ，波形格式标志,4byte
                    buffer = Encoding.UTF8.GetBytes("FMT ");
                    fs.Write(buffer, 0, 4);
                    valueStr = Encoding.Default.GetString(buffer);

                    int wBitsPerSample = this._channelInfo.flags == BASSFileFlag.BASS_SAMPLE_8BITS ? 8 : 16;
                    //00000010H，16PCM，我的理解是用16bit的数据表示一个量化结果,4byte
                    buffer = BitConverter.GetBytes(wBitsPerSample);
                    fs.Write(buffer, 0, 4);

                    //为1时表示线性PCM编码，大于1时表示有压缩的编码。这里是0001H,2byte
                    buffer = BitConverter.GetBytes((Int16)1);
                    fs.Write(buffer, 0, 2);

                    //[声道数] 1为单声道，2为双声道,2byte
                    buffer = BitConverter.GetBytes((Int16)this._channelInfo.chans);
                    fs.Write(buffer, 0, 2);

                    //[采样频率],4byte
                    buffer = BitConverter.GetBytes(this._freq);
                    fs.Write(buffer, 0, 4);

                    //[播放速度(传输速度)] Byte率=采样频率*音频通道数*每次采样得到的样本位数/8，00005622H，也就是22050Byte/s=11025*1*16/2,4byte
                    buffer = BitConverter.GetBytes(this._channelInfo.chans * wBitsPerSample * this._freq / 8);
                    fs.Write(buffer, 0, 4);

                    //块对齐=通道数*每次采样得到的样本位数/8，0002H，也就是2=1*16/8,2byte
                    short blockAlign = (short)(this._channelInfo.chans * wBitsPerSample / 8);
                    buffer = BitConverter.GetBytes(blockAlign);
                    fs.Write(buffer, 0, 2);

                    //[采样位数] 样本数据位数，0010H即16，一个量化样本占2byte,2byte
                    buffer = BitConverter.GetBytes((Int16)16);
                    fs.Write(buffer, 0, 2);

                    //data标志,4byte
                    buffer = Encoding.UTF8.GetBytes("data");
                    fs.Write(buffer, 0, 4);

                    //Wav文件实际音频数据所占的大小，这里是001437C8H即1325000，再加上2CH就正好是1325044，整个文件的大小,4byte               
                    buffer = BitConverter.GetBytes(dataSize);
                    fs.Write(buffer, 0, 4);

                    ////写入数据
                    //short[] data = new short[dataSize];
                    //Array.Copy(this._srcData, s, data, 0, data.Length);

                    //BinaryWriter bw = new BinaryWriter(fs);
                    //for (int i = 0; i < data.Length; i++)
                    //{
                    //    bw.Write(data[i]);
                    //}

                    buffer = new byte[dataSize];
                    fsr.Seek(s, SeekOrigin.Begin);
                    fsr.Read(buffer, 0, buffer.Length);

                    fs.Write(buffer, 0, buffer.Length);

                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// 保存选中区域到文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveSelectionToFile_bk(string filePath)
        {
            //如果文件存在则先删除
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            /*************************************************************************************************************************
             * wav文件格式
             * 起始地址 占用空间   本地址数字的含义
             * 00H      4byte      RIFF，资源交换文件标志。
             * 04H      4byte      从下一个地址开始到文件尾的总字节数。高位字节在后面，这里就是001437ECH，换成十进制是1325036byte，算上这之前的8byte就正好1325044byte了。
             * 08H      4byte      WAVE，代表wav文件格式。
             * 0CH      4byte      FMT ，波形格式标志             
             * 10H      4byte      00000010H，16PCM，我的理解是用16bit的数据表示一个量化结果。
             * 14H      2byte      为1时表示线性PCM编码，大于1时表示有压缩的编码。这里是0001H。
             * 16H      2byte      1为单声道，2为双声道，这里是0001H。
             * 18H      4byte      采样频率，这里是00002B11H，也就是11025Hz。
             * 1CH      4byte      Byte率=采样频率*音频通道数*每次采样得到的样本位数/8，00005622H，也就是22050Byte/s=11025*1*16/2。
             * 20H      2byte      块对齐=通道数*每次采样得到的样本位数/8，0002H，也就是2=1*16/8。
             * 22H      2byte      样本数据位数，0010H即16，一个量化样本占2byte。
             * 24H      4byte      data，一个标志而已。
             * 28H      4byte      Wav文件实际音频数据所占的大小，这里是001437C8H即1325000，再加上2CH就正好是1325044，整个文件的大小。
             * 2CH      不定       量化数据。
             *************************************************************************************************************************/

            long s = this._ss * this._bassDataTotalLength / this._srcLeftChannelDataLength;
            long e = this._se * this._bassDataTotalLength / this._srcLeftChannelDataLength;
            int dataSize = (int)(e - s) * 2;

            using (FileStream fs = File.OpenWrite(filePath))
            {
                byte[] buffer;

                //RIFF，资源交换文件标志,4byte
                buffer = Encoding.UTF8.GetBytes("RIFF");
                fs.Write(buffer, 0, 4);

                //从RIFF下一个地址开始到文件尾的总字节数,4byte
                int length = dataSize + 44 - 8;
                buffer = BitConverter.GetBytes(length);
                fs.Write(buffer, 0, 4);

                //WAVE，代表wav文件格式,4byte
                buffer = Encoding.UTF8.GetBytes("WAVE");
                fs.Write(buffer, 0, 4);

                //fmt ，波形格式标志,4byte
                buffer = Encoding.UTF8.GetBytes("FMT ");
                fs.Write(buffer, 0, 4);

                int wBitsPerSample = this._channelInfo.flags == BASSFileFlag.BASS_SAMPLE_8BITS ? 8 : 16;

                //00000010H，16PCM，我的理解是用16bit的数据表示一个量化结果,4byte
                buffer = BitConverter.GetBytes(wBitsPerSample);
                fs.Write(buffer, 0, 4);

                //为1时表示线性PCM编码，大于1时表示有压缩的编码。这里是0001H,2byte
                buffer = BitConverter.GetBytes(1);
                fs.Write(buffer, 0, 2);

                //[声道数] 1为单声道，2为双声道,2byte
                buffer = BitConverter.GetBytes(this._channelInfo.chans);
                fs.Write(buffer, 0, 2);

                //[采样频率],4byte
                buffer = BitConverter.GetBytes(this._channelInfo.freq);
                fs.Write(buffer, 0, 2);

                //[播放速度(传输速度)] Byte率=采样频率*音频通道数*每次采样得到的样本位数/8，00005622H，也就是22050Byte/s=11025*1*16/2,4byte
                buffer = BitConverter.GetBytes(this._channelInfo.chans * wBitsPerSample / 8);
                fs.Write(buffer, 0, 4);

                //块对齐=通道数*每次采样得到的样本位数/8，0002H，也就是2=1*16/8,2byte
                short blockAlign = (short)(this._channelInfo.chans * wBitsPerSample / 8);
                buffer = BitConverter.GetBytes(blockAlign);
                fs.Write(buffer, 0, 2);

                //[采样位数] 样本数据位数，0010H即16，一个量化样本占2byte,2byte
                buffer = BitConverter.GetBytes(this._channelInfo.freq);
                fs.Write(buffer, 0, 2);

                //data标志,4byte
                buffer = Encoding.UTF8.GetBytes("data");
                fs.Write(buffer, 0, 4);

                //Wav文件实际音频数据所占的大小，这里是001437C8H即1325000，再加上2CH就正好是1325044，整个文件的大小,4byte               
                buffer = BitConverter.GetBytes(dataSize);
                fs.Write(buffer, 0, 4);

                //写入数据
                short[] data = new short[e - s];
                Array.Copy(this._srcData, s, data, 0, data.Length);

                BinaryWriter bw = new BinaryWriter(fs);
                for (int i = 0; i < data.Length; i++)
                {
                    bw.Write(data[i]);
                }

                fs.Flush();
            }
        }

        /// <summary>
        /// 快进快退
        /// </summary>
        /// <param name="flag">快进快退标识[true:快进;false:快退]</param>
        /// <param name="time">快进快退的跳跃时间,单位/秒</param>
        public void FastSpeedReversePlay(bool flag, double time)
        {
            if (this._handle == -1)
            {
                return;
            }

            double currentPos = this.Position;
            double newPos;
            if (flag)
            {
                newPos = currentPos + time;
                if (newPos > this._durationTime)
                {
                    newPos = this._durationTime;
                }
            }
            else
            {
                newPos = currentPos - time;
                if (newPos < 0)
                {
                    newPos = 0;
                }
            }

            this.Position = newPos;
        }

        /// <summary>
        /// 声音淡入淡出
        /// </summary>
        /// <param name="volValue">音量目标值</param>
        /// <param name="duration">持续时间,单位/毫秒</param>
        public void VoiceSlide(float volValue, int duration)
        {
            if (this._handle == -1)
            {
                return;
            }

            //float value = 0.2f; 
            //int duration=2*1000;
            Bass.BASS_ChannelSlideAttribute(this._handle, BASSAttribute.BASS_ATTRIB_VOL, volValue, duration);
        }

        /// <summary>
        /// 获取播放声道特性值
        /// </summary>
        /// <param name="attri">要获取的特性</param>
        /// <returns>值</returns>
        public float GetChannelAttribute(BASSAttribute attri)
        {
            if (this._handle == -1)
            {
                return 0f;
            }

            return Bass.BASS_ChannelGetAttribute(this._handle, attri);
        }

        /// <summary>
        /// 设置播放声道特性值
        /// </summary>
        /// <param name="attri">要设置的特性</param>
        /// <param name="value">值</param>
        public void SetChannelAttribute(BASSAttribute attri, float value)
        {
            if (this._handle == -1)
            {
                return;
            }

            Bass.BASS_ChannelSetAttribute(this._handle, attri, value);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="filePath"></param>
        public void write(string filePath)
        {
            var info = this._channelInfo;
            WAVEFORMATEX wf = new WAVEFORMATEX();
            wf.wFormatTag = 1;
            wf.nChannels = (short)info.chans;
            wf.wBitsPerSample = (short)(this._channelInfo.flags == BASSFileFlag.BASS_SAMPLE_8BITS ? 8 : 16);
            wf.nBlockAlign = (short)(this._channelInfo.chans * wf.wBitsPerSample / 8);
            wf.nSamplesPerSec = info.freq;
            wf.nAvgBytesPerSec = wf.nSamplesPerSec * wf.nBlockAlign;

            using (FileStream fs = File.OpenWrite(filePath))
            {
                byte[] buffer;

                buffer = Encoding.UTF8.GetBytes(@"RIFF\0\0\0\0WAVEfmt \20\0\0\0");
                fs.Write(buffer, 0, buffer.Length);

                buffer = this.StructToBytes(wf);
                fs.Write(buffer, 0, buffer.Length);

                buffer = Encoding.UTF8.GetBytes(@"data\0\0\0\0");
                fs.Write(buffer, 0, buffer.Length);

                //写入数据
                long s = this._ss * this._bassDataTotalLength / this._srcLeftChannelDataLength;
                long e = this._se * this._bassDataTotalLength / this._srcLeftChannelDataLength;
                short[] data = new short[e - s];
                Array.Copy(this._srcData, s, data, 0, data.Length);

                BinaryWriter bw = new BinaryWriter(fs);
                for (int i = 0; i < data.Length; i++)
                {
                    bw.Write(data[i]);
                }

                fs.Seek(4, SeekOrigin.Begin);
                buffer = BitConverter.GetBytes(fs.Length - 8);
                fs.Write(buffer, 0, 4);

                fs.Seek(40, SeekOrigin.Begin);
                buffer = BitConverter.GetBytes(fs.Length - 44);
                fs.Write(buffer, 0, 4);

                fs.Flush();
            }
        }

        /// <summary>
        /// 将结构体类型转换为字节数据
        /// </summary>
        /// <param name="structObj"></param>
        /// <returns></returns> 
        private byte[] StructToBytes(object structObj)
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(structObj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            System.Runtime.InteropServices.Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷贝到byte 数组
            System.Runtime.InteropServices.Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            System.Runtime.InteropServices.Marshal.FreeHGlobal(structPtr);
            return bytes;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct WAVEFORMATEX
    {
        /// <summary>
        /// 
        /// </summary>
        public short wFormatTag;

        /// <summary>
        /// 
        /// </summary> 
        public short nChannels;

        /// <summary>
        /// 
        /// </summary>
        public int nSamplesPerSec;

        /// <summary>
        /// 
        /// </summary>
        public int nAvgBytesPerSec;

        /// <summary>
        /// 
        /// </summary>
        public short nBlockAlign;

        /// <summary>
        /// 
        /// </summary>
        public short wBitsPerSample;

        /// <summary>
        /// 
        /// </summary>
        public short cbSize;
    }
}
