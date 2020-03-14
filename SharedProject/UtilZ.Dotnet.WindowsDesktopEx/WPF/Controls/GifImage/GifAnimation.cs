using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// gif动画解析及显示类
    /// </summary>
    internal class GifAnimation : Viewbox
    {
        /// <summary>
        /// 图片显示控件
        /// </summary>
        private readonly Canvas _canvas;

        /// <summary>
        /// 全部帧集合
        /// </summary>
        private List<GifFrame> _frameList = null;

        /// <summary>
        /// 播放帧计数
        /// </summary>
        private int _frameCounter = 0;

        /// <summary>
        /// 总帧数
        /// </summary>
        private int _numberOfFrames = 0;

        /// <summary>
        /// 播放帧数
        /// </summary>
        private int _numberOfLoops = -1;

        /// <summary>
        /// 播放帧索引
        /// </summary>
        private int _currentLoop = 0;

        /// <summary>
        /// gif图片宽度
        /// </summary>
        private int _logicalWidth = 0;

        /// <summary>
        /// gif图片高度
        /// </summary>
        private int _logicalHeight = 0;

        /// <summary>
        /// 帧播放时间计时器
        /// </summary>
        private DispatcherTimer _frameTimer = null;

        /// <summary>
        /// 当前解析的帧
        /// </summary>
        private GifFrame _currentParseGifFrame = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GifAnimation()
        {
            this._canvas = new Canvas();
            this.Child = this._canvas;
        }

        /// <summary>
        /// 重置变量参数
        /// </summary>
        private void Reset()
        {
            if (_frameList != null)
            {
                this._frameList.Clear();
            }
            this._frameList = null;
            this._frameCounter = 0;
            this._numberOfFrames = 0;
            this._numberOfLoops = -1;
            this._currentLoop = 0;
            this._logicalWidth = 0;
            this._logicalHeight = 0;
            if (this._frameTimer != null)
            {
                this._frameTimer.Stop();
                this._frameTimer = null;
            }
        }

        #region PARSE
        /// <summary>
        /// 解析二进制gif文件
        /// </summary>
        /// <param name="gifData">二进制gif数据</param>
        private void ParseGif(byte[] gifData)
        {
            this._frameList = new List<GifFrame>();
            this._currentParseGifFrame = new GifFrame();
            this.ParseGifDataStream(gifData, 0);
        }

        /// <summary>
        /// 解析gif数据块
        /// </summary>
        /// <param name="gifData">gif数据流</param>
        /// <param name="offset">流偏移量</param>
        /// <returns>数据结尾偏移量</returns>
        private int ParseBlock(byte[] gifData, int offset)
        {
            switch (gifData[offset])
            {
                case 0x21:
                    if (gifData[offset + 1] == 0xF9)
                    {
                        return this.ParseGraphicControlExtension(gifData, offset);
                    }
                    else
                    {
                        return this.ParseExtensionBlock(gifData, offset);
                    }
                case 0x2C:
                    offset = this.ParseGraphicBlock(gifData, offset);
                    this._frameList.Add(_currentParseGifFrame);
                    this._currentParseGifFrame = new GifFrame();
                    return offset;
                case 0x3B:
                    return -1;
                default:
                    throw new Exception("GIF format incorrect: missing graphic block or special-purpose block. ");
            }
        }

        /// <summary>
        /// 解析每帧的控制块
        /// </summary>
        /// <param name="gifData">gif数据流</param>
        /// <param name="offset">数据偏移量</param>
        /// <returns>解析之后的偏移量</returns>
        private int ParseGraphicControlExtension(byte[] gifData, int offset)
        {
            int returnOffset = offset;
            int length = gifData[offset + 2];
            returnOffset = offset + length + 2 + 1;

            byte packedField = gifData[offset + 3];
            this._currentParseGifFrame.DisposalMethod = (packedField & 0x1C) >> 2;

            int delay = BitConverter.ToUInt16(gifData, offset + 4);
            this._currentParseGifFrame.DelayTime = delay;
            while (gifData[returnOffset] != 0x00)
            {
                returnOffset = returnOffset + gifData[returnOffset] + 1;
            }

            returnOffset++;

            return returnOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gifData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int ParseLogicalScreen(byte[] gifData, int offset)
        {
            this._logicalWidth = BitConverter.ToUInt16(gifData, offset);
            this._logicalHeight = BitConverter.ToUInt16(gifData, offset + 2);
            byte packedField = gifData[offset + 4];
            bool hasGlobalColorTable = (int)(packedField & 0x80) > 0 ? true : false;
            int currentIndex = offset + 7;

            if (hasGlobalColorTable)
            {
                int colorTableLength = packedField & 0x07;
                colorTableLength = (int)Math.Pow(2, colorTableLength + 1) * 3;
                currentIndex = currentIndex + colorTableLength;
            }

            return currentIndex;
        }

        /// <summary>
        /// 解析每帧的图像块
        /// </summary>
        /// <param name="gifData">gif数据流</param>
        /// <param name="offset">数据偏移量</param>
        /// <returns>解析之后的偏移量</returns>
        private int ParseGraphicBlock(byte[] gifData, int offset)
        {
            this._currentParseGifFrame.Left = BitConverter.ToUInt16(gifData, offset + 1);
            this._currentParseGifFrame.Top = BitConverter.ToUInt16(gifData, offset + 3);
            this._currentParseGifFrame.GifWidth = BitConverter.ToUInt16(gifData, offset + 5);
            this._currentParseGifFrame.GifHeight = BitConverter.ToUInt16(gifData, offset + 7);
            if (this._currentParseGifFrame.GifWidth > this._logicalWidth)
            {
                this._logicalWidth = this._currentParseGifFrame.GifWidth;
            }
            if (this._currentParseGifFrame.GifHeight > this._logicalHeight)
            {
                this._logicalHeight = this._currentParseGifFrame.GifHeight;
            }
            byte packedField = gifData[offset + 9];
            bool hasLocalColorTable = (int)(packedField & 0x80) > 0 ? true : false;

            int currentIndex = offset + 9;
            if (hasLocalColorTable)
            {
                int colorTableLength = packedField & 0x07;
                colorTableLength = (int)Math.Pow(2, colorTableLength + 1) * 3;
                currentIndex = currentIndex + colorTableLength;
            }
            currentIndex++;

            currentIndex++;

            while (gifData[currentIndex] != 0x00)
            {
                int length = gifData[currentIndex];
                currentIndex = currentIndex + gifData[currentIndex];
                currentIndex++;
            }
            currentIndex = currentIndex + 1;
            return currentIndex;
        }

        /// <summary>
        /// 解析数据块
        /// </summary>
        /// <param name="gifData">gif数据流</param>
        /// <param name="offset">流偏移量</param>
        /// <returns>数据结尾偏移量</returns>
        private int ParseExtensionBlock(byte[] gifData, int offset)
        {
            int returnOffset = offset;
            int length = gifData[offset + 2];
            returnOffset = offset + length + 2 + 1;
            if (gifData[offset + 1] == 0xFF && length > 10)
            {
                string netscape = System.Text.ASCIIEncoding.ASCII.GetString(gifData, offset + 3, 8);
                if (netscape == "NETSCAPE")
                {
                    this._numberOfLoops = BitConverter.ToUInt16(gifData, offset + 16);
                    if (this._numberOfLoops > 0)
                    {
                        this._numberOfLoops++;
                    }
                }
            }

            while (gifData[returnOffset] != 0x00)
            {
                returnOffset = returnOffset + gifData[returnOffset] + 1;
            }

            returnOffset++;
            return returnOffset;
        }

        /// <summary>
        /// 解析gif头
        /// </summary>
        /// <param name="gifData">gif数据流</param>
        /// <param name="offset">流偏移量</param>
        /// <returns>头位置偏移量</returns>
        private int ParseHeader(byte[] gifData, int offset)
        {
            string str = System.Text.ASCIIEncoding.ASCII.GetString(gifData, offset, 3);
            if (str != "GIF")
            {
                throw new Exception("Not a proper GIF file: missing GIF header");
            }

            //根据gif文件的结果规定头位置偏移6个字节,所以返回常数6
            return 6;
        }

        /// <summary>
        /// 解析gif数据流
        /// </summary>
        /// <param name="gifData">gif数据流</param>
        /// <param name="offset">流偏移量</param>
        private void ParseGifDataStream(byte[] gifData, int offset)
        {
            offset = this.ParseHeader(gifData, offset);
            offset = this.ParseLogicalScreen(gifData, offset);

            while (offset != -1)
            {
                offset = this.ParseBlock(gifData, offset);
            }
        }

        #endregion

        /// <summary>
        /// 根据gif流创建gif
        /// </summary>
        /// <param name="memoryStream">gif流</param>
        public void CreateGifAnimation(MemoryStream memoryStream)
        {
            //重置变量参数
            this.Reset();

            byte[] gifData = memoryStream.GetBuffer();  // Use GetBuffer so that there is no memory copy
            GifBitmapDecoder decoder = new GifBitmapDecoder(memoryStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            this._numberOfFrames = decoder.Frames.Count;
            try
            {
                this.ParseGif(gifData);
            }
            catch
            {
                throw new FileFormatException("解析gif图片失败");
            }

            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                this._frameList[i].Source = decoder.Frames[i];
                this._frameList[i].Visibility = System.Windows.Visibility.Hidden;
                this._canvas.Children.Add(this._frameList[i]);
                Canvas.SetLeft(this._frameList[i], this._frameList[i].Left);
                Canvas.SetTop(this._frameList[i], this._frameList[i].Top);
                Canvas.SetZIndex(this._frameList[i], i);
            }

            this._canvas.Height = this._logicalHeight;
            this._canvas.Width = this._logicalWidth;
            this._frameList[0].Visibility = System.Windows.Visibility.Visible;

            for (int i = 0; i < this._frameList.Count; i++)
            {
                Console.WriteLine(this._frameList[i].DisposalMethod.ToString() + " " + this._frameList[i].GifWidth.ToString() + " " + this._frameList[i].DelayTime.ToString());
            }

            if (this._frameList.Count > 1)
            {
                if (this._numberOfLoops == -1)
                {
                    this._numberOfLoops = 1;
                }
                this._frameTimer = new System.Windows.Threading.DispatcherTimer();
                this._frameTimer.Tick += NextFrame;
                this._frameTimer.Interval = new TimeSpan(0, 0, 0, 0, this._frameList[0].DelayTime * 10);
                this._frameTimer.Start();
            }
        }

        /// <summary>
        /// 播放下一帧事件方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NextFrame(object sender, EventArgs e)
        {
            this._frameTimer.Stop();
            if (this._numberOfFrames == 0) return;
            if (this._frameList[this._frameCounter].DisposalMethod == 2)
            {
                this._frameList[this._frameCounter].Visibility = System.Windows.Visibility.Hidden;
            }
            if (this._frameList[this._frameCounter].DisposalMethod >= 3)
            {
                this._frameList[this._frameCounter].Visibility = System.Windows.Visibility.Hidden;
            }
            this._frameCounter++;

            if (this._frameCounter < this._numberOfFrames)
            {
                this._frameList[this._frameCounter].Visibility = System.Windows.Visibility.Visible;
                this._frameTimer.Interval = new TimeSpan(0, 0, 0, 0, _frameList[_frameCounter].DelayTime * 10);
                this._frameTimer.Start();
            }
            else
            {
                if (this._numberOfLoops != 0)
                {
                    this._currentLoop++;
                }
                if (this._currentLoop < this._numberOfLoops || this._numberOfLoops == 0)
                {
                    for (int f = 0; f < _frameList.Count; f++)
                    {
                        this._frameList[f].Visibility = System.Windows.Visibility.Hidden;
                    }
                    this._frameCounter = 0;
                    this._frameList[this._frameCounter].Visibility = System.Windows.Visibility.Visible;
                    this._frameTimer.Interval = new TimeSpan(0, 0, 0, 0, this._frameList[this._frameCounter].DelayTime * 10);
                    this._frameTimer.Start();
                }
            }
        }
    }
}
