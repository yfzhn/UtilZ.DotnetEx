using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilZ.Dotnet.Ex.Log;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    internal sealed class VoiceDataResampleSegment<T> : IDisposable where T : IComparable
    {
        public int Order { get; private set; }
        private readonly T[][] _channelDataList;
        public T[][] ChannelDataList
        {
            get
            {
                return this._channelDataList;
            }
        }

        public int ChannelDataLength { get; private set; }

        private readonly Stream _stream;
        private readonly BinaryReader _br;
        private readonly VoicePlotData _voicePlotData;
        private readonly long _begeinIndex;
        private readonly long _endIndex;
        private readonly int _compressPointCount;

        public VoiceDataResampleSegment(int order, Stream stream, VoicePlotData voicePlotData,
            long begeinIndex, long endIndex, int compressPointCount)
        {
            this.Order = order;

            this._channelDataList = new T[voicePlotData.ChannelCount][];
            int count = (int)(2 * (endIndex - begeinIndex) / voicePlotData.PointLength);//乘以2是因为每段都取最小最大两个值
            for (int i = 0; i < voicePlotData.ChannelCount; i++)
            {
                this._channelDataList[i] = new T[count];
            }

            this._stream = stream;
            this._br = new BinaryReader(stream);
            this._voicePlotData = voicePlotData;
            this._begeinIndex = begeinIndex;
            this._endIndex = endIndex;
            this._compressPointCount = compressPointCount;
        }

        public void Dispose()
        {
            this._stream.Close();
            this._stream.Dispose();
        }


        public void Resample()
        {
            try
            {
                IComparable[] pointMinArr = new IComparable[this._voicePlotData.ChannelCount];
                IComparable[] pointMaxArr = new IComparable[this._voicePlotData.ChannelCount];

                //初始化
                //this.ResampleInit(pointMinArr, pointMaxArr);

                //处理
                this.ChannelDataLength = PrimitiveResample(pointMinArr, pointMaxArr);
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
        }

        private int PrimitiveResample(IComparable[] pointMinArr, IComparable[] pointMaxArr)
        {
            IComparable tmp;
            this._stream.Position = this._begeinIndex;
            int index = 0;

            while (this._stream.Position < this._endIndex)
            {
                switch (this._voicePlotData.SampleBit)
                {
                    case VoicePlotSampleBit.Sample8Bit:
                        tmp = this._br.ReadByte();
                        break;
                    case VoicePlotSampleBit.Sample16Bit:
                        tmp = this._br.ReadInt16();
                        break;
                    case VoicePlotSampleBit.Sample32Bit:
                        tmp = this._br.ReadSingle();
                        break;
                    default:
                        throw new NotSupportedException($"不支持的采样位数:{this._voicePlotData.SampleBit}");
                }

                this._channelDataList[0][index] = (T)tmp;
                index++;
            }

            return index;
        }

        private int PrimitiveResample_bk(IComparable[] pointMinArr, IComparable[] pointMaxArr)
        {
            IComparable tmp;
            int cannelDataLength = 0;
            bool nextSeg = true;
            this._stream.Position = this._begeinIndex;

            while (this._stream.Position < this._endIndex)
            {
                nextSeg = true;

                for (int i = 0; i < this._compressPointCount; i++)
                {
                    for (int j = 0; j < this._voicePlotData.ChannelCount; j++)
                    {
                        if (this._stream.Position >= this._endIndex)
                        {
                            return cannelDataLength;
                        }

                        switch (this._voicePlotData.SampleBit)
                        {
                            case VoicePlotSampleBit.Sample8Bit:
                                tmp = this._br.ReadByte();
                                break;
                            case VoicePlotSampleBit.Sample16Bit:
                                tmp = this._br.ReadInt16();
                                break;
                            case VoicePlotSampleBit.Sample32Bit:
                                tmp = this._br.ReadSingle();
                                break;
                            default:
                                throw new NotSupportedException($"不支持的采样位数:{this._voicePlotData.SampleBit}");
                        }

                        if (nextSeg)
                        {
                            pointMinArr[j] = tmp;
                            pointMaxArr[j] = tmp;
                        }
                        else
                        {
                            //CompareTo一个值，指示要比较的对象的相对顺序。返回值的含义如下：
                            //值 含义 小于零 此实例按排序顺序在 obj 前面。 零 此实例与 obj 在排序顺序中出现的位置相同。
                            //大于零 此实例按排序顺序在 obj 后面。
                            if (tmp.CompareTo(pointMinArr[j]) < 0)
                            {
                                pointMinArr[j] = tmp;
                            }

                            if (tmp.CompareTo(pointMaxArr[j]) > 0)
                            {
                                pointMaxArr[j] = tmp;
                            }
                        }


                    }

                    nextSeg = false;
                }

                for (int k = 0; k < this._voicePlotData.ChannelCount; k++)
                {
                    this._channelDataList[k][cannelDataLength] = (T)pointMinArr[k];
                    this._channelDataList[k][cannelDataLength + 1] = (T)pointMaxArr[k];
                }

                cannelDataLength += 2;
            }

            return cannelDataLength;
        }

        private void ResampleInit(IComparable[] pointMinArr, IComparable[] pointMaxArr)
        {
            IComparable tmp;
            if (this._begeinIndex >= this._voicePlotData.PointLength)
            {
                this._stream.Position = this._begeinIndex - this._voicePlotData.PointLength;
                for (int j = 0; j < this._voicePlotData.ChannelCount; j++)
                {
                    switch (this._voicePlotData.SampleBit)
                    {
                        case VoicePlotSampleBit.Sample8Bit:
                            tmp = this._br.ReadByte();
                            break;
                        case VoicePlotSampleBit.Sample16Bit:
                            tmp = this._br.ReadInt16();
                            break;
                        case VoicePlotSampleBit.Sample32Bit:
                            tmp = this._br.ReadSingle();
                            break;
                        default:
                            throw new NotSupportedException($"不支持的采样位数:{this._voicePlotData.SampleBit}");
                    }

                    pointMinArr[j] = tmp;
                    pointMaxArr[j] = tmp;
                }
            }
            else
            {
                switch (this._voicePlotData.SampleBit)
                {
                    case VoicePlotSampleBit.Sample8Bit:
                        tmp = (byte)0;
                        break;
                    case VoicePlotSampleBit.Sample16Bit:
                        tmp = (short)0;
                        break;
                    case VoicePlotSampleBit.Sample32Bit:
                        tmp = (float)0;
                        break;
                    default:
                        throw new NotSupportedException($"不支持的采样位数:{this._voicePlotData.SampleBit}");
                }

                for (int j = 0; j < this._voicePlotData.ChannelCount; j++)
                {
                    pointMinArr[j] = tmp;
                    pointMaxArr[j] = tmp;
                }
                this._stream.Position = this._begeinIndex;
            }
        }
    }
}

