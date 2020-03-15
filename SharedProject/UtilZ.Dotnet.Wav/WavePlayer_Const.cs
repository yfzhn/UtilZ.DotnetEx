using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav
{
    // WAV播放控件-常量
    public partial class WavePlayer
    {
        /// <summary>
        /// 一次数据采样率
        /// </summary>
        private const int FIRSTSAMPLE = 1000000;

        /// <summary>
        /// 数据偏移参数,获取数据时会偏移,那么在计算时间位置时需要回移
        /// </summary>
        private const int OFFSETPARA = 2;

        /// <summary>
        /// 各种画笔默认宽度
        /// </summary>
        private const float DEFAULLINEWIDTH = 0.5f;

        /// <summary>
        /// 浮点数精度值
        /// </summary>
        private const float PRECISION = 0.000001f;
    }
}
