using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    internal class ZoomInfo
    {
        private const int _NONE = 1;

        /// <summary>
        /// 缩放倍数
        /// </summary>
        private int _zoomMultiple = _NONE;

        internal int ZoomMultiple
        {
            get { return this._zoomMultiple; }
        }

        public ZoomInfo()
        {

        }

        /// <summary>
        /// 是否有缩放
        /// </summary>
        /// <returns></returns>
        internal bool HasZoom()
        {
            return this._zoomMultiple != _NONE;
        }

        internal void Reset()
        {
            this._zoomMultiple = _NONE;
        }

        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="plotPara"></param>
        /// <param name="minPointCount">图上最少点数</param>
        internal void ZoomIn(WavePlotPara plotPara, uint minPointCount = 5)
        {
            if (plotPara.SourcePcmDataLength / this._zoomMultiple <= minPointCount)
            {
                return;
            }

            this._zoomMultiple *= 2;

            this.UpdatePlotPara(plotPara);
        }

        /// <summary>
        /// 缩小
        /// </summary>
        internal void ZoomOut(WavePlotPara plotPara)
        {
            if (this._zoomMultiple <= _NONE)
            {
                return;
            }

            this._zoomMultiple /= 2;

            if (this._zoomMultiple <= _NONE)
            {
                plotPara.SBTOMillisecond = PlotConstant.ZEROR_D;
                plotPara.UpdateSETOMillisecond(PlotConstant.ZEROR_D);
                return;
            }

            this.UpdatePlotPara(plotPara);
        }

        private void UpdatePlotPara(WavePlotPara plotPara)
        {
            double setoMillisecond = plotPara.GetSETOMillisecond();
            double middle = plotPara.SBTOMillisecond + (setoMillisecond - plotPara.SBTOMillisecond) / 2;

            double showAreaMillisecond = plotPara.DurationMillisecond / this._zoomMultiple;
            double showAreaHalfMillisecond = showAreaMillisecond / 2;
            plotPara.SBTOMillisecond = middle - showAreaHalfMillisecond;
            plotPara.UpdateSETOMillisecond(middle + showAreaHalfMillisecond);
        }
    }
}
