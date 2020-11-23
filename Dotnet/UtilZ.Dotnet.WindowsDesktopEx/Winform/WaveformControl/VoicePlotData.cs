using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 语音图形参数
    /// </summary>
    public class VoicePlotData
    {
        #region 构造函数初始化字段以及发展
        private VoicePlotDataType _dataType;
        /// <summary>
        /// 语音图形数据类型
        /// </summary>
        public VoicePlotDataType DataType
        {
            get { return this._dataType; }
        }

        private byte[] _buffer;
        /// <summary>
        /// 内存数据
        /// </summary>
        public byte[] Buffer
        {
            get { return this._buffer; }
        }

        private string _filePath;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return this._filePath; }
        }

        private int _channelCount;
        /// <summary>
        /// 声道数
        /// </summary>
        public int ChannelCount
        {
            get { return this._channelCount; }
        }

        /// <summary>
        /// 采样位数
        /// </summary>
        public VoicePlotSampleBit SampleBit { get; private set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public int SampleRate { get; private set; }

        private long _index;
        /// <summary>
        /// 数据源中起始位置,小于等于0从头开始
        /// </summary>
        public long Index
        {
            get { return this._index; }
        }

        private long _length;
        /// <summary>
        /// 数据源中使用数据长度,小于等于0使用全部
        /// </summary>
        public long Length
        {
            get { return this._length; }
        }

        private long _maxIndex;
        /// <summary>
        /// 获取数据最大索引
        /// </summary>
        public long MaxIndex
        {
            get { return this._maxIndex; }
        }

        private DateTime _baseTime;
        /// <summary>
        /// 起始的基准时间,其它时间皆基于此值往后增加多少毫秒
        /// </summary>
        public DateTime BaseTime
        {
            get { return this._baseTime; }
        }
        #endregion




        private int _pointLength = 0;
        /// <summary>
        /// 一个数据点所占用数据长度
        /// </summary>
        internal int PointLength
        {
            get { return this._pointLength; }
        }

        private int _secondLength = 0;
        /// <summary>
        /// 一秒数据长度
        /// </summary>
        internal int SecondLength
        {
            get { return this._secondLength; }
        }

        /// <summary>
        /// 数据点最大值
        /// </summary>
        internal int PointMaxValue { get; private set; }




        private VoicePlotData(VoicePlotDataType dataType, int channelCount, VoicePlotSampleBit sampleBit,
            int sampleRate, long offset, long length, DateTime baseTime)
        {
            if (channelCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(channelCount));
            }

            if (channelCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(channelCount));
            }

            this._dataType = dataType;
            this._channelCount = channelCount;
            this.SampleBit = sampleBit;
            this.SampleRate = sampleRate;
            this._index = offset;
            this._length = length;
            this._pointLength = channelCount * (int)sampleBit / 8;
            this._secondLength = this._pointLength * sampleRate;
            this._baseTime = baseTime;

            int pointMaxValue;
            switch (sampleBit)
            {
                case VoicePlotSampleBit.Sample8Bit:
                    pointMaxValue = byte.MaxValue;
                    break;
                case VoicePlotSampleBit.Sample16Bit:
                    pointMaxValue = short.MaxValue;
                    break;
                case VoicePlotSampleBit.Sample32Bit:
                    pointMaxValue = Convert.ToInt32(float.MaxValue);
                    break;
                default:
                    throw new NotSupportedException($"不支持的采样位数:{sampleBit}");
            }
            this.PointMaxValue = pointMaxValue;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseTime">起始的基准时间</param>
        /// <param name="buffer">内存数据</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="sampleBit">采样位数</param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="offset">数据源中起始位置,小于等于0从头开始</param>
        /// <param name="length">数据源中使用数据长度,小于等于0使用全部</param>
        public VoicePlotData(DateTime baseTime, byte[] buffer, int channelCount, VoicePlotSampleBit sampleBit, int sampleRate, int offset = -1, int length = -1)
            : this(VoicePlotDataType.Buffer, channelCount, sampleBit, sampleRate, offset, length, baseTime)
        {
            this._buffer = buffer;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseTime">起始的基准时间</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="sampleBit">采样位数</param>
        /// <param name="sampleRate">采样率</param>
        /// <param name="offset">数据源中起始位置,小于等于0从头开始</param>
        /// <param name="length">数据源中使用数据长度,小于等于0使用全部</param>
        public VoicePlotData(DateTime baseTime, string filePath, int channelCount, VoicePlotSampleBit sampleBit, int sampleRate, long offset = -1, long length = -1)
            : this(VoicePlotDataType.File, channelCount, sampleBit, sampleRate, offset, length, baseTime)
        {
            this._filePath = filePath;
        }



        internal void Init()
        {
            if (this._index < 0)
            {
                this._index = 0L;
            }

            long maxLength;
            switch (this._dataType)
            {
                case VoicePlotDataType.Buffer:
                    maxLength = this._buffer.Length - this._index;
                    break;
                case VoicePlotDataType.File:
                    using (var stream = new FileStream(this._filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        maxLength = stream.Length - this._index;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (this._length <= 0)
            {
                this._length = maxLength;
            }
            else
            {
                if (this._length > maxLength)
                {
                    this._length = maxLength;
                }
            }
            this._maxIndex = this._index + this._length;


            //if (!this._baseTime.HasValue)
            //{
            //    this._baseTime = DateTime.Now;
            //}
        }






        private VoicePlotChannelData[] _channelDataArr = null;
        /// <summary>
        /// 重采样后的数据
        /// </summary>
        internal VoicePlotChannelData[] ChannelDataArr
        {
            get { return this._channelDataArr; }
        }

        internal bool HasChannelData
        {
            get
            {
                return this._channelDataArr != null && this._channelDataArr.Length > 0;
            }
        }

        private double _durationMillisecond;
        /// <summary>
        /// 重采样数据时长,单位/秒
        /// </summary>
        internal double DurationMillisecond
        {
            get { return this._durationMillisecond; }
        }

        private int _resampleTotalCount = 0;
        internal int ResampleTotalCount
        {
            get { return this._resampleTotalCount; }
        }

        /*****************************************************************************
         * a                      b                     c                       d
         * |----------------------|---------------------|-----------------------|--------->
         *
         * a:BaseTime
         * b:SBTO
         * c:SETO
         * d:DurationMillisecond
         *****************************************************************************/


        private double _showBeginTimeOffsetMillisecond = 0;
        /// <summary>
        /// 获取或设置相对于起始的基准时间的显示起始
        /// SBTO=>showBeginTimeOffset
        /// </summary>
        internal double SBTOMillisecond
        {
            get { return this._showBeginTimeOffsetMillisecond; }
            set { this._showBeginTimeOffsetMillisecond = value; }
        }

        private double _showEndTimeOffsetMillisecond = 0;
        /// <summary>
        /// SETO=>ShowEndTimeOffsetMillisecond
        /// </summary>
        internal void UpdateSETOMillisecond(double setoMillisecond)
        {
            this._showEndTimeOffsetMillisecond = setoMillisecond;
        }

        internal double GetSETOMillisecond()
        {
            double setoMillisecond = this._showEndTimeOffsetMillisecond;
            if (setoMillisecond <= PlotConstant.ZEROR_D)
            {
                setoMillisecond = this.DurationMillisecond;
            }

            return setoMillisecond;
        }


        internal bool ContainsShowTime(double offsetTimeMilliseconds)
        {
            if (offsetTimeMilliseconds < this._showBeginTimeOffsetMillisecond || offsetTimeMilliseconds > this.GetSETOMillisecond())
            {
                //不在范围内
                return false;
            }

            return true;
        }


        internal void Resample(int targetCount, long begeinIndex, long endIndex)
        {
            if (targetCount < 1)
            {
                this._channelDataArr = null;
                this._durationMillisecond = double.NaN;
                return;
            }

            //分片
            List<VoiceDataResampleSegment<IComparable>> segmentList = this.SpliSlice(targetCount, begeinIndex, endIndex);

            //并行处理
            Parallel.ForEach(segmentList, (t) => { t.Resample(); });

            //全并并行处理结果
            this._channelDataArr = this.MergeResampleResult(segmentList);
            this._durationMillisecond = ((double)(endIndex - begeinIndex)) * 1000 / this._secondLength;
        }

        private VoicePlotChannelData[] MergeResampleResult(List<VoiceDataResampleSegment<IComparable>> segmentList)
        {
            int totalCount = segmentList.Sum(t => t.ChannelDataLength);
            IComparable[][] globalViewData = new IComparable[this._channelCount][];
            for (int i = 0; i < globalViewData.Length; i++)
            {
                globalViewData[i] = new IComparable[totalCount];
            }
            this._resampleTotalCount = totalCount;

            int index = 0;
            for (int i = 0; i < segmentList.Count; i++)
            {
                for (int j = 0; j < globalViewData.Length; j++)
                {
                    Array.Copy(segmentList[i].ChannelDataList[j], 0, globalViewData[j], index, segmentList[i].ChannelDataLength);
                }
                index += segmentList[i].ChannelDataLength;
            }

            var arr = new VoicePlotChannelData[this._channelCount];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new VoicePlotChannelData(this.GetChannelName(i), globalViewData[i]);
            }

            return arr;
        }

        private string GetChannelName(int index)
        {
            // 声道数据集合
            // 单声道:L
            // 双声道:L-R
            // 四声道:L-R-LS-RS
            // 5.1声道FFT数据顺序: L-R-C-LFE-LS-RS
            // 7.1声道FFT数据顺序: L-R-C-LFE-LS-RS-不晓得了


            // 单声道:L
            if (this._channelCount == 1)
            {
                return "单声道";
            }


            // 双声道:L-R
            if (this._channelCount == 2)
            {
                switch (index)
                {
                    case 0:
                        return "L";
                    case 1:
                        return "R";
                }
            }

            // 四声道:L-R-LS-RS
            if (this._channelCount == 4)
            {
                switch (index)
                {
                    case 0:
                        return "L";
                    case 1:
                        return "R";
                    case 2:
                        return "LS";
                    case 3:
                        return "RS";
                }
            }

            // 5.1声道FFT数据顺序: L-R-C-LFE-LS-RS
            if (this._channelCount == 6)
            {
                switch (index)
                {
                    case 0:
                        return "L";
                    case 1:
                        return "R";
                    case 2:
                        return "C";
                    case 3:
                        return "LFE";
                    case 4:
                        return "LS";
                    case 5:
                        return "RS";
                }
            }

            // 7.1声道FFT数据顺序: L-R-C-LFE-LS-RS-不晓得了
            if (this._channelCount >= 8)
            {
                switch (index)
                {
                    case 0:
                        return "L";
                    case 1:
                        return "R";
                    case 2:
                        return "C";
                    case 3:
                        return "LFE";
                    case 4:
                        return "LS";
                    case 5:
                        return "RS";
                    case 6:
                        return "RC";
                }

                return $"C{index}";
            }

            return string.Empty;
        }

        private List<VoiceDataResampleSegment<IComparable>> SpliSlice(int targetCount, long begeinIndex, long endIndex)
        {
            long totalLength = endIndex - begeinIndex;//总数据长度
            long totalPointCount = totalLength / this._pointLength;//总点数
            int compressPointCount = (int)(totalPointCount / targetCount);

            int processorCount = Environment.ProcessorCount;//CPU核心数
            processorCount = 1;
            long processorProcessPointCount = totalPointCount / processorCount;//每处理器处理的数据点数
            long mod = totalPointCount % processorCount;//余数
            if (mod > 0)
            {
                processorProcessPointCount += 1;
            }


            long segmentDataSize = processorProcessPointCount * this._pointLength;
            var segmentList = new List<VoiceDataResampleSegment<IComparable>>();
            long startIndex = begeinIndex;
            int order = 0;

            for (int i = 0; i < processorCount; i++)
            {
                Stream stream;
                switch (this._dataType)
                {
                    case VoicePlotDataType.Buffer:
                        stream = new MemoryStream(this._buffer);
                        break;
                    case VoicePlotDataType.File:
                        stream = new FileStream(this._filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        break;
                    default:
                        throw new NotImplementedException(this._dataType.ToString());
                }

                var channelDataSegment = new VoiceDataResampleSegment<IComparable>(order, stream, this, startIndex, startIndex + segmentDataSize, compressPointCount);
                segmentList.Add(channelDataSegment);
                order++;

                //更新起始索引位置
                startIndex = startIndex += segmentDataSize;
                if (startIndex + segmentDataSize > endIndex)
                {
                    segmentDataSize = (endIndex - startIndex) / this._pointLength;
                }
            }

            return segmentList;
        }
    }
}
