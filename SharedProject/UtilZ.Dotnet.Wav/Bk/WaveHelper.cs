using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Bk
{
    /// <summary>
    /// 
    /// </summary>
    internal class WaveHelper
    {
        /// <summary>
        /// 解析头为44字节的wav波形文件,另外一种55头字节的文件以后再写算法
        /// </summary>
        /// <param name="wavFile">WAV波形文件路径</param>
        /// <returns>WAV文件信息</returns>
        public static WaveInfo ParseWav(string wavFile)
        {
            //注:BitConverter专门用于值类型数据与二进制数据相互转换的,但此处没用,是为了提高效率,虽然系统的也是采用位运算,但是层级过多
            WaveInfo wavInfo = new WaveInfo();
            try
            {
                wavInfo.WavFile = wavFile;
                using (FileStream fs = File.Open(wavFile, FileMode.Open))
                {
                    long currentLocation = 0;
                    byte[] buffer;
                    int count = 0;
                    char[] chs;

                    //读取 RIFF,含义:资源交换文件标志（RIFF）;[数据类型:string] 注:WAVE文档的保存格式的标准是RIFF
                    currentLocation = fs.Seek(0x00, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    chs = Encoding.Default.GetChars(buffer);
                    wavInfo.IDRIFF = chs;

                    //读取 文件长度,含义:从下个地址开始到文件尾的总字节数;[数据类型:ulong] 注:文件长度=文件实际长度-8
                    currentLocation = fs.Seek(0x04, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    ulong fsize = (ulong)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
                    wavInfo.Size = BitConverter.ToInt32(buffer, 0);

                    //读取 WAVE，含义:WAVE代表波形文件格式;[数据类型:string]
                    currentLocation = fs.Seek(0x08, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    chs = Encoding.Default.GetChars(buffer);
                    wavInfo.RiffType = chs;

                    //读取 fmt,含义:波形格式标志（fmt ），最后一位空格;[数据类型:string]
                    currentLocation = fs.Seek(0x0c, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    chs = Encoding.Default.GetChars(buffer);
                    wavInfo.IDFMT = chs;

                    //量化位数,分为8位，16位，24位三种
                    currentLocation = fs.Seek(0x10, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.SampleSize = BitConverter.ToInt32(buffer, 0);

                    //读取 格式类别,含义:格式种类（值为1时，表示数据为线性PCM编码）;[数据类型:ushort] 注:为1时表示线性PCM编码，大于1时表示有压缩的编码
                    currentLocation = fs.Seek(0x14, SeekOrigin.Begin);
                    buffer = new byte[2];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.FormatTag = BitConverter.ToInt16(buffer, 0);

                    //读取 声道数,单声道和立体声,含义: 1为单声道，2为双声道;[数据类型:ushort]
                    currentLocation = fs.Seek(0x16, SeekOrigin.Begin);
                    buffer = new byte[2];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.Channels = BitConverter.ToInt16(buffer, 0);

                    //读取 采样频率,取样频率,取样频率一般有11025Hz(11kHz) ，22050Hz(22kHz)和44100Hz(44kHz) 三种,含义: 每秒播放一个数据传输率的数据,一秒内从数据传输率内采样的次数
                    currentLocation = fs.Seek(0x18, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.SamplesPerSec = BitConverter.ToInt32(buffer, 0);

                    //读取 数据传输率,含义:播波形数据传输速率（每秒平均字节数,每秒所需播放字节数）,Bit率;[数据类型:long] 注: Bite率=采样频率*声道数*采样样本位数/8
                    currentLocation = fs.Seek(0x1c, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.Bytes = BitConverter.ToInt32(buffer, 0);

                    //读取 数据块对齐单位(每个采样需要的字节数),含义:DATA数据块长度，字节;块调整值=通道数*采样位数/8;[数据类型:ushort]
                    currentLocation = fs.Seek(0x20, SeekOrigin.Begin);
                    buffer = new byte[2];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.BlockAlign = BitConverter.ToInt16(buffer, 0);

                    //读取 采样位数,含义:PCM位宽;[数据类型:ushort]
                    currentLocation = fs.Seek(0x22, SeekOrigin.Begin);
                    buffer = new byte[2];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.BitsPerSample = BitConverter.ToInt16(buffer, 0);

                    //读取 data，含义:标记data;[数据类型:string] 注:“fact”,该部分一下是可选部分，即可能有，可能没有,一般到WAV文件由某些软件转换而成时，包含这部分
                    currentLocation = fs.Seek(0x24, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    chs = Encoding.Default.GetChars(buffer);
                    wavInfo.DataFlag = chs;

                    //读取 音频数据长度;含义:音频数据长度;[数据类型:ulong] 注:音频数据长度=文件长度-头长度
                    currentLocation = fs.Seek(0x28, SeekOrigin.Begin);
                    buffer = new byte[4];
                    count = fs.Read(buffer, 0, buffer.Length);
                    wavInfo.DataSize = BitConverter.ToInt32(buffer, 0);

                    //头信息
                    //currentLocation = fs.Seek(0x0, SeekOrigin.Begin);
                    //byte[] headerBuffer = new byte[0x2c];
                    //count = fs.Read(headerBuffer, 0, headerBuffer.Length);
                    //wavInfo.Header = headerBuffer;

                    //量化数据
                    //currentLocation = fs.Seek(0x2c, SeekOrigin.Begin);
                    //byte[] dataBuffer = new byte[fs.Length - 0x2c];
                    //count = fs.Read(dataBuffer, 0, dataBuffer.Length);
                    //wavInfo.Data = dataBuffer;

                    //播放时间
                    wavInfo.PlayTime = wavInfo.DataSize / wavInfo.Bytes;

                    //..数据读取
                    //Compression code信息在WAV文件字段的第21、22个byte，通过十六进制查看器我们可以看到一个WAV文件的压缩码类型
                    //WAV格式文件所占容量（B) = （取样频率 X量化位数X 声道） X 时间 / 8 (字节= 8bit) 
                    switch (wavInfo.FormatTag)
                    {
                        case 0://(0x0000) Unknown 
                            break;
                        case 1://(0x0001) PCM/uncompressed ,读取pcm1无压缩数据编码
                            WaveHelper.ReadDataPCM1(wavInfo, fs);
                            break;
                        case 2://(0x0002) Microsoft ADPCM 
                            break;
                        case 6://(0x0006) ITU G.711 a-law,读取pcm1有压缩数据编码
                            WaveHelper.ReadDataPCM6(wavInfo, fs);
                            break;
                        case 7://(0x0007) ITU G.711 Âµ-law 
                            break;
                        case 17://(0x0011) IMA ADPCM 
                            break;
                        case 20://(0x0016) ITU G.723 ADPCM (Yamaha) 
                            break;
                        case 49://(0x0031) GSM 6.10 
                            break;
                        case 64://(0x0040) ITU G.721 ADPCM 
                            break;
                        case 80://0x0050) MPEG 
                            break;
                        case 65535://(0xFFFF) Experimental
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解析波形文件出错," + ex.Message, ex);
            }

            return wavInfo;
        }

        /// <summary>
        /// 读取pcm1有压缩数据编码,方法未完
        /// </summary>
        /// <param name="wavInfo"></param>
        /// <param name="fs"></param>
        private static void ReadDataPCM6(WaveInfo wavInfo, FileStream fs)
        {
            //long dataLength = wavInfo.DataSize / (wavInfo.Bytes / wavInfo.SamplesPerSec / wavInfo.Channels) / 2;
            //bool isDoubleChannel = wavInfo.Channels == 2;
            //short[] leftChannelData = new short[dataLength];
            //short[] rightChannelData = null;
            //if (isDoubleChannel)
            //{
            //    rightChannelData = new short[dataLength];
            //}

            //byte[] buffer = null;
            //int count = 0;

            //long ds = wavInfo.DataSize / 8;

            //while (true)
            //{
            //    buffer = new byte[8];
            //    count = fs.Read(buffer, 0, buffer.Length);
            //}

            ////BinaryReader br = new BinaryReader(fs);
            ////for (int i = 0; i < dataLength; i++)
            ////{
            ////    leftChannelData[i] = br.ReadInt16();
            ////    if (isDoubleChannel)
            ////    {
            ////        rightChannelData[i] = br.ReadInt16();
            ////    }
            ////}

            //wavInfo.LeftChannelData = leftChannelData;
            //wavInfo.RightChanbnlData = rightChannelData;
        }

        /// <summary>
        /// 读取pcm1无压缩数据编码
        /// </summary>
        /// <param name="wavInfo"></param>
        /// <param name="fs"></param>
        private static void ReadDataPCM1(WaveInfo wavInfo, FileStream fs)
        {
            long dataLength = wavInfo.DataSize / (wavInfo.Bytes / wavInfo.SamplesPerSec / wavInfo.Channels) / 2;
            bool isDoubleChannel = wavInfo.Channels == 2;
            short[] leftChannelData = new short[dataLength];
            short[] rightChannelData = null;
            if (isDoubleChannel)
            {
                rightChannelData = new short[dataLength];
            }

            BinaryReader br = new BinaryReader(fs);
            for (int i = 0; i < dataLength; i++)
            {
                leftChannelData[i] = br.ReadInt16();
                if (isDoubleChannel)
                {
                    rightChannelData[i] = br.ReadInt16();
                }
            }

            wavInfo.LeftChannelData = leftChannelData;
            wavInfo.RightChanbnlData = rightChannelData;
        }
    }
}
