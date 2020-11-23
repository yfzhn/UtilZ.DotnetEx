using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.WaveformControl
{
    /// <summary>
    /// 波形图控件-方法
    /// </summary>
    public partial class WaveControl
    {
        private VoicePlotData _plotPara = null;
        //private SelectedInfo _selectedInfo = null;
        private readonly ZoomInfo _zoomInfo = new ZoomInfo();

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            this._plotPara = null;

            //全部绘制
            Graphics graphics = this._grafx.Graphics;

            //清空所有已绘制的图形
            graphics.Clear(this.BackColor);

            this.RefreshInvalidateArea();//刷新
        }

        /// <summary>
        /// 设置语音图
        /// </summary>
        /// <param name="plotPara">图形参数</param>
        public void SetPlotData(VoicePlotData plotPara)
        {
            this._plotPara = plotPara;
            if (plotPara != null)
            {
                plotPara.Init();
                int targetCount = this.GetResamplePointCount();
                plotPara.Resample(targetCount, plotPara.Index, plotPara.Length);
            }

            this.AllDraw();
        }

        private int GetResamplePointCount()
        {
            return (int)(this._content.Area.Width * this._drawDensity);
        }

        private void SizeChangedResample()
        {

        }
    }
}
