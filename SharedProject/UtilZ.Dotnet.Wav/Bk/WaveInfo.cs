using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Bk
{
    /// <summary>
    /// WAV波形文件信息
    /// </summary>
    internal class WaveInfo
    {
        /************************************************************
         * 标准Wave PCM格式的音频文件头格式为一个44字节长的结构
         ************************************************************/

        private string _wavFile = string.Empty;
        /// <summary>
        /// Wav文件路径
        /// </summary>
        public string WavFile
        {
            get { return _wavFile; }
            set { _wavFile = value; }
        }

        private char[] _IDRIFF;
        /// <summary>
        /// [00H~03H,4字节]资源交换文件标志（RIFF）,4字符长度
        /// </summary>
        public char[] IDRIFF
        {
            get { return _IDRIFF; }
            set { _IDRIFF = value; }
        }

        private long _size = 0;
        /// <summary>
        /// [04H~07H,4字节]文件长度,含义:从下个地址开始到文件尾的总字节数,文件长度=文件实际长度-8
        /// </summary>
        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }

        private char[] _riffType;
        /// <summary>
        /// [08H~0BH,4字节]WAVE代表波形文件格式,4字符长度
        /// </summary>
        public char[] RiffType
        {
            get { return _riffType; }
            set { _riffType = value; }
        }

        private char[] _IDFMT;
        /// <summary>
        /// [0CH~0FH,4字节]波形格式标志（fmt ），最后一位空格,4字符长度
        /// </summary>
        public char[] IDFMT
        {
            get { return _IDFMT; }
            set { _IDFMT = value; }
        }

        private char[] _IDFACT;
        /// <summary>
        ///  [24H~27H,4字节]“fact”,该部分一下是可选部分，即可能有，可能没有,一般到WAV文件由某些软件转换而成时，包含这部分,4字符
        /// </summary>
        public char[] IDFACT
        {
            get { return _IDFACT; }
            set { _IDFACT = value; }
        }

        private int _sampleSize = 0;
        /// <summary>
        /// 量化位数,分为8位，16位，24位三种
        /// </summary>
        public int SampleSize
        {
            get { return _sampleSize; }
            set { _sampleSize = value; }
        }

        private int _formatTag = 0;
        /// <summary>
        /// [14H~15H,2字节]格式种类（值为1时，表示数据为线性PCM编码）,为1时表示线性PCM编码，大于1时表示有压缩的编码
        /// </summary>
        public int FormatTag
        {
            get { return _formatTag; }
            set { _formatTag = value; }
        }

        private int _channels = 0;
        /// <summary>
        /// [16H~17H,2字节]声道数, 1为单声道，2为双声道
        /// </summary>
        public int Channels
        {
            get { return _channels; }
            set { _channels = value; }
        }

        private long _samplesPerSec = 0;
        /// <summary>
        /// 采样频率,取样频率,取样频率一般有11025Hz(11kHz) ，22050Hz(22kHz)和44100Hz(44kHz) 三种,含义: 每秒播放一个数据传输率的数据,一秒内从数据传输率内采样的次数
        /// </summary>
        public long SamplesPerSec
        {
            get { return _samplesPerSec; }
            set { _samplesPerSec = value; }
        }

        private long _bytes = 0;
        /// <summary>
        /// [1CH~1FH,4字节]数据传输速率（每秒需要播放的字节数）,Bit率;注: Bit率=采样频率*声道数*采样样本位数/8
        /// </summary>
        public long Bytes
        {
            get { return _bytes; }
            set { _bytes = value; }
        }

        private int _blockAlign = 0;
        /// <summary>
        /// [20H~21H,2字节]DATA数据块长度，字节;块对齐=通道数*每次采样得到的样本位数/8
        /// </summary>
        public int BlockAlign
        {
            get { return _blockAlign; }
            set { _blockAlign = value; }
        }

        private int _bitsPerSample = 0;
        /// <summary>
        /// [22H~23H,2字节]采样位数,PCM位宽,每个采样需要的bit数
        /// </summary>
        public int BitsPerSample
        {
            get { return _bitsPerSample; }
            set { _bitsPerSample = value; }
        }

        private char[] _dataFlag;
        /// <summary>
        /// [24H~27H,4字节]数据标志符（data）
        /// </summary>
        public char[] DataFlag
        {
            get { return _dataFlag; }
            set { _dataFlag = value; }
        }

        private long _dataSize = 0;
        /// <summary>
        ///[28H~2BH,4字节] DATA总数据长度字节,音频数据长度
        /// </summary>
        public long DataSize
        {
            get { return _dataSize; }
            set { _dataSize = value; }
        }

        private byte[] _header;
        /// <summary>
        /// 文件头信息,头大小0x2c
        /// </summary>
        public byte[] Header
        {
            get { return _header; }
            set { _header = value; }
        }

        private byte[] _data;
        /// <summary>
        /// 量化数据
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private double _playTime = 0;
        /// <summary>
        /// 总播放时间
        /// </summary>
        public double PlayTime
        {
            get { return _playTime; }
            set { _playTime = value; }
        }

        private short[] _leftChannelData;
        /// <summary>
        /// 左声道数据
        /// </summary>
        public short[] LeftChannelData
        {
            get { return _leftChannelData; }
            set { _leftChannelData = value; }
        }

        private short[] _rightChannelData;
        /// <summary>
        /// 右声道数据
        /// </summary>
        public short[] RightChanbnlData
        {
            get { return _rightChannelData; }
            set { _rightChannelData = value; }
        }
    }
}
