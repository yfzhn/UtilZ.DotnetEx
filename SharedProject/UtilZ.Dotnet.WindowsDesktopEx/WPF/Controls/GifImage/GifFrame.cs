using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.WPF.Controls
{
    /// <summary>
    /// gif帧
    /// </summary>
    internal class GifFrame : System.Windows.Controls.Image
    {
        /// <summary>
        /// 播放延时时间
        /// </summary>
        public int DelayTime { get; set; }

        /// <summary>
        /// 帧处理方法标识
        /// </summary>
        public int DisposalMethod { get; set; }

        /// <summary>
        /// 图像的左边位置
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// 图像的上边位置
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// gif图像的宽度
        /// </summary>
        public int GifWidth { get; set; }

        /// <summary>
        /// gif图像高度
        /// </summary>
        public int GifHeight { get; set; }
    }
}
