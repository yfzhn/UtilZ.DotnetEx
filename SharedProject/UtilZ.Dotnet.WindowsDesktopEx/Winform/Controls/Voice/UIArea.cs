using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// UI区域
    /// </summary>
    public enum UIArea
    {
        /// <summary>
        /// 顶部全局 
        /// </summary>
        GlobalView,

        /// <summary>
        /// 整体视图中缩放后的显示区域
        /// </summary>
        GlobalViewZoomDisplay,

        /// <summary>
        /// 整体视图中非缩放后的显示区域
        /// </summary>
        GlobalViewZoomNormal,

        /// <summary>
        /// 波形图区域
        /// </summary>
        Wave,

        /// <summary>
        /// 波形图选中区域
        /// </summary>
        WaveSelected,

        /// <summary>
        /// 语谱图
        /// </summary>
        Voice,

        /// <summary>
        /// 语谱图选中区域
        /// </summary>
        VoiceSelected,


        /// <summary>
        /// 时间区域
        /// </summary>
        TimeArea,

        /// <summary>
        /// 幅度区域
        /// </summary>
        DbArea,

        /// <summary>
        /// 幅度区域
        /// </summary>
        HzArea,

        /// <summary>
        /// 其它区域
        /// </summary>
        Other
    }
}
