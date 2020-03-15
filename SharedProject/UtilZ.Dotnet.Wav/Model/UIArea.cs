using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilZ.Dotnet.Wav.Model
{
    /// <summary>
    /// UI区域
    /// </summary>
    public enum UIArea
    {
        /// <summary>
        /// 缩略波形图区域
        /// </summary>
        ZoomArea,

        /// <summary>
        /// 缩略波形图中主波形显示区域
        /// </summary>
        ZoomDisplayArea,

        /// <summary>
        /// 时间区域
        /// </summary>
        TimeArea,

        /// <summary>
        /// 主波形图区域
        /// </summary>
        WavArea,

        ///// <summary>
        ///// 主波形图选中区域
        ///// </summary>
        //WavSelectedArea,

        /// <summary>
        /// 幅度区域
        /// </summary>
        DbArea,

        /// <summary>
        /// 空白区域
        /// </summary>
        None
    }
}
