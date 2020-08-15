using System;
using System.Collections.Generic;
using System.Text;

namespace UtilZ.Dotnet.WindowsDesktopEx.Winform.Controls
{
    /// <summary>
    /// 波形图参数
    /// </summary>
    public class WavePlotPara
    {
        /// <summary>
        /// 起始的基准时间,其它时间皆基于此值往后增加多少毫秒
        /// </summary>
        public DateTime BaseTime { get; private set; }

        /// <summary>
        /// 总时长,单位/毫秒
        /// </summary>
        public double DurationMillisecond { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseTime">起始的基准时间,其它时间皆基于此值往后增加多少毫秒</param>
        /// <param name="durationMillisecond">总时长,单位/毫秒</param>
        public WavePlotPara(DateTime baseTime, double durationMillisecond)
        {
            this.BaseTime = baseTime;
            this.DurationMillisecond = durationMillisecond;
        }



        /*****************************************************************************
         * a                      b                     c                       d
         * |----------------------|---------------------|-----------------------|--------->
         *
         * a:BaseTime
         * b:SBTO
         * c:SETO
         * d:DurationMillisecond
         *****************************************************************************/


        internal int SourcePcmDataLength { get; set; }

        internal int GlobalViewPcmDataLength { get; set; }

        internal int DrawPcmDataLength { get; set; }

        private double _showBeginTimeOffsetMillisecond = 0;
        /// <summary>
        /// 获取或设置相对于起始的基准时间的显示起始
        /// SBTO=>showBeginTimeOffset
        /// </summary>
        internal double SBTOMillisecond
        {
            get { return this._showBeginTimeOffsetMillisecond; }
            set { this._showBeginTimeOffsetMillisecond = value; }
        }

        private double _showEndTimeOffsetMillisecond = 0;
        /// <summary>
        /// SETO=>ShowEndTimeOffsetMillisecond
        /// </summary>
        internal void UpdateSETOMillisecond(double setoMillisecond)
        {
            this._showEndTimeOffsetMillisecond = setoMillisecond;
        }

        internal double GetSETOMillisecond()
        {
            double setoMillisecond = this._showEndTimeOffsetMillisecond;
            if (setoMillisecond <= PlotConstant.ZEROR_D)
            {
                setoMillisecond = this.DurationMillisecond;
            }

            return setoMillisecond;
        }


        internal bool ContainsShowTime(double offsetTimeMilliseconds)
        {
            if (offsetTimeMilliseconds < this._showBeginTimeOffsetMillisecond || offsetTimeMilliseconds > this.GetSETOMillisecond())
            {
                //不在范围内
                return false;
            }

            return true;
        }
    }
}
