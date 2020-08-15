using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 一个声道绘图数据
    /// </summary>
    public class ChannelPlotData
    {
        private short[] _pcmData;
        /// <summary>
        /// 线性数据
        /// </summary>
        public short[] PCMData
        {
            get { return this._pcmData; }
        }

        private string _name;
        /// <summary>
        /// 声道名称
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        private short[] _globalViewData = null;
        internal short[] GlobalViewData
        {
            get { return this._globalViewData; }
            set { this._globalViewData = value; }
        }


        private short[] _drawData = null;
        internal short[] DrawData
        {
            get { return this._drawData; }
        }

        /// <summary>
        /// 构造函数 
        /// </summary>
        /// <param name="pcmData">线性数据</param>
        /// <param name="name"></param>
        public ChannelPlotData(short[] pcmData, string name)
        {
            this._pcmData = pcmData;
            this._name = name;
        }



        /// <summary>
        /// 对声道FFT数据进行重采样
        /// </summary>
        /// <param name="targetCount">目标数据个数</param>
        /// <param name="begeinIndex">原始数据起始索引</param>
        /// <param name="endIndex">原始数据结束索引</param>
        /// <returns>FFT中最大值</returns>
        internal void Resample(int targetCount, int begeinIndex, int endIndex)
        {
            this._drawData = this.PrimitiveResample(targetCount, begeinIndex, endIndex);

            //if (this._fftData == null || targetCount >= this._fftData.Length)
            //{
            //    this._drawData = this._fftData;
            //    return;
            //}

            //int srcCount = endIndex - begeinIndex;//原始数据个数
            //if (srcCount <= 0)
            //{
            //    this._drawData = new float[0];
            //    return;
            //}

            //int sampleCount;//采样数据个数
            //bool addinFlag = targetCount > srcCount;//是否需要重采样添加数据
            //if (addinFlag)
            //{
            //    sampleCount = (int)srcCount;
            //}
            //else
            //{
            //    sampleCount = targetCount;
            //}

            //int sampleSegmentCount = sampleCount / 2;//采样段数,因为是一段数据中取出最大值和最小值,所以目标点数除以2
            //if (sampleCount % 2 != 0)//如果目标数据个数为奇数,则采样个数+1
            //{
            //    sampleSegmentCount++;
            //}

            //int reSample = srcCount / sampleSegmentCount;//采样率
            //if (reSample == 0)//如果采样率为0,则总的数据个数少于目标个数,则直接将原始数据拷贝到目标数组
            //{
            //    this._drawData = new float[sampleCount];
            //    Array.Copy(this._fftData, begeinIndex, this._drawData, 0, this._drawData.Length);
            //    return;
            //}

            //if (srcCount % sampleSegmentCount != 0)
            //{
            //    reSample++;
            //}

            //int processorCount = Environment.ProcessorCount;//CPU核心数
            //int coreSampleSegmentCount = sampleSegmentCount / processorCount;//每一核分配到的采样段个数
            //if (sampleSegmentCount % processorCount != 0)
            //{
            //    coreSampleSegmentCount++;
            //}

            ////计算数据段大小
            //int segDataSize = coreSampleSegmentCount * reSample;//采样数据段大小

            ////拆分数据段
            //List<ChannelDataSegment> segments = new List<ChannelDataSegment>();
            //int startIndex = begeinIndex;
            //for (int i = 0; i < processorCount; i++)
            //{
            //    ChannelDataSegment channelDataSegment = new ChannelDataSegment();
            //    channelDataSegment.SrcData = this._fftData;
            //    channelDataSegment.Sample = reSample;

            //    //计算索引位置
            //    channelDataSegment.BegeinIndex = startIndex;
            //    channelDataSegment.EndIndex = startIndex + segDataSize;
            //    if (channelDataSegment.EndIndex > endIndex)
            //    {
            //        channelDataSegment.Flag = true;
            //        channelDataSegment.EndIndex = endIndex;
            //    }
            //    else
            //    {
            //        channelDataSegment.Flag = false;
            //    }

            //    segments.Add(channelDataSegment);

            //    //更新起始索引位置
            //    startIndex = channelDataSegment.EndIndex;
            //}

            ////并行处理
            //Parallel.ForEach(segments, this.PrimitiveResample);
            ////foreach (var segment in segments)
            ////{
            ////    this.RepeatSample(segment);
            ////}

            ////拼接数据
            //var resultLength = segments.Sum((item) => { return item.Result.Length; });
            //this._drawData = new float[resultLength];
            //int dstIndex = 0;
            //foreach (var segment in segments)
            //{
            //    Array.Copy(segment.Result, 0, this._drawData, dstIndex, segment.Result.Length);
            //    dstIndex += segment.Result.Length;
            //}
        }

        /// <summary>
        /// 对声道FFT数据进行重采样
        /// </summary>
        /// <param name="targetCount">目标数据个数</param>
        /// <param name="begeinIndex">原始数据起始索引</param>
        /// <param name="endIndex">原始数据结束索引</param>
        /// <returns>FFT中最大值</returns>
        internal short[] PrimitiveResample(int targetCount, int begeinIndex, int endIndex)
        {
            if (this._pcmData == null || targetCount >= this._pcmData.Length)
            {
                return this._pcmData;
            }

            int srcCount = endIndex - begeinIndex;//原始数据个数
            if (srcCount <= 0)
            {
                return new short[0];
            }

            int sampleCount;//采样数据个数
            bool addinFlag = targetCount > srcCount;//是否需要重采样添加数据
            if (addinFlag)
            {
                sampleCount = (int)srcCount;
            }
            else
            {
                sampleCount = targetCount;
            }

            int sampleSegmentCount = sampleCount / 2;//采样段数,因为是一段数据中取出最大值和最小值,所以目标点数除以2
            if (sampleCount % 2 != 0)//如果目标数据个数为奇数,则采样个数+1
            {
                sampleSegmentCount++;
            }

            short[] result;
            int reSample = srcCount / sampleSegmentCount;//采样率
            if (reSample == 0)//如果采样率为0,则总的数据个数少于目标个数,则直接将原始数据拷贝到目标数组
            {
                result = new short[sampleCount];
                Array.Copy(this._pcmData, begeinIndex, result, 0, result.Length);
                return result;
            }

            if (srcCount % sampleSegmentCount != 0)
            {
                reSample++;
            }

            int processorCount = Environment.ProcessorCount;//CPU核心数
            int coreSampleSegmentCount = sampleSegmentCount / processorCount;//每一核分配到的采样段个数
            if (sampleSegmentCount % processorCount != 0)
            {
                coreSampleSegmentCount++;
            }

            //计算数据段大小
            int segDataSize = coreSampleSegmentCount * reSample;//采样数据段大小

            //拆分数据段
            List<ChannelDataSegment> segments = new List<ChannelDataSegment>();
            int startIndex = begeinIndex;
            for (int i = 0; i < processorCount; i++)
            {
                ChannelDataSegment channelDataSegment = new ChannelDataSegment();
                channelDataSegment.SrcData = this._pcmData;
                channelDataSegment.Sample = reSample;

                //计算索引位置
                channelDataSegment.BegeinIndex = startIndex;
                channelDataSegment.EndIndex = startIndex + segDataSize;
                if (channelDataSegment.EndIndex > endIndex)
                {
                    channelDataSegment.Flag = true;
                    channelDataSegment.EndIndex = endIndex;
                }
                else
                {
                    channelDataSegment.Flag = false;
                }

                segments.Add(channelDataSegment);

                //更新起始索引位置
                startIndex = channelDataSegment.EndIndex;
            }

            //并行处理
            Parallel.ForEach(segments, this.PrimitiveResample);
            //foreach (var segment in segments)
            //{
            //    this.RepeatSample(segment);
            //}

            //拼接数据
            var resultLength = segments.Sum((item) => { return item.Result.Length; });
            result = new short[resultLength];
            int dstIndex = 0;
            foreach (var segment in segments)
            {
                Array.Copy(segment.Result, 0, result, dstIndex, segment.Result.Length);
                dstIndex += segment.Result.Length;
            }

            return result;
        }

        /// <summary>
        /// 重采样--指针
        /// </summary>
        /// <param name="channelDataSegment">重采样数据段信息</param>
        private void PrimitiveResample(ChannelDataSegment channelDataSegment)
        {
            try
            {
                int sample = channelDataSegment.Sample;
                int length = channelDataSegment.EndIndex - channelDataSegment.BegeinIndex;
                int segmentTargetCount = length / sample;
                if (length % sample != 0)
                {
                    segmentTargetCount += 1;
                }

                segmentTargetCount = segmentTargetCount * 2;
                short[] result = new short[segmentTargetCount];
                short[] srcData = channelDataSegment.SrcData;

                if (length <= segmentTargetCount)
                {
                    if (length != result.Length)
                    {
                        result = new short[length];
                    }

                    Array.Copy(srcData, channelDataSegment.BegeinIndex, result, 0, result.Length);
                }
                else
                {
                    short lMaxValue, lMinValue;
                    short tmpValue;
                    int position = 0;
                    int i, innerEndIndex;
                    int endIndex = channelDataSegment.EndIndex;
                    int currentEndIndex;
                    if (channelDataSegment.Flag)
                    {
                        currentEndIndex = endIndex - sample;
                    }
                    else
                    {
                        currentEndIndex = endIndex;
                    }

                    for (i = channelDataSegment.BegeinIndex; i < currentEndIndex; i += sample)
                    {
                        innerEndIndex = i + sample;
                        lMaxValue = srcData[i];
                        lMinValue = lMaxValue;
                        for (int j = i + 1; j < innerEndIndex; j++)
                        {
                            tmpValue = srcData[j];
                            if (tmpValue > lMaxValue)
                            {
                                lMaxValue = tmpValue;
                            }
                            else if (tmpValue < lMinValue)
                            {
                                lMinValue = tmpValue;
                            }
                        }

                        result[position] = lMinValue;
                        result[position + 1] = lMaxValue;
                        position += 2;
                    }

                    if (channelDataSegment.Flag)
                    {
                        //最后 一次
                        innerEndIndex = i;
                        if (innerEndIndex > endIndex)
                        {
                            innerEndIndex = endIndex;
                        }

                        i -= sample;
                        position -= 2;

                        lMaxValue = srcData[i];
                        lMinValue = lMaxValue;
                        for (int j = i + 1; j < innerEndIndex; j++)
                        {
                            tmpValue = srcData[j];
                            if (tmpValue > lMaxValue)
                            {
                                lMaxValue = tmpValue;
                            }
                            else if (tmpValue < lMinValue)
                            {
                                lMinValue = tmpValue;
                            }
                        }

                        if (position >= 0)
                        {
                            result[position] = lMinValue;
                            result[position + 1] = lMaxValue;
                        }
                    }
                }

                channelDataSegment.Result = result;
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }
    }


    /// <summary>
    /// 声道数据段信息
    /// </summary>
    internal class ChannelDataSegment
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        public short[] SrcData { get; set; }

        /// <summary>
        /// 起始位置
        /// </summary>
        public int BegeinIndex { get; set; }

        /// <summary>
        /// 结束位置
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// 最后一段数据是否特殊处理
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public int Sample { get; set; }

        /// <summary>
        /// 结果数据
        /// </summary>
        public short[] Result { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChannelDataSegment()
        {

        }
    }
}
