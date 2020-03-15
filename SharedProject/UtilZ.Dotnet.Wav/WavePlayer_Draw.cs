using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using UtilZ.Dotnet.Wav.ExBass;
using UtilZ.Dotnet.Wav.Model;

namespace UtilZ.Dotnet.Wav
{
    // WAV播放控件-绘图
    public partial class WavePlayer
    {
        /// <summary>
        /// 刷新波形图
        /// </summary>
        /// <param name="paintZoomWav">是否重绘缩略波形图[true:重绘;false:不重绘]</param>
        /// <param name="paintTime">是否需要重绘时间[true:重绘;false:不重绘]</param>
        /// <param name="paintDb">是否需要重绘幅度[true:重绘;false:不重绘]</param>
        /// <param name="paintLogo">是否绘制Logo</param>
        /// <param name="zoomDataSampleFlag">缩略波形数据是否重采样[true:重采样;false:不重采样]</param>
        /// <param name="wavDataSampleFlag">主波形数据是否重采样[true:重采样;false:不重采样]</param>
        private void RefreshWave(bool paintZoomWav, bool paintTime, bool paintDb, bool paintLogo, bool zoomDataSampleFlag, bool wavDataSampleFlag)
        {
            if (this._grafx == null)
            {
                return;
            }

            try
            {
                Graphics graphics = this._grafx.Graphics;
                //重置平移
                graphics.ResetTransform();

                //清空所有已绘制的图形
                graphics.Clear(this._backgroudColor);

                //如果数据他要0.则清空
                if (this._srcLeftChannelDataLength == 0)
                {
                    this.Refresh();
                    return;
                }

                //重采样后的数据个数
                int targetCount = this._zoomArea.Width * this._qualityCoefficient;
                if (zoomDataSampleFlag)
                {
                    //缩略波形声道数据重采样
                    this.ChannelSampleDecrease(this._isStereo, targetCount, this._srcSampleLeftData, this._srcSampleRightData, 0, -1, ref this._zoomLeftData, ref this._zoomRightData);
                }

                //声道数据重采样
                if (wavDataSampleFlag)
                {
                    long begeinIndex = 0;
                    long endIndex = 0;
                    if (this._srcData != null)
                    {
                        int dataLength = this._srcSampleLeftData.Length;
                        begeinIndex = (this._ws * dataLength) / this._srcLeftChannelDataLength;
                        endIndex = (this._we * dataLength) / this._srcLeftChannelDataLength;
                    }

                    this.ChannelSampleDecrease(this._isStereo, targetCount, this._srcSampleLeftData, this._srcSampleRightData, begeinIndex, endIndex, ref this._leftData, ref this._rightData);
                }

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

                //绘制缩略波形
                if (paintZoomWav)
                {
                    //重置平移
                    graphics.ResetTransform();

                    //填充缩略波形背景
                    if (this._enableZoomWavBackground)
                    {
                        graphics.FillRectangle(this._zoomBackgroundBrush, this._zoomArea);
                    }

                    //平移
                    graphics.TranslateTransform(this._zoomArea.X, this._zoomArea.Y, MatrixOrder.Prepend);

                    //绘制缩略波形图背景
                    this.DrawZoomWavBackgroud(graphics);

                    //绘制波形
                    this.DrawWav(this._isStereo, this._isMergeChanel, this._zoomArea, graphics, this._zoomLeftData, this._zoomRightData, this._zoomLeftChannelPen, this._zoomRightChannelPen, this._isDrawWavMidLine, this._wavMidLinePen, this._isDrawChannelDivideLine, this._channelDivideLinePen);

                    //绘制显示区域样式
                    this.DrawDisplayAreaStyle(graphics);
                }

                //绘制时间
                if (paintTime)
                {
                    this.DrawTime(graphics, this._timeBackgroundBrush, this._enableTimeBackground, this._timeArea, this._ws, this._we, this._channelInfo.chans, this._durationTime, this._srcLeftChannelDataLength, this._timePen, this._fontBrush, this._stringFont);
                }

                //绘制Logo
                //if (paintLogo)
                //{
                //    this.DrawLogo(graphics);
                //}
                //this.DrawLogo(graphics);


                //这个地方有个小问题,暂时没有找着原因,就是明明不需要绘制幅度的时候,不绘制幅度幅度区域会出现横向线条,只增不减,没搞懂原因//求那位大神解决下,目前就每次都绘制幅度区域了
                //绘制幅度
                //if (paintDb)
                //{
                this.DrawDb(graphics, this._isMergeChanel);
                //}

                //重置平移
                graphics.ResetTransform();
                //填充主波形背景
                if (this._enableWavAreaBackground)
                {
                    graphics.FillRectangle(this._wavAreaBackgroundBrush, this._wavArea);
                }

                //填充主波形选中背景
                graphics.FillRectangle(this._seleactionAreaBrush, this._wavSelectedArea);

                //绘制主波形图
                if (this._isMergeChanel)
                {
                    //合并左右声道波形
                    this.DrawWav(this._isStereo, true, this._wavArea, graphics, this._leftData, this._rightData, this._leftChannelPen, this._rightChannelPen, this._isDrawWavMidLine, this._wavMidLinePen, this._isDrawChannelDivideLine, this._channelDivideLinePen);
                }
                else
                {
                    //分离左右声道波形
                    this.DrawWav(this._isStereo, false, this._wavArea, graphics, this._leftData, this._rightData, this._leftChannelPen, this._rightChannelPen, this._isDrawWavMidLine, this._wavMidLinePen, this._isDrawChannelDivideLine, this._channelDivideLinePen);
                }

                //绘制播放位置指示线
                this.DrawPlayLocationLine(graphics);

                //刷新
                if (paintZoomWav && paintTime && paintDb && paintLogo || this._partRefreshAreas.Count == 0)
                {
                    //刷新控件
                    this.Refresh();
                }
                else
                {
                    //局部刷新
                    foreach (Rectangle reg in this._partRefreshAreas)
                    {
                        this.Invalidate(reg);
                    }

                    //清空局部刷新区域集合
                    this.ClearPartRefreshArea();
                    //更新区域
                    this.Update();
                }
            }
            catch (Exception ex)
            {
                OnRaiseLog(ex);
            }
        }

        /// <summary>
        /// 绘制播放位置指示线
        /// </summary>
        /// <param name="graphics">Graphics</param>
        private void DrawPlayLocationLine(Graphics graphics)
        {
            //重置平移
            graphics.ResetTransform();

            //缩略波形图播放位置指示线
            PointF zoomP1 = new PointF(this._zoomPlayLineX, this._zoomArea.Y);
            PointF zoomP2 = new PointF(this._zoomPlayLineX, this._zoomArea.Y + this._zoomArea.Height);
            graphics.DrawLine(this._playLineChannelPen, zoomP1, zoomP2);

            //主波形图播放位置指示线
            PointF wavP1 = new PointF(this._wavPlayLineX, this._wavArea.Y);
            PointF wavP2 = new PointF(this._wavPlayLineX, this._wavArea.Y + this._wavArea.Height);
            graphics.DrawLine(this._playLineChannelPen, wavP1, wavP2);
        }

        /// <summary>
        /// 绘制幅度
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="isMergeChanel">波形是否合并</param>
        private void DrawDb(Graphics graphics, bool isMergeChanel)
        {
            //重置平移
            graphics.ResetTransform();

            //幅度波形图背景
            if (this._enableDbAreaBackground)
            {
                graphics.FillRectangle(this._dbBackgroundBrush, this._dbArea);
            }

            //绘制幅度线
            graphics.DrawLine(this._dbPen, this._dbArea.X, this._dbArea.Y, this._dbArea.X, this._dbArea.Y + this._dbArea.Height);

            //平移
            graphics.TranslateTransform(this._dbArea.X, this._dbArea.Y, MatrixOrder.Prepend);

            /********************
            * ____ 33000
            * |_   
            * |___ 0
            * |_   
            * |___ +-33000
            * |_
            * |___ 0
            * |_
            * |___ -3300 
            ********************/

            //幅度频率最大值frequency
            int dbMax = 33000;
            int dividedMaxCount = 5;//最多刻度数
            float dividedHeight = 40f;//每一段的刻度的高度
            int drawCount;//幅度绘制次数
            float height;
            if (isMergeChanel)
            {
                height = (float)this._dbArea.Height - this._dbPen.Width;
                drawCount = 1;
            }
            else
            {
                height = (float)this._dbArea.Height / 2 - this._dbPen.Width;
                drawCount = 2;
            }

            float width = (float)this._dbArea.Width;
            float fontHeignt = graphics.MeasureString("0", this._stringFont).Height;
            float minusWidth = graphics.MeasureString("-", this._stringFont).Width - 4;
            float fontDividedSpace = 2f;//标量与刻度之间的距离
            float x1 = 0, minX2 = 5f, maxX = 10f, y;//刻度坐标值
            float fx1 = maxX + fontDividedSpace, fx2 = fx1 + minusWidth, fy;//刻度值坐标
            float zeroP = height / 2;//圆点位置的Y坐标     
            int dividedCount = (int)Math.Ceiling(zeroP / dividedHeight);//刻度数
            if (dividedCount > dividedMaxCount)
            {
                dividedCount = dividedMaxCount;
            }

            //重新计算每个大刻度的高度
            dividedHeight = zeroP / dividedCount;
            float hasfDividedHeight = zeroP / (dividedCount * 2);//小刻度高度
            int divideValue;
            string divideValueStr;//刻度值字符串
            string channelType = "L";//声道标识
            float zeroDivdideWidth = graphics.MeasureString("0", this._stringFont).Width;//0刻度值宽度

            //绘制第一个大刻度
            y = 0;
            graphics.DrawLine(this._dbPen, x1, y, maxX, y);

            for (int i = 0; i < drawCount; i++)
            {
                //绘制第一个大刻度刻度值
                divideValueStr = dbMax.ToString();
                fy = 3;
                graphics.DrawString(divideValueStr, this._stringFont, this._fontBrush, fx2, fy);

                for (float j = 0; j < dividedCount; j++)
                {
                    //正小刻度
                    y = dividedHeight * j + hasfDividedHeight;
                    graphics.DrawLine(this._dbPen, x1, y, minX2, y);

                    //负小刻度
                    y = height - y;
                    graphics.DrawLine(this._dbPen, x1, y, minX2, y);

                    //正大刻度
                    y = dividedHeight * (j + 1);
                    graphics.DrawLine(this._dbPen, x1, y, maxX, y);

                    //正大刻度值
                    divideValue = (int)(dbMax * (1 - (j + 1) / dividedCount));//刻度值
                    divideValueStr = divideValue.ToString();
                    fy = y - fontHeignt / 2;
                    graphics.DrawString(divideValueStr, this._stringFont, this._fontBrush, fx2, fy);

                    //绘制声道标识,当左右声道 波形为不合并时,且刻度值为0时才绘制
                    if (divideValue == 0 && !isMergeChanel)
                    {
                        graphics.DrawString(channelType, this._stringFont, this._fontBrush, fx2 + zeroDivdideWidth, fy);
                    }

                    //负大刻度
                    y = height - y;
                    graphics.DrawLine(this._dbPen, x1, y, maxX, y);

                    //负大刻度值
                    divideValueStr = (0 - divideValue).ToString();
                    if (j == dividedCount - 1)
                    {
                        fy = height - fontHeignt;

                        //负大刻度
                        y = height;
                        graphics.DrawLine(this._dbPen, x1, y, maxX, y);
                        divideValueStr = (0 - dbMax).ToString();
                    }
                    else
                    {
                        fy = y - fontHeignt / 2;
                    }

                    graphics.DrawString(divideValueStr, this._stringFont, this._fontBrush, fx1, fy);
                }

                //如果非合并声道波形,则平移继续绘制右声道波形幅度
                if (!isMergeChanel && i == 0)
                {
                    //绘制分隔线
                    graphics.DrawLine(this._dbPen, 0, y, width, y);

                    //平移一个高度
                    graphics.TranslateTransform(0, height);

                    channelType = "R";
                }
            }
        }

        /// <summary>
        /// 绘制Logo
        /// </summary>
        /// <param name="graphics">Graphics</param>
        private void DrawLogo(Graphics graphics)
        {
            //重置平移
            graphics.ResetTransform();

            //绘制Logo区域背景
            if (this._enableLogoAreaBackground)
            {
                graphics.FillRectangle(this._logoBackgroundBrush, this._logoArea);
            }

            //..以后再来想绘制的内容,要不就绘制个屁吧..哈哈哈
            //决定了绘制频谱吧
            //心情不好,不绘了,以后再说吧..反正这个只是打酱油的

            //var totalLength = Bass.BASS_ChannelGetLength(this._handle, BASSMode.BASS_POS_BYTE);
            //var time = Bass.BASS_ChannelBytes2Seconds(this._handle, totalLength);//时间,秒
            //time = Math.Round(time, 3);

            short[] data = new short[32];
            int count = Bass.BASS_ChannelGetData(this._handle, data, (uint)data.Length);
            if (count == -1)
            {
                return;
            }

            count = count / WavePlayer.OFFSETPARA;
            var srcData = new short[count];
            Array.Copy(data, 0, srcData, 0, srcData.Length);
            short[] leftData;
            short[] rightData;
            if (this._isStereo)
            {
                uint channelDataLength = (uint)(count / 2);
                leftData = new short[channelDataLength];
                rightData = new short[channelDataLength];
                int position = 0;
                for (int i = 0; i < srcData.Length; i += 2)
                {
                    leftData[position] = srcData[i];
                    rightData[position] = srcData[i + 1];
                    position++;
                }
            }
            else
            {
                leftData = srcData;
                rightData = null;
            }

            //区域宽度减去6是为了频谱的左边距和右边距及两个通道的频谱的间隔分别为2个像素
            /*********
             |-----|
             | * * |
             | * * |
             |_*_*_|
             *********/
            float spectrumWidth = ((float)this._logoArea.Width - 6) / 2;

        }

        /// <summary>
        /// 绘制显示区域样式
        /// </summary>
        /// <param name="graphics">Graphics</param>
        private void DrawDisplayAreaStyle(Graphics graphics)
        {
            //重置平移
            graphics.ResetTransform();
            //平移
            graphics.TranslateTransform(this._zoomArea.X, this._zoomArea.Y, MatrixOrder.Prepend);
            //绘制边框
            graphics.DrawRectangle(this._zoomDisplayAreaPen, this._zoomDisplayArea.X, this._zoomDisplayArea.Y, this._zoomDisplayArea.Width, this._zoomDisplayArea.Height);

            //绘制内部细线
            float zoomDAEndX = this._zoomDisplayArea.X + this._zoomDisplayArea.Width;
            float zoomDABeginX = this._zoomDisplayArea.X;
            float zoomDAY1 = this._zoomDisplayArea.Y;
            float zoomDAY2 = zoomDAY1 + this._zoomDisplayArea.Height;
            int interval = 2;
            if (this._zoomDisplayArea.Width > 20)
            {
                //左边竖线
                this.DrawVerLine(graphics, zoomDABeginX + interval, zoomDABeginX + 10, zoomDAY1, zoomDAY2, this._zoomDisplayAreaPen, interval);
                //右边竖线
                this.DrawVerLine(graphics, zoomDAEndX - 8, zoomDAEndX - 1, zoomDAY1, zoomDAY2, this._zoomDisplayAreaPen, interval);
            }
            else
            {
                //宽度小于20就左右边一起了
                this.DrawVerLine(graphics, zoomDABeginX + interval, zoomDAEndX, zoomDAY1, zoomDAY2, this._zoomDisplayAreaPen, interval);
            }
        }

        /// <summary>
        /// 绘制垂直线
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="beginX">起始X</param>
        /// <param name="endX">结束X</param>
        /// <param name="beginY">起始Y</param>
        /// <param name="endY">结束Y</param>
        /// <param name="pen">画笔</param>
        /// <param name="interval">像素间隔</param>
        private void DrawVerLine(Graphics graphics, float beginX, float endX, float beginY, float endY, Pen pen, int interval)
        {
            PointF zoomDAP1, zoomDAP2;
            if (endX - beginX < 1)//如果一个点都没有则至少绘制一条线
            {
                zoomDAP1 = new PointF(beginX, beginY);
                zoomDAP2 = new PointF(beginX, endY);
                graphics.DrawLine(this._zoomDisplayAreaPen, zoomDAP1, zoomDAP2);
            }
            else
            {
                for (float x = beginX; x < endX; x += interval)
                {
                    zoomDAP1 = new PointF(x, beginY);
                    zoomDAP2 = new PointF(x, endY);
                    graphics.DrawLine(this._zoomDisplayAreaPen, zoomDAP1, zoomDAP2);
                }
            }
        }

        /// <summary>
        /// 绘制缩略波形图背景
        /// </summary>
        /// <param name="graphics">Graphics</param>
        private void DrawZoomWavBackgroud(Graphics graphics)
        {
            if (this._zoomSelectedArea.Width > 0)
            {
                float y = this._zoomDisplayArea.Y;
                float height = this._zoomDisplayArea.Height;
                float ssX = this._zoomSelectedArea.X;//显示波形区域在缩略波形中的对应的区域中的选中区域起始X(选中区域起始X)
                float seX = ssX + this._zoomSelectedArea.Width;//显示波形区域在缩略波形中的对应的区域中的选中区域结束X(选中区域结束X)
                float dsX = this._zoomDisplayArea.X;//显示波形区域在缩略波形中的对应的区域中的选中区域起始X(显示区域起始X)
                float deX = dsX + this._zoomDisplayArea.Width;//显示波形区域在缩略波形中的对应的区域中的选中区域结束X(显示区域结束X)

                if (ssX < dsX)
                {
                    #region
                    if (seX < dsX)
                    {
                        /***************************************************************************
                         * 选中区域起始于显示区域起之前,也结束于显示区域起之前
                         * |--ssX---seX---dsX---deX--|
                         ***************************************************************************/

                        //显示波形区域在缩略波形中的对应的区域背景
                        graphics.FillRectangle(this._zoomDisplayAreaBrush, this._zoomDisplayArea);

                        //显示波形区域在缩略波形中的对应的区域中的选中区域背景
                        graphics.FillRectangle(this._zoomSelectedBackgroundBrush, this._zoomSelectedArea);
                    }
                    else if (seX > deX)
                    {
                        /***************************************************************************
                         * 选中区域起始于显示区域起之前,结束于显示区域起之后
                         * |--ssX---dsX---deX---seX--|
                         ***************************************************************************/

                        //显示波形区域在缩略波形中的对应的区域中的选中区域背景
                        graphics.FillRectangle(this._zoomSelectedBackgroundBrush, this._zoomSelectedArea);

                        //显示波形区域在缩略波形中的对应的区域背景
                        graphics.FillRectangle(this._zoomDisplaySelectedOverlapAreaBrush, this._zoomDisplayArea);
                    }
                    else
                    {
                        /***************************************************************************
                         * 选中区域起始于显示区域起之前,也结束于显示区域起之前
                         * |--ssX---dsX---seX---deX--|
                         ***************************************************************************/

                        //重叠左边区域背景
                        RectangleF regLeftArea = new RectangleF(ssX, y, dsX - ssX, height);
                        graphics.FillRectangle(this._zoomSelectedBackgroundBrush, regLeftArea);

                        //重叠区域背景
                        RectangleF regOverlapArea = new RectangleF(dsX, y, seX - dsX, height);
                        if (regOverlapArea.Width > 0)
                        {
                            graphics.FillRectangle(this._zoomDisplaySelectedOverlapAreaBrush, regOverlapArea);
                        }

                        //重叠区域右边背景
                        RectangleF regRightArea = new RectangleF(seX, y, deX - seX, height);
                        if (regRightArea.Width > 0)
                        {
                            graphics.FillRectangle(this._zoomDisplayAreaBrush, regRightArea);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region
                    if (ssX > deX)
                    {
                        /***************************************************************************
                        * 选中区域起始于显示区域起之后,结束于显示区域起之后
                        * |--dsX---deX---bX---seX--|
                        ***************************************************************************/

                        //显示波形区域在缩略波形中的对应的区域背景
                        graphics.FillRectangle(this._zoomDisplayAreaBrush, this._zoomDisplayArea);

                        //显示波形区域在缩略波形中的对应的区域中的选中区域背景
                        graphics.FillRectangle(this._zoomSelectedBackgroundBrush, this._zoomSelectedArea);
                    }
                    else if (seX > deX)
                    {
                        /***************************************************************************
                        * 选中区域起始于显示区域起之后,结束于显示区域起之后
                        * |--dsX---ssX---deX---seX--|
                        ***************************************************************************/

                        //重叠左边区域背景
                        RectangleF regLeftArea = new RectangleF(dsX, y, ssX - dsX, height);
                        graphics.FillRectangle(this._zoomDisplayAreaBrush, regLeftArea);

                        //重叠区域背景
                        RectangleF regOverlapArea = new RectangleF(ssX, y, deX - ssX, height);
                        if (regOverlapArea.Width > 0)
                        {
                            graphics.FillRectangle(this._zoomDisplaySelectedOverlapAreaBrush, regOverlapArea);
                        }

                        //重叠区域右边背景
                        RectangleF regRightArea = new RectangleF(deX, y, seX - deX, height);
                        if (regRightArea.Width > 0)
                        {
                            graphics.FillRectangle(this._zoomSelectedBackgroundBrush, regRightArea);
                        }
                    }
                    else
                    {
                        /***************************************************************************
                        * 选中区域起始于显示区域起之后,结束于显示区域起之后
                        * |--dsX---ssX---seX---deX--|
                        ***************************************************************************/

                        //显示波形区域在缩略波形中的对应的区域背景
                        graphics.FillRectangle(this._zoomDisplayAreaBrush, this._zoomDisplayArea);

                        //显示波形区域在缩略波形中的对应的区域中的选中区域背景
                        graphics.FillRectangle(this._zoomDisplaySelectedOverlapAreaBrush, this._zoomSelectedArea);
                    }
                    #endregion
                }
            }
            else
            {
                //显示波形区域在缩略波形中的对应的区域背景
                graphics.FillRectangle(this._zoomDisplayAreaBrush, this._zoomDisplayArea);
            }
        }

        /// <summary>
        /// 绘制时间
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="timeBackgroundBrush">时间背景画刷</param>
        /// <param name="timeArea">时间区域</param>
        /// <param name="ws">当前绘制的主波形数据起始索引,对应的数据为原始数据</param>
        /// <param name="we">当前绘制的主波形数据结束索引,对应的数据为原始数据</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="totalDurationTime">当前文件持续时长</param>
        /// <param name="srcLeftChannelDataLength">原始左声道数据长度</param>
        /// <param name="pen">刻度画笔</param>
        /// <param name="fontBrush">标量字体笔刷</param>
        /// <param name="font">标量字体</param>
        private void DrawTime_bk2(Graphics graphics, Brush timeBackgroundBrush, Rectangle timeArea, long ws, long we, int channelCount, double totalDurationTime, int srcLeftChannelDataLength, Pen pen, Brush fontBrush, Font font)
        {
            //时间图背景
            graphics.ResetTransform();
            graphics.FillRectangle(timeBackgroundBrush, timeArea);

            //平移
            graphics.TranslateTransform(timeArea.X, timeArea.Y, MatrixOrder.Prepend);

            //绘制时间
            double ts = totalDurationTime * ws / srcLeftChannelDataLength;
            double te = totalDurationTime * we / srcLeftChannelDataLength;
            //this.DrawTimeLine(graphics, timeArea, ts, te, pen, fontBrush, font);
            //return;
            float timePrecision = 0.000001f;//时间精度值
            float durationTime = (float)(te - ts);
            if (durationTime < timePrecision)
            {
                //如果持续时间小于等于0,则不绘制
                return;
            }

            float width = (float)timeArea.Width;
            float height = (float)timeArea.Height;
            float divideMinWidth = 30f;//时刻刻度最小宽度值
            float divideTime;//每个刻度对应的时间
            float divideCount;//大刻度个数
            float hasfDividedWidth;//半个大刻度的宽度,没有使用大刻度宽度除2,是为了提高精度
            float dividedWidth = width / durationTime;//一段大刻度宽度
            if (dividedWidth - divideMinWidth < timePrecision)
            {
                //如果计算出的刻度宽度小于最小刻度宽度,则使用最小刻度宽度
                dividedWidth = divideMinWidth;
                hasfDividedWidth = dividedWidth / 2;

                //每个刻度对应的时间
                divideTime = durationTime * dividedWidth / width;

                //大刻度个数
                divideCount = width / dividedWidth;
                if (width % dividedWidth != 0)
                {
                    divideCount++;
                }
            }
            else
            {
                hasfDividedWidth = width / (durationTime * 2);

                //每个刻度对应的时间
                //divideTime = durationTime * dividedWidth / width;
                //divideTime = durationTime * (width / durationTime) / width;
                divideTime = 1;

                //大刻度个数
                //divideCount = (int)Math.Ceiling(width / (width / durationTime));
                divideCount = (int)Math.Ceiling(durationTime);
            }

            int fontDividedSpace = 4;//标量与刻度之间的距离          
            int dividedMaxHeight = 10;//大刻度的高度
            int dividedMinHeight = 5;//小刻度的高度 
            float minY = height - dividedMinHeight - fontDividedSpace;//小刻度起始Y
            float maxY = height - dividedMaxHeight - fontDividedSpace;//大刻度起始Y
            string divideValueStr;//刻度值字符串

            int timeDivideDigits = 2;//时刻刻度值保留的小数位数
            float time = (float)Math.Round(ts, timeDivideDigits);//大刻度的刻度值
            int preDividedLength = time.ToString().Length;//前一次刻度值长度
            int fontScale = 3;//刻度值字体偏移倍数
            float fontPositionDisplaced = preDividedLength * font.SizeInPoints / fontScale;//字体偏左移量
            float fx, fy = 1;//刻度值坐标
            float x = 0;//刻度坐标

            //画时间线横线
            graphics.DrawLine(pen, 0, height, width, height);

            //第一条竖线            
            x = (float)(dividedWidth * (ts - (int)ts));//刻度左边点坐标
            if (x > width)
            {
                //如果超出UI范围,则退出时间绘制
                return;
            }

            /*****************************************************************
             * 
             *      |       |
             *__|___|___|___|___|______________________________________________
             *****************************************************************/
            if (x > hasfDividedWidth)
            {
                //如果第一个大刻度线的X大于等于刻度宽度值的一半,则大刻度线的左边还有一个小刻度
                float x1 = x - hasfDividedWidth;
                graphics.DrawLine(pen, x1, height, x1, minY);
            }

            //绘制第一个大刻度竖线
            graphics.DrawLine(pen, x, height, x, maxY);

            //第一条大刻竖线对应的刻度值
            fx = x - fontPositionDisplaced;
            if (fx < 0)
            {
                fx = -2;
            }

            divideValueStr = time.ToString();
            graphics.DrawString(divideValueStr, font, fontBrush, fx, fy);

            //绘制余下的时间刻度及刻度值
            int currentDividedLength;
            SizeF divideValueStrSize;
            for (int i = 1; i < divideCount; i++)
            {
                //小刻度
                x += hasfDividedWidth;
                if (x + pen.Width > width)
                {
                    return;
                }

                graphics.DrawLine(pen, x, minY, x, height);

                //大刻度
                x += hasfDividedWidth;
                if (x + pen.Width > width)
                {
                    return;
                }

                graphics.DrawLine(pen, x, maxY, x, height);

                //当前的刻度值字符串                
                divideValueStr = Math.Round((ts + divideTime * i), timeDivideDigits).ToString();
                currentDividedLength = divideValueStr.Length;
                if (currentDividedLength > preDividedLength)
                {
                    preDividedLength = currentDividedLength;
                    fontPositionDisplaced = preDividedLength * font.Size / fontScale;
                }

                //微调时间数值的位置
                fx = x - fontPositionDisplaced;
                //测量刻度值所点的宽度
                //TextRenderer.MeasureText(divideValueStr, font);
                divideValueStrSize = graphics.MeasureString(divideValueStr, font);
                if (fx + divideValueStrSize.Width > width)
                {
                    //如果刻度值字符串绘制到了时间区域之外,则不再绘制
                    return;
                }

                graphics.DrawString(divideValueStr, font, fontBrush, fx, fy);
            }
        }

        /// <summary>
        /// 绘制时间
        /// </summary>
        /// <param name="graphics">Graphics</param>
        /// <param name="timeBackgroundBrush">时间背景画刷</param>
        /// <param name="enableTimeBackground">是否启用时间背景</param>
        /// <param name="timeArea">时间区域</param>
        /// <param name="ws">当前绘制的主波形数据起始索引,对应的数据为原始数据</param>
        /// <param name="we">当前绘制的主波形数据结束索引,对应的数据为原始数据</param>
        /// <param name="channelCount">声道数</param>
        /// <param name="totalDurationTime">当前文件持续时长</param>
        /// <param name="srcLeftChannelDataLength">原始左声道数据长度</param>
        /// <param name="pen">刻度画笔</param>
        /// <param name="fontBrush">标量字体笔刷</param>
        /// <param name="font">标量字体</param>
        private void DrawTime(Graphics graphics, Brush timeBackgroundBrush, bool enableTimeBackground, Rectangle timeArea, long ws, long we, int channelCount, double totalDurationTime, int srcLeftChannelDataLength, Pen pen, Brush fontBrush, Font font)
        {
            //重置平移
            graphics.ResetTransform();

            //时间图背景
            if (enableTimeBackground)
            {
                graphics.FillRectangle(timeBackgroundBrush, timeArea);
            }

            //平移
            graphics.TranslateTransform(timeArea.X, timeArea.Y, MatrixOrder.Prepend);

            //绘制时间
            double ts = totalDurationTime * ws / srcLeftChannelDataLength;
            double te = totalDurationTime * we / srcLeftChannelDataLength;
            //this.DrawTimeLine(graphics, timeArea, ts, te, pen, fontBrush, font);
            //return;

            float durationTime = (float)(te - ts);
            if (durationTime < WavePlayer.PRECISION)
            {
                //如果持续时间小于等于0,则不绘制
                return;
            }

            float width = (float)timeArea.Width;
            float height = (float)(timeArea.Height - Math.Ceiling(pen.Width));//此处减Math.Ceiling(pen.Width)是因为画笔会有个宽度,加上这个宽度后会绘制到主波形区域
            float divideMinWidth = 30f;//时刻刻度最小宽度值
            float divideTime;//每个刻度对应的时间
            float divideCount;//大刻度个数
            float hasfDividedWidth;//半个大刻度的宽度,没有使用大刻度宽度除2,是为了提高精度
            float dividedWidth = width / durationTime;//一段大刻度宽度
            if (dividedWidth - divideMinWidth < WavePlayer.PRECISION)
            {
                //如果计算出的刻度宽度小于最小刻度宽度,则使用最小刻度宽度
                dividedWidth = divideMinWidth;
                hasfDividedWidth = dividedWidth / 2;

                //每个刻度对应的时间
                divideTime = durationTime * dividedWidth / width;

                //大刻度个数
                divideCount = width / dividedWidth;
                //if (width % dividedWidth != 0)
                //{
                //    divideCount++;
                //}

                var tt = divideCount * divideTime;
            }
            else
            {
                hasfDividedWidth = width / (durationTime * 2);

                //每个刻度对应的时间
                //divideTime = durationTime * dividedWidth / width;
                //divideTime = durationTime * (width / durationTime) / width;
                divideTime = 1;

                //大刻度个数
                //divideCount = (int)Math.Ceiling(width / (width / durationTime));
                divideCount = (int)Math.Ceiling(durationTime);
            }

            int fontDividedSpace = 4;//标量与刻度之间的距离          
            int dividedMaxHeight = 10;//大刻度的高度
            int dividedMinHeight = 5;//小刻度的高度 
            float minY = height - dividedMinHeight - fontDividedSpace;//小刻度起始Y
            float maxY = height - dividedMaxHeight - fontDividedSpace;//大刻度起始Y
            string divideValueStr;//刻度值字符串

            int timeDivideDigits = 1;//时刻刻度值保留的小数位数
            float time = (float)Math.Round(ts, timeDivideDigits);//大刻度的刻度值
            int preDividedLength = time.ToString().Length;//前一次刻度值长度
            int fontScale = 3;//刻度值字体偏移倍数
            float fontPositionDisplaced = preDividedLength * font.SizeInPoints / fontScale;//字体偏左移量
            float fx, fy = 1;//刻度值坐标
            float x = 0;//刻度坐标

            //画时间线横线
            graphics.DrawLine(pen, 0, height, width, height);

            //绘制第一条大刻度竖线
            graphics.DrawLine(pen, x, maxY, x, height);

            //第一条大刻竖线对应的刻度值
            fx = x - fontPositionDisplaced;
            if (fx < 0)
            {
                fx = -2;
            }

            divideValueStr = time.ToString();
            graphics.DrawString(divideValueStr, font, fontBrush, fx, fy);

            //绘制余下的时间刻度及刻度值
            int currentDividedLength;
            SizeF divideValueStrSize;
            for (int i = 0; i < divideCount; i++)
            {
                //小刻度
                x += hasfDividedWidth;
                if (x + pen.Width > width)
                {
                    return;
                }

                graphics.DrawLine(pen, x, minY, x, height);

                //大刻度
                x += hasfDividedWidth;
                if (x + pen.Width > width)
                {
                    return;
                }

                graphics.DrawLine(pen, x, maxY, x, height);

                //当前的刻度值字符串                
                divideValueStr = Math.Round((ts + divideTime * (i + 1)), timeDivideDigits).ToString();
                currentDividedLength = divideValueStr.Length;
                if (currentDividedLength > preDividedLength)
                {
                    preDividedLength = currentDividedLength;
                    fontPositionDisplaced = preDividedLength * font.Size / fontScale;
                }

                //微调时间数值的位置
                fx = x - fontPositionDisplaced;

                //测量刻度值所点的宽度
                //TextRenderer.MeasureText(divideValueStr, font);
                divideValueStrSize = graphics.MeasureString(divideValueStr, font);
                if (fx + divideValueStrSize.Width > width)
                {
                    //如果刻度值字符串绘制到了时间区域之外,则向左微调
                    fx = fx - 2;
                    if (fx + divideValueStrSize.Width > width)
                    {
                        //如果刻度值字符串绘制到了时间区域之外,则不再绘制
                        return;
                    }
                }

                graphics.DrawString(divideValueStr, font, fontBrush, fx, fy);
            }
        }

        /// <summary>
        /// 绘制波形
        /// </summary>
        /// <param name="isStereo">是否是立体声</param>
        /// <param name="isMergeChannel">是否合并左右声道</param>
        /// <param name="wavArea">波形绘制区域</param>
        /// <param name="graphics">Graphics</param>
        /// <param name="leftData">左声道数据</param>
        /// <param name="rightData">右声道数据</param>
        /// <param name="leftChannelPen">左声道画笔</param>
        /// <param name="rightChannelPen">右声道画笔</param>
        /// <param name="isDrawWavMidLine">是否绘制波形中线</param>
        /// <param name="wavMidLinePen">波形中线画笔</param>
        /// <param name="isDrawChannelDivideLine">是否绘制声道分隔线</param>
        /// <param name="channelDivideLinePen">声道分隔线画笔</param>
        private void DrawWav(bool isStereo, bool isMergeChannel, Rectangle wavArea, Graphics graphics, short[] leftData, short[] rightData, Pen leftChannelPen, Pen rightChannelPen, bool isDrawWavMidLine, Pen wavMidLinePen, bool isDrawChannelDivideLine, Pen channelDivideLinePen)
        {
            #region 参数检查
            if (isStereo)
            {
                //如果左右声道数据都为空或长度为0,则返回
                if (leftData == null || leftData.Length == 0)
                {
                    if (rightData == null || rightData.Length == 0)
                    {
                        return;
                    }
                    else
                    {
                        leftData = new short[rightData.Length];
                    }
                }
                else if (rightData == null || rightData.Length == 0)
                {
                    rightData = new short[leftData.Length];
                }
            }
            else
            {
                //如果左右声道数据都为空或长度为0,则返回
                if (leftData == null || leftData.Length == 0)
                {
                    if (rightData == null || rightData.Length == 0)
                    {
                        return;
                    }
                    else
                    {
                        leftData = rightData;
                    }
                }
            }
            #endregion

            int dataLength = leftData.Length;
            float width = wavArea.Width;
            float height = wavArea.Height;
            float spacing = width / (dataLength - 1);//两个点之间的间隔,减1是因为线段数比点数少1,两个点组成一条线
            if (dataLength == 0)
            {
                return;
            }
            else if (dataLength == 1)
            {
                spacing = width;
            }
            else
            {
                spacing = width / (dataLength - 1);//两个点之间的间隔,减1是因为线段数比点数少1,两个点组成一条线
            }

            float zoom;
            float offsetGraphics;

            float lx = 0 - spacing, ly = 0;//左声道坐标
            PointF[] leftDataPoints = new PointF[dataLength];
            PointF[] rightDataPoints;

            graphics.ResetTransform();
            //平移波形绘制区域到原点坐标
            graphics.TranslateTransform(wavArea.X, wavArea.Y, MatrixOrder.Prepend);

            if (isMergeChannel)
            {
                offsetGraphics = height / 2;
                zoom = short.MaxValue / offsetGraphics;

                //向上平移画布
                graphics.TranslateTransform(0, offsetGraphics, MatrixOrder.Prepend);

                //绘波形中线
                if (isDrawWavMidLine)
                {
                    graphics.DrawLine(wavMidLinePen, 0, 0, width, 0);
                }

                if (isStereo)
                {
                    rightDataPoints = new PointF[dataLength];
                    float rx = 0 - spacing, ry = 0;//右声道坐标
                    for (int i = 0; i < dataLength; i++)
                    {
                        //绘制左声道波形
                        lx = lx + spacing;
                        ly = leftData[i] / zoom;
                        leftDataPoints[i] = new PointF(lx, ly);

                        //绘制右声道的波形
                        rx = rx + spacing;
                        ry = rightData[i] / zoom;
                        rightDataPoints[i] = new PointF(rx, ry);
                    }

                    graphics.DrawLines(leftChannelPen, leftDataPoints);
                    graphics.DrawLines(rightChannelPen, rightDataPoints);

                    //GraphicsPath graphicsPath = new GraphicsPath();
                    //graphicsPath.AddLines(leftDataPoints);
                    //graphics.DrawPath(leftChannelPen, graphicsPath);
                    //graphicsPath.Dispose();
                }
                else
                {
                    for (int i = 0; i < dataLength; i++)
                    {
                        //绘制左声道波形
                        lx = lx + spacing;
                        ly = leftData[i] / zoom;
                        leftDataPoints[i] = new PointF(lx, ly);
                    }

                    graphics.DrawLines(leftChannelPen, leftDataPoints);
                }
            }
            else
            {
                float leftChanelOffsetGraphics = height / 4;
                float rightChanelOffsetGraphics = height / 2;//因为左声道先移了1/4,所以此处只需要再移1/2,总共移3/4
                float midLineOffsetGraphics;
                //向上平移画布1/4
                graphics.TranslateTransform(0, leftChanelOffsetGraphics, MatrixOrder.Prepend);

                zoom = short.MaxValue / leftChanelOffsetGraphics;
                leftDataPoints = new PointF[dataLength];
                if (isStereo)
                {
                    rightDataPoints = new PointF[dataLength];
                    float rx = 0 - spacing, ry = 0;//右声道坐标
                    for (int i = 0; i < dataLength; i++)
                    {
                        //绘制左声道波形
                        lx = lx + spacing;
                        ly = leftData[i] / zoom;
                        leftDataPoints[i] = new PointF(lx, ly);

                        //绘制右声道的波形
                        rx = rx + spacing;
                        ry = rightData[i] / zoom;
                        rightDataPoints[i] = new PointF(rx, ry);
                    }

                    //绘左声道中线
                    if (isDrawWavMidLine)
                    {
                        graphics.DrawLine(wavMidLinePen, 0, 0, width, 0);
                    }

                    //绘制左声道波形
                    graphics.DrawLines(leftChannelPen, leftDataPoints);

                    //向上平移画布1/2
                    graphics.TranslateTransform(0, rightChanelOffsetGraphics, MatrixOrder.Prepend);

                    //绘右声道中线
                    if (isDrawWavMidLine)
                    {
                        graphics.DrawLine(wavMidLinePen, 0, 0, width, 0);
                    }

                    //绘制右声道波形
                    graphics.DrawLines(rightChannelPen, rightDataPoints);
                    midLineOffsetGraphics = 0 - leftChanelOffsetGraphics;
                }
                else
                {
                    //单声道
                    for (int i = 0; i < dataLength; i++)
                    {
                        lx = lx + spacing;
                        ly = leftData[i] / zoom;
                        leftDataPoints[i] = new PointF(lx, ly);
                    }

                    //绘左声道中线
                    if (isDrawWavMidLine)
                    {
                        graphics.DrawLine(wavMidLinePen, 0, 0, width, 0);
                    }

                    //绘制左声道波形
                    graphics.DrawLines(leftChannelPen, leftDataPoints);
                    midLineOffsetGraphics = rightChanelOffsetGraphics;
                }

                //绘制左右声道分隔线
                graphics.TranslateTransform(0, midLineOffsetGraphics, MatrixOrder.Prepend);
                graphics.DrawLine(channelDivideLinePen, 0, 0, width, 0);
            }
        }
    }
}
