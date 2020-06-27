using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UtilZ.Dotnet.Wav.ExBass;
using UtilZ.Dotnet.Wav.Model;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Diagnostics;
using UtilZ.Dotnet.Wav.WavMaterial;

namespace UtilZ.Dotnet.Wav
{
    /// <summary>
    /// WAV播放控件
    /// </summary>
    // 摘要:
    //     提供一个可用来创建其他控件的空控件。
    //[DesignerCategory("UserControl")]
    //[DefaultEvent("Load")]
    //[Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner))]
    //[ComVisible(true)]
    //[ClassInterface(ClassInterfaceType.AutoDispatch)]
    //[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public partial class WavePlayer : Control
    {
        /// <summary>
        /// WAV播放控件实例数
        /// </summary>
        private static int _instanceCount = 0;

        #region 私有变量
        /// <summary>
        /// 音频文件句柄未打开过文件为-1
        /// </summary>
        private int _handle = -1;

        /// <summary>
        /// 文件信息
        /// </summary>
        private BASS_CHANNELINFO_INTERNAL _channelInfo;

        /// <summary>
        /// bass获取到的总数据长度
        /// </summary>
        private long _bassDataTotalLength = 0;

        /// <summary>
        /// 原始左声道数据长度
        /// </summary>
        private int _srcLeftChannelDataLength = 0;

        /// <summary>
        /// 原始数据
        /// </summary>
        private short[] _srcData = null;

        /// <summary>
        /// 左声道一次重采样后的数据
        /// </summary>
        private short[] _srcSampleLeftData = null;

        /// <summary>
        /// 右声道一次重采样后的数据
        /// </summary>
        private short[] _srcSampleRightData = null;

        /// <summary>
        /// 缩略波形图左声道绘图数据
        /// </summary>
        private short[] _zoomLeftData = null;

        /// <summary>
        /// 缩略波形图右声道绘图数据数据
        /// </summary>
        private short[] _zoomRightData = null;

        /// <summary>
        /// 左声道绘图数据
        /// </summary>
        private short[] _leftData = null;

        /// <summary>
        /// 右声道绘图数据数据
        /// </summary>
        private short[] _rightData = null;

        /// <summary>
        /// 图形双缓冲缓冲区对象
        /// </summary>
        private BufferedGraphics _grafx = null;

        /// <summary>
        /// 缩略波形区域
        /// </summary>
        private Rectangle _zoomArea;

        /// <summary>
        /// 显示波形区域在缩略波形中的对应的区域
        /// </summary>
        private RectangleF _zoomDisplayArea;

        /// <summary>
        /// 显示波形区域在缩略波形中的对应的区域中的选中区域
        /// </summary>
        private RectangleF _zoomSelectedArea;

        /// <summary>
        /// 时间区域
        /// </summary>
        private Rectangle _timeArea;

        /// <summary>
        /// Logo区域
        /// </summary>
        private Rectangle _logoArea;

        /// <summary>
        /// 幅度区域
        /// </summary>
        private Rectangle _dbArea;

        /// <summary>
        /// 主波形区域
        /// </summary>
        private Rectangle _wavArea;

        /// <summary>
        /// 主波形选中区域
        /// </summary>
        private RectangleF _wavSelectedArea;

        /// <summary>
        /// 当前局部刷新区域集合
        /// </summary>
        private readonly List<Rectangle> _partRefreshAreas = new List<Rectangle>();

        /// <summary>
        /// 缩略波形单个点对应的宽度
        /// </summary>
        private float _zoomDataWidth;

        /// <summary>
        /// 当前绘制的主波形数据起始索引,对应的数据为原始数据
        /// </summary>
        private long _ws = 0;

        /// <summary>
        /// 当前绘制的主波形数据结束索引,对应的数据为原始数据
        /// </summary>
        private long _we = 0;

        /// <summary>
        /// 当前选中的主波形数据起始索引,对应的数据为原始数据
        /// </summary>
        private long _ss = 0;

        /// <summary>
        /// 当前选中的主波形区域数据结束索引,对应的数据为原始数据
        /// </summary>
        private long _se = 0;

        /// <summary>
        /// 缩略波形图当前播放指示线坐标X
        /// </summary>
        private float _zoomPlayLineX = 0;

        /// <summary>
        /// 主波形图当前播放指示线坐标X
        /// </summary>
        private float _wavPlayLineX = 0;

        /// <summary>
        /// 播放位置指示线更新Timer
        /// </summary>
        private readonly Timer _playLocationLineTimer = new Timer();

        /// <summary>
        /// 主波形播放位置指示线上次指示线所在的位置是否在主波形显示段内
        /// </summary>
        private bool _isWavPlayLocationLingLastInArea = false;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public WavePlayer()
        {
            //创建一个实例则实例数加1
            System.Threading.Interlocked.Increment(ref WavePlayer._instanceCount);

            //大小
            this.Size = new System.Drawing.Size(200, 130);
            //最小大小
            this.MinimumSize = this.Size;

            //设置绘制样式
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            //初始化播放位置指示线Timer
            this._playLocationLineTimer.Interval = 100;
            this._playLocationLineTimer.Tick += PlayLocationLineTimer_Tick;
        }

        /// <summary>
        /// 播放位置计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayLocationLineTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                long position = Bass.BASS_ChannelGetPosition(this._handle, BASSMode.BASS_POS_BYTE);
                float wavLocation = (float)position / (this._channelInfo.chans * WavePlayer.OFFSETPARA);

                //如果开启环听,且播放到环听的结尾处,则设置播放位置到选中区域开始处
                if (this._isRingHear && (this._se - this._ss) > 0 && wavLocation - this._se > 0)
                {
                    long pos = this._ss * this._channelInfo.chans * WavePlayer.OFFSETPARA;//计算选中区域开始位置
                    bool ret = Bass.BASS_ChannelSetPosition(this._handle, pos, BASSMode.BASS_POS_BYTE);//设置播放位置到选中区域开始处
                    if (ret)
                    {
                        position = Bass.BASS_ChannelGetPosition(this._handle, BASSMode.BASS_POS_BYTE);
                        wavLocation = (float)position / (this._channelInfo.chans * WavePlayer.OFFSETPARA);
                    }
                }

                float oldZoomPlayLineX = this._zoomPlayLineX;
                //缩略波形播放位置指示线X
                this._zoomPlayLineX = this._zoomArea.X + ((float)(position * this._zoomArea.Width)) / this._bassDataTotalLength;

                //添加播放位置指示线更新区域[需要刷新返回true,失败返回fals]
                bool refreshFlag = this.AddPlayLocationLineUpdateAreaWidth(oldZoomPlayLineX, this._zoomPlayLineX, this._zoomArea.Y, this._zoomArea.Height, this._playLineChannelPen.Width);

                //计算主波形播放位置指示线
                this.CalculateWavPlayLocationLine(wavLocation);

                //刷新波形图
                if (refreshFlag)
                {
                    this.RefreshWave(true, false, false, false, false, false);
                }

                //如果本次取回的播放位置大于等于总长度,则表示播放完成
                if (position >= this._bassDataTotalLength)
                {
                    this._playLocationLineTimer.Stop();
                    //触发播放完成事件
                    this.OnRaisePlayEnd();
                }
                else
                {
                    //调整缩略波形中对应的主波形显示段,刷新主波形+时间
                    bool isLastInArea = this._isWavPlayLocationLingLastInArea;
                    if (!isLastInArea && wavLocation >= this._ws && wavLocation <= this._we)
                    {
                        //如果主波形播放位置指示线不在主波形范围内,且本次在,则设置值为true
                        this._isWavPlayLocationLingLastInArea = true;
                    }

                    //如果上次主波形播放位置在范围内,则当播放到主波形显示段结束时,自动调整到下一段
                    if (isLastInArea && wavLocation >= this._we && wavLocation < this._srcLeftChannelDataLength)
                    {
                        //缩略波形对应主波形中显示段改变
                        this.ZoomWavDisplaySegmentAreaChange(this._we - this._ws);
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
        }

        /// <summary>
        /// 添加播放位置指示线更新区域[需要刷新返回true,失败返回fals]
        /// </summary>
        /// <param name="oldPlayLineX">旧的播放位置指示线X</param>
        /// <param name="newPlayLineX">新的播放位置指示线X</param>
        /// <param name="y">刷新区域Y</param>
        /// <param name="height">刷新区域高度</param>
        /// <param name="penWidth">绘制笔宽度</param>
        /// <returns>需要刷新返回true,失败返回fals</returns>
        private bool AddPlayLocationLineUpdateAreaWidth(float oldPlayLineX, float newPlayLineX, float y, float height, float penWidth)
        {
            bool refreshFlag = false;
            float zoomPlayLineWidth = newPlayLineX - oldPlayLineX;
            if (zoomPlayLineWidth < 0)
            {
                /******************************************
                 * |  newPlayLineX        oldPlayLineX    |
                 * |________|_______________________|_____|
                 ******************************************/

                refreshFlag |= this.AddPartRefreshArea(new RectangleF(oldPlayLineX, y, penWidth, height));
                refreshFlag |= this.AddPartRefreshArea(new RectangleF(newPlayLineX, y, penWidth, height));
            }
            else
            {
                /******************************************
                 * |    oldPlayLineX      newPlayLineX    |
                 * |________|_______________________|_____|
                 ******************************************/
                zoomPlayLineWidth = zoomPlayLineWidth + penWidth;
                refreshFlag |= this.AddPartRefreshArea(new RectangleF(oldPlayLineX, y, zoomPlayLineWidth, height));
            }

            return refreshFlag;
        }

        /// <summary>
        /// 缩略波形对应主波形中显示段改变
        /// </summary>
        /// <param name="offset">调整偏移量</param>
        private void ZoomWavDisplaySegmentAreaChange(long offset)
        {
            long ws = this._ws + offset;
            long we = this._we + offset;
            if (ws < 0)
            {
                ws = 0;
                we = this._we - this._ws;
            }
            else if (we > this._srcLeftChannelDataLength)
            {
                we = this._srcLeftChannelDataLength;
                ws = we - (this._we - this._ws);
            }

            this._ws = ws;
            this._we = we;

            //缩略波形图中主波形显示段改变,则局部刷新缩略波选中区域+显示区域+主波形区域+时间区域
            this.AddPartRefreshArea(this._zoomDisplayArea);//缩略波形旧的主波形显示段区域
            this.AddPartRefreshArea(this._zoomSelectedArea);//缩略波形旧的选中区域

            //更新波形选中区域
            this.UpdateSelectedArea();

            this.AddPartRefreshArea(this._zoomDisplayArea);//缩略波形新的主波形显示段区域
            this.AddPartRefreshArea(this._zoomSelectedArea);//缩略波形新的选中区域
            this.AddPartRefreshArea(this._wavArea);//主波形区域
            this.AddPartRefreshArea(this._timeArea);//时间区域

            //计算bass处于非播放时播放位置指示线X坐标
            this.CalculateWavPlayLocationLine();

            //刷新波形图
            this.RefreshWave(true, true, false, false, false, true);
        }

        /// <summary>
        /// 计算bass处于非播放时播放位置指示线X坐标
        /// </summary>
        private void CalculateWavPlayLocationLine()
        {
            try
            {
                //当没有打开文件或是播放状态为正在播放,则无视
                if (this._handle == -1 || this.GrtPlayState() == PlayStatus.PLAYING)
                {
                    return;
                }

                long position = Bass.BASS_ChannelGetPosition(this._handle, BASSMode.BASS_POS_BYTE);
                //缩略波形播放位置指示线X
                this._zoomPlayLineX = this._zoomArea.X + ((float)(position * this._zoomArea.Width)) / this._bassDataTotalLength;

                //左声道数据对应的播放位置
                float wavLocation = (float)position / (this._channelInfo.chans * WavePlayer.OFFSETPARA);

                //计算主波形播放位置指示线
                this.CalculateWavPlayLocationLine(wavLocation);
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
        }

        /// <summary>
        /// 计算主波形播放位置指示线X坐标
        /// </summary>
        /// <param name="wavLocation">主波形当前播放位置</param>
        private void CalculateWavPlayLocationLine(float wavLocation)
        {
            float oldWavPlayLineX = this._wavPlayLineX;
            if (wavLocation < this._ws || wavLocation > this._we)
            {
                //如果主波形播放位置指示线不在主波形范围内,则X坐标设定为-10,不会刷新显示
                this._wavPlayLineX = -10;
                if (oldWavPlayLineX > 0 && oldWavPlayLineX < this.Width)
                {
                    //如果主波形图中的旧的播放位置指示线在主波形区域内,内添加指示线区域为局部刷新区域
                    this.AddPartRefreshArea(new RectangleF(oldWavPlayLineX, this._wavArea.Y, this._playLineChannelPen.Width, this._wavArea.Height));
                }
            }
            else
            {
                //主波形播放位置指示线X
                this._wavPlayLineX = (wavLocation - this._ws) * this._wavArea.Width / (this._we - this._ws) + this._wavArea.X;

                //添加播放位置指示线更新区域[需要刷新返回true,失败返回fals]
                this.AddPlayLocationLineUpdateAreaWidth(oldWavPlayLineX, this._wavPlayLineX, this._wavArea.Y, this._wavArea.Height, this._playLineChannelPen.Width);
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        private void LoadFile(string fileName)
        {
            // 停止
            this.Stop();

            //添加除幅度区域外的区域为更新区域
            this.AddPartRefreshArea(new Rectangle(0, 0, this._zoomArea.Width, this.Height));

            //如果要播放的文件不存在,则清空波形图
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                this._srcLeftChannelDataLength = 0;
                //当前控件大小改变后,刷新波形图
                this.RefreshWave(true, true, false, false, true, true);
                return;
            }

            try
            {
                //左声道原始数据
                short[] srcLeftData = null;

                //右声道原始数据
                short[] srcRightData = null;

                //初始化波形数据
                this._srcData = null;
                bool initWavInfoRet = this.InitWavInfo(fileName, out this._channelInfo, out srcLeftData, out srcRightData, out this._bassDataTotalLength, out this._durationTime, out this._isStereo);
                //bool initWavInfoRet = this.InitWavInfo_bk(fileName, out this._channelInfo, out srcLeftData, out srcRightData, out this._bassDataTotalLength, out this._durationTime, out this._isStereo);
                //重置操作区域
                this.ResetOperArea();

                //是否初始化成功
                if (initWavInfoRet)
                {
                    this._srcLeftChannelDataLength = srcLeftData.Length;
                    //缩略波形单个点对应的宽度
                    this._zoomDataWidth = (float)this._zoomArea.Width / this._srcLeftChannelDataLength;

                    this._ws = 0;
                    this._we = this._srcLeftChannelDataLength;

                    //声道数据重采样
                    if (this._we > WavePlayer.FIRSTSAMPLE)
                    {
                        this.ChannelSampleDecrease(this._isStereo, WavePlayer.FIRSTSAMPLE, srcLeftData, srcRightData, 0, this._srcLeftChannelDataLength, ref this._srcSampleLeftData, ref this._srcSampleRightData);
                    }
                    else
                    {
                        this._srcSampleLeftData = new short[this._srcLeftChannelDataLength];
                        Array.Copy(srcLeftData, 0, this._srcSampleLeftData, 0, this._srcLeftChannelDataLength);

                        if (this._isStereo)
                        {
                            this._srcSampleRightData = new short[this._srcLeftChannelDataLength];
                            Array.Copy(srcRightData, 0, this._srcSampleRightData, 0, this._srcLeftChannelDataLength);
                        }
                    }
                }
                else
                {
                    this._srcLeftChannelDataLength = 0;
                    this._bassDataTotalLength = 0;
                }

                //当前控件大小改变后,刷新波形图
                this.RefreshWave(true, true, true, false, true, true);

                //释放已打开要播放的文件
                if (this._handle != 0 && this._handle != -1)
                {
                    if (!Bass.BASS_StreamFree(this._handle))
                    {
                        this.OnRaiseLog(BassErrorCode.GetErrorInfo());
                    }
                }

                //打开文件播放
                this._handle = Bass.BASS_StreamCreateFile(false, fileName, 0, 0, BASSFileFlag.BASS_UNICODE);
                if (Bass.BASS_ErrorGetCode() != BassErrorCode.BASS_OK)
                {
                    this._handle = -1;//打开文件失败.重置为-1.否则打开失败时值为0
                    this.OnRaiseLog(BassErrorCode.GetErrorInfo());
                    return;
                }

                //是否自动播放
                if (this._autoPlay)
                {
                    this.Play();
                }
            }
            catch (Exception ex)
            {
                int errCode = Bass.BASS_ErrorGetCode();
                switch (errCode)
                {
                    case BassErrorCode.BASS_ERROR_FILEFORM:
                        throw new ApplicationException("不支持的文件格式", ex);
                    case BassErrorCode.BASS_ERROR_NOTAVAIL:
                        //throw new ApplicationException("没有找到音频输出设备", ex);//暂时写为不提示
                        break;
                    case BassErrorCode.BASS_ERROR_FILEOPEN:
                        throw new ApplicationException(string.Format("文件{0}不能打开或被占用", fileName), ex);
                    case BassErrorCode.BASS_ERROR_CODEC:
                        throw new ApplicationException("找不到相应的解码器,请安装最新版的Windows Media codecs", ex);
                    case BassErrorCode.BASS_ERROR_FORMAT:
                        throw new ApplicationException("音频输出设备不支持文件的采样率", ex);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// 声道重采样减少数据
        /// </summary>
        /// <param name="isStereo">是否是立体声</param>
        /// <param name="targetCount">目标数据个数</param>
        /// <param name="srcLeftData">左声道原始数据</param>
        /// <param name="srcRightData">右声道原始数据</param>
        /// <param name="begeinIndex">原始数据起始索引</param>
        /// <param name="endIndex">原始数据结束索引</param>
        /// <param name="leftData">重采样后的左声道数据</param>
        /// <param name="rightData">重采样后的右声道数据</param>
        private void ChannelSampleDecrease(bool isStereo, int targetCount, short[] srcLeftData, short[] srcRightData, long begeinIndex, long endIndex, ref short[] leftData, ref short[] rightData)
        {
            if (srcLeftData == null || targetCount == 0)
            {
                leftData = new short[0];
                rightData = new short[0];
                return;
            }

            if (begeinIndex == -1)
            {
                begeinIndex = 0;
            }

            if (endIndex <= 0)
            {
                endIndex = srcLeftData.Length;
            }

            long srcCount = endIndex - begeinIndex;//原始数据个数
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

            long sampleSegmentCount = sampleCount / 2;//采样段数,因为是一段数据中取出最大值和最小值,所以目标点数除以2
            if (sampleCount % 2 != 0)//如果目标数据个数为奇数,则采样个数+1
            {
                sampleSegmentCount++;
            }

            long sample = srcCount / sampleSegmentCount;//采样率
            if (sample == 0)//如果采样率为0,则总的数据个数少于目标个数,则直接将原始数据拷贝到目标数组
            {
                leftData = new short[sampleCount];
                Array.Copy(srcLeftData, begeinIndex, leftData, 0, leftData.Length);

                if (isStereo)
                {
                    rightData = new short[sampleCount];
                    Array.Copy(srcRightData, begeinIndex, rightData, 0, rightData.Length);
                }

                return;
            }
            else if (srcCount % sampleSegmentCount != 0)
            {
                sample++;
            }

            int processorCount = Environment.ProcessorCount;//CPU核心数
            long coreSampleSegmentCount = sampleSegmentCount / processorCount;//每一核分配到的采样段个数
            if (sampleSegmentCount % processorCount != 0)
            {
                coreSampleSegmentCount++;
            }

            //计算数据段大小
            long segDataSize = coreSampleSegmentCount * sample;//采样数据段大小

            //拆分数据段
            List<ChannelDataSegment> segments = new List<ChannelDataSegment>();
            long startIndex = begeinIndex;
            for (int i = 0; i < processorCount; i++)
            {
                ChannelDataSegment channelDataSegment = new ChannelDataSegment();
                channelDataSegment.IsStereo = isStereo;
                channelDataSegment.SrcLeftData = srcLeftData;
                channelDataSegment.SrcRightData = srcRightData;
                channelDataSegment.Sample = sample;

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
            Parallel.ForEach(segments, this.RepeatSample);
            //foreach (var segment in segments)
            //{
            //    this.RepeatSample(segment);
            //}

            //拼接数据
            var size = segments.Sum((item) => { return item.LeftData.Length; });
            leftData = new short[size];
            int dstIndex = 0;
            if (isStereo)
            {
                rightData = new short[size];
                foreach (var segment in segments)
                {
                    Array.Copy(segment.LeftData, 0, leftData, dstIndex, segment.LeftData.Length);
                    Array.Copy(segment.RightData, 0, rightData, dstIndex, segment.RightData.Length);
                    dstIndex += segment.LeftData.Length;
                }
            }
            else
            {
                foreach (var segment in segments)
                {
                    Array.Copy(segment.LeftData, 0, leftData, dstIndex, segment.LeftData.Length);
                    dstIndex += segment.LeftData.Length;
                }
            }

            //if (addinFlag)//重采样添加数据
            //{
            //    this.ChannelSampleIncrement(isStereo, targetCount, leftData, rightData, ref leftData, ref rightData);
            //}
        }

        /// <summary>
        /// 重采样--指针
        /// </summary>
        /// <param name="channelDataSegment">重采样数据段信息</param>
        private void RepeatSample(ChannelDataSegment channelDataSegment)
        {
            try
            {
                long sample = channelDataSegment.Sample;
                long length = channelDataSegment.EndIndex - channelDataSegment.BegeinIndex;
                long segmentTargetCount = length / sample;
                if (length % sample != 0)
                {
                    segmentTargetCount += 1;
                }

                segmentTargetCount = segmentTargetCount * 2;
                short[] leftData = new short[segmentTargetCount];
                short[] rightData = null;

                short[] srcLeftData = channelDataSegment.SrcLeftData;
                short[] srcRightData = channelDataSegment.SrcRightData;
                short lMaxValue, lMinValue;
                short tmpValue;
                int position = 0;
                long i, innerEndIndex;
                long endIndex = channelDataSegment.EndIndex;
                long currentEndIndex;
                if (channelDataSegment.Flag)
                {
                    currentEndIndex = endIndex - sample;
                }
                else
                {
                    currentEndIndex = endIndex;
                }

                if (channelDataSegment.IsStereo)
                {
                    short rMaxValue, rMinValue;
                    //双声道
                    rightData = new short[leftData.Length];
                    if (length <= segmentTargetCount)
                    {
                        if (length != leftData.Length)
                        {
                            leftData = new short[length];
                            rightData = new short[length];
                        }

                        Array.Copy(srcLeftData, channelDataSegment.BegeinIndex, leftData, 0, leftData.Length);
                        Array.Copy(srcRightData, channelDataSegment.BegeinIndex, rightData, 0, rightData.Length);
                    }
                    else
                    {
                        for (i = channelDataSegment.BegeinIndex; i < currentEndIndex; i += sample)
                        {
                            innerEndIndex = i + sample;
                            lMaxValue = srcLeftData[i];
                            lMinValue = lMaxValue;
                            rMaxValue = srcRightData[i];
                            rMinValue = rMaxValue;
                            for (long j = i + 1; j < innerEndIndex; j++)
                            {
                                tmpValue = srcLeftData[j];
                                if (tmpValue > lMaxValue)
                                {
                                    lMaxValue = tmpValue;
                                }
                                else if (tmpValue < lMinValue)
                                {
                                    lMinValue = tmpValue;
                                }

                                tmpValue = srcRightData[j];
                                if (tmpValue > rMaxValue)
                                {
                                    rMaxValue = tmpValue;
                                }
                                else if (tmpValue < rMinValue)
                                {
                                    rMinValue = tmpValue;
                                }
                            }

                            leftData[position] = lMinValue;
                            leftData[position + 1] = lMaxValue;
                            rightData[position] = rMinValue;
                            rightData[position + 1] = rMaxValue;
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
                            lMaxValue = srcLeftData[i];
                            lMinValue = lMaxValue;
                            rMaxValue = srcRightData[i];
                            rMinValue = rMaxValue;
                            for (long j = i + 1; j < innerEndIndex; j++)
                            {
                                tmpValue = srcLeftData[j];
                                if (tmpValue > lMaxValue)
                                {
                                    lMaxValue = tmpValue;
                                }
                                else if (tmpValue < lMinValue)
                                {
                                    lMinValue = tmpValue;
                                }

                                tmpValue = srcRightData[j];
                                if (tmpValue > rMaxValue)
                                {
                                    rMaxValue = tmpValue;
                                }
                                else if (tmpValue < rMinValue)
                                {
                                    rMinValue = tmpValue;
                                }
                            }

                            if (position >= 0)
                            {
                                leftData[position] = lMinValue;
                                leftData[position + 1] = lMaxValue;
                                rightData[position] = rMinValue;
                                rightData[position + 1] = rMaxValue;
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else
                {
                    //单声道
                    rightData = null;
                    if (length <= segmentTargetCount)
                    {
                        if (length != leftData.Length)
                        {
                            leftData = new short[length];
                        }

                        Array.Copy(srcLeftData, channelDataSegment.BegeinIndex, leftData, 0, leftData.Length);
                    }
                    else
                    {
                        for (i = channelDataSegment.BegeinIndex; i < currentEndIndex; i += sample)
                        {
                            innerEndIndex = i + sample;
                            lMaxValue = srcLeftData[i];
                            lMinValue = lMaxValue;
                            for (long j = i + 1; j < innerEndIndex; j++)
                            {
                                tmpValue = srcLeftData[j];
                                if (tmpValue > lMaxValue)
                                {
                                    lMaxValue = tmpValue;
                                }
                                else if (tmpValue < lMinValue)
                                {
                                    lMinValue = tmpValue;
                                }
                            }

                            leftData[position] = lMinValue;
                            leftData[position + 1] = lMaxValue;
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
                            lMaxValue = srcLeftData[i];
                            lMinValue = lMaxValue;
                            for (long j = i + 1; j < innerEndIndex; j++)
                            {
                                tmpValue = srcLeftData[j];
                                if (tmpValue > lMaxValue)
                                {
                                    lMaxValue = tmpValue;
                                }
                                else if (tmpValue < lMinValue)
                                {
                                    lMinValue = tmpValue;
                                }
                            }

                            leftData[position] = lMinValue;
                            leftData[position + 1] = lMaxValue;
                        }
                    }
                }

                channelDataSegment.LeftData = leftData;
                channelDataSegment.RightData = rightData;
            }
            catch (Exception ex)
            {
                this.OnRaiseLog(ex);
            }
        }

        /// <summary>
        /// 初始化波形数据[成功返回ttue;失败返回false]
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="channelInfo">声道信息</param>
        /// <param name="leftData">左声道数据</param>
        /// <param name="rightData">右声道数据</param>
        /// <param name="totalLength">bass获取到的总数据长度</param>
        /// <param name="time">播放时长</param>
        /// <param name="isStereo">当前文件是否是立体声,即双声道[true:双声道;false:单声道]</param>
        /// <returns>成功返回ttue;失败返回false</returns>
        private bool InitWavInfo_bk(string fileName, out BASS_CHANNELINFO_INTERNAL channelInfo, out short[] leftData, out short[] rightData,
            out long totalLength, out double time, out bool isStereo)
        {
            int handle = Bass.BASS_StreamCreateFile(false, fileName, 0, 0, BASSFileFlag.BASS_UNICODE | BASSFileFlag.BASS_STREAM_DECODE);
            //获取文件信息
            channelInfo = Bass.BASS_ChannelGetInfo(handle);
            //if (channelInfo.chans > 2)
            //{
            //    throw new ApplicationException("不支持双声道以上的多声道");
            //}

            totalLength = Bass.BASS_ChannelGetLength(handle, BASSMode.BASS_POS_BYTE);
            time = Bass.BASS_ChannelBytes2Seconds(handle, totalLength);//时间,秒
            time = Math.Round(time, 3);
            Bass.BASS_StreamFree(handle);

            rightData = null;
            isStereo = false;
            //leftData = new short[totalLength / 2 + 2];
            List<short> list = new List<short>();

            var muilt = float.MaxValue / short.MaxValue;
            muilt = 100;
            using (var fs = File.OpenRead(fileName))
            {
                int count = (int)(totalLength / 2);
                count = 480;

                //尝试的参数
                count = 960;
                muilt = 150;

                var lastPos = fs.Length - 1;
                float[] data = new float[count + 2];

                while (fs.Position < lastPos)
                {
                    var mod = (int)((fs.Length - fs.Position) / 2);
                    if (mod < count)
                    {
                        count = mod;
                        data = new float[count + 2];
                    }

                    var br = new BinaryReader(fs);
                    int index = 0;
                    while (index < count)
                    {
                        data[index] = br.ReadInt16();
                        index++;
                    }

                    //Fourier.ForwardReal(data, count, FourierOptions.Matlab);

                    for (int i = 0; i < data.Length; i++)
                    {
                        list.Add((short)(data[i] / muilt));
                    }
                }
            }

            leftData = list.ToArray();
            return true;
        }


        /// <summary>
        /// 初始化波形数据[成功返回ttue;失败返回false]
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="channelInfo">声道信息</param>
        /// <param name="leftData">左声道数据</param>
        /// <param name="rightData">右声道数据</param>
        /// <param name="totalLength">bass获取到的总数据长度</param>
        /// <param name="time">播放时长</param>
        /// <param name="isStereo">当前文件是否是立体声,即双声道[true:双声道;false:单声道]</param>
        /// <returns>成功返回ttue;失败返回false</returns>
        private bool InitWavInfo(string fileName, out BASS_CHANNELINFO_INTERNAL channelInfo, out short[] leftData, out short[] rightData,
            out long totalLength, out double time, out bool isStereo)
        {
            int handle = -1;
            try
            {
                handle = Bass.BASS_StreamCreateFile(false, fileName, 0, 0, BASSFileFlag.BASS_UNICODE | BASSFileFlag.BASS_STREAM_DECODE);
                //获取文件信息
                channelInfo = Bass.BASS_ChannelGetInfo(handle);
                //if (channelInfo.chans > 2)
                //{
                //    throw new ApplicationException("不支持双声道以上的多声道");
                //}

                totalLength = Bass.BASS_ChannelGetLength(handle, BASSMode.BASS_POS_BYTE);
                time = Bass.BASS_ChannelBytes2Seconds(handle, totalLength);//时间,秒
                time = Math.Round(time, 3);


                short[] shortData = new short[totalLength];
                int byteCount = Bass.BASS_ChannelGetData(handle, shortData, (uint)totalLength);

                //byte[] data = new byte[totalLength];
                //int charCount = Bass.BASS_ChannelGetData(handle, data,0);
                //int charCount = Bass.BASS_ChannelGetData(handle, shortData, (uint)(BASS_ChannelGetDataLengthMode.BASS_DATA_FFT2048 | BASS_ChannelGetDataLengthMode.BASS_DATA_FFT_COMPLEX));
                //int charCount = Bass.BASS_ChannelGetData(handle, shortData, (uint)(BASS_ChannelGetDataLengthMode.BASS_DATA_FFT2048));
                if (byteCount == -1)
                {
                    this.OnRaiseLog(BassErrorCode.GetErrorInfo());

                    leftData = null;
                    rightData = null;
                    isStereo = false;
                    return false;
                }

                int channelFFTDataByteCount = byteCount / channelInfo.chans;//每个声道的FFT数据字节长度
                int channelFFTDataShortCount = channelFFTDataByteCount / 2;//因为输出数据是short,而数据长度为char的数据长度,short=char*2,所以此处除以2
                isStereo = channelInfo.chans >= 2;

                if (isStereo)
                {
                    leftData = new short[channelFFTDataShortCount];
                    rightData = new short[channelFFTDataShortCount];
                    int index = 0;
                    int offsetCount = channelInfo.chans - 2;

                    for (int i = 0; i < channelFFTDataShortCount; i++)
                    {
                        try
                        {
                            //The return values are interleaved in the same order as the channel's sample data, eg. stereo = left,right,left,etc. 
                            //5.1声道FFT数据顺序: L-R-C-LFE-LS-RS

                            leftData[i] = shortData[index++];
                            rightData[i] = shortData[index++];

                            index += offsetCount;
                            //index++;
                            //index++;

                            //index++;
                            //index++;
                        }
                        catch (Exception ex)
                        {
                            this.OnRaiseLog(ex);
                        }
                    }
                }
                else
                {
                    leftData = new short[channelFFTDataShortCount];
                    rightData = null;

                    for (int i = 0; i < channelFFTDataShortCount; i++)
                    {
                        try
                        {
                            leftData[i] = shortData[i];
                        }
                        catch (Exception ex)
                        {
                            this.OnRaiseLog(ex);
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                totalLength = 0;
                throw;
            }
            finally
            {
                if (handle != -1)
                {
                    Bass.BASS_StreamFree(handle);
                }
            }
        }

        /// <summary>
        /// 更新UI布局区域
        /// </summary>
        private void UpdateUIArea()
        {
            /**********************************************
            * 波形控件UI布局示意图                       *
            * |-----------------------------------|----| *
            * |        缩略波形图区域             | L  | *
            * |                                   | O  | *
            * |-----------------------------------| G  | *
            * |        时间区域                   | O  | *
            * |-----------------------------------|----| *
            * |                                   | 幅 | *
            * |        主波形图区域               | 度 | *
            * |                                   | 值 | *
            * |___________________________________|____| *
            **********************************************/

            //因为边界会重叠,所以从上往下从左往右都需要处理边界
            int topBoundary = 0;//上边界
            int leftBoundary = 0;//左边界

            //主区域宽度
            int width = this.Width - this._dbWidth - 1;

            //缩略波形区域
            this._zoomArea = new Rectangle(0, 0, width, this._zoomHeight);

            //时间区域
            topBoundary = topBoundary + 1;
            this._timeArea = new Rectangle(0, this._zoomHeight + topBoundary, width, this._timeHeight);

            //LOG区域
            this._logoArea = new Rectangle(width + leftBoundary, 0, this._dbWidth, this._zoomHeight + this._timeHeight + topBoundary);

            //幅度区域
            topBoundary = topBoundary + 1;
            leftBoundary = leftBoundary + 1;
            //(int)Math.Ceiling(this._playLineChannelPen.Width)这个值主要是播放位置指示线所占的宽度作为间隙
            //否则当播放到结束位置时,会将播放位置指示线绘制到幅度区域上
            int dbX = width + leftBoundary + (int)Math.Ceiling(this._playLineChannelPen.Width);
            int dbY = this._zoomHeight + this._timeHeight + topBoundary;
            int dbHeight = this.Height - dbY;
            this._dbArea = new Rectangle(dbX, dbY, this._dbWidth, dbHeight);

            //主波形区域
            this._wavArea = new Rectangle(0, dbY, width, dbHeight);

            //重新计算缩略波形单个点对应的宽度
            if (this._srcLeftChannelDataLength != 0)
            {
                //缩略波形单个点对应的宽度
                this._zoomDataWidth = (float)this._zoomArea.Width / this._srcLeftChannelDataLength;
            }

            //更新波形选中区域
            this.UpdateSelectedArea();
        }

        /// <summary>
        /// 更新波形选中区域
        /// </summary>
        /// <param name="x">主波形选中区域起始X</param>
        /// <param name="width">主波形选中区域宽度</param>
        private void UpdateSelectedArea(float x, float width)
        {
            if (this._leftData == null || this._leftData.Length == 0)
            {
                //重置操作区域
                this.ResetOperArea();
                return;
            }

            //主波形选中区域
            this._wavSelectedArea = new RectangleF(x, this._wavArea.Y, width, this._wavArea.Height);

            //显示波形区域在缩略波形中的对应的区域
            float zoomDaX1 = this._zoomArea.X + (int)Math.Ceiling(this._zoomDataWidth * this._ws);
            float zoomDaX2 = this._zoomArea.X + (int)Math.Ceiling(this._zoomDataWidth * this._we);
            this._zoomDisplayArea = new RectangleF(zoomDaX1, this._zoomArea.Y, zoomDaX2 - zoomDaX1, this._zoomArea.Height);

            //显示波形区域在缩略波形中的对应的区域中的选中区域
            float zoomSaX1 = this._zoomArea.X + this._zoomDataWidth * this._ss;
            float zoomSaX2 = this._zoomArea.X + this._zoomDataWidth * this._se;
            this._zoomSelectedArea = new RectangleF(zoomSaX1, this._zoomArea.Y, zoomSaX2 - zoomSaX1, this._zoomArea.Height);
        }

        /// <summary>
        /// 重置操作区域
        /// </summary>
        private void ResetOperArea()
        {
            this._ss = 0;
            this._se = 0;
            this._wavSelectedArea = new Rectangle(0, this._wavArea.Y, 0, this._wavArea.Height);
            this._zoomDisplayArea = this._zoomArea;
            this._zoomSelectedArea = new Rectangle(0, this._zoomArea.Y, 0, this._zoomArea.Height);
            this._zoomPlayLineX = this._zoomArea.X - this._playLineChannelPen.Width - 1;
            this._wavPlayLineX = this._wavArea.X - this._playLineChannelPen.Width - 1;

            //清空局部刷新区域集合
            this.ClearPartRefreshArea();
        }

        /// <summary>
        /// 计算主波形选中区域起始结束索引
        /// </summary>
        /// <param name="x">主波形选中区域起始X</param>
        /// <param name="width">主波形选中区域宽度</param>
        private void CalculateWavSelectedAreaIndex(float x, float width)
        {
            if (this._srcLeftChannelDataLength == 0 || width == 0)
            {
                this._ss = 0;
                this._se = 0;
                return;
            }

            this._ss = (long)Math.Floor(((this._we - this._ws) * x) / this._wavArea.Width + this._ws);
            this._se = (long)Math.Ceiling(((this._we - this._ws) * (x + width)) / this._wavArea.Width + this._ws);
            if (this._se > this._srcLeftChannelDataLength)
            {
                this._se = this._srcLeftChannelDataLength;
            }
        }

        /// <summary>
        /// 获取指定点所对应在UI中的区域
        /// </summary>
        /// <param name="point">指定点</param>
        /// <returns>指定点所对应在UI中的区域</returns>
        private UIArea GetUIArea(Point point)
        {
            UIArea area;
            if (point.Y <= this._zoomDisplayArea.Bottom && point.Y >= this._zoomDisplayArea.Top && point.X >= this._zoomDisplayArea.Left && point.X <= this._zoomDisplayArea.Right)
            {
                area = UIArea.ZoomDisplayArea;
            }
            else if (point.Y <= this._zoomArea.Bottom && point.Y >= this._zoomArea.Top && point.X >= this._zoomArea.Left && point.X <= this._zoomArea.Right)
            {
                area = UIArea.ZoomArea;
            }
            //else if (point.Y <= this._wavSelectedArea.Bottom && point.Y >= this._wavSelectedArea.Top && point.X >= this._wavSelectedArea.Left && point.X <= this._wavSelectedArea.Right)
            //{
            //    area = UIArea.WavSelectedArea;
            //}
            else if (point.Y <= this._wavArea.Bottom && point.Y >= this._wavArea.Top && point.X >= this._wavArea.Left && point.X <= this._wavArea.Right)
            {
                area = UIArea.WavArea;
            }
            else if (point.Y <= this._timeArea.Bottom && point.Y >= this._timeArea.Top && point.X >= this._timeArea.Left && point.X <= this._timeArea.Right)
            {
                area = UIArea.TimeArea;
            }
            else if (point.Y <= this._dbArea.Bottom && point.Y >= this._dbArea.Top && point.X >= this._dbArea.Left && point.X <= this._dbArea.Right)
            {
                area = UIArea.DbArea;
            }
            else
            {
                area = UIArea.None;
            }

            return area;
        }

        /// <summary>
        /// 更新波形选中区域
        /// </summary>
        private void UpdateSelectedArea()
        {
            //重新计算主波形选中区域
            float selectedX, selectedWidth;
            this.CalculateWavSelectedArea(out selectedX, out selectedWidth);

            //更新波形选中区域
            this.UpdateSelectedArea(selectedX, selectedWidth);
        }

        /// <summary>
        /// 重新计算主波形选中区域位置及宽度
        /// </summary>
        /// <param name="selectedX">主波形选中区域位置</param>
        /// <param name="selectedWidth">宽度</param>
        private void CalculateWavSelectedArea(out float selectedX, out float selectedWidth)
        {
            /************************************************************
             * 旧的区域
             *            |--wavSelectedArea---|
             * |__________|____________________|________________|
             * |-----------------oldWavArea---------------------|
             * 
             *  更新后的区域
             *               |wavSelectedArea|
             *      |________|_______________|_____________|
             *      |----------WavArea---------------------| 
             *************************************************************/

            if (this._ss >= this._we || this._se <= this._ws || this._ss == this._se)
            {
                /***************************************************************************
                * 选中区域不在当前显示范围内
                * |------------------------------|
                * |    ss --- se --- ws --- we   |
                * |    ws --- we --- ss --- se   |
                * |    ws --- ss|se(重合)--- we  |
                * |------------------------------|
                ***************************************************************************/
                selectedX = this._wavArea.X;
                selectedWidth = 0;
            }
            else if (this._ss <= this._ws)
            {
                selectedX = this._wavArea.X;
                if (this._se >= this._we)
                {
                    /***************************************************************************
                    * |------------------------------|
                    * |    ss --- ws --- we --- se   |
                    * |------------------------------|
                    ***************************************************************************/
                    selectedWidth = this._wavArea.Width;
                }
                else
                {
                    /***************************************************************************
                    * |------------------------------|
                    * |    ss --- ws --- se --- we   |
                    * |------------------------------|
                    ***************************************************************************/
                    selectedWidth = (float)(this._wavArea.Width * (this._se - this._ws)) / (this._we - this._ws);
                }
            }
            else
            {
                /***************************************************************************
                * |------------------------------|
                * |    ws --- ss --- we --- se   |
                * |    ws --- ss --- se --- we   |
                * |------------------------------|
                ***************************************************************************/
                if (this._se >= this._we)
                {
                    if (this._ss <= this._ws)
                    {
                        /***************************************************************************
                        * |------------------------------|
                        * |    ss --- ws --- we --- se   |
                        * |------------------------------|
                        ***************************************************************************/
                        selectedX = this._wavArea.X;
                        selectedWidth = this._wavArea.Width;
                    }
                    else
                    {
                        /***************************************************************************
                        * |------------------------------|
                        * |    ws --- ss --- we --- se   |
                        * |------------------------------|
                        ***************************************************************************/
                        selectedX = this._wavArea.X + (float)(this._wavArea.Width * (this._ss - this._ws)) / (this._we - this._ws);
                        selectedWidth = this._wavArea.Width + this._wavArea.X - selectedX;
                    }
                }
                else
                {
                    /***************************************************************************
                    * |------------------------------|
                    * |    ws --- ss --- se --- we   |
                    * |------------------------------|
                    ***************************************************************************/
                    selectedX = this._wavArea.X + (float)(this._wavArea.Width * (this._ss - this._ws)) / (this._we - this._ws);
                    selectedWidth = (float)(this._wavArea.Width * (this._se - this._ws)) / (this._we - this._ws) - selectedX;
                }
            }
        }

        /// <summary>
        /// 添加局部刷新区域[添加成功返回true,失败返回false]
        /// </summary>
        /// <param name="reg">需要局部刷新的矩形区域</param>
        /// <returns>添加成功返回true,失败返回false</returns>
        private bool AddPartRefreshArea(RectangleF reg)
        {
            if (reg.Width < 0.0000001 || reg.Height < 0.0000001 ||
                reg.X + reg.Width < 0 || reg.X > this.Width ||
                reg.Y + reg.Height < 0 || reg.Y > this.Height)
            {
                //不在当前控件范围内,无视
                return false;
            }

            //新建个矩形用于完全覆盖要更新的区域
            this._partRefreshAreas.Add(new Rectangle((int)reg.X - 1, (int)reg.Y - 1, (int)reg.Width + 3, (int)reg.Height + 3));
            return true;
        }

        /// <summary>
        /// 添加局部刷新区域[添加成功返回true,失败返回false]
        /// </summary>
        /// <param name="reg">需要局部刷新的矩形区域</param>
        /// <returns>添加成功返回true,失败返回false</returns>
        private bool AddPartRefreshArea(Rectangle reg)
        {
            if (reg.Width == 0 || reg.Height == 0 ||
                reg.X + reg.Width < 0 || reg.X > this.Width ||
                reg.Y + reg.Height < 0 || reg.Y > this.Height)
            {
                //不在当前控件范围内,无视
                return false;
            }

            //新建个矩形用于完全覆盖要更新的区域
            //this._partRefreshAreas.Add(new Rectangle(reg.X - 1, reg.Y - 1, reg.Width + 2, reg.Height + 2));
            this._partRefreshAreas.Add(reg);
            return true;
        }

        /// <summary>
        /// 清空局部刷新区域集合
        /// </summary>
        private void ClearPartRefreshArea()
        {
            this._partRefreshAreas.Clear();
        }
    }
}
